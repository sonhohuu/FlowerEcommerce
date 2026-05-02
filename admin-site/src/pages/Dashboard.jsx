import { useState } from "react";
import { MOCK_CATEGORIES, MOCK_ORDERS } from "../data/mockData";
import StatCard from "../components/ui/StatCard";
import { fmt } from "../utils/format";
import Badge from "../components/ui/Badge";

const Dashboard = () => {
  const revenueData = [120, 180, 140, 220, 190, 280, 310, 260, 340, 290, 380, 420];
  const months = ["T1","T2","T3","T4","T5","T6","T7","T8","T9","T10","T11","T12"];
  const maxVal = Math.max(...revenueData);
 
  return (
    <div>
      <div style={{ marginBottom: 28 }}>
        <h1 style={{ fontSize: 26, fontWeight: 800, color: "var(--text)", margin: 0, letterSpacing: "-0.5px" }}>
          Tổng quan <span style={{ color: "#6366f1" }}>Dashboard</span>
        </h1>
        <p style={{ color: "var(--muted)", margin: "6px 0 0", fontSize: 14 }}>
          Thứ Bảy · 02 tháng 5, 2026 · Dữ liệu cập nhật theo thời gian thực
        </p>
      </div>
 
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 16, marginBottom: 24 }}>
        <StatCard icon="chart" label="Doanh thu tháng" value="124M₫" delta={10} deltaLabel="tuần trước" accent="#6366f1" />
        <StatCard icon="orders" label="Đơn hàng" value="2,847" delta={7} deltaLabel="tuần trước" accent="#8b5cf6" />
        <StatCard icon="users" label="Người dùng" value="842" delta={-2} deltaLabel="tuần trước" accent="#ec4899" />
        <StatCard icon="products" label="Sản phẩm" value="114" delta={0} deltaLabel="không đổi" accent="#f59e0b" />
      </div>
 
      <div style={{ display: "grid", gridTemplateColumns: "2fr 1fr", gap: 16, marginBottom: 16 }}>
        {/* Revenue Chart */}
        <div style={{ background: "var(--card)", borderRadius: 16, padding: 24, border: "1px solid var(--border)" }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
            <div>
              <h3 style={{ margin: 0, fontSize: 15, fontWeight: 700, color: "var(--text)" }}>Doanh thu theo tháng</h3>
              <p style={{ margin: "4px 0 0", fontSize: 12, color: "var(--muted)" }}>Năm 2026</p>
            </div>
            <span style={{ fontSize: 12, color: "#6366f1", fontWeight: 600 }}>+18% YoY</span>
          </div>
          <div style={{ display: "flex", alignItems: "flex-end", gap: 8, height: 140 }}>
            {revenueData.map((v, i) => (
              <div key={i} style={{ flex: 1, display: "flex", flexDirection: "column", alignItems: "center", gap: 6 }}>
                <div style={{
                  width: "100%", height: `${(v / maxVal) * 120}px`,
                  background: i === 4 ? "#6366f1" : "var(--border)",
                  borderRadius: "4px 4px 0 0", transition: "background .2s",
                  minHeight: 4, cursor: "pointer",
                }}
                  onMouseEnter={e => e.currentTarget.style.background = "#6366f1"}
                  onMouseLeave={e => e.currentTarget.style.background = i === 4 ? "#6366f1" : "var(--border)"}
                />
                <span style={{ fontSize: 9, color: "var(--muted)", fontWeight: 500 }}>{months[i]}</span>
              </div>
            ))}
          </div>
        </div>
 
        {/* Top Categories */}
        <div style={{ background: "var(--card)", borderRadius: 16, padding: 24, border: "1px solid var(--border)" }}>
          <h3 style={{ margin: "0 0 18px", fontSize: 15, fontWeight: 700, color: "var(--text)" }}>Danh mục nổi bật</h3>
          {MOCK_CATEGORIES.map((cat) => (
            <div key={cat.id} style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 14 }}>
              <span style={{ fontSize: 20 }}>{cat.icon}</span>
              <div style={{ flex: 1 }}>
                <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 4 }}>
                  <span style={{ fontSize: 13, fontWeight: 600, color: "var(--text)" }}>{cat.name}</span>
                  <span style={{ fontSize: 12, color: "var(--muted)" }}>{cat.productCount} SP</span>
                </div>
                <div style={{ height: 4, borderRadius: 99, background: "var(--border)" }}>
                  <div style={{ height: "100%", borderRadius: 99, background: cat.color,
                    width: `${(cat.productCount / 35) * 100}%` }} />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
 
      {/* Recent Orders */}
      <div style={{ background: "var(--card)", borderRadius: 16, padding: 24, border: "1px solid var(--border)" }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 18 }}>
          <h3 style={{ margin: 0, fontSize: 15, fontWeight: 700, color: "var(--text)" }}>Đơn hàng gần đây</h3>
          <span style={{ fontSize: 12, color: "#6366f1", fontWeight: 600, cursor: "pointer" }}>Xem tất cả →</span>
        </div>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead>
            <tr style={{ color: "var(--muted)", fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              <th style={{ textAlign: "left", padding: "0 0 12px" }}>Mã đơn</th>
              <th style={{ textAlign: "left", padding: "0 0 12px" }}>Khách hàng</th>
              <th style={{ textAlign: "right", padding: "0 0 12px" }}>Tổng tiền</th>
              <th style={{ textAlign: "center", padding: "0 0 12px" }}>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            {MOCK_ORDERS.slice(0, 5).map(o => (
              <tr key={o.id} style={{ borderTop: "1px solid var(--border)" }}>
                <td style={{ padding: "12px 0", fontWeight: 700, color: "#6366f1" }}>{o.id}</td>
                <td style={{ padding: "12px 0", color: "var(--text)" }}>{o.customer}</td>
                <td style={{ padding: "12px 0", textAlign: "right", fontWeight: 700, color: "var(--text)" }}>{fmt(o.total)}</td>
                <td style={{ padding: "12px 0", textAlign: "center" }}><Badge status={o.status} /></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Dashboard;