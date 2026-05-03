import { api } from "./index";

const BASE = "https://localhost:7150";

const buildFormData = (data) => {
  const fd = new FormData();

  // ── Scalar fields ──────────────────────────────────────
  const scalarFields = [
    "name", "description", "price", "originalPrice",
    "isContactPrice", "sku", "categoryId",
  ];

  scalarFields.forEach((key) => {
    const val = data[key];
    if (val !== undefined && val !== null && val !== "") {
      fd.append(key, val);
    }
  });

  // ── SizePrices: gửi từng item theo index ───────────────
  // Server nhận: SizePrices[0].Label, SizePrices[0].Price, ...
  if (Array.isArray(data.sizePrices) && data.sizePrices.length > 0) {
    data.sizePrices.forEach((sp, i) => {
      fd.append(`SizePrices[${i}].Label`, sp.label ?? sp.Label ?? "");
      fd.append(`SizePrices[${i}].Price`, sp.price ?? sp.Price ?? 0);
    });
  }

  // ── FileAttachMents: multiple files ────────────────────
  // Server nhận: FileAttachMents (List<IFormFile>)
  if (Array.isArray(data.fileAttachMents) && data.fileAttachMents.length > 0) {
    data.fileAttachMents.forEach((file) => {
      fd.append("FileAttachMents", file); // cùng key → array phía server
    });
  }

  return fd;
};

export const productApi = {
  getAll: (params = {}) => {
    const query = new URLSearchParams({
      page: params.page || 1,
      size: params.size || 10,
      ...(params.search     && { search: params.search }),
      ...(params.categoryId && { categoryId: params.categoryId }),
    }).toString();
    return api.get(`/api/product?${query}`);
  },

  getById: (id) => api.get(`/api/product/${id}`),

  create: async (data) => {
    const res = await fetch("/api/product", {
      method: "POST",
      body: buildFormData(data),
      // KHÔNG set Content-Type — browser tự thêm boundary
    });
    const json = await res.json();
    if (!json.success) throw new Error(json.message || "Tạo thất bại");
    return json.data;
  },

  update: async (id, data) => {
    const res = await fetch(`${BASE}/api/product/${id}`, {
      method: "PUT",
      body: buildFormData(data),
    });
    const json = await res.json();
    if (!json.success) throw new Error(json.message || "Cập nhật thất bại");
    return json.data;
  },

  delete: (id) => api.delete(`/api/product/${id}`),
};