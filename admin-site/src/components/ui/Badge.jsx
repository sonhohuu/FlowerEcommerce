import { STATUS_CONFIG } from "../../constants/statusConfig";

const Badge = ({ status }) => {
  const cfg = STATUS_CONFIG[status] || { label: status, bg: "#f3f4f6", color: "#6b7280" };
  return (
    <span style={{
      display: "inline-block", 
      padding: "2px 10px", 
      borderRadius: 99,
      fontSize: 11, 
      fontWeight: 600, 
      letterSpacing: "0.03em",
      background: cfg.bg, 
      color: cfg.color,
    }}>{cfg.label}</span>
  );
};

export default Badge;