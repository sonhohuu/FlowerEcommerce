import { api } from "./index";

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

  create: (data) => api.upload("/api/product", buildFormData(data), "POST"),

  update: (id, data) => api.upload(`/api/product/${id}`, buildFormData(data), "PUT"),

  delete: (id) => api.delete(`/api/product/${id}`),
};