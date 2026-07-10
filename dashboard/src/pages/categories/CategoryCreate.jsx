import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createCategory } from "../../api/categories";
import PageHeader from "../../components/PageHeader";
import { Field, TextInput, TextArea, Button } from "../../components/Form";
import { ErrorBlock } from "../../components/Status";

export default function CategoryCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ name: "", description: "", keywords: "" });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  const update = (field) => (e) => setForm((f) => ({ ...f, [field]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const category = await createCategory(form);
      navigate(`/categories/${encodeURIComponent(category?.name ?? form.name)}`);
    } catch (err) {
      setError(err.message);
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-lg">
      <PageHeader eyebrow="Organization" title="New category" />

      <form onSubmit={handleSubmit} className="space-y-5 rounded-sm border border-line bg-white p-6">
        {error && <ErrorBlock message={error} />}

        <Field label="Name" htmlFor="name">
          <TextInput
            id="name"
            required
            value={form.name}
            onChange={update("name")}
            placeholder="Food"
          />
        </Field>

        <Field label="Description" htmlFor="description">
          <TextInput
            id="description"
            value={form.description}
            onChange={update("description")}
            placeholder="Dining and groceries"
          />
        </Field>

        <Field
          label="Keywords"
          htmlFor="keywords"
          hint="Comma-separated terms used to auto-match transactions."
        >
          <TextArea
            id="keywords"
            rows={3}
            value={form.keywords}
            onChange={update("keywords")}
            placeholder="restaurant, grocery, coffee"
          />
        </Field>

        <div className="flex justify-end gap-3 pt-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Saving…" : "Save category"}
          </Button>
        </div>
      </form>
    </div>
  );
}
