export function Field({ label, hint, children, htmlFor }) {
  return (
    <label htmlFor={htmlFor} className="block">
      <span className="block text-xs font-medium uppercase tracking-wide text-muted mb-1.5">
        {label}
      </span>
      {children}
      {hint && <span className="mt-1 block text-xs text-muted">{hint}</span>}
    </label>
  );
}

const baseInput =
  "w-full rounded-sm border border-line bg-white px-3 py-2 text-sm text-ink placeholder:text-muted/70 focus:outline-none focus:ring-2 focus:ring-offset-0";

export function TextInput(props) {
  return (
    <input
      {...props}
      className={`${baseInput} ${props.className ?? ""}`}
      style={{ "--tw-ring-color": "var(--color-ledger)", ...props.style }}
    />
  );
}

export function TextArea(props) {
  return (
    <textarea
      {...props}
      className={`${baseInput} ${props.className ?? ""}`}
      style={{ "--tw-ring-color": "var(--color-ledger)", ...props.style }}
    />
  );
}

export function Select(props) {
  return (
    <select
      {...props}
      className={`${baseInput} ${props.className ?? ""}`}
      style={{ "--tw-ring-color": "var(--color-ledger)", ...props.style }}
    />
  );
}

export function Button({ variant = "primary", className = "", ...props }) {
  const variants = {
    primary: "text-white",
    ghost: "border border-line text-ink hover:bg-paper-dim",
    delete: "border border-line text-red-600 hover:bg-paper-dim",
  };
  const style =
    variant === "primary" ? { backgroundColor: "var(--color-ink)" } : undefined;

  return (
    <button
      {...props}
      style={style}
      className={`inline-flex items-center gap-2 rounded-sm px-4 py-2 text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${variants[variant]} ${className}`}
    />
  );
}
