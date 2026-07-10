// Thin wrapper around fetch for talking to the ASP.NET backend.
// All calls go through Vite's dev proxy (see vite.config.js), so we only
// ever need the relative "/api/..." path here — no absolute host to manage.

export class ApiError extends Error {
  constructor(message, status) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

async function request(path, options = {}) {
  const response = await fetch(path, {
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
    ...options,
  });

  // 204 No Content or empty bodies: nothing to parse.
  const raw = await response.text();
  const data = raw ? safeJsonParse(raw) : null;

  if (!response.ok) {
    const message =
      (data && (data.message || data.title)) ||
      `Request failed with status ${response.status}`;
    throw new ApiError(message, response.status);
  }

  return data;
}

function safeJsonParse(raw) {
  try {
    return JSON.parse(raw);
  } catch {
    return raw;
  }
}

export const apiGet = (path) => request(path, { method: "GET" });

export const apiPost = (path, body) =>
  request(path, { method: "POST", body: JSON.stringify(body) });

export const apiPut = (path, body) =>
  request(path, { method: "PUT", body: JSON.stringify(body) });

export const apiDelete = (path) => request(path, { method: "DELETE" });
