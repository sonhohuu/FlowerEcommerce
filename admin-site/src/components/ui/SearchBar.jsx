import Icon from "../common/Icon";

const SearchBar = ({ value, onChange, placeholder }) => (
  <div style={{ position: "relative" }}>
    <span style={{ position: "absolute", left: 12, top: "50%", transform: "translateY(-50%)", color: "var(--muted)" }}>
      <Icon name="search" size={15} />
    </span>
    <input value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder}
      style={{
        paddingLeft: 36, paddingRight: 14, height: 38, borderRadius: 10, border: "1px solid var(--border)",
        background: "var(--bg)", color: "var(--text)", fontSize: 13, outline: "none", width: 220,
        fontFamily: "inherit",
      }} />
  </div>
);

export default SearchBar;