const Modal = ({ title, onClose, children }) => (
  <div style={{
    position: "fixed", inset: 0, background: "rgba(0,0,0,.45)", zIndex: 1000,
    display: "flex", alignItems: "center", justifyContent: "center",
  }} onClick={onClose}>
    <div onClick={e => e.stopPropagation()} style={{
      background: "var(--card)", borderRadius: 18, padding: 28, width: 480, maxWidth: "90vw",
      boxShadow: "0 24px 60px rgba(0,0,0,.25)", border: "1px solid var(--border)",
    }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 22 }}>
        <h3 style={{ margin: 0, fontSize: 17, fontWeight: 700, color: "var(--text)" }}>{title}</h3>
        <button onClick={onClose} style={{ background: "none", border: "none", cursor: "pointer",
          color: "var(--muted)", fontSize: 20, lineHeight: 1 }}>×</button>
      </div>
      {children}
    </div>
  </div>
);

export default Modal;