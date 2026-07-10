import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getUser, deleteUser } from "../../api/users";
import { listTransactionsForUser } from "../../api/transactions";
import PageHeader from "../../components/PageHeader";
import { Button } from "../../components/Form";
import { Loading, ErrorBlock, Empty } from "../../components/Status";
import Amount from "../../components/Amount";
import { useNavigate } from "react-router-dom";

export default function UserDetail() {
  const navigate = useNavigate();
  const { id } = useParams();
  const [state, setState] = useState({ loading: true, error: null, user: null });
  const [txState, setTxState] = useState({ loading: true, error: null, transactions: [] });

  const [showConfirmDelete, setShowConfirmDelete] = useState(false);
  const [deleteError, setDeleteError] = useState(null);

  useEffect(() => {
    setState({ loading: true, error: null, user: null });
    getUser(id)
      .then((user) => setState({ loading: false, error: null, user }))
      .catch((err) => setState({ loading: false, error: err.message, user: null }));
  }, [id]);

  useEffect(() => {
    setTxState({ loading: true, error: null, transactions: [] });
    listTransactionsForUser(id)
      .then((transactions) =>
        setTxState({ loading: false, error: null, transactions: transactions ?? [] })
      )
      .catch((err) => setTxState({ loading: false, error: err.message, transactions: [] }));
  }, [id]);

  const handleDelete = () => {
    deleteUser(id)
      .then(() => {
        navigate("/users");
      })
      .catch((err) => {
        setDeleteError(err.message);
        setShowConfirmDelete(false);
      });
  }

  if (state.loading) return <Loading label="Loading user…" />;
  if (state.error) return <ErrorBlock message={state.error} />;
  if (!state.user) return <Empty title="User not found" />;

  const { user } = state;

  return (
    <div>
      <PageHeader
        eyebrow="User"
        title={user.name}
        action={
          <div className="flex gap-2">
            <Link to={`/transactions/import?userId=${user.id}`}>
              <Button variant="ghost">Import statement</Button>
            </Link>
            <Link to={`/transactions/new?userId=${user.id}`}>
              <Button>Log transaction</Button>
            </Link>
            <Button variant="delete" onClick={() => setShowConfirmDelete(true)}>
              Delete User
            </Button>
          </div>
        }
      />

      {showConfirmDelete && (
        <div className="mb-4 p-4 bg-rust/10 border border-rust/20 rounded-sm">
          <p className="text-sm text-rust">
            Are you sure you want to delete this user? This action cannot be undone.
          </p>
          <div className="mt-2 flex gap-2">
            <Button
              variant="delete"
              onClick={handleDelete}
            >
              Yes, delete user
            </Button>
            <Button
              variant="ghost"
              onClick={() => {
                setShowConfirmDelete(false);
              }}
            >
              Cancel
            </Button>
          </div>
        </div>
      )}

      {deleteError && <ErrorBlock message={deleteError} />}

      <dl className="grid grid-cols-2 gap-4 sm:grid-cols-3 mb-10">
        <div className="rounded-sm border border-line bg-white p-4">
          <dt className="text-xs uppercase tracking-wide text-muted">Email</dt>
          <dd className="mt-1 text-sm text-ink">{user.email}</dd>
        </div>
        <div className="rounded-sm border border-line bg-white p-4">
          <dt className="text-xs uppercase tracking-wide text-muted">Joined</dt>
          <dd className="mt-1 text-sm text-ink">
            {user.createdAt ? new Date(user.createdAt).toLocaleDateString() : "—"}
          </dd>
        </div>
        <div className="rounded-sm border border-line bg-white p-4">
          <dt className="text-xs uppercase tracking-wide text-muted">User ID</dt>
          <dd className="mt-1 truncate font-mono text-xs text-muted" title={user.id}>
            {user.id}
          </dd>
        </div>
      </dl>

      <p className="font-display text-lg text-ink mb-3">Transactions</p>

      {txState.loading && <Loading label="Loading transactions…" />}
      {txState.error && <ErrorBlock message={txState.error} />}

      {!txState.loading && !txState.error && txState.transactions.length === 0 && (
        <Empty
          title="No transactions yet"
          hint={`${user.name} hasn't logged any spending.`}
          action={
            <Link to={`/transactions/new?userId=${user.id}`}>
              <Button>Log transaction</Button>
            </Link>
          }
        />
      )}

      {!txState.loading && !txState.error && txState.transactions.length > 0 && (
        <div className="overflow-hidden rounded-sm border border-line bg-white">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="rule text-xs uppercase tracking-wide text-muted">
                <th className="px-4 py-3 font-medium">Date</th>
                <th className="px-4 py-3 font-medium">Merchant</th>
                <th className="px-4 py-3 font-medium">Description</th>
                <th className="px-4 py-3 font-medium text-right">Amount</th>
              </tr>
            </thead>
            <tbody>
              {txState.transactions.map((tx) => (
                <tr key={tx.id} className="rule last:border-b-0 hover:bg-paper-dim/50">
                  <td className="px-4 py-3 font-mono text-xs text-muted">
                    {new Date(tx.date).toLocaleDateString()}
                  </td>
                  <td className="px-4 py-3">
                    <Link to={`/transactions/${tx.id}`} className="font-medium text-ink hover:underline">
                      {tx.merchant}
                    </Link>
                  </td>
                  <td className="px-4 py-3 text-ink-soft">{tx.description}</td>
                  <td className="px-4 py-3 text-right">
                    <Amount value={tx.amount} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
