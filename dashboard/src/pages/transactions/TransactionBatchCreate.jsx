import { useEffect, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { listUsers } from "../../api/users";
import { listCategories } from "../../api/categories";
import { createTransactionBatch } from "../../api/transactions";
import PageHeader from "../../components/PageHeader";
import { Field, TextInput, Select, Button } from "../../components/Form";
import { ErrorBlock } from "../../components/Status";

const today = () => new Date().toISOString().slice(0, 10);

const blankRow = (userId = "") => ({
  key: crypto.randomUUID(),
  userId,
  date: today(),
  merchant: "",
  amount: "",
  description: "",
  categoryId: "",
});

export default function TransactionBatchCreate() {
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loadingOptions, setLoadingOptions] = useState(true);
  const [rows, setRows] = useState([blankRow()]);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    Promise.all([listUsers(), listCategories()])
      .then(([u, c]) => {
        setUsers(u ?? []);
        setCategories(c ?? []);
        setRows([blankRow(u?.[0]?.id ?? "")]);
        setLoadingOptions(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoadingOptions(false);
      });
  }, []);

  const updateRow = (key, field) => (e) =>
    setRows((rs) => rs.map((r) => (r.key === key ? { ...r, [field]: e.target.value } : r)));

  const addRow = () => setRows((rs) => [...rs, blankRow(rs[rs.length - 1]?.userId ?? users[0]?.id ?? "")]);
  const removeRow = (key) => setRows((rs) => (rs.length > 1 ? rs.filter((r) => r.key !== key) : rs));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const payload = rows.map((r) => ({
        userId: r.userId,
        date: new Date(r.date).toISOString(),
        merchant: r.merchant,
        amount: Number(r.amount),
        description: r.description,
        categoryId: r.categoryId ? Number(r.categoryId) : null,
      }));
      await createTransactionBatch(payload);
      navigate(rows[0]?.userId ? `/transactions?userId=${rows[0].userId}` : "/transactions");
    } catch (err) {
      setError(err.message);
      setSubmitting(false);
    }
  };

  if (!loadingOptions && users.length === 0) {
    return (
      <div className="max-w-lg">
        <PageHeader eyebrow="Activity" title="Batch add transactions" />
        <ErrorBlock message="You need at least one user before logging transactions." />
        <div className="mt-4">
          <Link to="/users/new">
            <Button>Add a user</Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div>
      <PageHeader eyebrow="Activity" title="Batch add transactions" />

      <p className="mb-5 text-sm text-muted">
        Leave category blank to let the backend's ML categorizer assign one automatically.
      </p>

      <form onSubmit={handleSubmit} className="space-y-4">
        {error && <ErrorBlock message={error} />}

        <div className="overflow-x-auto rounded-sm border border-line bg-white">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="rule text-xs uppercase tracking-wide text-muted">
                <th className="px-3 py-3 font-medium">User</th>
                <th className="px-3 py-3 font-medium">Date</th>
                <th className="px-3 py-3 font-medium">Merchant</th>
                <th className="px-3 py-3 font-medium">Amount</th>
                <th className="px-3 py-3 font-medium">Description</th>
                <th className="px-3 py-3 font-medium">Category (optional)</th>
                <th className="px-3 py-3" />
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => (
                <tr key={row.key} className="rule last:border-b-0 align-top">
                  <td className="px-3 py-2 min-w-[140px]">
                    <Select required value={row.userId} onChange={updateRow(row.key, "userId")}>
                      {users.map((u) => (
                        <option key={u.id} value={u.id}>
                          {u.name}
                        </option>
                      ))}
                    </Select>
                  </td>
                  <td className="px-3 py-2 min-w-[140px]">
                    <TextInput
                      type="date"
                      required
                      value={row.date}
                      onChange={updateRow(row.key, "date")}
                    />
                  </td>
                  <td className="px-3 py-2 min-w-[140px]">
                    <TextInput
                      required
                      value={row.merchant}
                      onChange={updateRow(row.key, "merchant")}
                      placeholder="Target"
                    />
                  </td>
                  <td className="px-3 py-2 min-w-[110px]">
                    <TextInput
                      type="number"
                      step="0.01"
                      required
                      value={row.amount}
                      onChange={updateRow(row.key, "amount")}
                      placeholder="42.50"
                    />
                  </td>
                  <td className="px-3 py-2 min-w-[160px]">
                    <TextInput
                      value={row.description}
                      onChange={updateRow(row.key, "description")}
                      placeholder="Household items"
                    />
                  </td>
                  <td className="px-3 py-2 min-w-[140px]">
                    <Select value={row.categoryId} onChange={updateRow(row.key, "categoryId")}>
                      <option value="">Let ML categorize it</option>
                      {categories.map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.name}
                        </option>
                      ))}
                    </Select>
                  </td>
                  <td className="px-3 py-2">
                    <button
                      type="button"
                      onClick={() => removeRow(row.key)}
                      disabled={rows.length === 1}
                      className="text-xs text-muted hover:text-rust disabled:opacity-30"
                    >
                      Remove
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="flex items-center justify-between">
          <Button type="button" variant="ghost" onClick={addRow}>
            + Add row
          </Button>
          <Button type="submit" disabled={submitting || loadingOptions}>
            {submitting ? "Saving…" : `Save ${rows.length} transaction${rows.length === 1 ? "" : "s"}`}
          </Button>
        </div>
      </form>
    </div>
  );
}
