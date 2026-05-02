const Btn = ({ children, variant = "primary", onClick, small }) => {
  const styles = {
    primary: { background: "#6366f1", color: "#fff", border: "none" },
    secondary: { background: "var(--card)", color: "var(--text)", border: "1px solid var(--border)" },
    danger: { background: "#fee2e2", color: "#dc2626", border: "1px solid #fca5a5" },
  };
  return (
    <button onClick={onClick} style={{
      ...styles[variant], display: "inline-flex", alignItems: "center", gap: 6,
      padding: small ? "6px 14px" : "9px 18px", borderRadius: 10, fontSize: small ? 12 : 13,
      fontWeight: 600, cursor: "pointer", fontFamily: "inherit", transition: "opacity .15s",
    }}
      onMouseEnter={e => e.currentTarget.style.opacity = "0.85"}
      onMouseLeave={e => e.currentTarget.style.opacity = "1"}
    >{children}</button>
  );
};

export default Btn;