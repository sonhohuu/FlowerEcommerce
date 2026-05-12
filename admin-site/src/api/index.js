// const BASE_URL = "";

// export const api = {
//   get: async (path) => {
//     const res = await fetch(`${BASE_URL}${path}`);
//     if (!res.ok) throw new Error(`HTTP ${res.status}`);
//     const json = await res.json();
//     return json.data;
//   },
//   post: async (path, body) => {
//     const res = await fetch(`${BASE_URL}${path}`, {
//       method: "POST",
//       headers: { "Content-Type": "application/json" },
//       body: JSON.stringify(body),
//     });
//     const json = await res.json();
//     return json.data;
//   },
//   put: async (path, body) => {
//     const res = await fetch(`${BASE_URL}${path}`, {
//       method: "PUT",
//       headers: { "Content-Type": "application/json" },
//       body: JSON.stringify(body),
//     });
//     const json = await res.json();
//     return json.data;
//   },
//   delete: async (path) => {
//     const res = await fetch(`${BASE_URL}${path}`, { method: "DELETE" });
//     const json = await res.json();
//     return json.data;
//   },
// };

import { tokenHelper } from '../utils/token';

// ── Refresh token logic ───────────────────────────────────────────────────────
let isRefreshing = false;
let waitQueue = []; // { resolve, reject }[]
 
const processQueue = (error, newToken = null) => {
  waitQueue.forEach(({ resolve, reject }) =>
    error ? reject(error) : resolve(newToken)
  );
  waitQueue = [];
};
 
async function refreshAccessToken() {
  const refreshToken = tokenHelper.getRefresh();
  if (!refreshToken) throw new Error("No refresh token");
 
  const res = await fetch("/api/auth/refresh-token", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  });
 
  const json = await res.json();
  if (!res.ok || !json.success) throw new Error(json.message || "Refresh failed");
 
  tokenHelper.save(json.data.tokenModel);
  return json.data.tokenModel.accessToken;
}
 
// ── Core request ──────────────────────────────────────────────────────────────
async function request(path, options = {}) {
  const url = `${path}`;
 
  // Build headers
  const headers = { ...(options.headers ?? {}) };
 
  const token = tokenHelper.getAccess();
  if (token) headers["Authorization"] = `Bearer ${token}`;
 
  // Không set Content-Type nếu body là FormData (browser tự thêm boundary)
  if (!(options.body instanceof FormData)) {
    headers["Content-Type"] = "application/json";
  }
 
  let res = await fetch(url, { ...options, headers });
 
  // ── 401 → refresh rồi retry ───────────────────────────────────────────────
  if (res.status === 401) {
    // Đã có request khác đang refresh → xếp hàng chờ
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        waitQueue.push({ resolve, reject });
      }).then((newToken) => {
        headers["Authorization"] = `Bearer ${newToken}`;
        return fetch(url, { ...options, headers }).then((r) => r.json()).then(handleJson);
      });
    }
 
    isRefreshing = true;
    try {
      const newToken = await refreshAccessToken();
      processQueue(null, newToken);
      headers["Authorization"] = `Bearer ${newToken}`;
      res = await fetch(url, { ...options, headers }); // retry với token mới
    } catch (err) {
      processQueue(err);
      tokenHelper.clear();
      // Báo cho App biết để redirect về Login
      window.dispatchEvent(new Event("auth:logout"));
      throw err;
    } finally {
      isRefreshing = false;
    }
  }
 
  return handleJson(await res.json());
}
 
function handleJson(json) {
  if (json.success === false) throw new Error(json.message || "Request failed");
  return json.data;
}
 
// ── Public API ────────────────────────────────────────────────────────────────
export const api = {
  get: (path) =>
    request(path, { method: "GET" }),
 
  post: (path, body) =>
    request(path, { method: "POST", body: JSON.stringify(body) }),
 
  put: (path, body) =>
    request(path, { method: "PUT", body: JSON.stringify(body) }),
 
  delete: (path) =>
    request(path, { method: "DELETE" }),
 
  // FormData (upload file, multipart) — không JSON.stringify
  upload: (path, formData, method = "POST") =>
    request(path, { method, body: formData }),
};
