import { useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { listUsers } from "../../api/users";
import { listCategories } from "../../api/categories";
import { listTransactionsForUser } from "../../api/transactions";
import PageHeader from "../../components/PageHeader";
import { Select, Button } from "../../components/Form";
import { Loading, ErrorBlock, Empty } from "../../components/Status";
import Amount from "../../components/Amount";
import AnalysisGroups from "../../components/AnalysisGroups";
import { groupByMonth, groupByYear } from "../../utils/groupTransactions";
import { formatDateOnly } from "../../utils/date";

const VIEWS = [
  { key: "all", label: "All" },
  { key: "month", label: "By month" },
  { key: "year", label: "By year" },
];

export default function TransactionsList() {
  const [searchParams, setSearchParams] = useSearchParams();
  const userId = searchParams.get("userId") ?? "";
  const view = searchParams.get("view") ?? "month";

  const [users, setUsers] = useState({ loading: true, error: null, list: [] });
  const [categories, setCategories] = useState([]);
  const [tx, setTx] = useState({ loading: false, error: null, list: [] });

  useEffect(() => {
    listUsers()
      .then((list) => {
        setUsers({ loading: false, error: null, list: list ?? [] });
        // Default to the first user so the page isn't empty on first load.
        if (!userId && list?.length) {
          setSearchParams({ userId: list[0].id, view }, { replace: true });
        }
      })
      .catch((err) => setUsers({ loading: false, error: err.message, list: [] }));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    listCategories()
      .then((list) => setCategories(list ?? []))
      .catch(() => setCategories([]));
  }, []);

  useEffect(() => {
    if (!userId) return;
    setTx({ loading: true, error: null, list: [] });
    listTransactionsForUser(userId)
      .then((list) => setTx({ loading: false, error: null, list: list ?? [] }))
      .catch((err) => setTx({ loading: false, error: err.message, list: [] }));
  }, [userId]);

  const categoryMap = useMemo(() => new Map(categories.map((c) => [c.id, c.name])), [categories]);

  const sortedList = useMemo(
    () => [...tx.list].sort((a, b) => b.date.localeCompare(a.date)),
    [tx.list]
  );
  const monthGroups = useMemo(() => groupByMonth(tx.list, categoryMap), [tx.list, categoryMap]);
  const yearGroups = useMemo(() => groupByYear(tx.list, categoryMap), [tx.list, categoryMap]);

  const selectedUser = users.list.find((u) => u.id === userId);

  const setView = (nextView) => setSearchParams({ userId, view: nextView });

  return (
    <div>
      <PageHeader
        eyebrow="Activity"
        title="Transactions"
        action={
          <div className="flex gap-2">
            <Link to="/transactions/import">
              <Button variant="ghost">Import statement</Button>
            </Link>
            <Link to="/transactions/batch">
              <Button variant="ghost">Batch add</Button>
            </Link>
            <Link to={userId ? `/transactions/new?userId=${userId}` : "/transactions/new"}>
              <Button>New transaction</Button>
            </Link>
          </div>
        }
      />

      <div className="mb-6 flex flex-wrap items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <label htmlFor="user-filter" className="text-xs font-medium uppercase tracking-wide text-muted">
            User
          </label>
          {users.loading ? (
            <span className="text-sm text-muted">Loading users…</span>
          ) : users.list.length === 0 ? (
            <span className="text-sm text-muted">
              No users yet —{" "}
              <Link to="/users/new" className="underline">
                add one first
              </Link>
              .
            </span>
          ) : (
            <Select
              id="user-filter"
              value={userId}
              onChange={(e) => setSearchParams({ userId: e.target.value, view })}
              className="max-w-xs"
            >
              {users.list.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.name}
                </option>
              ))}
            </Select>
          )}
        </div>

        {userId && (
          <div className="inline-flex rounded-sm border border-line bg-white p-0.5">
            {VIEWS.map((v) => (
              <button
                key={v.key}
                type="button"
                onClick={() => setView(v.key)}
                className={`rounded-sm px-3 py-1.5 text-xs font-medium transition-colors ${
                  view === v.key ? "bg-ink text-white" : "text-ink-soft hover:bg-paper-dim"
                }`}
              >
                {v.label}
              </button>
            ))}
          </div>
        )}
      </div>

      {users.error && <ErrorBlock message={users.error} />}

      {userId && (
        <>
          {tx.loading && <Loading label="Loading transactions…" />}
          {tx.error && <ErrorBlock message={tx.error} />}

          {!tx.loading && !tx.error && tx.list.length === 0 && (
            <Empty
              title="No transactions"
              hint={`${selectedUser?.name ?? "This user"} hasn't logged any spending yet.`}
              action={
                <Link to={`/transactions/new?userId=${userId}`}>
                  <Button>New transaction</Button>
                </Link>
              }
            />
          )}

          {!tx.loading && !tx.error && tx.list.length > 0 && view === "all" && (
            <div className="overflow-hidden rounded-sm border border-line bg-white">
              <table className="w-full text-left text-sm">
                <thead>
                  <tr className="rule text-xs uppercase tracking-wide text-muted">
                    <th className="px-4 py-3 font-medium">Date</th>
                    <th className="px-4 py-3 font-medium">Merchant</th>
                    <th className="px-4 py-3 font-medium">Category</th>
                    <th className="px-4 py-3 font-medium">Description</th>
                    <th className="px-4 py-3 font-medium text-right">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  {sortedList.map((t) => (
                    <tr key={t.id} className="rule last:border-b-0 hover:bg-paper-dim/50">
                      <td className="px-4 py-3 font-mono text-xs text-muted whitespace-nowrap">
                        {formatDateOnly(t.date)}
                      </td>
                      <td className="px-4 py-3">
                        <Link to={`/transactions/${t.id}`} className="font-medium text-ink hover:underline">
                          {t.merchant}
                        </Link>
                      </td>
                      <td className="px-4 py-3 text-ink-soft">
                        {categoryMap.get(t.categoryId) ?? "Uncategorized"}
                      </td>
                      <td className="px-4 py-3 text-ink-soft">{t.description}</td>
                      <td className="px-4 py-3 text-right">
                        <Amount value={t.amount} />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {!tx.loading && !tx.error && tx.list.length > 0 && view === "month" && (
            <AnalysisGroups groups={monthGroups} />
          )}

          {!tx.loading && !tx.error && tx.list.length > 0 && view === "year" && (
            <AnalysisGroups groups={yearGroups} />
          )}
        </>
      )}
    </div>
  );
}
