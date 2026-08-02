using BudgetingApp.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using Word = UglyToad.PdfPig.Content.Word;

namespace BudgetingApp.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService>? _logger;
        private readonly HashSet<string> _summaryLineKeywords;

        // Deliberately narrow, multi-word phrases for non-transaction summary/total
        // rows (e.g. "Total of Payment Activity", "New Balance"). A broad rule like
        // "skip anything starting with 'Total'" is NOT used here because it's too
        // risky - real merchants legitimately start with that word (e.g. "TOTAL
        // WINE & MORE", "TOTAL WIRELESS"), and a prefix match would silently drop
        // those transactions. These specific phrases are how statements label
        // summary rows and are unlikely to collide with a merchant name.
        private static readonly string[] DefaultSummaryLineKeywords =
        {
            "total of payment activity",
            "total of new activity",
            "total payments and credits",
            "total purchases and other charges",
            "total fees",
            "total interest charged",
            "total interest",
            "new balance",
            "previous balance",
            "opening balance",
            "closing balance",
            "statement balance",
            "minimum payment due",
            "minimum payment",
            "account summary",
            "please pay by"
        };

        /// <param name="logger">Optional logger.</param>
        /// <param name="additionalSummaryLineKeywords">
        /// Optional extra phrases (case-insensitive) identifying non-transaction
        /// summary/total rows specific to a bank template you're supporting, merged
        /// with <see cref="DefaultSummaryLineKeywords"/>. Use this instead of
        /// editing the parser if a particular statement has its own summary
        /// wording that's slipping through (or being over-filtered).
        /// </param>
        public DocumentService(
            ILogger<DocumentService>? logger = null,
            IEnumerable<string>? additionalSummaryLineKeywords = null)
        {
            _logger = logger;

            _summaryLineKeywords = new HashSet<string>(DefaultSummaryLineKeywords, StringComparer.OrdinalIgnoreCase);
            if (additionalSummaryLineKeywords != null)
            {
                foreach (var keyword in additionalSummaryLineKeywords)
                {
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        _summaryLineKeywords.Add(keyword.Trim());
                    }
                }
            }
        }

        private bool IsLikelySummaryLine(string lineText)
        {
            var normalized = lineText.ToLowerInvariant();
            foreach (var keyword in _summaryLineKeywords)
            {
                if (normalized.Contains(keyword.ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<IEnumerable<CreateTransactionDTO>> ProcessPdfAsync(Guid userId, Stream pdfStream)
        {
            if (pdfStream == null) throw new ArgumentNullException(nameof(pdfStream));

            // PdfPig needs random access; buffer the stream if it isn't seekable
            // (e.g. a request body stream).
            Stream workingStream = pdfStream;
            MemoryStream? buffer = null;
            if (!pdfStream.CanSeek)
            {
                buffer = new MemoryStream();
                await pdfStream.CopyToAsync(buffer);
                buffer.Position = 0;
                workingStream = buffer;
            }

            try
            {
                // PdfPig parsing is synchronous/CPU-bound; offload to the thread pool
                // so we don't block the request pipeline.
                return await Task.Run(() => ExtractTransactions(userId, workingStream));
            }
            finally
            {
                buffer?.Dispose();
            }
        }

        // ---------------------------------------------------------------------------
        // Core extraction pipeline
        // ---------------------------------------------------------------------------

        private List<CreateTransactionDTO> ExtractTransactions(Guid userId, Stream pdfStream)
        {
            var results = new List<CreateTransactionDTO>();

            using PdfDocument document = PdfDocument.Open(pdfStream);

            if (document.NumberOfPages == 0)
            {
                return results;
            }

            // Build line-grouped, position-aware representation of every page up front.
            var pagesLines = document.GetPages()
                .Select(ExtractLines)
                .ToList();

            var allLines = pagesLines.SelectMany(l => l).ToList();

            var format = DetectStatementFormat(allLines);
            var statementYear = InferStatementYear(allLines);

            _logger?.LogInformation(
                "Detected statement format {Format} with inferred year {Year} for user {UserId}",
                format, statementYear, userId);

            List<CreateTransactionDTO> parsed;

            if (format == StatementFormat.MultiColumn)
            {
                var columns = DetectColumnLayout(allLines);
                if (columns == null)
                {
                    // Fell back to multi-column detection by keyword but couldn't
                    // locate a header row with usable column positions; degrade
                    // gracefully to single-column parsing rather than returning nothing.
                    _logger?.LogWarning(
                        "Multi-column keywords found but no header row located; falling back to single-column parsing.");
                    parsed = ParseSingleAmountColumnLines(allLines, statementYear);
                }
                else
                {
                    parsed = ParseMultiColumnLines(allLines, columns, statementYear);
                }
            }
            else
            {
                parsed = ParseSingleAmountColumnLines(allLines, statementYear);
            }

            foreach (var dto in parsed)
            {
                dto.UserId = userId;
                results.Add(dto);
            }

            return results;
        }

        // ---------------------------------------------------------------------------
        // Line extraction (words -> visual lines, preserving column positions)
        // ---------------------------------------------------------------------------

        private sealed class PdfLine
        {
            public int PageNumber { get; init; }
            public double Y { get; init; }
            public List<Word> Words { get; } = new();
            public string Text => string.Join(" ", Words.Select(w => w.Text));
        }

        private static List<PdfLine> ExtractLines(Page page)
        {
            const double yTolerance = 3.0;

            var words = page.GetWords().ToList();
            var lines = new List<PdfLine>();

            // Process top-to-bottom so lines come out in reading order.
            foreach (var word in words.OrderByDescending(w => w.BoundingBox.Top))
            {
                var line = lines.FirstOrDefault(l => Math.Abs(l.Y - word.BoundingBox.Top) <= yTolerance);
                if (line == null)
                {
                    line = new PdfLine { PageNumber = page.Number, Y = word.BoundingBox.Top };
                    lines.Add(line);
                }
                line.Words.Add(word);
            }

            foreach (var line in lines)
            {
                line.Words.Sort((a, b) => a.BoundingBox.Left.CompareTo(b.BoundingBox.Left));
            }

            return lines.OrderByDescending(l => l.Y).ToList();
        }

        // ---------------------------------------------------------------------------
        // Format + column detection
        // ---------------------------------------------------------------------------

        private enum StatementFormat
        {
            SingleAmountColumn,
            MultiColumn
        }

        private static readonly Regex WithdrawalHeaderPattern =
            new(@"with ?drawal|funds out|withdrawn|debit(s)?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DepositHeaderPattern =
            new(@"deposit(s)?|funds in|credit(s)?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex BalanceHeaderPattern =
            new(@"balance", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static StatementFormat DetectStatementFormat(List<PdfLine> allLines)
        {
            var fullText = string.Join(" ", allLines.Select(l => l.Text));

            bool hasWithdrawal = WithdrawalHeaderPattern.IsMatch(fullText);
            bool hasDeposit = DepositHeaderPattern.IsMatch(fullText);

            return (hasWithdrawal && hasDeposit) ? StatementFormat.MultiColumn : StatementFormat.SingleAmountColumn;
        }

        private sealed class ColumnLayout
        {
            public double WithdrawalX { get; set; }
            public double DepositX { get; set; }
            public double? BalanceX { get; set; }
        }

        /// <summary>
        /// Looks for a header row containing both a withdrawal/debit label and a
        /// deposit/credit label, and records their horizontal center positions so
        /// subsequent numeric tokens can be bucketed into the correct column.
        /// </summary>
        private static ColumnLayout? DetectColumnLayout(List<PdfLine> allLines)
        {
            foreach (var line in allLines)
            {
                var withdrawalWords = FindHeaderPhrase(line.Words, WithdrawalHeaderPattern);
                var depositWords = FindHeaderPhrase(line.Words, DepositHeaderPattern);

                if (withdrawalWords != null && depositWords != null)
                {
                    var balanceWords = FindHeaderPhrase(line.Words, BalanceHeaderPattern);

                    return new ColumnLayout
                    {
                        WithdrawalX = CenterX(withdrawalWords),
                        DepositX = CenterX(depositWords),
                        BalanceX = balanceWords != null ? CenterX(balanceWords) : (double?)null
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// PDF text extraction tokenizes each visual "word" (whitespace-separated
        /// run) as its own <see cref="Word"/>, so a two-word header label like
        /// "Funds Out" or "Withdrawal Amount" arrives as two separate tokens and
        /// never matches a header pattern when tokens are checked individually.
        /// This checks runs of 1-3 consecutive words joined together against the
        /// pattern so multi-word headers are still detected, and returns the
        /// matching words so their combined bounding box can be used for the
        /// column's X position.
        /// </summary>
        private static List<Word>? FindHeaderPhrase(List<Word> words, Regex pattern)
        {
            for (int windowSize = 1; windowSize <= 3 && windowSize <= words.Count; windowSize++)
            {
                for (int i = 0; i + windowSize <= words.Count; i++)
                {
                    var window = words.Skip(i).Take(windowSize).ToList();
                    var joined = string.Join(" ", window.Select(w => w.Text));
                    if (pattern.IsMatch(joined))
                    {
                        return window;
                    }
                }
            }

            return null;
        }

        private static double CenterX(Word w) => (w.BoundingBox.Left + w.BoundingBox.Right) / 2.0;

        private static double CenterX(List<Word> words) =>
            (words.Min(w => w.BoundingBox.Left) + words.Max(w => w.BoundingBox.Right)) / 2.0;

        /// <summary>
        /// Best-effort guess at the statement's year, used to fill in dates printed
        /// without a year (e.g. "Jan 15"). Falls back to null (caller uses current
        /// year) if nothing is found.
        /// </summary>
        private static int? InferStatementYear(List<PdfLine> allLines)
        {
            // Look at the first couple of pages only; statement period / year is
            // almost always near the top.
            var text = string.Join(" ", allLines.Take(200).Select(l => l.Text));
            var match = Regex.Match(text, @"\b(20\d{2})\b");
            return match.Success ? int.Parse(match.Value, CultureInfo.InvariantCulture) : (int?)null;
        }

        // ---------------------------------------------------------------------------
        // Date parsing
        // ---------------------------------------------------------------------------

        private static readonly string[] DateFormatsWithYear =
        {
            "M/d/yyyy", "M/d/yy", "MM/dd/yyyy", "MM/dd/yy",
            "yyyy-MM-dd", "yyyy/MM/dd",
            "MMM d, yyyy", "MMM d yyyy", "MMM d,yyyy",
            "MMMM d, yyyy", "MMMM d yyyy",
            "d MMM yyyy", "d-MMM-yyyy", "dd-MMM-yyyy",
            "d MMMM yyyy"
        };

        private static readonly string[] DateFormatsNoYear =
        {
            "MMM d", "MMM dd", "MMMM d",
            "d MMM", "dd MMM",
            "MM/dd", "M/d"
        };

        /// <summary>
        /// Attempts to parse a date starting at the beginning of a line's word list.
        /// Tries combinations of the first 1-3 words (to handle "Jan", "15", "2024"
        /// as separate word tokens as well as "Jan 15, 2024" as fewer tokens).
        /// </summary>
        private static DateOnly? TryExtractLeadingDate(List<Word> words, int? statementYear, out int consumedWordCount)
        {
            consumedWordCount = 0;
            if (words.Count == 0) return null;

            int maxTake = Math.Min(3, words.Count);

            for (int take = maxTake; take >= 1; take--)
            {
                var candidate = string.Join(" ", words.Take(take).Select(w => w.Text)).Trim().TrimEnd(',');

                foreach (var fmt in DateFormatsWithYear)
                {
                    if (DateTime.TryParseExact(candidate, fmt, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out var dt))
                    {
                        consumedWordCount = take;
                        return DateOnly.FromDateTime(dt);
                    }
                }
            }

            int maxTakeNoYear = Math.Min(2, words.Count);
            for (int take = maxTakeNoYear; take >= 1; take--)
            {
                var candidate = string.Join(" ", words.Take(take).Select(w => w.Text)).Trim().TrimEnd(',');

                foreach (var fmt in DateFormatsNoYear)
                {
                    if (DateTime.TryParseExact(candidate, fmt, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out var dt))
                    {
                        var year = statementYear ?? DateTime.UtcNow.Year;
                        consumedWordCount = take;
                        return new DateOnly(year, dt.Month, dt.Day);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Some statements (notably Amex) print two dates at the start of each line -
        /// a transaction date followed by a posting date, e.g. "Jun 28 Jun 29 MERCHANT
        /// NAME ...". This wraps TryExtractLeadingDate to also detect and skip over a
        /// second immediately-following date so it doesn't get absorbed into the
        /// merchant name. The first date (transaction date) is what's returned/used.
        /// </summary>
        private static DateOnly? TryExtractTransactionDate(List<Word> words, int? statementYear, out int consumedWordCount)
        {
            var date = TryExtractLeadingDate(words, statementYear, out var firstCount);
            consumedWordCount = firstCount;
            if (date == null) return null;

            var remaining = words.Skip(firstCount).ToList();
            var secondDate = TryExtractLeadingDate(remaining, statementYear, out var secondCount);
            if (secondDate != null)
            {
                consumedWordCount += secondCount;
            }

            return date;
        }

        // ---------------------------------------------------------------------------
        // Amount parsing
        // ---------------------------------------------------------------------------

        // Matches typical currency tokens: $1,234.56 / 1234.56 / (1234.56) / 1234.56- /
        // -$1,234.56 / $-1,234.56. Requires a decimal component so it doesn't match
        // account/reference numbers.
        private static readonly Regex AmountPattern =
            new(@"^\(?[-\$]{0,2}\d{1,3}(,\d{3})*\.\d{2}\)?-?$", RegexOptions.Compiled);

        private static bool IsAmountToken(string text) => AmountPattern.IsMatch(text.Trim());

        // Some PDFs render a leading minus sign as its own word (whitespace between
        // the "-" and the number), e.g. "- 45.00" for an Amex refund/credit. This
        // catches that so the sign isn't lost.
        private static bool IsStandaloneMinusToken(string text)
        {
            var t = text.Trim();
            return t == "-" || t == "\u2013" || t == "\u2014";
        }

        /// <summary>
        /// Scans words starting at <paramref name="startIndex"/> for the first
        /// amount, accounting for a possible standalone minus-sign token
        /// immediately preceding it. Returns the parsed (signed) amount, the index
        /// of the first word belonging to it (the minus sign if present, otherwise
        /// the amount token itself), and how many words it spans.
        /// </summary>
        private static (decimal Amount, int Index, int WordSpan)? FindFirstAmount(List<Word> words, int startIndex)
        {
            for (int i = startIndex; i < words.Count; i++)
            {
                var text = words[i].Text;

                if (IsStandaloneMinusToken(text) && i + 1 < words.Count && IsAmountToken(words[i + 1].Text))
                {
                    var amount = -Math.Abs(ParseAmount(words[i + 1].Text));
                    return (amount, i, 2);
                }

                if (IsAmountToken(text))
                {
                    return (ParseAmount(text), i, 1);
                }
            }

            return null;
        }

        private static decimal ParseAmount(string text)
        {
            text = text.Trim();
            bool negative = false;

            if (text.StartsWith("(") && text.EndsWith(")"))
            {
                negative = true;
                text = text[1..^1];
            }

            if (text.EndsWith("-"))
            {
                negative = true;
                text = text[..^1];
            }

            if (text.StartsWith("-"))
            {
                negative = true;
                text = text[1..];
            }

            text = text.Replace("$", string.Empty).Replace(",", string.Empty).Trim();

            var value = decimal.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);
            return negative ? -value : value;
        }

        // ---------------------------------------------------------------------------
        // Strategy 1: single signed-amount column (e.g. Amex-style credit card)
        // ---------------------------------------------------------------------------

        private List<CreateTransactionDTO> ParseSingleAmountColumnLines(List<PdfLine> lines, int? statementYear)
        {
            var results = new List<CreateTransactionDTO>();
            DateOnly? lastKnownDate = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line.Text)) continue;
                if (IsLikelySummaryLine(line.Text)) continue;

                var words = line.Words;
                var date = TryExtractTransactionDate(words, statementYear, out var dateWordCount);

                // Find the first amount-shaped token on the line (leftmost). For
                // single-amount-column statements this is normally the only
                // currency-shaped token; if a trailing running balance is also
                // present, taking the first (not last) amount favors the
                // transaction amount over the balance.
                var found = FindFirstAmount(words, date != null ? dateWordCount : 0);
                int amountIndex = found?.Index ?? -1;
                decimal? amount = found?.Amount;

                if (date == null && amount == null)
                {
                    // No date, no amount -> not a transaction line, skip it.
                    continue;
                }

                if (date != null)
                {
                    lastKnownDate = date;
                }

                if (amount == null)
                {
                    // A line that's only a date (e.g. a date header/divider) - nothing to record.
                    continue;
                }

                if (lastKnownDate == null)
                {
                    // Can't build a valid transaction without a date reference yet.
                    continue;
                }

                var merchantStart = date != null ? dateWordCount : 0;
                var merchantWords = words.Skip(merchantStart).Take(Math.Max(0, amountIndex - merchantStart));
                var merchant = string.Join(" ", merchantWords.Select(w => w.Text)).Trim();

                if (string.IsNullOrWhiteSpace(merchant))
                {
                    merchant = "Unknown";
                }

                results.Add(new CreateTransactionDTO
                {
                    Date = lastKnownDate.Value,
                    Merchant = merchant,
                    Amount = amount.Value
                });
            }

            return results;
        }

        // ---------------------------------------------------------------------------
        // Strategy 2: multi-column withdrawal/deposit (e.g. RBC-style bank statement)
        // ---------------------------------------------------------------------------

        private List<CreateTransactionDTO> ParseMultiColumnLines(
            List<PdfLine> lines, ColumnLayout columns, int? statementYear)
        {
            const double columnTolerance = 25.0; // points; tune per template if needed

            var results = new List<CreateTransactionDTO>();
            DateOnly? lastKnownDate = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line.Text)) continue;
                if (IsLikelySummaryLine(line.Text)) continue;

                var words = line.Words;
                var date = TryExtractTransactionDate(words, statementYear, out var dateWordCount);

                var amountWords = words.Where(w => IsAmountToken(w.Text)).ToList();

                var withdrawalWord = amountWords
                    .FirstOrDefault(w => Math.Abs(CenterX(w) - columns.WithdrawalX) <= columnTolerance);
                var depositWord = amountWords
                    .FirstOrDefault(w => Math.Abs(CenterX(w) - columns.DepositX) <= columnTolerance);

                decimal? amount = null;

                if (withdrawalWord != null)
                {
                    // Money out -> positive.
                    amount = Math.Abs(ParseAmount(withdrawalWord.Text));
                }
                else if (depositWord != null)
                {
                    // Money in -> negative.
                    amount = -Math.Abs(ParseAmount(depositWord.Text));
                }

                if (date == null && amount == null)
                {
                    // No date, no amount -> not a transaction line, skip it.
                    continue;
                }

                if (date != null)
                {
                    lastKnownDate = date;
                }

                if (amount == null || lastKnownDate == null)
                {
                    continue;
                }

                var merchantStart = date != null ? dateWordCount : 0;

                // Exclude the balance column (and any other amount-shaped tokens,
                // e.g. a running total) plus the matched withdrawal/deposit token
                // from the merchant text.
                var excluded = new HashSet<Word>(amountWords);

                var merchantWords = words
                    .Skip(merchantStart)
                    .Where(w => !excluded.Contains(w))
                    .Select(w => w.Text);

                var merchant = string.Join(" ", merchantWords).Trim();
                if (string.IsNullOrWhiteSpace(merchant))
                {
                    merchant = "Unknown";
                }

                results.Add(new CreateTransactionDTO
                {
                    Date = lastKnownDate.Value,
                    Merchant = merchant,
                    Amount = amount.Value
                });
            }

            return results;
        }
    }
}