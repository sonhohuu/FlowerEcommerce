import { api } from "./index";

const BASE = "https://localhost:7150";

export const categoryApi = {
  getAll: () => api.get("/api/category"),

  create: async (data) => {
    const res = await fetch(`/api/category`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: data.name }),
    });
    const json = await res.json();
    if (!json.success) throw new Error(json.message || "Tạo thất bại");
    return json.data;
  },

  update: async (id, data) => {
    const res = await fetch(`${BASE}/api/category/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: data.name }),
    });
    const json = await res.json();
    if (!json.success) throw new Error(json.message || "Cập nhật thất bại");
    return json.data;
  },

  delete: async (id) => {
    const res = await fetch(`/api/category/${id}`, {
      method: "DELETE",
    });
    const json = await res.json();
    if (!json.success) throw new Error(json.message || "Xoá thất bại");
    return json.data;
  },
};