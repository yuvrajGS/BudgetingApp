import { useEffect, useState } from "react";
import { useNavigate, useSearchParams, Link } from "react-router-dom";
import { listUsers } from "../../api/users";
import { listCategories } from "../../api/categories";
import { createTransaction } from "../../api/transactions";
import PageHeader from "../../components/PageHeader";
import { Field, TextInput, TextArea, Select, Button } from "../../components/Form";
import { ErrorBlock } from "../../components/Status";

const today = () => new Date().toISOString().slice(0, 10);

export default function TransactionCreate() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const presetUserId = searchParams.get("userId") ?? "";

  const [users, setUsers] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loadingOptions, setLoadingOptions] = useState(true);
  const [optionsError, setOptionsError] = useState(null);

  const [form, setForm] = useState({
    userId: presetUserId,
    date: today(),
    merchant: "",
    amount: "",
    description: "",
    categoryId: "",
  });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    Promise.all([listUsers(), listCategories()])
      .then(([u, c]) => {
        setUsers(u ?? []);
        setCategories(c ?? []);
        setForm((f) => ({ ...f, userId: f.userId || u?.[0]?.id || "" }));
        setLoadingOptions(false);
      })
      .catch((err) => {
        setOptionsError(err.message);
        setLoadingOptions(false);
      });
  }, []);

  const update = (field) => (e) => setForm((f) => ({ ...f, [field]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const transaction = await createTransaction({
        userId: form.userId,
        date: new Date(form.date).toISOString(),
        merchant: form.merchant,
        amount: Number(form.amount),
        description: form.description,
        categoryId: form.categoryId ? Number(form.categoryId) : null,
      });
      navigate(transaction?.id ? `/transactions/${transaction.id}` : `/transactions?userId=${form.userId}`);
    } catch (err) {
      setError(err.message);
      setSubmitting(false);
    }
  };

  if (!loadingOptions && users.length === 0) {
    return (
      <div className="max-w-lg">
        <PageHeader eyebrow="Activity" title="New transaction" />
        <ErrorBlock message="You need at least one user before logging a transaction." />
        <div className="mt-4">
          <Link to="/users/new">
            <Button>Add a user</Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-lg">
      <PageHeader eyebrow="Activity" title="New transaction" />

      <form onSubmit={handleSubmit} className="space-y-5 rounded-sm border border-line bg-white p-6">
        {error && <ErrorBlock message={error} />}
        {optionsError && <ErrorBlock message={optionsError} />}

        <Field label="User" htmlFor="userId">
          <Select id="userId" required value={form.userId} onChange={update("userId")}>
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.name}
              </option>
            ))}
          </Select>
        </Field>

        <div className="grid grid-cols-2 gap-4">
          <Field label="Date" htmlFor="date">
            <TextInput id="date" type="date" required value={form.date} onChange={update("date")} />
          </Field>
          <Field label="Amount" htmlFor="amount">
            <TextInput
              id="amount"
              type="number"
              step="0.01"
              required
              value={form.amount}
              onChange={update("amount")}
              placeholder="42.50"
            />
          </Field>
        </div>

        <Field label="Merchant" htmlFor="merchant">
          <TextInput
            id="merchant"
            required
            value={form.merchant}
            onChange={update("merchant")}
            placeholder="Target"
          />
        </Field>

        <Field label="Description" htmlFor="description">
          <TextArea
            id="description"
            rows={2}
            value={form.description}
            onChange={update("description")}
            placeholder="Household items"
          />
        </Field>

        <Field
          label="Category"
          htmlFor="categoryId"
          hint="Optional — leave blank and the backend's ML categorizer will assign one."
        >
          <Select id="categoryId" value={form.categoryId} onChange={update("categoryId")}>
            <option value="">Let ML categorize it</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </Select>
        </Field>

        <div className="flex justify-end gap-3 pt-2">
          <Button type="submit" disabled={submitting || loadingOptions}>
            {submitting ? "Saving…" : "Save transaction"}
          </Button>
        </div>
      </form>
    </div>
  );
}
