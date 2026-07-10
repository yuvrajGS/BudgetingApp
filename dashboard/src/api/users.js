import { apiGet, apiPost, apiDelete } from "./client";

// GET /api/user
export const listUsers = () => apiGet("/api/user");

// GET /api/user/{id}
export const getUser = (id) => apiGet(`/api/user/${encodeURIComponent(id)}`);

// POST /api/user
// body: { name, email }
export const createUser = (body) => apiPost("/api/user", body);

// DELETE /api/user/{id}
export const deleteUser = (id) => apiDelete(`/api/user/${encodeURIComponent(id)}`);
