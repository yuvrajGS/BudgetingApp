import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { listUsers } from "../api/users";
import { listCategories } from "../api/categories";
import PageHeader from "../components/PageHeader";
import { Loading, ErrorBlock } from "../components/Status";

function StatCard({ label, value, to, accent }) {
  return (
    <Link
      to={to}
      className="block rounded-sm border border-line bg-white p-5 hover:shadow-sm transition-shadow"
    >
      <div className="flex items-center justify-between">
        <p className="text-xs font-medium uppercase tracking-wide text-muted">
          {label}
        </p>
        <span
          className="h-2 w-2 rounded-full"
          style={{ backgroundColor: accent }}
          aria-hidden="true"
        />
      </div>
      <p className="mt-3 font-display text-4xl font-semibold text-ink">
        {value}
      </p>
    </Link>
  );
}

export default function Dashboard() {
  const [state, setState] = useState({ loading: true, error: null, users: [], categories: [] });

  const load = () => {
    setState((s) => ({ ...s, loading: true, error: null }));
    Promise.all([listUsers(), listCategories()])
      .then(([users, categories]) =>
        setState({ loading: false, error: null, users: users ?? [], categories: categories ?? [] })
      )
      .catch((err) => setState((s) => ({ ...s, loading: false, error: err.message })));
  };

  useEffect(load, []);

  return (
    <div>
      <PageHeader eyebrow="Overview" title="Budget Dashboard" />

      {state.loading && <Loading label="Pulling the latest figures…" />}
      {state.error && <ErrorBlock message={state.error} onRetry={load} />}

      {!state.loading && !state.error && (
        <>
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
            <StatCard
              label="Users"
              value={state.users.length}
              to="/users"
              accent="var(--color-ink-soft, #3a473f)"
            />
            <StatCard
              label="Categories"
              value={state.categories.length}
              to="/categories"
              accent="var(--color-rust)"
            />
            <StatCard
              label="Transactions"
              value="View"
              to="/transactions"
              accent="var(--color-ledger)"
            />
          </div>

          <div className="mt-10 rounded-sm border border-line bg-white p-6">
            <p className="font-display text-lg text-ink">Where to start</p>
            <ul className="mt-3 space-y-2 text-sm text-ink-soft">
              <li>
                • Add a <Link to="/users/new" className="underline decoration-line underline-offset-2 hover:decoration-ink">user</Link> before recording their spending.
              </li>
              <li>
                • Set up <Link to="/categories/new" className="underline decoration-line underline-offset-2 hover:decoration-ink">categories</Link> to group transactions by merchant keywords.
              </li>
              <li>
                • Log a single purchase or import several at once from <Link to="/transactions/new" className="underline decoration-line underline-offset-2 hover:decoration-ink">Transactions</Link>.
              </li>
            </ul>
          </div>
        </>
      )}
    </div>
  );
}
