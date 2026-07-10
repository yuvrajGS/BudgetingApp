import { apiGet, apiPost } from "./client";

// GET /api/category
export const listCategories = () => apiGet("/api/category");

// GET /api/category/{name}  — lookup is by name, not numeric id.
export const getCategoryByName = (name) =>
  apiGet(`/api/category/${encodeURIComponent(name)}`);

// POST /api/category
// body: { name, description, keywords }
export const createCategory = (body) => apiPost("/api/category", body);
