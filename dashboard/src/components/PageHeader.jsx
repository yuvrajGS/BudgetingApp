export default function PageHeader({ eyebrow, title, action }) {
  return (
    <div className="mb-8 flex items-start justify-between gap-4 rule pb-6">
      <div>
        {eyebrow && (
          <p className="text-xs font-medium uppercase tracking-[0.14em] text-muted mb-1">
            {eyebrow}
          </p>
        )}
        <h1 className="font-display text-3xl font-semibold tracking-tight text-ink">
          {title}
        </h1>
      </div>
      {action}
    </div>
  );
}
