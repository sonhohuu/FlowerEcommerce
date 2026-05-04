import { api } from "./index";
 
export const userApi = {
  getAll: (params = {}) => {
    const query = new URLSearchParams({
      page:     params.page || 1,
      pageSize: params.pageSize || 20,
      ...(params.search && { search: params.search }),
      ...(params.role   && { role:   params.role }),
    }).toString();
    return api.get(`/api/user?${query}`);
  },
 
  toggleStatus: (id, status) =>
    api.put(`/api/user/${id}/status`, { status }),
};