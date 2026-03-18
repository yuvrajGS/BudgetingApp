"""
normalize_service.py — Merchant name normalisation for embedding-based classification.

Strips noise that would prevent all-MiniLM-L6-v2 from correctly clustering merchant names:
  - Payment-processor prefixes  (SQ *, TST*, PP*, PAYPAL *, …)
  - Random alphanumeric suffixes / transaction codes
  - Terminal / location IDs     (#12345, store codes, 00210-08)
  - Legal-entity suffixes       (Inc., Ltd, LLC, Corp, Whse, …)
  - URLs and domain fragments   (HELP.UBER.COM, APPLE.COM/BILL)
  - Phone numbers               (1-800-555-0175, 4029357733)
  - Trailing punctuation / whitespace

Public API
----------
    normalise(raw: str) -> str
"""

import re
import unicodedata

# ---------------------------------------------------------------------------
# Processor prefixes
# ---------------------------------------------------------------------------

# Sorted longest-first so greedier patterns win.
_PROCESSOR_PREFIXES: list[str] = sorted(
    [
        "PAYPAL *", "PAYPAL*",
        "SQ *", "SQ*",
        "TST*", "TST *",
        "PP *", "PP*",
        "SP *", "SP*",       # Stripe
        "APL*",              # Apple Pay
        "AMZN*", "AMZN *",
        "VZWRLSS*",          # Verizon Wireless
        "PMT*",
        "CKO*",              # Checkout.com
    ],
    key=len,
    reverse=True,
)

# ---------------------------------------------------------------------------
# Compiled regexes  (built once at module load — never recompiled)
# ---------------------------------------------------------------------------

_RE_LEGAL = re.compile(
    r"(?i)(?:"
    + "|".join([
        r"\bINC\.?\b", r"\bLTD\.?\b", r"\bLLC\.?\b", r"\bL\.L\.C\.?\b",
        r"\bCORP\.?\b", r"\bCO\.?\b",  r"\bPLC\.?\b", r"\bGMBH\b",
        r"\bS\.A\.?\b", r"\bN\.V\.?\b", r"\bB\.V\.?\b",
        r"\bSTORES\b",  r"\bWHSE\b",   r"\bWAREHOUSE\b",
        r"\bGROUP\b",   r"\bHOLDINGS?\b", r"\bENTERPRISES?\b",
        r"\bINTERNATIONAL\b", r"\bINTL\.?\b",
        r"\bNORTH\s+AMERICA\b", r"\bUSA\b", r"\bUS\b",
        r"\bLIMITED\b",
    ])
    + r")",
)

# Full URLs, subdomain.domain.tld, and bare .com/.net/.io suffixes
_RE_URL = re.compile(
    r"(?:https?://|www\.)\S+"
    r"|\b\w+(?:\.\w+)+\.(?:com|org|net|io|co|app|gov|edu)(?:/\S*)?"
    r"|(?<=\w)\.(?:com|org|net|io|co|app|gov|edu)(?:/\S*)?",
    re.IGNORECASE,
)

# NA phone numbers and raw 10-12 digit runs (account / terminal numbers)
_RE_PHONE = re.compile(
    r"\b(?:\+?1[-.\s]?)?(?:\(?\d{3}\)?[-.\s]?)\d{3}[-.\s]?\d{4}\b"
    r"|\b\d{10,12}\b",
)

# Location / terminal IDs: #12345, store codes (00210-08), 5-9 standalone digits
_RE_TERMINAL = re.compile(
    r"#\s*\d+"
    r"|\b\d{3,6}-\d{2,6}\b"
    r"|\b\d{5,9}\b",
)

# Random alphanumeric reference codes: ≥6 chars, must contain both letters and digits
_RE_ALPHANUM_CODE = re.compile(
    r"\b(?=[A-Z0-9]*[0-9])(?=[A-Z0-9]*[A-Z])[A-Z0-9]{6,}\b"
)

_RE_WHITESPACE   = re.compile(r"\s{2,}")
_RE_TRAIL_PUNCT  = re.compile(r"[*,.\-/\\|]+$")
_RE_LEAD_PUNCT   = re.compile(r"^[*,.\-/\\|]+")

# ---------------------------------------------------------------------------
# Public API
# ---------------------------------------------------------------------------

def normalise(raw: str) -> str:
    """Return a clean, human-readable merchant name suitable for embedding.

    Parameters
    ----------
    raw:
        Raw merchant string as it appears on a bank statement or payment
        terminal receipt, e.g. ``"SQ *JOES COFFEE #042 4029357733"``.

    Returns
    -------
    str
        Normalised merchant name in Title Case, e.g. ``"Joe's Coffee"``.
        Falls back to ``raw.strip().title()`` if nothing meaningful remains
        after stripping, so callers always receive a non-empty string when
        the input itself is non-empty.
    """
    s = _normalise_unicode(raw)
    s = _strip_processor_prefixes(s)   # 1. expose real brand name first
    s = _RE_URL.sub(" ", s)            # 2. remove URLs / domain suffixes
    s = _RE_PHONE.sub(" ", s)          # 3. remove phone / account numbers
    s = _RE_TERMINAL.sub(" ", s)       # 4. remove terminal / location IDs
    s = _RE_ALPHANUM_CODE.sub(" ", s)  # 5. remove random reference codes
    s = _RE_LEGAL.sub(" ", s)          # 6. remove legal-entity suffixes
    s = _RE_LEAD_PUNCT.sub("", s)      # 7. tidy leading punctuation
    s = _RE_TRAIL_PUNCT.sub("", s)     #    tidy trailing punctuation
    s = _RE_WHITESPACE.sub(" ", s)     #    collapse multiple spaces
    s = s.strip(" *-.,/\\|")
    return s.title() if s else raw.strip().title()


# ---------------------------------------------------------------------------
# Private helpers
# ---------------------------------------------------------------------------

def _normalise_unicode(text: str) -> str:
    """NFC-normalise and strip non-printable control characters."""
    text = unicodedata.normalize("NFC", text)
    return "".join(ch for ch in text if unicodedata.category(ch)[0] != "C")


def _strip_processor_prefixes(s: str) -> str:
    """Strip known payment-processor prefixes and resolve asterisk separators."""
    upper = s.upper()
    for prefix in _PROCESSOR_PREFIXES:
        if upper.startswith(prefix):
            s = s[len(prefix):]
            upper = s.upper()

    # Resolve remaining asterisk separators:
    #   "BRAND.COM*RANDOMCODE"  → keep before (brand)
    #   "BRAND* REAL NAME"      → keep after (merchant)
    if "*" in s:
        before, after = s.split("*", maxsplit=1)
        before, after = before.strip(), after.strip()
        after_is_code = bool(_RE_ALPHANUM_CODE.fullmatch(after.replace(" ", "")))
        if after_is_code or (len(before.split()) >= len(after.split()) and len(before) >= 3):
            s = before
        elif len(after) >= 3:
            s = after

    return s.strip()