import { useState, useEffect, useCallback } from "react";
import SearchBar from "../components/ui/SearchBar";
import Modal from "../components/ui/Modal";
import Field from "../components/common/Field";
import Select from "../components/ui/Select";
import Btn from "../components/ui/Button";
import { fmt } from "../utils/format";
import { orderApi } from "../api/orderApi";  // ✅ bỏ khoảng trắng thừa

// ── Status config khớp enum server ───────────────────────────────────────────
const STATUS_CONFIG = {
  Confirming:   { label: "Chờ xác nhận", color: "#f59e0b", bg: "#fef3c7" },
  Processing:   { label: "Đang xử lý",   color: "#6366f1", bg: "#ede9fe" },
  OnDelivering: { label: "Đang giao",    color: "#0ea5e9", bg: "#e0f2fe" },
  Success:      { label: "Thành công",   color: "#16a34a", bg: "#dcfce7" },
  Failed:       { label: "Thất bại",     color: "#dc2626", bg: "#fee2e2" },
};

const STATUSES = ["all", ...Object.keys(STATUS_CONFIG)];

const COLS = [
  { width: "20%" }, // Mã đơn
  { width: "17%" }, // Khách hàng
  { width: "8%"  }, // SP
  { width: "13%" }, // Tổng tiền
  { width: "11%" }, // Thanh toán
  { width: "14%" }, // Trạng thái
  { width: "10%" }, // Ngày
  { width: "7%"  }, // Chi tiết
];

// ── Status Badge ──────────────────────────────────────────────────────────────
const StatusBadge = ({ status }) => {
  const cfg = STATUS_CONFIG[status] ?? { label: status, color: "#6b7280", bg: "#f3f4f6" };
  return (
    <span style={{
      fontSize: 11, fontWeight: 700, padding: "3px 10px",
      borderRadius: 99, background: cfg.bg, color: cfg.color, whiteSpace: "nowrap",
    }}>{cfg.label}</span>
  );
};

// ── Pagination ────────────────────────────────────────────────────────────────
const Pagination = ({ page, totalPages, onChange }) => {
  if (totalPages <= 1) return null;

  const pages = [];
  const left  = Math.max(2, page - 2);
  const right = Math.min(totalPages - 1, page + 2);
  pages.push(1);
  if (left > 2) pages.push("...");
  for (let i = left; i <= right; i++) pages.push(i);
  if (right < totalPages - 1) pages.push("...");
  if (totalPages > 1) pages.push(totalPages);

  const btn = (active, disabled) => ({
    minWidth: 34, height: 34, borderRadius: 8, border: "1px solid var(--border)",
    background: active ? "#6366f1" : "var(--card)",
    color: active ? "#fff" : "var(--text)",
    fontWeight: active ? 700 : 500, fontSize: 13,
    cursor: disabled ? "not-allowed" : "pointer",
    opacity: disabled ? 0.4 : 1,
    display: "flex", alignItems: "center", justifyContent: "center", padding: "0 6px",
  });

  return (
    <div style={{ display: "flex", alignItems: "center", gap: 6 }}>
      <button style={btn(false, page === 1)} disabled={page === 1} onClick={() => onChange(page - 1)}>←</button>
      {pages.map((p, i) =>
        p === "..." ? (
          <span key={`e-${i}`} style={{ color: "var(--muted)", padding: "0 4px" }}>…</span>
        ) : (
          <button key={p} style={btn(p === page, false)} onClick={() => onChange(p)}>{p}</button>
        )
      )}
      <button style={btn(false, page === totalPages)} disabled={page === totalPages} onClick={() => onChange(page + 1)}>→</button>
    </div>
  );
};

// ── Main ──────────────────────────────────────────────────────────────────────
const Orders = () => {
  const [orders,       setOrders]       = useState([]);
  const [search,       setSearch]       = useState("");
  const [filterStatus, setFilterStatus] = useState("all");
  const [page,         setPage]         = useState(1);
  const [totalPages,   setTotalPages]   = useState(1);
  const [total,        setTotal]        = useState(0);
  const [loading,      setLoading]      = useState(false);
  const [error,        setError]        = useState(null);
  const [selected,     setSelected]     = useState(null);
  const [updating,     setUpdating]     = useState(false);

  // ── Fetch ─────────────────────────────────────────────────────────────────
  const fetchOrders = useCallback(async (p = 1, q = "", status = "all") => {
    setLoading(true);
    setError(null);
    try {
      const payload = await orderApi.getAll({ page: p, size: 10, search: q, status });
      if (payload?.items) {
        setOrders(payload.items);
        setTotal(payload.total ?? 0);
        setTotalPages(payload.totalPages ?? 1);
      } else {
        setError("Không thể tải dữ liệu.");
      }
    } catch {
      setError("Lỗi kết nối máy chủ.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    const t = setTimeout(() => { setPage(1); fetchOrders(1, search, filterStatus); }, 400);
    return () => clearTimeout(t);
  }, [search, filterStatus, fetchOrders]);

  useEffect(() => { fetchOrders(page, search, filterStatus); }, [page]); // eslint-disable-line

  // ── Update status via orderApi.update ─────────────────────────────────────
  const handleUpdateStatus = async (id, newStatus) => {
    setUpdating(true);
    try {
      await orderApi.update(id, { status: newStatus }); // ✅ dùng update chung
      setOrders(prev => prev.map(o => o.id === id ? { ...o, status: newStatus } : o));
      setSelected(prev => prev ? { ...prev, status: newStatus } : null);
    } catch {
      alert("Cập nhật trạng thái thất bại.");
    } finally {
      setUpdating(false);
    }
  };

  return (
    <div>
      {/* Header */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, color: "var(--text)", margin: 0 }}>Đơn hàng</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>{total} đơn hàng</p>
        </div>
        <SearchBar value={search} onChange={setSearch} placeholder="Tìm mã đơn, khách hàng..." />
      </div>

      {/* Filter tabs */}
      <div style={{ display: "flex", gap: 8, marginBottom: 20, flexWrap: "wrap" }}>
        {STATUSES.map(s => (
          <button key={s} onClick={() => { setFilterStatus(s); setPage(1); }} style={{
            padding: "7px 16px", borderRadius: 99, fontSize: 12, fontWeight: 600, cursor: "pointer",
            border: filterStatus === s ? "none" : "1px solid var(--border)",
            background: filterStatus === s ? "#6366f1" : "var(--card)",
            color: filterStatus === s ? "#fff" : "var(--muted)",
            transition: "all .15s",
          }}>
            {s === "all" ? "Tất cả" : STATUS_CONFIG[s]?.label ?? s}
          </button>
        ))}
      </div>

      {/* Table */}
      <div style={{ background: "var(--card)", borderRadius: 16, border: "1px solid var(--border)", overflow: "hidden" }}>
        {loading && (
          <div style={{ padding: 40, textAlign: "center", color: "var(--muted)", fontSize: 14 }}>Đang tải...</div>
        )}
        {!loading && error && (
          <div style={{ padding: 40, textAlign: "center", color: "#dc2626", fontSize: 14 }}>{error}</div>
        )}
        {!loading && !error && orders.length === 0 && (
          <div style={{ padding: 40, textAlign: "center", color: "var(--muted)", fontSize: 14 }}>Không tìm thấy đơn hàng nào.</div>
        )}

        {!loading && !error && orders.length > 0 && (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13, tableLayout: "fixed" }}>
            <colgroup>{COLS.map((c, i) => <col key={i} style={c} />)}</colgroup>
            <thead style={{ background: "var(--bg)" }}>
              <tr style={{ color: "var(--muted)", fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.05em" }}>
                <th style={{ textAlign: "left",   padding: "14px 20px" }}>Mã đơn</th>
                <th style={{ textAlign: "left",   padding: "14px 16px" }}>Khách hàng</th>
                <th style={{ textAlign: "center", padding: "14px 16px" }}>SP</th>
                <th style={{ textAlign: "right",  padding: "14px 16px" }}>Tổng tiền</th>
                <th style={{ textAlign: "center", padding: "14px 16px" }}>Thanh toán</th>
                <th style={{ textAlign: "center", padding: "14px 16px" }}>Trạng thái</th>
                <th style={{ textAlign: "center", padding: "14px 16px" }}>Ngày</th>
                <th style={{ textAlign: "center", padding: "14px 20px" }}>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(o => (
                <tr key={o.id} style={{ borderTop: "1px solid var(--border)" }}>
                  <td style={{ padding: "14px 20px" }}>
                    <div style={{ fontWeight: 700, color: "#6366f1", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}
                      title={o.orderCode}>{o.orderCode}</div>
                  </td>
                  <td style={{ padding: "14px 16px" }}>
                    <div style={{ fontWeight: 600, color: "var(--text)", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>{o.customerName}</div>
                    <div style={{ fontSize: 11, color: "var(--muted)" }}>{o.phoneNumber}</div>
                  </td>
                  <td style={{ padding: "14px 16px", textAlign: "center", color: "var(--muted)", fontWeight: 600 }}>
                    {o.details?.length ?? 0}
                  </td>
                  <td style={{ padding: "14px 16px", textAlign: "right", fontWeight: 700, color: "var(--text)" }}>
                    {fmt(o.totalAmount)}
                  </td>
                  <td style={{ padding: "14px 16px", textAlign: "center", fontSize: 12, color: "var(--muted)" }}>
                    {o.paymentMethod}
                  </td>
                  <td style={{ padding: "14px 16px", textAlign: "center" }}>
                    <StatusBadge status={o.status} />
                  </td>
                  <td style={{ padding: "14px 16px", textAlign: "center", color: "var(--muted)", fontSize: 12 }}>
                    {o.orderDate}
                  </td>
                  <td style={{ padding: "14px 20px", textAlign: "center" }}>
                    <button onClick={() => setSelected(o)} style={{
                      background: "var(--bg)", border: "1px solid var(--border)",
                      borderRadius: 8, padding: "6px 12px", cursor: "pointer",
                      color: "#6366f1", fontSize: 12, fontWeight: 600,
                    }}>Xem</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {!loading && totalPages > 1 && (
          <div style={{ borderTop: "1px solid var(--border)", padding: "12px 20px", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <span style={{ fontSize: 12, color: "var(--muted)" }}>
              Trang {page} / {totalPages} &nbsp;·&nbsp; {total} đơn hàng
            </span>
            <Pagination page={page} totalPages={totalPages} onChange={setPage} />
          </div>
        )}
      </div>

      {/* Modal chi tiết */}
      {selected && (
        <Modal title={`Chi tiết đơn ${selected.orderCode}`} onClose={() => setSelected(null)}>
          {/* Thông tin */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 14, marginBottom: 20 }}>
            {[
              ["Khách hàng", selected.customerName],
              ["SĐT",        selected.phoneNumber],
              ["Ngày đặt",   selected.orderDate],
              ["Thanh toán", selected.paymentMethod],
              ["Địa chỉ",    selected.address],
              ["Ghi chú",    selected.note || "—"],
              ["Tổng tiền",  fmt(selected.totalAmount)],
              ["Trạng thái", <StatusBadge key="st" status={selected.status} />],
            ].map(([k, v]) => (
              <div key={k}>
                <div style={{ fontSize: 11, fontWeight: 700, color: "var(--muted)", textTransform: "uppercase", letterSpacing: "0.05em", marginBottom: 4 }}>{k}</div>
                <div style={{ fontSize: 13, fontWeight: 600, color: "var(--text)" }}>{v}</div>
              </div>
            ))}
          </div>

          {/* Sản phẩm */}
          <div style={{ marginBottom: 20 }}>
            <div style={{ fontSize: 11, fontWeight: 700, color: "var(--muted)", textTransform: "uppercase", letterSpacing: "0.05em", marginBottom: 10 }}>
              Sản phẩm ({selected.details?.length ?? 0})
            </div>
            <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
              {selected.details?.map((d, i) => (
                <div key={i} style={{
                  display: "flex", alignItems: "center", gap: 12,
                  padding: "10px 14px", borderRadius: 10,
                  background: "var(--bg)", border: "1px solid var(--border)",
                }}>
                  <img src={d.productImage} alt={d.productName}
                    style={{ width: 48, height: 48, borderRadius: 8, objectFit: "cover", flexShrink: 0 }}
                    onError={e => { e.target.style.display = "none"; }}
                  />
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ fontWeight: 600, color: "var(--text)", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                      {d.productName}
                    </div>
                    <div style={{ fontSize: 12, color: "var(--muted)" }}>
                      {d.label} &nbsp;·&nbsp; SL: {d.quantity}
                    </div>
                  </div>
                  <div style={{ fontWeight: 700, color: "var(--text)", flexShrink: 0 }}>{fmt(d.price)}</div>
                </div>
              ))}
            </div>
          </div>

          {/* Cập nhật trạng thái */}
          <Field label="Cập nhật trạng thái">
            <Select
              value={selected.status}
              onChange={s => handleUpdateStatus(selected.id, s)}
              options={Object.entries(STATUS_CONFIG).map(([value, { label }]) => ({ value, label }))}
              disabled={updating}
            />
          </Field>

          <div style={{ display: "flex", justifyContent: "flex-end", marginTop: 12 }}>
            <Btn variant="secondary" onClick={() => setSelected(null)}>Đóng</Btn>
          </div>
        </Modal>
      )}
    </div>
  );
};

export default Orders;