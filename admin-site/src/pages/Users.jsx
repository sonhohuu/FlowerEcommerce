import { useState } from "react";
import { MOCK_USERS } from "../data/mockData";
import SearchBar from "../components/ui/SearchBar";
import Badge from "../components/ui/Badge";
import { fmt } from "../utils/format";

const Users = () => {
  const [search, setSearch] = useState("");
  const [users, setUsers] = useState(MOCK_USERS);
 
  const filtered = users.filter(u =>
    u.name.toLowerCase().includes(search.toLowerCase()) ||
    u.email.toLowerCase().includes(search.toLowerCase())
  );
 
  const roleColors = { admin: "#6366f1", moderator: "#8b5cf6", customer: "#6b7280" };
  const roleLabels = { admin: "Admin", moderator: "Moderator", customer: "Khách hàng" };
 
  const toggleStatus = (id) => {
    setUsers(prev => prev.map(u => u.id === id ? { ...u, status: u.status === "active" ? "inactive" : "active" } : u));
  };
 
  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 24 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, color: "var(--text)", margin: 0 }}>Người dùng</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>{users.length} tài khoản</p>
        </div>
        <div style={{ display: "flex", gap: 10 }}>
          <SearchBar value={search} onChange={setSearch} placeholder="Tìm người dùng..." />
        </div>
      </div>
 
      <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12, marginBottom: 24 }}>
        {[
          { label: "Tổng tài khoản", value: users.length, color: "#6366f1" },
          { label: "Đang hoạt động", value: users.filter(u => u.status === "active").length, color: "#16a34a" },
          { label: "Vô hiệu hoá", value: users.filter(u => u.status === "inactive").length, color: "#dc2626" },
        ].map(({ label, value, color }) => (
          <div key={label} style={{ background: "var(--card)", borderRadius: 14, padding: "18px 22px",
            border: "1px solid var(--border)", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <span style={{ fontSize: 13, color: "var(--muted)", fontWeight: 500 }}>{label}</span>
            <span style={{ fontSize: 24, fontWeight: 800, color }}>{value}</span>
          </div>
        ))}
      </div>
 
      <div style={{ background: "var(--card)", borderRadius: 16, border: "1px solid var(--border)", overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead style={{ background: "var(--bg)" }}>
            <tr style={{ color: "var(--muted)", fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              <th style={{ textAlign: "left", padding: "14px 20px" }}>Người dùng</th>
              <th style={{ textAlign: "left", padding: "14px 16px" }}>Vai trò</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Đơn hàng</th>
              <th style={{ textAlign: "right", padding: "14px 16px" }}>Tổng chi</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Tham gia</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Trạng thái</th>
              <th style={{ textAlign: "center", padding: "14px 20px" }}>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map(u => (
              <tr key={u.id} style={{ borderTop: "1px solid var(--border)" }}>
                <td style={{ padding: "14px 20px" }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
                    <div style={{ width: 36, height: 36, borderRadius: "50%", background: "#6366f1",
                      display: "flex", alignItems: "center", justifyContent: "center", color: "#fff",
                      fontSize: 14, fontWeight: 700, flexShrink: 0 }}>{u.avatar}</div>
                    <div>
                      <div style={{ fontWeight: 600, color: "var(--text)" }}>{u.name}</div>
                      <div style={{ fontSize: 11, color: "var(--muted)" }}>{u.email}</div>
                    </div>
                  </div>
                </td>
                <td style={{ padding: "14px 16px" }}>
                  <span style={{ fontSize: 11, fontWeight: 700, padding: "3px 10px", borderRadius: 99,
                    background: `${roleColors[u.role]}18`, color: roleColors[u.role] }}>
                    {roleLabels[u.role]}
                  </span>
                </td>
                <td style={{ padding: "14px 16px", textAlign: "center", color: "var(--text)", fontWeight: 600 }}>{u.orders}</td>
                <td style={{ padding: "14px 16px", textAlign: "right", fontWeight: 700, color: "var(--text)" }}>{u.spent > 0 ? fmt(u.spent) : "—"}</td>
                <td style={{ padding: "14px 16px", textAlign: "center", color: "var(--muted)", fontSize: 12 }}>{u.joined}</td>
                <td style={{ padding: "14px 16px", textAlign: "center" }}><Badge status={u.status} /></td>
                <td style={{ padding: "14px 20px", textAlign: "center" }}>
                  <button onClick={() => toggleStatus(u.id)} style={{
                    background: u.status === "active" ? "#fee2e2" : "#dcfce7",
                    border: `1px solid ${u.status === "active" ? "#fca5a5" : "#86efac"}`,
                    color: u.status === "active" ? "#dc2626" : "#16a34a",
                    borderRadius: 8, padding: "5px 12px", cursor: "pointer", fontSize: 11, fontWeight: 700,
                  }}>
                    {u.status === "active" ? "Khoá" : "Mở khoá"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Users;