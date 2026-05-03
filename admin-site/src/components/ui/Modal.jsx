const Modal = ({ title, onClose, children }) => (
  <div
    style={{
      position: "fixed", inset: 0,
      background: "rgba(0,0,0,.45)",
      zIndex: 1000,
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      padding: "16px",          // ← tránh modal chạm mép màn hình
    }}
    onClick={onClose}
  >
    <div
      onClick={e => e.stopPropagation()}
      style={{
        background: "var(--card)",
        borderRadius: 18,
        width: 500,
        maxWidth: "100%",
        maxHeight: "90vh",       // ← giới hạn chiều cao
        display: "flex",
        flexDirection: "column", // ← header cố định, content scroll
        boxShadow: "0 24px 60px rgba(0,0,0,.25)",
        border: "1px solid var(--border)",
      }}
    >
      {/* Header — cố định, không scroll */}
      <div style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "20px 24px 16px",
        borderBottom: "1px solid var(--border)",
        flexShrink: 0,           // ← không bị co
      }}>
        <h3 style={{ margin: 0, fontSize: 17, fontWeight: 700, color: "var(--text)" }}>
          {title}
        </h3>
        <button
          onClick={onClose}
          style={{
            background: "none", border: "none", cursor: "pointer",
            color: "var(--muted)", fontSize: 22, lineHeight: 1,
            padding: "0 4px",
          }}
        >×</button>
      </div>

      {/* Body — scroll được */}
      <div style={{
        overflowY: "auto",
        padding: "20px 24px 24px",
        flex: 1,
      }}>
        {children}
      </div>
    </div>
  </div>
);

export default Modal;