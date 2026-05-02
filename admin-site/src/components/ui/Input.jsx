const Input = ({ value, onChange, placeholder, type = "text" }) => (
  <input type={type} value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder}
    style={{
      width: "100%", height: 40, padding: "0 12px", borderRadius: 10, border: "1px solid var(--border)",
      background: "var(--bg)", color: "var(--text)", fontSize: 13, outline: "none", fontFamily: "inherit", boxSizing: "border-box",
    }} />
);

export default Input;