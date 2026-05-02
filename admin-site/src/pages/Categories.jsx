import { useState } from "react";
import { MOCK_CATEGORIES } from "../data/mockData";
import Btn from "../components/ui/Button";
import Modal from "../components/ui/Modal";
import Field from "../components/common/Field";
import Input from "../components/ui/Input";
import Icon from "../components/common/Icon";

const Categories = () => {
  const [categories, setCategories] = useState(MOCK_CATEGORIES);
  const [modal, setModal] = useState(false);
  const [form, setForm] = useState({ name: "", slug: "", icon: "📦" });
 
  const handleAdd = () => {
    setCategories(prev => [...prev, { ...form, id: Date.now(), productCount: 0, color: "#6366f1" }]);
    setModal(false);
  };
  const handleDelete = (id) => setCategories(prev => prev.filter(c => c.id !== id));
 
  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 24 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, color: "var(--text)", margin: 0 }}>Thể loại</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>{categories.length} danh mục</p>
        </div>
        <Btn onClick={() => setModal(true)}><Icon name="plus" size={14} /> Thêm thể loại</Btn>
      </div>
 
      <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 16 }}>
        {categories.map(cat => (
          <div key={cat.id} style={{ background: "var(--card)", borderRadius: 16, padding: 22,
            border: "1px solid var(--border)", position: "relative", overflow: "hidden" }}>
            <div style={{ position: "absolute", top: -20, right: -20, width: 80, height: 80,
              borderRadius: "50%", background: `${cat.color}15` }} />
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
              <div style={{ width: 48, height: 48, borderRadius: 14, background: `${cat.color}15`,
                display: "flex", alignItems: "center", justifyContent: "center", fontSize: 24, marginBottom: 12 }}>
                {cat.icon}
              </div>
              <button onClick={() => handleDelete(cat.id)} style={{ background: "none", border: "none",
                cursor: "pointer", color: "var(--muted)", opacity: 0.5 }}><Icon name="trash" size={14} /></button>
            </div>
            <h3 style={{ margin: "0 0 4px", fontSize: 16, fontWeight: 700, color: "var(--text)" }}>{cat.name}</h3>
            <p style={{ margin: "0 0 14px", fontSize: 12, color: "var(--muted)" }}>/{cat.slug}</p>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <span style={{ fontSize: 22, fontWeight: 800, color: cat.color }}>{cat.productCount}</span>
              <span style={{ fontSize: 12, color: "var(--muted)" }}>sản phẩm</span>
            </div>
            <div style={{ marginTop: 12, height: 4, borderRadius: 99, background: "var(--border)" }}>
              <div style={{ height: "100%", borderRadius: 99, background: cat.color,
                width: `${Math.min((cat.productCount / 35) * 100, 100)}%` }} />
            </div>
          </div>
        ))}
      </div>
 
      {modal && (
        <Modal title="Thêm thể loại" onClose={() => setModal(false)}>
          <Field label="Tên thể loại"><Input value={form.name} onChange={v => setForm(f => ({ ...f, name: v }))} placeholder="VD: Áo khoác" /></Field>
          <Field label="Slug"><Input value={form.slug} onChange={v => setForm(f => ({ ...f, slug: v }))} placeholder="VD: ao-khoac" /></Field>
          <Field label="Icon (emoji)"><Input value={form.icon} onChange={v => setForm(f => ({ ...f, icon: v }))} placeholder="📦" /></Field>
          <div style={{ display: "flex", gap: 10, justifyContent: "flex-end", marginTop: 8 }}>
            <Btn variant="secondary" onClick={() => setModal(false)}>Huỷ</Btn>
            <Btn onClick={handleAdd}>Thêm</Btn>
          </div>
        </Modal>
      )}
    </div>
  );
};

export default Categories;