import { useState } from "react";
import { MOCK_ORDERS } from "../data/mockData";
import SearchBar from "../components/ui/SearchBar";
import Badge from "../components/ui/Badge";
import { fmt } from "../utils/format";
import Modal from "../components/ui/Modal";
import Field from "../components/common/Field";
import Select from "../components/ui/Select";
import Btn from "../components/ui/Button";
import { STATUS_CONFIG } from "../constants/statusConfig";

const Orders = () => {
  const [search, setSearch] = useState("");
  const [filterStatus, setFilterStatus] = useState("all");
  const [orders, setOrders] = useState(MOCK_ORDERS);
  const [selected, setSelected] = useState(null);
 
  const statuses = ["all", "pending", "processing", "shipped", "delivered", "cancelled"];
  const filtered = orders.filter(o => {
    const matchSearch = o.customer.toLowerCase().includes(search.toLowerCase()) || o.id.includes(search);
    const matchStatus = filterStatus === "all" || o.status === filterStatus;
    return matchSearch && matchStatus;
  });
 
  const updateStatus = (id, status) => {
    setOrders(prev => prev.map(o => o.id === id ? { ...o, status } : o));
    setSelected(null);
  };
 
  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, color: "var(--text)", margin: 0 }}>Đơn hàng</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>{filtered.length} đơn hàng</p>
        </div>
        <SearchBar value={search} onChange={setSearch} placeholder="Tìm đơn hàng..." />
      </div>
 
      {/* Filter Tabs */}
      <div style={{ display: "flex", gap: 8, marginBottom: 20 }}>
        {statuses.map(s => (
          <button key={s} onClick={() => setFilterStatus(s)} style={{
            padding: "7px 16px", borderRadius: 99, fontSize: 12, fontWeight: 600, cursor: "pointer",
            border: filterStatus === s ? "none" : "1px solid var(--border)",
            background: filterStatus === s ? "#6366f1" : "var(--card)",
            color: filterStatus === s ? "#fff" : "var(--muted)",
            transition: "all .15s",
          }}>
            {s === "all" ? "Tất cả" : STATUS_CONFIG[s]?.label || s}
          </button>
        ))}
      </div>
 
      <div style={{ background: "var(--card)", borderRadius: 16, border: "1px solid var(--border)", overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead style={{ background: "var(--bg)" }}>
            <tr style={{ color: "var(--muted)", fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              <th style={{ textAlign: "left", padding: "14px 20px" }}>Mã đơn</th>
              <th style={{ textAlign: "left", padding: "14px 16px" }}>Khách hàng</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>SP</th>
              <th style={{ textAlign: "right", padding: "14px 16px" }}>Tổng tiền</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Thanh toán</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Trạng thái</th>
              <th style={{ textAlign: "center", padding: "14px 20px" }}>Ngày</th>
              <th style={{ textAlign: "center", padding: "14px 20px" }}>Chi tiết</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map(o => (
              <tr key={o.id} style={{ borderTop: "1px solid var(--border)" }}>
                <td style={{ padding: "14px 20px", fontWeight: 700, color: "#6366f1" }}>{o.id}</td>
                <td style={{ padding: "14px 16px", color: "var(--text)", fontWeight: 500 }}>{o.customer}</td>
                <td style={{ padding: "14px 16px", textAlign: "center", color: "var(--muted)" }}>{o.items}</td>
                <td style={{ padding: "14px 16px", textAlign: "right", fontWeight: 700, color: "var(--text)" }}>{fmt(o.total)}</td>
                <td style={{ padding: "14px 16px", textAlign: "center", fontSize: 12, color: "var(--muted)" }}>{o.payment}</td>
                <td style={{ padding: "14px 16px", textAlign: "center" }}><Badge status={o.status} /></td>
                <td style={{ padding: "14px 20px", textAlign: "center", color: "var(--muted)", fontSize: 12 }}>{o.date}</td>
                <td style={{ padding: "14px 20px", textAlign: "center" }}>
                  <button onClick={() => setSelected(o)} style={{ background: "var(--bg)", border: "1px solid var(--border)",
                    borderRadius: 8, padding: "6px 12px", cursor: "pointer", color: "#6366f1", fontSize: 12, fontWeight: 600 }}>
                    Xem
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
 
      {selected && (
        <Modal title={`Chi tiết đơn ${selected.id}`} onClose={() => setSelected(null)}>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 20 }}>
            {[
              ["Khách hàng", selected.customer], ["Ngày đặt", selected.date],
              ["Số sản phẩm", selected.items], ["Thanh toán", selected.payment],
              ["Tổng tiền", fmt(selected.total)], ["Trạng thái", <Badge status={selected.status} />],
            ].map(([k, v]) => (
              <div key={k}>
                <div style={{ fontSize: 11, fontWeight: 700, color: "var(--muted)", textTransform: "uppercase", letterSpacing: "0.05em", marginBottom: 4 }}>{k}</div>
                <div style={{ fontSize: 14, fontWeight: 600, color: "var(--text)" }}>{v}</div>
              </div>
            ))}
          </div>
          <Field label="Cập nhật trạng thái">
            <Select value={selected.status} onChange={s => updateStatus(selected.id, s)}
              options={["pending","processing","shipped","delivered","cancelled"].map(s => ({ value: s, label: STATUS_CONFIG[s]?.label || s }))} />
          </Field>
          <div style={{ display: "flex", justifyContent: "flex-end", marginTop: 8 }}>
            <Btn variant="secondary" onClick={() => setSelected(null)}>Đóng</Btn>
          </div>
        </Modal>
      )}
    </div>
  );
};

export default Orders;