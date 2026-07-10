import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createUser } from "../../api/users";
import PageHeader from "../../components/PageHeader";
import { Field, TextInput, Button } from "../../components/Form";
import { ErrorBlock } from "../../components/Status";

export default function UserCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ name: "", email: "" });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  const update = (field) => (e) => setForm((f) => ({ ...f, [field]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const user = await createUser(form);
      navigate(user?.id ? `/users/${user.id}` : "/users");
    } catch (err) {
      setError(err.message);
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-lg">
      <PageHeader eyebrow="People" title="New user" />

      <form onSubmit={handleSubmit} className="space-y-5 rounded-sm border border-line bg-white p-6">
        {error && <ErrorBlock message={error} />}

        <Field label="Name" htmlFor="name">
          <TextInput
            id="name"
            required
            value={form.name}
            onChange={update("name")}
            placeholder="Alex Rivera"
          />
        </Field>

        <Field label="Email" htmlFor="email">
          <TextInput
            id="email"
            type="email"
            required
            value={form.email}
            onChange={update("email")}
            placeholder="alex@example.com"
          />
        </Field>

        <div className="flex justify-end gap-3 pt-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Saving…" : "Save user"}
          </Button>
        </div>
      </form>
    </div>
  );
}
