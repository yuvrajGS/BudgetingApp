import { useEffect, useState } from "react";
import { useNavigate, useSearchParams, Link } from "react-router-dom";
import { listUsers } from "../../api/users";
import { createTransactionBatch } from "../../api/transactions";
import { extractPdfLines } from "../../utils/pdfText";
import { parseStatementLines } from "../../utils/statementParser";
import PageHeader from "../../components/PageHeader";
import { Field, TextInput, Select, Button } from "../../components/Form";
import { ErrorBlock, Empty } from "../../components/Status";

const blankRow = () => ({
  key: crypto.randomUUID(),
  date: "",
  merchant: "",
  description: "",
  amount: "",
});

export default function TransactionImport() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const [users, setUsers] = useState([]);
  const [loadingUsers, setLoadingUsers] = useState(true);
  const [userId, setUserId] = useState(searchParams.get("userId") ?? "");

  const [fileName, setFileName] = useState(null);
  const [parsing, setParsing] = useState(false);
  const [parseError, setParseError] = useState(null);
  const [skippedCount, setSkippedCount] = useState(0);
  const [rawLines, setRawLines] = useState([]);
  const [showRaw, setShowRaw] = useState(false);

  const [rows, setRows] = useState([]);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState(null);

  useEffect(() => {
    listUsers()
      .then((list) => {
        setUsers(list ?? []);
        setUserId((current) => current || list?.[0]?.id || "");
        setLoadingUsers(false);
      })
      .catch((err) => {
        setParseError(err.message);
        setLoadingUsers(false);
      });
  }, []);

  const handleFile = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setFileName(file.name);
    setParsing(true);
    setParseError(null);
    setRows([]);
    try {
      const lines = await extractPdfLines(file);
      setRawLines(lines);
      const { rows: parsedRows, skipped } = parseStatementLines(lines, {
        fallbackYear: new Date().getFullYear(),
      });
      setSkippedCount(skipped);
      setRows(parsedRows.map((r) => ({ ...r, key: crypto.randomUUID() })));
    } catch (err) {
      setParseError(`Couldn't read that PDF: ${err.message}`);
    } finally {
      setParsing(false);
    }
  };

  const updateRow = (key, field) => (e) =>
    setRows((rs) => rs.map((r) => (r.key === key ? { ...r, [field]: e.target.value } : r)));

  const removeRow = (key) => setRows((rs) => rs.filter((r) => r.key !== key));
  const addRow = () => setRows((rs) => [...rs, blankRow()]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitError(null);
    if (!userId) {
      setSubmitError("Choose which user this statement belongs to.");
      return;
    }
    if (rows.length === 0) {
      setSubmitError("There's nothing to submit yet.");
      return;
    }
    setSubmitting(true);
    try {
      const payload = rows.map((r) => ({
        userId,
        date: new Date(r.date).toISOString(),
        merchant: r.merchant || r.description || "Unknown merchant",
        amount: Number(r.amount),
        description: r.description,
        categoryId: null, // let the backend's ML categorizer assign it
      }));
      await createTransactionBatch(payload);
      navigate(`/transactions?userId=${userId}&view=all`);
    } catch (err) {
      setSubmitError(err.message);
      setSubmitting(false);
    }
  };

  return (
    <div>
      <PageHeader eyebrow="Activity" title="Import bank statement" />

      <p className="mb-6 max-w-2xl text-sm text-ink-soft">
        Upload a PDF statement and this'll pull out the date, merchant, and amount for each line
        it can confidently read. Statement layouts vary a lot between banks, so double-check the
        table below before submitting — categories aren't set here since the backend's ML
        categorizer assigns those automatically once transactions are created.
      </p>

      <div className="rounded-sm border border-line bg-white p-6 mb-6">
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
          <Field label="User" htmlFor="import-user">
            <Select
              id="import-user"
              value={userId}
              onChange={(e) => setUserId(e.target.value)}
              disabled={loadingUsers || users.length === 0}
            >
              {users.length === 0 && <option value="">No users yet</option>}
              {users.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.name}
                </option>
              ))}
            </Select>
          </Field>

          <Field label="Statement PDF" htmlFor="import-file">
            <input
              id="import-file"
              type="file"
              accept="application/pdf"
              onChange={handleFile}
              className="block w-full text-sm text-ink-soft file:mr-3 file:rounded-sm file:border-0 file:bg-ink file:px-3 file:py-2 file:text-sm file:font-medium file:text-white hover:file:opacity-90"
            />
          </Field>
        </div>

        {users.length === 0 && !loadingUsers && (
          <div className="mt-4">
            <ErrorBlock message="You need at least one user before importing a statement." />
            <div className="mt-3">
              <Link to="/users/new">
                <Button>Add a user</Button>
              </Link>
            </div>
          </div>
        )}

        {parsing && <p className="mt-4 text-sm text-muted">Reading {fileName}…</p>}
        {parseError && <div className="mt-4"><ErrorBlock message={parseError} /></div>}

        {!parsing && fileName && !parseError && (
          <p className="mt-4 text-sm text-muted">
            Found {rows.length} likely transaction{rows.length === 1 ? "" : "s"} in {fileName}
            {skippedCount > 0 && (
              <>
                {" "}
                — {skippedCount} line{skippedCount === 1 ? "" : "s"} couldn't be read automatically.
              </>
            )}{" "}
            <button
              type="button"
              onClick={() => setShowRaw((s) => !s)}
              className="underline decoration-line underline-offset-2 hover:decoration-ink"
            >
              {showRaw ? "Hide" : "Show"} extracted text
            </button>
          </p>
        )}

        {showRaw && (
          <pre className="mt-3 max-h-48 overflow-y-auto rounded-sm bg-paper-dim p-3 font-mono text-xs text-ink-soft whitespace-pre-wrap">
            {rawLines.join("\n") || "No text found in this PDF."}
          </pre>
        )}
      </div>

      {rows.length === 0 && fileName && !parsing && !parseError ? (
        <Empty
          title="Nothing parsed automatically"
          hint="The statement's layout didn't match what the parser expects. Add rows manually below, or try a different export from your bank."
          action={
            <Button variant="ghost" onClick={addRow} type="button">
              + Add row manually
            </Button>
          }
        />
      ) : (
        rows.length > 0 && (
          <form onSubmit={handleSubmit} className="space-y-4">
            {submitError && <ErrorBlock message={submitError} />}

            <div className="overflow-x-auto rounded-sm border border-line bg-white">
              <table className="w-full text-left text-sm">
                <thead>
                  <tr className="rule text-xs uppercase tracking-wide text-muted">
                    <th className="px-3 py-3 font-medium">Date</th>
                    <th className="px-3 py-3 font-medium">Merchant</th>
                    <th className="px-3 py-3 font-medium">Description</th>
                    <th className="px-3 py-3 font-medium">Amount</th>
                    <th className="px-3 py-3" />
                  </tr>
                </thead>
                <tbody>
                  {rows.map((row) => (
                    <tr key={row.key} className="rule last:border-b-0 align-top">
                      <td className="px-3 py-2 min-w-[140px]">
                        <TextInput
                          type="date"
                          required
                          value={row.date}
                          onChange={updateRow(row.key, "date")}
                        />
                      </td>
                      <td className="px-3 py-2 min-w-[160px]">
                        <TextInput
                          required
                          value={row.merchant}
                          onChange={updateRow(row.key, "merchant")}
                          placeholder="Target"
                        />
                      </td>
                      <td className="px-3 py-2 min-w-[200px]">
                        <TextInput
                          value={row.description}
                          onChange={updateRow(row.key, "description")}
                          placeholder="Household items"
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
                      <td className="px-3 py-2">
                        <button
                          type="button"
                          onClick={() => removeRow(row.key)}
                          className="text-xs text-muted hover:text-rust"
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
              <Button type="submit" disabled={submitting || !userId}>
                {submitting ? "Saving…" : `Save ${rows.length} transaction${rows.length === 1 ? "" : "s"}`}
              </Button>
            </div>
          </form>
        )
      )}
    </div>
  );
}
