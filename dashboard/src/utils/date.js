export function parseDateOnly(dateString) {
  if (!dateString) return null;

  const [year, month, day] = dateString.split("-").map(Number);

  return new Date(year, month - 1, day);
}

export function formatDateOnly(dateString) {
  const date = parseDateOnly(dateString);

  if (!date) return "";

  return date.toLocaleDateString(undefined, {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}