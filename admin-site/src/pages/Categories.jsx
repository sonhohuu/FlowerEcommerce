import { useState } from "react";
import { useCategories } from "../hooks/useCategories";
import { categoryApi } from "../api/categoryApi";
import Btn from "../components/ui/Button";
import Modal from "../components/ui/Modal";
import Field from "../components/common/Field";
import Input from "../components/ui/Input";
import Icon from "../components/common/Icon";

const COLORS = ["#6366f1", "#8b5cf6", "#ec4899", "#f59e0b", "#10b981", "#3b82f6", "#ef4444", "#14b8a6"];

const emptyForm = { name: "" };

export default function Categories() {
  const { categories, loading, error, refetch } = useCategories();

  const [modal, setModal]       = useState(null);
  const [selected, setSelected] = useState(null);
  const [form, setForm]         = useState(emptyForm);
  const [saving, setSaving]     = useState(false);

  const openAdd = () => {
    setForm(emptyForm);
    setSelected(null);
    setModal("add");
  };

  const openEdit = (cat) => {
    setSelected(cat);
    setForm({ name: cat.name });
    setModal("edit");
  };

  const openDelete = (cat) => {
    setSelected(cat);
    setModal("delete");
  };

  const closeModal = () => {
    setModal(null);
    setForm(emptyForm);
    setSelected(null);
  };

  const handleSave = async () => {
    if (!form.name.trim()) return alert("Vui lòng nhập tên danh mục");
    setSaving(true);
    try {
      if (modal === "add") {
        await categoryApi.create({ name: form.name.trim() });
      } else {
        await categoryApi.update(selected.id, { name: form.name.trim() });
      }
      await refetch();
      closeModal();
    } catch (e) {
      alert("Lỗi: " + e.message);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!selected) return;
    setSaving(true);
    try {
      await categoryApi.delete(selected.id);
      await refetch();
      closeModal();
    } catch (e) {
      alert("Lỗi xoá: " + e.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) return (
    <div style={{ padding: 60, textAlign: "center", color: "var(--muted)" }}>Đang tải...</div>
  );
  if (error) return (
    <div style={{ padding: 60, textAlign: "center", color: "#dc2626" }}>Lỗi: {error}</div>
  );

  return (
    <div>
      {/* Header */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 24 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, color: "var(--text)", margin: 0 }}>Thể loại</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>
            {categories.length} danh mục
          </p>
        </div>
        <Btn onClick={openAdd}><Icon name="plus" size={14} /> Thêm thể loại</Btn>
      </div>

      {/* Grid */}
      {categories.length === 0 ? (
        <div style={{ padding: 60, textAlign: "center", color: "var(--muted)" }}>
          Chưa có danh mục nào
        </div>
      ) : (
        <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 16 }}>
          {categories.map((cat, idx) => {
            const color = COLORS[idx % COLORS.length];
            return (
              <div key={cat.id} style={{
                background: "var(--card)", borderRadius: 16,
                border: "1px solid var(--border)",
                position: "relative",
                overflow: "hidden",
              }}>
                
                {/* Content */}
                <div style={{ padding: 22, position: "relative", zIndex: 1 }}>
                  {/* Icon + actions */}
                  <div style={{
                    display: "flex", justifyContent: "space-between",
                    alignItems: "flex-start", marginBottom: 12,
                  }}>
                    <div style={{
                      width: 48, height: 48, borderRadius: 14,
                      background: `${color}18`,
                      display: "flex", alignItems: "center", justifyContent: "center",
                      fontSize: 22, fontWeight: 800, color,
                    }}>
                      {cat.name.charAt(0).toUpperCase()}
                    </div>

                    <div style={{ display: "flex", gap: 6 }}>
                      <button
                        onClick={() => openEdit(cat)}
                        style={{
                          background: "var(--bg)", border: "1px solid var(--border)",
                          borderRadius: 8, padding: "5px 8px",
                          cursor: "pointer", color: "#6366f1",
                          display: "flex", alignItems: "center", justifyContent: "center",
                        }}
                      >
                        <Icon name="edit" size={13} />
                      </button>
                      <button
                        onClick={() => openDelete(cat)}
                        style={{
                          background: "#fee2e2", border: "1px solid #fca5a5",
                          borderRadius: 8, padding: "5px 8px",
                          cursor: "pointer", color: "#dc2626",
                          display: "flex", alignItems: "center", justifyContent: "center",
                        }}
                      >
                        <Icon name="trash" size={13} />
                      </button>
                    </div>
                  </div>

                  {/* Info */}
                  <h3 style={{ margin: "0 0 4px", fontSize: 16, fontWeight: 700, color: "var(--text)" }}>
                    {cat.name}
                  </h3>
                  <p style={{ margin: 0, fontSize: 12, color: "var(--muted)" }}>
                    /{cat.slug}
                  </p>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* Modal thêm / sửa */}
      {(modal === "add" || modal === "edit") && (
        <Modal
          title={modal === "add" ? "Thêm danh mục" : `Sửa: ${selected?.name}`}
          onClose={closeModal}
        >
          <Field label="Tên danh mục *">
            <Input
              value={form.name}
              onChange={v => setForm(f => ({ ...f, name: v }))}
              placeholder="VD: Cần Câu Lure"
            />
          </Field>
          <p style={{ fontSize: 12, color: "var(--muted)", margin: "0 0 16px" }}>
            Slug sẽ được tự động tạo từ tên bởi server.
          </p>
          <div style={{ display: "flex", gap: 10, justifyContent: "flex-end" }}>
            <Btn variant="secondary" onClick={closeModal}>Huỷ</Btn>
            <Btn onClick={handleSave} disabled={saving}>
              {saving ? "Đang lưu..." : modal === "add" ? "Thêm" : "Lưu"}
            </Btn>
          </div>
        </Modal>
      )}

      {/* Modal xác nhận xoá */}
      {modal === "delete" && selected && (
        <Modal title="Xác nhận xoá" onClose={closeModal}>
          <p style={{ color: "var(--text)", fontSize: 14, margin: "0 0 6px" }}>
            Bạn có chắc muốn xoá danh mục
          </p>
          <p style={{ color: "#dc2626", fontWeight: 700, fontSize: 16, margin: "0 0 12px" }}>
            "{selected.name}"?
          </p>
          <p style={{ color: "var(--muted)", fontSize: 12, margin: "0 0 24px" }}>
            Hành động này không thể hoàn tác.
          </p>
          <div style={{ display: "flex", gap: 10, justifyContent: "flex-end" }}>
            <Btn variant="secondary" onClick={closeModal}>Huỷ</Btn>
            <Btn variant="danger" onClick={handleDelete} disabled={saving}>
              {saving ? "Đang xoá..." : "Xoá"}
            </Btn>
          </div>
        </Modal>
      )}
    </div>
  );
}