import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCategoryByName } from "../../api/categories";
import PageHeader from "../../components/PageHeader";
import { Loading, ErrorBlock, Empty } from "../../components/Status";

export default function CategoryDetail() {
  const { name } = useParams();
  const [state, setState] = useState({ loading: true, error: null, category: null });

  useEffect(() => {
    setState({ loading: true, error: null, category: null });
    getCategoryByName(name)
      .then((category) => setState({ loading: false, error: null, category }))
      .catch((err) => setState({ loading: false, error: err.message, category: null }));
  }, [name]);

  if (state.loading) return <Loading label="Loading category…" />;
  if (state.error) return <ErrorBlock message={state.error} />;
  if (!state.category) return <Empty title="Category not found" />;

  const { category } = state;

  return (
    <div>
      <PageHeader eyebrow="Category" title={category.name} />

      <div className="space-y-4">
        <div className="rounded-sm border border-line bg-white p-5">
          <p className="text-xs uppercase tracking-wide text-muted mb-1">Description</p>
          <p className="text-sm text-ink">{category.description || "—"}</p>
        </div>

        <div className="rounded-sm border border-line bg-white p-5">
          <p className="text-xs uppercase tracking-wide text-muted mb-1">
            Matching keywords
          </p>
          {category.keywords ? (
            <div className="flex flex-wrap gap-2">
              {category.keywords.split(",").map((keyword) => (
                <span
                  key={keyword}
                  className="rounded-full px-3 py-1 font-mono text-xs"
                  style={{
                    backgroundColor: "var(--color-rust-soft)",
                    color: "var(--color-rust)",
                  }}
                >
                  {keyword.trim()}
                </span>
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted">No keywords set.</p>
          )}
        </div>

        <div className="rounded-sm border border-line bg-white p-5">
          <p className="text-xs uppercase tracking-wide text-muted mb-1">Created</p>
          <p className="text-sm text-ink">
            {category.createdAt ? new Date(category.createdAt).toLocaleString() : "—"}
          </p>
        </div>
      </div>
    </div>
  );
}
