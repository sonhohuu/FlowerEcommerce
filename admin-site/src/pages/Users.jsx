import { useState, useEffect, useCallback } from "react";
import SearchBar from "../components/ui/SearchBar";
import Badge from "../components/ui/Badge";
import { fmt } from "../utils/format";
import { userApi } from "../api/userApi";

const PAGE_SIZE = 20;

const roleColors = {
  Admin: "#6366f1",
  Moderator: "#8b5cf6",
  Customer: "#6b7280",
};
const roleLabels = {
  Admin: "Admin",
  Moderator: "Moderator",
  Customer: "Khách hàng",
};

// ── Avatar từ chữ cái đầu username ──────────────────────
const Avatar = ({ name }) => (
  <div
    style={{
      width: 36,
      height: 36,
      borderRadius: "50%",
      background: "#6366f1",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      color: "#fff",
      fontSize: 14,
      fontWeight: 700,
      flexShrink: 0,
    }}
  >
    {name?.[0]?.toUpperCase() ?? "?"}
  </div>
);

// ── Pagination ───────────────────────────────────────────
const Pagination = ({ page, totalPages, onChange }) => {
  if (totalPages <= 1) return null;

  const pages = [];
  const delta = 2;
  const left = Math.max(2, page - delta);
  const right = Math.min(totalPages - 1, page + delta);

  pages.push(1);
  if (left > 2) pages.push("...");
  for (let i = left; i <= right; i++) pages.push(i);
  if (right < totalPages - 1) pages.push("...");
  if (totalPages > 1) pages.push(totalPages);

  const btnStyle = (active) => ({
    minWidth: 34,
    height: 34,
    borderRadius: 8,
    border: "1px solid var(--border)",
    background: active ? "#6366f1" : "var(--card)",
    color: active ? "#fff" : "var(--text)",
    fontWeight: active ? 700 : 500,
    fontSize: 13,
    cursor: "pointer",
    transition: "all .15s",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    padding: "0 6px",
  });

  return (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        gap: 6,
        justifyContent: "center",
        padding: "20px 0 4px",
      }}
    >
      <button
        style={{ ...btnStyle(false), opacity: page === 1 ? 0.4 : 1 }}
        disabled={page === 1}
        onClick={() => onChange(page - 1)}
      >
        ←
      </button>

      {pages.map((p, i) =>
        p === "..." ? (
          <span
            key={`ellipsis-${i}`}
            style={{ color: "var(--muted)", padding: "0 4px" }}
          >
            …
          </span>
        ) : (
          <button
            key={p}
            style={btnStyle(p === page)}
            onClick={() => onChange(p)}
          >
            {p}
          </button>
        ),
      )}

      <button
        style={{ ...btnStyle(false), opacity: page === totalPages ? 0.4 : 1 }}
        disabled={page === totalPages}
        onClick={() => onChange(page + 1)}
      >
        →
      </button>
    </div>
  );
};

// ── Main component ────────────────────────────────────────
const Users = () => {
  const [users, setUsers] = useState([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // ── Fetch ──────────────────────────────────────────────
  const fetchUsers = useCallback(async (p = 1, q = "") => {
    setLoading(true);
    setError(null);
    try {
      const res = await userApi.getAll({
        page: p,
        pageSize: PAGE_SIZE,
        search: q,
      });

      // Handle both: raw wrapper { success, data } OR pre-unwrapped data object
      const payload =
        res?.success !== undefined
          ? res.success
            ? res.data
            : null // wrapper present
          : (res?.data ?? res); // already unwrapped by api.get

      if (payload?.items) {
        setUsers(payload.items);
        setTotal(payload.total ?? 0);
        setTotalPages(payload.totalPages ?? 1);
      } else {
        setError(res?.message || "Không thể tải dữ liệu.");
      }
    } catch (e) {
      setError("Lỗi kết nối máy chủ.");
    } finally {
      setLoading(false);
    }
  }, []);

  // Search debounce
  useEffect(() => {
    const t = setTimeout(() => {
      setPage(1);
      fetchUsers(1, search);
    }, 400);
    return () => clearTimeout(t);
  }, [search, fetchUsers]);

  // Page change
  useEffect(() => {
    fetchUsers(page, search);
  }, [page]); // eslint-disable-line

  // ── Toggle status ──────────────────────────────────────
  const toggleStatus = async (id, currentStatus) => {
    const next = currentStatus === "Active" ? "Inactive" : "Active";
    // Optimistic update
    setUsers((prev) =>
      prev.map((u) => (u.id === id ? { ...u, status: next } : u)),
    );
    try {
      await userApi.toggleStatus(id, next);
    } catch {
      // Rollback nếu API lỗi
      setUsers((prev) =>
        prev.map((u) => (u.id === id ? { ...u, status: currentStatus } : u)),
      );
    }
  };

  // ── Stats từ page hiện tại (server đã filter) ─────────
  const activeCount = users.filter((u) => u.status === "Active").length;
  const inactiveCount = users.filter((u) => u.status === "Inactive").length;

  return (
    <div>
      {/* Header */}
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 24,
        }}
      >
        <div>
          <h1
            style={{
              fontSize: 22,
              fontWeight: 800,
              color: "var(--text)",
              margin: 0,
            }}
          >
            Người dùng
          </h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>
            {total} tài khoản
          </p>
        </div>
        <SearchBar
          value={search}
          onChange={setSearch}
          placeholder="Tìm người dùng..."
        />
      </div>

      {/* Stats */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 1fr)",
          gap: 12,
          marginBottom: 24,
        }}
      >
        {[
          { label: "Tổng tài khoản", value: total, color: "#6366f1" },
          { label: "Đang hoạt động", value: activeCount, color: "#16a34a" },
          { label: "Vô hiệu hoá", value: inactiveCount, color: "#dc2626" },
        ].map(({ label, value, color }) => (
          <div
            key={label}
            style={{
              background: "var(--card)",
              borderRadius: 14,
              padding: "18px 22px",
              border: "1px solid var(--border)",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <span
              style={{ fontSize: 13, color: "var(--muted)", fontWeight: 500 }}
            >
              {label}
            </span>
            <span style={{ fontSize: 24, fontWeight: 800, color }}>
              {value}
            </span>
          </div>
        ))}
      </div>

      {/* Table */}
      <div
        style={{
          background: "var(--card)",
          borderRadius: 16,
          border: "1px solid var(--border)",
          overflow: "hidden",
        }}
      >
        {/* Loading / Error */}
        {loading && (
          <div
            style={{
              padding: 40,
              textAlign: "center",
              color: "var(--muted)",
              fontSize: 14,
            }}
          >
            Đang tải...
          </div>
        )}
        {!loading && error && (
          <div
            style={{
              padding: 40,
              textAlign: "center",
              color: "#dc2626",
              fontSize: 14,
            }}
          >
            {error}
          </div>
        )}
        {!loading && !error && users.length === 0 && (
          <div
            style={{
              padding: 40,
              textAlign: "center",
              color: "var(--muted)",
              fontSize: 14,
            }}
          >
            Không tìm thấy người dùng nào.
          </div>
        )}

        {!loading && !error && users.length > 0 && (
          <table
            style={{
              width: "100%",
              borderCollapse: "collapse",
              fontSize: 13,
              tableLayout: "fixed",
            }}
          >
            <colgroup>
              <col style={{ width: "22%" }} /> {/* Người dùng */}
              <col style={{ width: "15%" }} /> {/* Vai trò */}
              <col style={{ width: "13%" }} /> {/* Đơn hàng */}
              <col style={{ width: "18%" }} /> {/* Tổng chi */}
              <col style={{ width: "17%" }} /> {/* Trạng thái */}
              <col style={{ width: "15%" }} /> {/* Thao tác */}
            </colgroup>

            <thead style={{ background: "var(--bg)" }}>
              <tr
                style={{
                  color: "var(--muted)",
                  fontSize: 11,
                  fontWeight: 700,
                  textTransform: "uppercase",
                  letterSpacing: "0.05em",
                }}
              >
                <th style={{ textAlign: "left", padding: "14px 20px" }}>
                  Người dùng
                </th>
                <th style={{ textAlign: "left", padding: "14px 20px" }}>
                  Vai trò
                </th>
                <th style={{ textAlign: "center", padding: "14px 20px" }}>
                  Đơn hàng
                </th>
                <th style={{ textAlign: "right", padding: "14px 20px" }}>
                  Tổng chi
                </th>
                <th style={{ textAlign: "center", padding: "14px 20px" }}>
                  Trạng thái
                </th>
                <th style={{ textAlign: "center", padding: "14px 20px" }}>
                  Thao tác
                </th>
              </tr>
            </thead>

            <tbody>
              {users.map((u) => (
                <tr key={u.id} style={{ borderTop: "1px solid var(--border)" }}>
                  {/* Avatar + name */}
                  <td style={{ padding: "14px 20px" }}>
                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                        gap: 12,
                        minWidth: 0,
                      }}
                    >
                      <Avatar name={u.userName} />
                      <div style={{ minWidth: 0 }}>
                        {" "}
                        {/* ← quan trọng để ellipsis hoạt động */}
                        <div
                          style={{
                            fontWeight: 600,
                            color: "var(--text)",
                            overflow: "hidden",
                            textOverflow: "ellipsis",
                            whiteSpace: "nowrap",
                          }}
                        >
                          {u.userName}
                        </div>
                        <div
                          style={{
                            fontSize: 11,
                            color: "var(--muted)",
                            overflow: "hidden",
                            textOverflow: "ellipsis",
                            whiteSpace: "nowrap",
                          }}
                        >
                          {u.email}
                        </div>
                      </div>
                    </div>
                  </td>

                  {/* Role badge */}
                  <td style={{ padding: "14px 20px" }}>
                    <span
                      style={{
                        fontSize: 11,
                        fontWeight: 700,
                        padding: "3px 10px",
                        borderRadius: 99,
                        background: `${roleColors[u.role] ?? "#6b7280"}18`,
                        color: roleColors[u.role] ?? "#6b7280",
                      }}
                    >
                      {roleLabels[u.role] ?? u.role}
                    </span>
                  </td>

                  {/* Order count */}
                  <td
                    style={{
                      padding: "14px 20px",
                      textAlign: "center",
                      color: "var(--text)",
                      fontWeight: 600,
                    }}
                  >
                    {u.orderCount}
                  </td>

                  {/* Total spent */}
                  <td
                    style={{
                      padding: "14px 20px",
                      textAlign: "right",
                      fontWeight: 700,
                      color: "var(--text)",
                    }}
                  >
                    {u.totalSpent > 0 ? fmt(u.totalSpent) : "—"}
                  </td>

                  {/* Status badge */}
                  <td style={{ padding: "14px 20px", textAlign: "center" }}>
                    <Badge
                      status={u.status === "Active" ? "active" : "inactive"}
                    />
                  </td>

                  {/* Toggle action */}
                  <td style={{ padding: "14px 20px", textAlign: "center" }}>
                    <button
                      onClick={() => toggleStatus(u.id, u.status)}
                      style={{
                        background:
                          u.status === "Active" ? "#fee2e2" : "#dcfce7",
                        border: `1px solid ${u.status === "Active" ? "#fca5a5" : "#86efac"}`,
                        color: u.status === "Active" ? "#dc2626" : "#16a34a",
                        borderRadius: 8,
                        padding: "5px 14px",
                        cursor: "pointer",
                        fontSize: 11,
                        fontWeight: 700,
                        whiteSpace: "nowrap",
                      }}
                    >
                      {u.status === "Active" ? "Khoá" : "Mở khoá"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {/* Pagination */}
        {!loading && totalPages > 1 && (
          <div
            style={{
              borderTop: "1px solid var(--border)",
              padding: "12px 20px",
            }}
          >
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <span style={{ fontSize: 12, color: "var(--muted)" }}>
                Trang {page} / {totalPages} &nbsp;·&nbsp; {total} người dùng
              </span>
              <Pagination
                page={page}
                totalPages={totalPages}
                onChange={setPage}
              />
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Users;
