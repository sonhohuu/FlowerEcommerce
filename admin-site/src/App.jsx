import { useState, useEffect } from 'react'

import Icon from "./components/common/Icon";
import Btn from "./components/ui/Button";
import Modal from "./components/ui/Modal";
import Field from "./components/common/Field";
import Input from "./components/ui/Input";
import { tokenHelper } from './utils/token';
import { authApi } from './api/authApi';
// Pages
import Dashboard from "./pages/Dashboard";
import Products from "./pages/Products";
import Categories from "./pages/Categories";
import Orders from "./pages/Orders";
import Users from "./pages/Users";
import Login from "./pages/Login";

// ─── JWT decode (không verify, chỉ đọc payload) ───────────────────────────────
function decodeJwt(token) {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch {
    return null;
  }
}
 
// ─── App ──────────────────────────────────────────────────────────────────────
export default function App() {
  const [page, setPage] = useState("dashboard");
  const [dark, setDark] = useState(false);
  // user: { userId, username, email, role } | null
  const [user, setUser] = useState(null);
  const [authChecked, setAuthChecked] = useState(false);
 
  useEffect(() => {
    // Kiểm tra cookie khi app khởi động
    const token = tokenHelper.getAccess();
    if (token) {
      const payload = decodeJwt(token);
      const isExpired = payload?.exp && Date.now() / 1000 > payload.exp;
      if (payload && !isExpired) {
        // Claim names khớp với JWT server trả về
        setUser({
          userId:   payload.UserId,
          username: payload.UserName,
          email:    payload.Email,
          role:     payload.RoleIds,  // "Administrator"
        });
      } else {
        tokenHelper.clear();
      }
    }
    setAuthChecked(true);
 
    // Lắng nghe khi refresh token hết hạn → force logout
    const handleForceLogout = () => {
      setUser(null);
      setPage("dashboard");
    };
    window.addEventListener("auth:logout", handleForceLogout);
    return () => window.removeEventListener("auth:logout", handleForceLogout);
  }, []);
 
  // data = json.data từ response login:
  // { tokenModel, userId, username, allowedRole }
  function handleLogin(data) {
    setUser({
      userId:   data.userId,
      username: data.username,
      email:    data.email ?? "",
      role:     data.allowedRole,   // "Administrator"
    });
    setPage("dashboard");
  }
 
  async function handleLogout() {
    await authApi.logout();  // gọi server + xóa cookie
    setUser(null);
    setPage("dashboard");
  }
 
  const theme = dark ? {
    "--bg": "#0f0f13", "--card": "#18181f", "--border": "#2a2a38",
    "--text": "#f1f0ff", "--muted": "#7a7a99",
  } : {
    "--bg": "#f4f4f8", "--card": "#ffffff", "--border": "#e8e8f0",
    "--text": "#12121a", "--muted": "#7a7a99",
  };
 
  const navItems = [
    { id: "dashboard", label: "Dashboard",   icon: "dashboard" },
    { id: "products",  label: "Sản phẩm",    icon: "products" },
    { id: "categories",label: "Thể loại",    icon: "categories" },
    { id: "orders",    label: "Đơn hàng",    icon: "orders" },
    { id: "users",     label: "Người dùng",  icon: "users" },
  ];
 
  const pages = {
    dashboard:  <Dashboard />,
    products:   <Products />,
    categories: <Categories />,
    orders:     <Orders />,
    users:      <Users />,
  };
 
  // Chờ kiểm tra auth xong
  if (!authChecked) return null;
 
  // Chưa đăng nhập → hiện Login page
  if (!user) return <Login onLogin={handleLogin} />;
 
  // Đã đăng nhập → hiện dashboard
  return (
    <div style={{
      ...theme,
      display: "flex", height: "100vh", width: "100vw",
      position: "fixed", top: 0, left: 0,
      fontFamily: "'DM Sans', system-ui, sans-serif",
      background: "var(--bg)", color: "var(--text)", overflow: "hidden",
    }}>
 
      {/* Sidebar */}
      <aside style={{ width: 220, background: "var(--card)", borderRight: "1px solid var(--border)",
        display: "flex", flexDirection: "column", flexShrink: 0 }}>
 
        {/* Logo */}
        <div style={{ padding: "20px 20px 16px", borderBottom: "1px solid var(--border)" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <div style={{ width: 34, height: 34, borderRadius: 10, background: "#6366f1",
              display: "flex", alignItems: "center", justifyContent: "center", color: "#fff", fontSize: 16, fontWeight: 800 }}>S</div>
            <div>
              <div style={{ fontSize: 14, fontWeight: 800, color: "var(--text)", letterSpacing: "-0.3px" }}>ShopAdmin</div>
              <div style={{ fontSize: 10, color: "var(--muted)", fontWeight: 500 }}>v1.0 · preview</div>
            </div>
          </div>
        </div>
 
        {/* Nav */}
        <nav style={{ flex: 1, padding: "12px 10px" }}>
          <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: "0.08em", color: "var(--muted)",
            textTransform: "uppercase", padding: "8px 10px 6px" }}>QUẢN LÝ</div>
          {navItems.map(item => (
            <button key={item.id} onClick={() => setPage(item.id)} style={{
              display: "flex", alignItems: "center", gap: 10, padding: "10px 12px", width: "100%",
              borderRadius: 10, border: "none", cursor: "pointer", fontFamily: "inherit",
              background: page === item.id ? "#6366f1" : "none",
              color: page === item.id ? "#fff" : "var(--muted)",
              fontSize: 13, fontWeight: page === item.id ? 700 : 500,
              marginBottom: 2, textAlign: "left", transition: "all .15s",
            }}>
              <Icon name={item.icon} size={16} />
              {item.label}
            </button>
          ))}
        </nav>
 
        {/* User + Logout */}
        <div style={{ padding: "14px 16px", borderTop: "1px solid var(--border)" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 10 }}>
            <div style={{ width: 32, height: 32, borderRadius: "50%", background: "#6366f1",
              display: "flex", alignItems: "center", justifyContent: "center", color: "#fff", fontWeight: 700, fontSize: 13 }}>
              {user.username?.[0]?.toUpperCase() || "A"}
            </div>
            <div style={{ flex: 1, overflow: "hidden" }}>
              <div style={{ fontSize: 12, fontWeight: 700, color: "var(--text)", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                {user.username}
              </div>
              <div style={{ fontSize: 10, color: "var(--muted)" }}>{user.role}</div>
            </div>
          </div>
          <button onClick={handleLogout} style={{
            width: "100%", padding: "7px 0", borderRadius: 8,
            background: "none", border: "1px solid var(--border)",
            color: "var(--muted)", fontSize: 12, cursor: "pointer",
            fontFamily: "inherit", transition: "all .15s",
          }}>
            Đăng xuất
          </button>
        </div>
      </aside>
 
      {/* Main */}
      <div style={{ flex: 1, display: "flex", flexDirection: "column", overflow: "hidden" }}>
 
        {/* Topbar */}
        <header style={{ height: 58, background: "var(--card)", borderBottom: "1px solid var(--border)",
          display: "flex", alignItems: "center", justifyContent: "space-between", padding: "0 24px", flexShrink: 0 }}>
          <div style={{ fontSize: 13, color: "var(--muted)" }}>
            Workspace <span style={{ color: "var(--text)", fontWeight: 600 }}>→ {navItems.find(n => n.id === page)?.label}</span>
          </div>
          <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
            <div style={{ position: "relative" }}>
              <div style={{ width: 36, height: 36, borderRadius: 10, background: "var(--bg)",
                display: "flex", alignItems: "center", justifyContent: "center", cursor: "pointer", color: "var(--muted)" }}>
                <Icon name="bell" size={16} />
              </div>
              <div style={{ position: "absolute", top: 6, right: 6, width: 8, height: 8, borderRadius: "50%",
                background: "#ec4899", border: "2px solid var(--card)" }} />
            </div>
            <button onClick={() => setDark(d => !d)} style={{
              width: 36, height: 36, borderRadius: 10, background: "var(--bg)",
              border: "none", cursor: "pointer", color: "var(--muted)",
              display: "flex", alignItems: "center", justifyContent: "center",
            }}>
              <Icon name={dark ? "sun" : "moon"} size={16} />
            </button>
            <div style={{ width: 34, height: 34, borderRadius: "50%", background: "#6366f1",
              display: "flex", alignItems: "center", justifyContent: "center", color: "#fff", fontWeight: 700, fontSize: 13 }}>
              {user.username?.[0]?.toUpperCase() || "A"}
            </div>
          </div>
        </header>
 
        {/* Content */}
        <main style={{ flex: 1, overflowY: "auto", padding: 28 }}>
          {pages[page]}
        </main>
      </div>
    </div>
  );
}
 

