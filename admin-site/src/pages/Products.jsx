import { useState } from "react";
import { MOCK_PRODUCTS, MOCK_CATEGORIES } from "../data/mockData";
import SearchBar from "../components/ui/SearchBar";
import Btn from "../components/ui/Button";
import Modal from "../components/ui/Modal";
import Field from "../components/common/Field";
import Input from "../components/ui/Input";
import Select from "../components/ui/Select";
import { fmt } from "../utils/format";
import Badge from "../components/ui/Badge";
import Icon from "../components/common/Icon";

const Products = () => {
  const [search, setSearch] = useState("");
  const [modal, setModal] = useState(null); // null | "add" | "edit"
  const [selected, setSelected] = useState(null);
  const [products, setProducts] = useState(MOCK_PRODUCTS);
  const [form, setForm] = useState({ name: "", category: "Áo", price: "", stock: "", status: "active" });
 
  const filtered = products.filter(p =>
    p.name.toLowerCase().includes(search.toLowerCase()) ||
    p.category.toLowerCase().includes(search.toLowerCase())
  );
 
  const openAdd = () => { setForm({ name: "", category: "Áo", price: "", stock: "", status: "active" }); setModal("add"); };
  const openEdit = (p) => { setSelected(p); setForm({ name: p.name, category: p.category, price: p.price, stock: p.stock, status: p.status }); setModal("edit"); };
  const handleSave = () => {
    if (modal === "add") {
      setProducts(prev => [...prev, { ...form, id: Date.now(), price: +form.price, stock: +form.stock, image: "📦" }]);
    } else {
      setProducts(prev => prev.map(p => p.id === selected.id ? { ...p, ...form, price: +form.price, stock: +form.stock } : p));
    }
    setModal(null);
  };
  const handleDelete = (id) => setProducts(prev => prev.filter(p => p.id !== id));
 
  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 24 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, color: "var(--text)", margin: 0 }}>Sản phẩm</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>{products.length} sản phẩm</p>
        </div>
        <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
          <SearchBar value={search} onChange={setSearch} placeholder="Tìm sản phẩm..." />
          <Btn onClick={openAdd}><Icon name="plus" size={14} /> Thêm sản phẩm</Btn>
        </div>
      </div>
 
      <div style={{ background: "var(--card)", borderRadius: 16, border: "1px solid var(--border)", overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead style={{ background: "var(--bg)" }}>
            <tr style={{ color: "var(--muted)", fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              <th style={{ textAlign: "left", padding: "14px 20px" }}>Sản phẩm</th>
              <th style={{ textAlign: "left", padding: "14px 16px" }}>Danh mục</th>
              <th style={{ textAlign: "right", padding: "14px 16px" }}>Giá</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Tồn kho</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Trạng thái</th>
              <th style={{ textAlign: "center", padding: "14px 20px" }}>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map(p => (
              <tr key={p.id} style={{ borderTop: "1px solid var(--border)" }}>
                <td style={{ padding: "14px 20px" }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
                    <div style={{ width: 40, height: 40, borderRadius: 10, background: "var(--bg)",
                      display: "flex", alignItems: "center", justifyContent: "center", fontSize: 20 }}>{p.image}</div>
                    <span style={{ fontWeight: 600, color: "var(--text)" }}>{p.name}</span>
                  </div>
                </td>
                <td style={{ padding: "14px 16px", color: "var(--muted)" }}>{p.category}</td>
                <td style={{ padding: "14px 16px", textAlign: "right", fontWeight: 700, color: "var(--text)" }}>{fmt(p.price)}</td>
                <td style={{ padding: "14px 16px", textAlign: "center", color: p.stock === 0 ? "#dc2626" : p.stock < 20 ? "#d97706" : "var(--text)", fontWeight: 600 }}>{p.stock}</td>
                <td style={{ padding: "14px 16px", textAlign: "center" }}><Badge status={p.status} /></td>
                <td style={{ padding: "14px 20px" }}>
                  <div style={{ display: "flex", gap: 8, justifyContent: "center" }}>
                    <button onClick={() => openEdit(p)} style={{ background: "var(--bg)", border: "1px solid var(--border)",
                      borderRadius: 8, padding: "6px 10px", cursor: "pointer", color: "#6366f1" }}><Icon name="edit" size={13} /></button>
                    <button onClick={() => handleDelete(p.id)} style={{ background: "#fee2e2", border: "1px solid #fca5a5",
                      borderRadius: 8, padding: "6px 10px", cursor: "pointer", color: "#dc2626" }}><Icon name="trash" size={13} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
 
      {modal && (
        <Modal title={modal === "add" ? "Thêm sản phẩm" : "Sửa sản phẩm"} onClose={() => setModal(null)}>
          <Field label="Tên sản phẩm"><Input value={form.name} onChange={v => setForm(f => ({ ...f, name: v }))} placeholder="Nhập tên..." /></Field>
          <Field label="Danh mục">
            <Select value={form.category} onChange={v => setForm(f => ({ ...f, category: v }))}
              options={MOCK_CATEGORIES.map(c => ({ value: c.name, label: c.name }))} />
          </Field>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <Field label="Giá (₫)"><Input type="number" value={form.price} onChange={v => setForm(f => ({ ...f, price: v }))} placeholder="0" /></Field>
            <Field label="Tồn kho"><Input type="number" value={form.stock} onChange={v => setForm(f => ({ ...f, stock: v }))} placeholder="0" /></Field>
          </div>
          <Field label="Trạng thái">
            <Select value={form.status} onChange={v => setForm(f => ({ ...f, status: v }))}
              options={[{ value: "active", label: "Hoạt động" }, { value: "out_of_stock", label: "Hết hàng" }, { value: "low_stock", label: "Sắp hết" }]} />
          </Field>
          <div style={{ display: "flex", gap: 10, justifyContent: "flex-end", marginTop: 8 }}>
            <Btn variant="secondary" onClick={() => setModal(null)}>Huỷ</Btn>
            <Btn onClick={handleSave}>Lưu</Btn>
          </div>
        </Modal>
      )}
    </div>
  );
};

export default Products;