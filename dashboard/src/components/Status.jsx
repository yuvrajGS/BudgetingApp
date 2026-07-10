export function Loading({ label = "Loading…" }) {
  return (
    <div className="flex items-center gap-2 py-10 text-sm text-muted">
      <span
        className="h-3 w-3 animate-pulse rounded-full"
        style={{ backgroundColor: "var(--color-ledger)" }}
        aria-hidden="true"
      />
      {label}
    </div>
  );
}

export function ErrorBlock({ message, onRetry }) {
  return (
    <div
      className="flex items-start justify-between gap-4 rounded-sm border px-4 py-3 text-sm"
      style={{
        borderColor: "var(--color-rust)",
        backgroundColor: "var(--color-rust-soft)",
        color: "var(--color-rust)",
      }}
      role="alert"
    >
      <div>
        <p className="font-medium">Couldn't load this.</p>
        <p className="mt-0.5 text-[13px] opacity-90">{message}</p>
      </div>
      {onRetry && (
        <button
          type="button"
          onClick={onRetry}
          className="shrink-0 rounded-sm border border-current px-3 py-1 text-xs font-medium hover:opacity-80"
        >
          Retry
        </button>
      )}
    </div>
  );
}

export function Empty({ title, hint, action }) {
  return (
    <div className="rounded-sm border border-dashed border-line px-6 py-12 text-center">
      <p className="font-display text-lg text-ink">{title}</p>
      {hint && <p className="mt-1 text-sm text-muted">{hint}</p>}
      {action && <div className="mt-4 flex justify-center">{action}</div>}
    </div>
  );
}
