import { useState } from "react";
import { authApi } from "../api/authApi";

export default function Login({ onLogin }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [remember, setRemember] = useState(false);
  const [loading, setLoading]   = useState(false);
  const [error, setError]       = useState("");
  const [showPw, setShowPw]     = useState(false);

  async function handleSubmit() {
    if (!username.trim() || !password.trim()) {
      setError("Vui lòng nhập đầy đủ thông tin.");
      return;
    }
    setError("");
    setLoading(true);
    try {
      // authApi.login: gọi API, lưu cookie, trả về json.data
      // json.data = { tokenModel, userId, username, allowedRole }
      const data = await authApi.login({ username, password });

      // Nếu không remember → override cookie thành session (xóa khi đóng tab)
      if (!remember) {
        document.cookie = `accessToken=${encodeURIComponent(data.tokenModel.accessToken)}; path=/; SameSite=Lax`;
        document.cookie = `refreshToken=${encodeURIComponent(data.tokenModel.refreshToken)}; path=/; SameSite=Lax`;
      }

      onLogin(data); // App.handleLogin nhận { tokenModel, userId, username, allowedRole }
    } catch (err) {
      setError(err.message || "Đăng nhập thất bại.");
    } finally {
      setLoading(false);
    }
  }

  function handleKey(e) {
    if (e.key === "Enter") handleSubmit();
  }

  return (
    <div style={{
      position: "fixed", inset: 0,
      background: "#EEEEE8",
      display: "flex", alignItems: "center", justifyContent: "center",
      fontFamily: "'Georgia', 'Times New Roman', serif",
    }}>
      <div style={{
        background: "#fff",
        borderRadius: 20,
        padding: "44px 48px 40px",
        width: 380,
        boxShadow: "0 4px 40px rgba(0,0,0,0.08)",
      }}>
        {/* Title */}
        <h1 style={{
          textAlign: "center", margin: "0 0 36px",
          fontFamily: "'Georgia', serif",
          fontWeight: 400, fontSize: 28,
          letterSpacing: "0.06em", color: "#12121a",
        }}>Login</h1>

        {/* Error */}
        {error && (
          <div style={{
            background: "#fef2f2", border: "1px solid #fecaca",
            borderRadius: 8, padding: "10px 14px",
            fontSize: 13, color: "#dc2626", marginBottom: 20,
            fontFamily: "system-ui, sans-serif",
          }}>{error}</div>
        )}

        <div style={{ textAlign: "left" }}>
          {/* Username */}
          <div style={{ marginBottom: 22 }}>
            <label style={{
              fontSize: 11, fontWeight: 700, letterSpacing: "0.1em",
              color: "#888", fontFamily: "system-ui, sans-serif",
              display: "block", marginBottom: 8, textAlign: "left",
            }}>USERNAME</label>
            <input
              type="text"
              value={username}
              onChange={e => setUsername(e.target.value)}
              onKeyDown={handleKey}
              placeholder="Enter your username"
              style={{
                width: "100%", border: "none", borderBottom: "1.5px solid #e0e0d8",
                padding: "8px 0", fontSize: 14, color: "#12121a",
                background: "transparent", outline: "none",
                fontFamily: "system-ui, sans-serif", boxSizing: "border-box",
              }}
            />
          </div>

          {/* Password */}
          <div style={{ marginBottom: 20 }}>
            <label style={{
              fontSize: 11, fontWeight: 700, letterSpacing: "0.1em",
              color: "#888", fontFamily: "system-ui, sans-serif",
              display: "block", marginBottom: 8, textAlign: "left",
            }}>PASSWORD</label>
            <div style={{ position: "relative" }}>
              <input
                type={showPw ? "text" : "password"}
                value={password}
                onChange={e => setPassword(e.target.value)}
                onKeyDown={handleKey}
                placeholder="Enter your password"
                style={{
                  width: "100%", border: "none", borderBottom: "1.5px solid #e0e0d8",
                  padding: "8px 28px 8px 0", fontSize: 14, color: "#12121a",
                  background: "transparent", outline: "none",
                  fontFamily: "system-ui, sans-serif", boxSizing: "border-box",
                }}
              />
              <button
                onClick={() => setShowPw(v => !v)}
                style={{
                  position: "absolute", right: 0, top: "50%", transform: "translateY(-50%)",
                  background: "none", border: "none", cursor: "pointer",
                  color: "#aaa", padding: 0, fontSize: 15,
                }}
                aria-label={showPw ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
              >
                {showPw ? "🙈" : "👁"}
              </button>
            </div>
          </div>

          {/* Remember + Forgot */}
          <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 28 }}>
            <label style={{
              display: "flex", alignItems: "center", gap: 8,
              fontSize: 13, color: "#555",
              fontFamily: "system-ui, sans-serif", cursor: "pointer",
            }}>
              <input
                type="checkbox"
                checked={remember}
                onChange={e => setRemember(e.target.checked)}
                style={{ width: 15, height: 15, accentColor: "#12121a", cursor: "pointer" }}
              />
              Remember me
            </label>
            <button style={{
              background: "none", border: "none", fontSize: 13,
              color: "#888", cursor: "pointer",
              fontFamily: "system-ui, sans-serif", padding: 0,
            }}>
              Forgot password?
            </button>
          </div>

          {/* Submit */}
          <button
            onClick={handleSubmit}
            disabled={loading}
            style={{
              width: "100%", padding: "15px 0",
              background: loading ? "#555" : "#12121a",
              color: "#fff", border: "none", borderRadius: 10,
              fontSize: 13, fontWeight: 700, letterSpacing: "0.12em",
              fontFamily: "system-ui, sans-serif",
              cursor: loading ? "not-allowed" : "pointer",
              transition: "background .2s",
              textTransform: "uppercase",
            }}
          >
            {loading ? "Đang đăng nhập..." : "Log In"}
          </button>

          {/* Register */}
          <p style={{
            textAlign: "center", fontSize: 13, color: "#888",
            marginTop: 22, marginBottom: 0,
            fontFamily: "system-ui, sans-serif",
          }}>
            Don't have an account?{" "}
            <a href="#" style={{ color: "#12121a", fontWeight: 700, textDecoration: "underline" }}>Register</a>
          </p>
        </div>
      </div>
    </div>
  );
}