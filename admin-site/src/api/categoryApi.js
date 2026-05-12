import { api } from "./index";

export const categoryApi = {
  getAll:        ()          => api.get(`/api/category`),
  create:        (data)      => api.post(`/api/category`, { name: data.name }),
  update:        (id, data)  => api.put(`/api/category/${id}`, { name: data.name }),
  delete:        (id)        => api.delete(`/api/category/${id}`),
};