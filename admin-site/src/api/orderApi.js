import { api } from "./index";

const ORDER_STATUS_MAP = {
  Confirming:   0,
  Processing:   1,
  OnDelivering: 2,
  Success:      3,
  Failed:       4,
};

export const orderApi = {
  getAll: (params = {}) => {
    const query = new URLSearchParams({
      page: params.page || 1,
      size: params.size || 10,
      ...(params.search                          && { search: params.search }),
      ...(params.status && params.status !== "all" && { status: params.status }),
    }).toString();
    return api.get(`/api/order?${query}`);
  },

  getById: (id) => api.get(`/api/order/${id}`),

  update: (id, data) =>
  api.put(`/api/order/${id}`, {
    ...(data.status !== undefined && { status: ORDER_STATUS_MAP[data.status] }),
    ...(data.customerName  !== undefined && { customerName:  data.customerName  }),
    ...(data.address       !== undefined && { address:       data.address       }),
    ...(data.phoneNumber   !== undefined && { phoneNumber:   data.phoneNumber   }),
    ...(data.note          !== undefined && { note:          data.note          }),
    ...(data.paymentMethod !== undefined && { paymentMethod: data.paymentMethod }),
    ...(data.items         !== undefined && { items:         data.items         }),
  }),
};