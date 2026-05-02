import Icon from "../common/Icon";

const StatCard = ({ icon, label, value, delta, deltaLabel, accent }) => (
  <div style={{
    background: "var(--card)", 
    borderRadius: 16, 
    padding: "22px 24px",
    border: "1px solid var(--border)", 
    position: "relative", 
    overflow: "hidden",
  }}>
    <div style={{ position: "absolute", top: 0, right: 0, width: 80, height: 80,
      background: `${accent}18`, borderRadius: "0 16px 0 80px" }} />
    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: 12 }}>
      <span style={{ fontSize: 13, color: "var(--muted)", fontWeight: 500 }}>{label}</span>
      <div style={{ width: 36, height: 36, borderRadius: 10, background: `${accent}20`,
        display: "flex", alignItems: "center", justifyContent: "center", color: accent }}>
        <Icon name={icon} size={18} />
      </div>
    </div>
    <div style={{ fontSize: 28, fontWeight: 800, color: "var(--text)", letterSpacing: "-0.5px", marginBottom: 6 }}>{value}</div>
    <div style={{ fontSize: 12, color: delta > 0 ? "#16a34a" : delta < 0 ? "#dc2626" : "var(--muted)", fontWeight: 500 }}>
      {delta > 0 ? "▲" : delta < 0 ? "▼" : "→"} {Math.abs(delta)}% {deltaLabel}
    </div>
  </div>
);

export default StatCard;