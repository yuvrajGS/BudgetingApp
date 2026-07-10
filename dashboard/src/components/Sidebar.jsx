import { NavLink } from "react-router-dom";

const NAV_ITEMS = [
  { to: "/", label: "Overview", end: true },
  { to: "/transactions", label: "Transactions", accent: "ledger" },
  { to: "/categories", label: "Categories", accent: "rust" },
  { to: "/users", label: "Users", accent: "ink-soft" },
];

const accentColor = {
  ledger: "var(--color-ledger)",
  rust: "var(--color-rust)",
  "ink-soft": "var(--color-ink-soft)",
};

export default function Sidebar() {
  return (
    <aside className="w-56 shrink-0 border-r border-line bg-paper-dim/60 flex flex-col">
      <div className="px-6 pt-8 pb-6">
        <p className="font-display text-2xl font-semibold tracking-tight text-ink">
          Ledger
        </p>
        <p className="mt-1 text-xs text-muted">Budget Dashboard</p>
      </div>

      <nav className="flex-1 px-3">
        <ul className="space-y-1">
          {NAV_ITEMS.map((item) => (
            <li key={item.to}>
              <NavLink
                to={item.to}
                end={item.end}
                className={({ isActive }) =>
                  [
                    "relative flex items-center gap-3 rounded-r-sm py-2.5 pl-4 pr-3 text-sm transition-colors",
                    isActive
                      ? "bg-white text-ink font-medium shadow-sm"
                      : "text-ink-soft hover:bg-white/60 hover:text-ink",
                  ].join(" ")
                }
              >
                {({ isActive }) => (
                  <>
                    <span
                      aria-hidden="true"
                      className={`absolute -left-3 h-6 w-3 rounded-l-sm transition-opacity ${
                        isActive ? "opacity-100" : "opacity-0"
                      }`}
                      style={{
                        backgroundColor:
                          accentColor[item.accent] ?? "var(--color-muted)",
                      }}
                    />
                    {item.accent && (
                      <span
                        aria-hidden="true"
                        className="h-1.5 w-1.5 rounded-full"
                        style={{ backgroundColor: accentColor[item.accent] }}
                      />
                    )}
                    {item.label}
                  </>
                )}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>

      <div className="px-6 py-5 border-t border-line">
        <p className="text-[11px] leading-relaxed text-muted">
          Connected via <span className="font-mono">/api</span> dev proxy to{" "}
          <span className="font-mono">localhost:7103</span>
        </p>
      </div>
    </aside>
  );
}
