import { apiPost } from "./client";

export const uploadDocument = (file, userId) => {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("userId", userId);
    return apiPost("/api/document/upload", formData);
}