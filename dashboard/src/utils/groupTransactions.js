// Groups a flat list of transactions into month or year buckets, each with
// a total, a count, and a per-category breakdown — the shape the analysis
// views need to answer "what am I spending on this month/year".

function categoryLabel(categoryId, categoryMap) {
  if (categoryId == null) return "Uncategorized";
  return categoryMap.get(categoryId) ?? `Category #${categoryId}`;
}

function summarize(transactions, categoryMap) {
  const total = transactions.reduce((sum, t) => sum + Number(t.amount ?? 0), 0);

  const byCategory = new Map();
  for (const t of transactions) {
    const key = t.categoryId ?? "uncategorized";
    const entry = byCategory.get(key) ?? {
      categoryId: t.categoryId ?? null,
      name: categoryLabel(t.categoryId, categoryMap),
      total: 0,
      count: 0,
    };
    entry.total += Number(t.amount ?? 0);
    entry.count += 1;
    byCategory.set(key, entry);
  }

  const categories = [...byCategory.values()].sort((a, b) => b.total - a.total);

  return { total, count: transactions.length, categories };
}

export function groupByMonth(transactions, categoryMap = new Map()) {
  const buckets = new Map();

  for (const t of transactions) {
    const d = new Date(t.date);
    if (Number.isNaN(d.getTime())) continue;
    const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}`;
    const list = buckets.get(key) ?? [];
    list.push(t);
    buckets.set(key, list);
  }

  return [...buckets.entries()]
    .sort((a, b) => (a[0] < b[0] ? 1 : -1)) // newest month first
    .map(([key, list]) => {
      const [year, month] = key.split("-");
      const label = new Date(Number(year), Number(month) - 1, 1).toLocaleDateString(undefined, {
        month: "long",
        year: "numeric",
      });
      return { key, label, transactions: list, ...summarize(list, categoryMap) };
    });
}

export function groupByYear(transactions, categoryMap = new Map()) {
  const buckets = new Map();

  for (const t of transactions) {
    const d = new Date(t.date);
    if (Number.isNaN(d.getTime())) continue;
    const key = String(d.getFullYear());
    const list = buckets.get(key) ?? [];
    list.push(t);
    buckets.set(key, list);
  }

  return [...buckets.entries()]
    .sort((a, b) => (a[0] < b[0] ? 1 : -1)) // newest year first
    .map(([key, list]) => ({ key, label: key, transactions: list, ...summarize(list, categoryMap) }));
}
