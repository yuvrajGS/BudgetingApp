const formatter = new Intl.NumberFormat("en-CA", {
  style: "currency",
  currency: "CAD",
});

// Negative amounts read as ledger-green (money in / neutral spend),
// positive amounts read as rust (refunds or corrections). Since most
// transaction amounts represent spend, sign is what carries meaning here.
export default function Amount({ value, className = "" }) {
  const numeric = Number(value ?? 0);
  const isNegative = numeric < 0;
  return (
    <span
      className={`font-mono tabular text-sm ${className}`}
      style={{ color: isNegative ? "var(--color-ledger)" : "var(--color-rust)" }}
    >
      {isNegative ? "+" : "-"}
      {formatter.format(Math.abs(numeric))}
    </span>
  );
}
