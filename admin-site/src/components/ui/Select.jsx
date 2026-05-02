const Select = ({ value, onChange, options }) => (
  <select value={value} onChange={e => onChange(e.target.value)}
    style={{
      width: "100%", height: 40, padding: "0 12px", borderRadius: 10, border: "1px solid var(--border)",
      background: "var(--bg)", color: "var(--text)", fontSize: 13, outline: "none", fontFamily: "inherit",
    }}>
    {options.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
  </select>
);

export default Select;