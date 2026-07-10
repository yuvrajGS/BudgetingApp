import { useState } from "react";
import { Link } from "react-router-dom";
import Amount from "./Amount";

// Renders a list of grouped buckets (from groupByMonth/groupByYear): a
// header with the period total, a category breakdown so you can see what
// drove spend, and an expandable list of the underlying transactions.
export default function AnalysisGroups({ groups }) {
  const [openKeys, setOpenKeys] = useState(() => new Set([groups[0]?.key]));

  const toggle = (key) =>
    setOpenKeys((prev) => {
      const next = new Set(prev);
      next.has(key) ? next.delete(key) : next.add(key);
      return next;
    });

  return (
    <div className="space-y-4">
      {groups.map((group) => {
        const isOpen = openKeys.has(group.key);
        const maxCategoryTotal = group.categories[0]?.total || 1;

        return (
          <div key={group.key} className="rounded-sm border border-line bg-white overflow-hidden">
            <button
              type="button"
              onClick={() => toggle(group.key)}
              className="w-full flex items-center justify-between gap-4 px-5 py-4 text-left hover:bg-paper-dim/40"
            >
              <div className="flex items-center gap-3">
                <span
                  aria-hidden="true"
                  className={`text-muted transition-transform ${isOpen ? "rotate-90" : ""}`}
                >
                  ›
                </span>
                <div>
                  <p className="font-display text-lg text-ink">{group.label}</p>
                  <p className="text-xs text-muted">
                    {group.count} transaction{group.count === 1 ? "" : "s"}
                  </p>
                </div>
              </div>
              <Amount value={group.total} className="text-lg" />
            </button>

            {isOpen && (
              <div className="border-t border-line px-5 py-4 space-y-5">
                {group.categories.length > 0 && (
                  <div>
                    <p className="text-xs font-medium uppercase tracking-wide text-muted mb-2">
                      By category
                    </p>
                    <div className="space-y-2">
                      {group.categories.map((c) => (
                        <div key={c.categoryId ?? "uncategorized"} className="flex items-center gap-3">
                          <span className="w-32 shrink-0 truncate text-sm text-ink-soft">
                            {c.name}
                          </span>
                          <div className="flex-1 h-2 rounded-full bg-paper-dim overflow-hidden">
                            <div
                              className="h-full rounded-full"
                              style={{
                                width: `${Math.max(4, (c.total / maxCategoryTotal) * 100)}%`,
                                backgroundColor:
                                  c.categoryId == null ? "var(--color-muted)" : "var(--color-rust)",
                              }}
                            />
                          </div>
                          <Amount value={c.total} className="w-20 shrink-0 text-right text-xs" />
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                <div>
                  <p className="text-xs font-medium uppercase tracking-wide text-muted mb-2">
                    Transactions
                  </p>
                  <table className="w-full text-left text-sm">
                    <tbody>
                      {group.transactions
                        .slice()
                        .sort((a, b) => new Date(b.date) - new Date(a.date))
                        .map((t) => (
                          <tr key={t.id} className="rule last:border-b-0">
                            <td className="py-2 pr-3 font-mono text-xs text-muted whitespace-nowrap">
                              {new Date(t.date).toLocaleDateString()}
                            </td>
                            <td className="py-2 pr-3">
                              <Link
                                to={`/transactions/${t.id}`}
                                className="font-medium text-ink hover:underline"
                              >
                                {t.merchant}
                              </Link>
                            </td>
                            <td className="py-2 pr-3 text-ink-soft">{t.description}</td>
                            <td className="py-2 text-right">
                              <Amount value={t.amount} />
                            </td>
                          </tr>
                        ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}
