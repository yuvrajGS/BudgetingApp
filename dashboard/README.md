# Ledger — Budget Dashboard

A React (Vite + JavaScript) frontend for the ASP.NET budgeting API, styled with Tailwind CSS.

## Getting started

```bash
npm install
npm run dev
```

The dev server proxies any request to `/api/*` to `https://localhost:5001` (see
`vite.config.js`). Update the `target` there if your ASP.NET API runs on a
different host or port. `secure: false` is set so the proxy accepts the
backend's local HTTPS dev certificate.

## Structure

- `src/api/` — one fetch-based module per resource (`users.js`, `categories.js`,
  `transactions.js`), plus a shared `client.js` wrapper for error handling.
- `src/components/` — shared UI: `Layout`/`Sidebar`, `PageHeader`, form
  primitives (`Field`, `TextInput`, `Select`, `Button`), status states
  (`Loading`, `ErrorBlock`, `Empty`), `Amount` currency formatter, and
  `AnalysisGroups` for the monthly/yearly breakdown views.
- `src/pages/users`, `src/pages/categories`, `src/pages/transactions` — list,
  detail, and create pages for each resource.
- `src/utils/` — `groupTransactions.js` (monthly/yearly grouping + category
  subtotals), `pdfText.js` (PDF → text via pdf.js), `statementParser.js`
  (heuristic line parser for bank statements).

## Adding transactions

Three ways in:

- **Single form** (`/transactions/new`) — one transaction, posts to
  `POST /api/transaction`.
- **Batch add** (`/transactions/batch`) — a spreadsheet-style table of rows,
  posts the array to `POST /api/transaction/batch`.
- **Statement import** (`/transactions/import`) — see below.

In both forms, category is optional: leave it blank and the backend's ML
categorizer is expected to assign one once the transaction is created.

## Monthly / yearly analysis

The Transactions page has a view toggle (**All / By month / By year**). The
month and year views group the selected user's transactions, show a total
per period, a category breakdown bar for each period, and an expandable list
of the underlying transactions.

## Statement import (`/transactions/import`)

Upload a bank statement PDF and it extracts text client-side (`pdfjs-dist`,
lazy-loaded so it doesn't bloat the main bundle), then runs a heuristic
parser that looks for lines containing both a date and a dollar amount.
Results land in an **editable table** — nothing is submitted until you
review it — then the whole batch posts to `POST /api/transaction/batch`
with `categoryId: null`.

Statement layouts vary a lot between banks, so the parser is intentionally
conservative: lines it can't confidently read are skipped and reported
("N lines couldn't be read automatically") rather than guessed at. You can
add rows manually if a statement doesn't parse well, or expand the raw
extracted text to see what the parser saw.

## API notes baked into the UI

- `GET /api/category/{name}` looks categories up **by name**, not id — the
  category detail route is `/categories/:name`.
- There is no "list all transactions" endpoint, only
  `GET /api/transaction/user/{userId}`. The Transactions page is filtered by
  a user picker for that reason.
- Transaction detail resolves the category name and user name client-side
  (via `listCategories()` / `getUser()`) since there's no
  get-category-by-id or embedded lookup endpoint.
