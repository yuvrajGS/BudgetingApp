import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { listCategories } from "../../api/categories";
import PageHeader from "../../components/PageHeader";
import { Button } from "../../components/Form";
import { Loading, ErrorBlock, Empty } from "../../components/Status";

export default function CategoriesList() {
  const [state, setState] = useState({ loading: true, error: null, categories: [] });

  const load = () => {
    setState((s) => ({ ...s, loading: true, error: null }));
    listCategories()
      .then((categories) => setState({ loading: false, error: null, categories: categories ?? [] }))
      .catch((err) => setState((s) => ({ ...s, loading: false, error: err.message })));
  };

  useEffect(load, []);

  return (
    <div>
      <PageHeader
        eyebrow="Organization"
        title="Categories"
        action={
          <Link to="/categories/new">
            <Button>New category</Button>
          </Link>
        }
      />

      {state.loading && <Loading label="Loading categories…" />}
      {state.error && <ErrorBlock message={state.error} onRetry={load} />}

      {!state.loading && !state.error && state.categories.length === 0 && (
        <Empty
          title="No categories yet"
          hint="Categories group transactions by merchant keywords, like groceries or rent."
          action={
            <Link to="/categories/new">
              <Button>New category</Button>
            </Link>
          }
        />
      )}

      {!state.loading && !state.error && state.categories.length > 0 && (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          {state.categories.map((category) => (
            <Link
              key={category.id}
              to={`/categories/${encodeURIComponent(category.name)}`}
              className="rounded-sm border border-line bg-white p-4 hover:shadow-sm transition-shadow"
            >
              <div className="flex items-center justify-between">
                <p className="font-display text-lg text-ink">{category.name}</p>
                <span
                  className="h-2 w-2 rounded-full"
                  style={{ backgroundColor: "var(--color-rust)" }}
                  aria-hidden="true"
                />
              </div>
              {category.description && (
                <p className="mt-1 text-sm text-ink-soft">{category.description}</p>
              )}
              {category.keywords && (
                <p className="mt-2 font-mono text-xs text-muted truncate">
                  {category.keywords}
                </p>
              )}
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
