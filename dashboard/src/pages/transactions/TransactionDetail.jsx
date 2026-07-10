import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getTransaction } from "../../api/transactions";
import { getUser } from "../../api/users";
import { listCategories } from "../../api/categories";
import PageHeader from "../../components/PageHeader";
import { Loading, ErrorBlock, Empty } from "../../components/Status";
import Amount from "../../components/Amount";

export default function TransactionDetail() {
  const { id } = useParams();
  const [state, setState] = useState({ loading: true, error: null, transaction: null });
  const [user, setUser] = useState(null);
  const [category, setCategory] = useState(null);

  useEffect(() => {
    setState({ loading: true, error: null, transaction: null });
    setUser(null);
    setCategory(null);

    getTransaction(id)
      .then((transaction) => {
        setState({ loading: false, error: null, transaction });
        if (transaction?.userId) {
          getUser(transaction.userId).then(setUser).catch(() => setUser(null));
        }
        if (transaction?.categoryId != null) {
          // The API only looks categories up by name, so pull the list and
          // match the id client-side.
          listCategories()
            .then((categories) =>
              setCategory((categories ?? []).find((c) => c.id === transaction.categoryId) ?? null)
            )
            .catch(() => setCategory(null));
        }
      })
      .catch((err) => setState({ loading: false, error: err.message, transaction: null }));
  }, [id]);

  if (state.loading) return <Loading label="Loading transaction…" />;
  if (state.error) return <ErrorBlock message={state.error} />;
  if (!state.transaction) return <Empty title="Transaction not found" />;

  const { transaction } = state;

  return (
    <div className="max-w-2xl">
      <PageHeader eyebrow="Transaction" title={transaction.merchant} />

      <div className="rounded-sm border border-line bg-white p-6">
        <div className="flex items-baseline justify-between rule pb-5 mb-5">
          <div>
            <p className="text-xs uppercase tracking-wide text-muted">Amount</p>
            <Amount value={transaction.amount} className="text-2xl" />
          </div>
          <p className="font-mono text-xs text-muted">
            {new Date(transaction.date).toLocaleDateString(undefined, {
              year: "numeric",
              month: "long",
              day: "numeric",
            })}
          </p>
        </div>

        <dl className="grid grid-cols-1 gap-5 sm:grid-cols-2">
          <div>
            <dt className="text-xs uppercase tracking-wide text-muted mb-1">User</dt>
            <dd className="text-sm text-ink">
              {user ? (
                <Link to={`/users/${user.id}`} className="hover:underline">
                  {user.name}
                </Link>
              ) : (
                <span className="font-mono text-xs text-muted">{transaction.userId}</span>
              )}
            </dd>
          </div>

          <div>
            <dt className="text-xs uppercase tracking-wide text-muted mb-1">Category</dt>
            <dd className="text-sm text-ink">
              {category ? (
                <Link
                  to={`/categories/${encodeURIComponent(category.name)}`}
                  className="hover:underline"
                >
                  {category.name}
                </Link>
              ) : (
                <span className="text-muted">
                  {transaction.categoryId != null ? `#${transaction.categoryId}` : "Uncategorized"}
                </span>
              )}
            </dd>
          </div>

          <div className="sm:col-span-2">
            <dt className="text-xs uppercase tracking-wide text-muted mb-1">Description</dt>
            <dd className="text-sm text-ink">{transaction.description || "—"}</dd>
          </div>

          <div className="sm:col-span-2">
            <dt className="text-xs uppercase tracking-wide text-muted mb-1">Transaction ID</dt>
            <dd className="font-mono text-xs text-muted truncate" title={transaction.id}>
              {transaction.id}
            </dd>
          </div>
        </dl>
      </div>
    </div>
  );
}
