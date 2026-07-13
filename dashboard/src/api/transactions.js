import { apiGet, apiPost, apiDelete, apiPut, apiPostFormData } from "./client";

// GET /api/transaction/{id}
export const getTransaction = (id) =>
  apiGet(`/api/transaction/${encodeURIComponent(id)}`);

// GET /api/transaction/user/{userId}
// There is no "list all transactions" endpoint — transactions are always
// listed in the context of a user.
export const listTransactionsForUser = (userId) =>
  apiGet(`/api/transaction/user/${encodeURIComponent(userId)}`);

// POST /api/transaction
// body: { userId, date, merchant, amount, description, categoryId }
export const createTransaction = (body) => apiPost("/api/transaction", body);

// POST /api/transaction/batch
// body: an array of the same shape as createTransaction's body
export const createTransactionBatch = (transactions) =>
  apiPost("/api/transaction/batch", transactions);

// PUT /api/transaction/{id}
// body: { date, merchant, amount, description, categoryId }
export const updateTransaction = (id, body) =>
  apiPut(`/api/transaction/${encodeURIComponent(id)}`, body);

// DELETE /api/transaction/{id}
export const deleteTransaction = (id) =>
  apiDelete(`/api/transaction/${encodeURIComponent(id)}`);

export const importTransactions = (file) => {
  const formData = new FormData();
  formData.append("file", file);
  return apiPostFormData("/api/transaction/import", formData);
};