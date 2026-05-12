import { tokenHelper } from '../utils/token';

export const authApi = {
  login: async ({ username, password }) => {
    // Gọi thẳng fetch, không qua request() vì chưa có token
    const res = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
    });
    const json = await res.json();
    if (!json.success) throw new Error(json.message || "Đăng nhập thất bại");
    tokenHelper.save(json.data.tokenModel);
    return json.data;
  },
 
  logout: async () => {
    try {
      await api.post("/api/auth/logout", {
        refreshToken: tokenHelper.getRefresh(),
      });
    } catch {
      // Kệ lỗi server, vẫn xóa cookie phía client
    } finally {
      tokenHelper.clear();
    }
  },
};