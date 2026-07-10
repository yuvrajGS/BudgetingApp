// Best-effort parser for bank statement text extracted from a PDF.
// Statement layouts vary a lot between banks, so this looks for lines that
// contain BOTH a date and a dollar amount and treats everything else on the
// line as the description. It's intentionally conservative — lines that
// don't clearly match are skipped rather than guessed at — because the
// result is always shown to the user in an editable table before anything
// is submitted.

const DATE_PATTERNS = [
  { re: /(\d{4})-(\d{2})-(\d{2})/, type: "iso" },
  { re: /(\d{1,2})\/(\d{1,2})\/(\d{2,4})/, type: "slash" },
  {
    re: /\b(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\.?\s+(\d{1,2})(?:,?\s+(\d{4}))?/i,
    type: "month",
  },
];

// Matches money-like tokens: optional parens/minus for negatives, optional
// $, digits with optional thousands separators, exactly two decimal places.
const AMOUNT_PATTERN = /\(?-?\$?\d{1,3}(?:,\d{3})*\.\d{2}\)?-?/g;

const MONTH_ABBRS = ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];

function parseAmountToken(token) {
  const isNegative = token.includes("(") || /^-/.test(token.trim()) || /-$/.test(token.trim());
  const numeric = token.replace(/[^0-9.]/g, "");
  if (!numeric) return null;
  const value = parseFloat(numeric);
  if (Number.isNaN(value)) return null;
  return isNegative ? -value : value;
}

function buildIsoDate(match, type, fallbackYear) {
  if (type === "iso") {
    const [, y, m, d] = match;
    return `${y}-${m}-${d}`;
  }
  if (type === "slash") {
    let [, m, d, y] = match;
    if (y.length === 2) y = (Number(y) > 50 ? "19" : "20") + y;
    return `${y}-${m.padStart(2, "0")}-${d.padStart(2, "0")}`;
  }
  if (type === "month") {
    const [, monthName, day, year] = match;
    const monthIndex = MONTH_ABBRS.indexOf(monthName.slice(0, 3).toLowerCase());
    if (monthIndex === -1) return null;
    const y = year || fallbackYear || new Date().getFullYear();
    const dt = new Date(Number(y), monthIndex, Number(day));
    if (Number.isNaN(dt.getTime())) return null;
    return dt.toISOString().slice(0, 10);
  }
  return null;
}

// Returns { rows, skipped } — rows are best-guess transactions, skipped is
// the count of non-empty lines that didn't clearly contain a date + amount
// (useful for telling the user "N lines couldn't be read automatically").
export function parseStatementLines(lines, { fallbackYear } = {}) {
  const rows = [];
  let skipped = 0;

  for (const raw of lines) {
    const line = raw.trim();
    if (!line) continue;

    let dateMatch = null;
    let dateType = null;
    for (const { re, type } of DATE_PATTERNS) {
      const m = line.match(re);
      if (m) {
        dateMatch = m;
        dateType = type;
        break;
      }
    }

    const amountMatches = line.match(AMOUNT_PATTERN);

    if (!dateMatch || !amountMatches || amountMatches.length === 0) {
      skipped += 1;
      continue;
    }

    // Statements often show "amount" then "running balance" on one line —
    // when two+ numbers are present, the second-to-last is usually the
    // transaction amount and the last is the balance.
    const amountToken =
      amountMatches.length >= 2 ? amountMatches[amountMatches.length - 2] : amountMatches[0];
    const amount = parseAmountToken(amountToken);
    const date = buildIsoDate(dateMatch, dateType, fallbackYear);

    if (amount == null || !date) {
      skipped += 1;
      continue;
    }

    let description = line.replace(dateMatch[0], "");
    for (const token of amountMatches) description = description.replace(token, "");
    description = description.replace(/\s{2,}/g, " ").trim();

    rows.push({
      date,
      merchant: description.slice(0, 60) || "Unknown merchant",
      description,
      amount: Math.abs(amount),
    });
  }

  return { rows, skipped };
}
