import { useState } from "react";
import SearchBar from "../components/ui/SearchBar";
import Btn from "../components/ui/Button";
import Modal from "../components/ui/Modal";
import Field from "../components/common/Field";
import Input from "../components/ui/Input";
import Select from "../components/ui/Select";
import { fmt } from "../utils/format";
import Badge from "../components/ui/Badge";
import Icon from "../components/common/Icon";
import { useProducts } from "../hooks/useProducts";
import { useCategories } from "../hooks/useCategories";
import { productApi } from "../api/productApi";

// ── Empty form state ───────────────────────────────────────────────
const emptyForm = {
  name:            "",
  description:     "",
  price:           "",
  originalPrice:   "",
  isContactPrice:  false,
  sku:             "",
  categoryId:      "",
  sizePrices:      [],   // [{ label: "", price: "" }]
  fileAttachMents: [],   // File[]
  previewUrls:     [],   // string[] — chỉ để preview UI
};

export default function Products() {
  const [search, setSearch]     = useState("");
  const [modal, setModal]       = useState(null); // null | "add" | "edit"
  const [selected, setSelected] = useState(null);
  const [form, setForm]         = useState(emptyForm);
  const [saving, setSaving]     = useState(false);

  const {
    items, total, totalPages, loading, error, params, setParams, refetch,
  } = useProducts();
  const { categories } = useCategories();

  // ── Search ─────────────────────────────────────────────────────
  const handleSearch = (val) => {
    setSearch(val);
    setParams(p => ({ ...p, search: val, page: 1 }));
  };

  // ── Open modal ─────────────────────────────────────────────────
  const openAdd = () => {
    setForm({ ...emptyForm, categoryId: categories[0]?.id || "" });
    setSelected(null);
    setModal("add");
  };

  const openEdit = (p) => {
    setSelected(p);
    setForm({
      ...emptyForm,
      name:           p.name           || "",
      description:    p.description    || "",
      price:          p.price          ?? "",
      originalPrice:  p.originalPrice  ?? "",
      isContactPrice: p.isContactPrice || false,
      sku:            p.sku            || "",
      categoryId:     p.categoryId     || "",
      sizePrices:     (p.sizePrices    || []).map(sp => ({
        label: sp.label || sp.Label || "",
        price: sp.price ?? sp.Price ?? "",
      })),
      // fileAttachMents để trống — chỉ upload ảnh mới khi muốn thay
    });
    setModal("edit");
  };

  const closeModal = () => {
    // Giải phóng object URLs tránh memory leak
    form.previewUrls.forEach(url => URL.revokeObjectURL(url));
    setForm(emptyForm);
    setModal(null);
  };

  // ── File handling ──────────────────────────────────────────────
  const handleFileChange = (e) => {
    const files    = Array.from(e.target.files);
    const previews = files.map(f => URL.createObjectURL(f));
    setForm(f => ({
      ...f,
      fileAttachMents: [...f.fileAttachMents, ...files],
      previewUrls:     [...f.previewUrls,     ...previews],
    }));
    // Reset input để có thể chọn lại cùng file
    e.target.value = "";
  };

  const removeFile = (i) => {
    setForm(f => {
      URL.revokeObjectURL(f.previewUrls[i]);
      return {
        ...f,
        fileAttachMents: f.fileAttachMents.filter((_, idx) => idx !== i),
        previewUrls:     f.previewUrls.filter((_, idx) => idx !== i),
      };
    });
  };

  // ── SizePrice handling ─────────────────────────────────────────
  const addSizePrice = () =>
    setForm(f => ({ ...f, sizePrices: [...f.sizePrices, { label: "", price: "" }] }));

  const removeSizePrice = (i) =>
    setForm(f => ({ ...f, sizePrices: f.sizePrices.filter((_, idx) => idx !== i) }));

  const updateSizePrice = (i, field, val) =>
    setForm(f => ({
      ...f,
      sizePrices: f.sizePrices.map((sp, idx) =>
        idx === i ? { ...sp, [field]: val } : sp
      ),
    }));

  // ── Save ───────────────────────────────────────────────────────
  const handleSave = async () => {
    if (!form.name.trim()) return alert("Vui lòng nhập tên sản phẩm");
    if (!form.isContactPrice && !form.price) return alert("Vui lòng nhập giá bán");

    setSaving(true);
    try {
      const payload = {
        name:            form.name.trim(),
        description:     form.description.trim(),
        price:           form.isContactPrice ? 0 : +form.price,
        originalPrice:   form.originalPrice !== "" ? +form.originalPrice : undefined,
        isContactPrice:  form.isContactPrice,
        sku:             form.sku.trim(),
        categoryId:      form.categoryId || undefined,
        sizePrices:      form.sizePrices
                           .filter(sp => sp.label.trim() && sp.price !== "")
                           .map(sp => ({ label: sp.label.trim(), price: +sp.price })),
        fileAttachMents: form.fileAttachMents, // File[]
      };

      if (modal === "add") await productApi.create(payload);
      else                 await productApi.update(selected.id, payload);

      refetch();
      closeModal();
    } catch (e) {
      alert("Lỗi: " + e.message);
    } finally {
      setSaving(false);
    }
  };

  // ── Delete ─────────────────────────────────────────────────────
  const handleDelete = async (id) => {
    if (!confirm("Xoá sản phẩm này?")) return;
    try {
      await productApi.delete(id);
      refetch();
    } catch (e) {
      alert("Lỗi xoá: " + e.message);
    }
  };

  // ── Render ─────────────────────────────────────────────────────
  if (loading) return (
    <div style={{ padding: 60, textAlign: "center", color: "var(--muted)" }}>
      Đang tải...
    </div>
  );
  if (error) return (
    <div style={{ padding: 60, textAlign: "center", color: "#dc2626" }}>
      Lỗi kết nối: {error}
    </div>
  );

  return (
    <div>
      {/* ── Header ── */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 24 }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 800, margin: 0, color: "var(--text)" }}>Sản phẩm</h1>
          <p style={{ color: "var(--muted)", margin: "4px 0 0", fontSize: 13 }}>{total} sản phẩm</p>
        </div>
        <div style={{ display: "flex", gap: 10 }}>
          <SearchBar value={search} onChange={handleSearch} placeholder="Tìm sản phẩm..." />
          <Btn onClick={openAdd}><Icon name="plus" size={14} /> Thêm sản phẩm</Btn>
        </div>
      </div>

      {/* ── Table ── */}
      <div style={{ background: "var(--card)", borderRadius: 16, border: "1px solid var(--border)", overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead style={{ background: "var(--bg)" }}>
            <tr style={{ color: "var(--muted)", fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              <th style={{ textAlign: "left",   padding: "14px 20px" }}>Sản phẩm</th>
              <th style={{ textAlign: "left",   padding: "14px 16px" }}>Danh mục</th>
              <th style={{ textAlign: "right",  padding: "14px 16px" }}>Giá bán</th>
              <th style={{ textAlign: "right",  padding: "14px 16px" }}>Giá gốc</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Tồn kho</th>
              <th style={{ textAlign: "center", padding: "14px 16px" }}>Hiển thị</th>
              <th style={{ textAlign: "center", padding: "14px 20px" }}>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {items.length === 0 ? (
              <tr>
                <td colSpan={7} style={{ padding: 40, textAlign: "center", color: "var(--muted)" }}>
                  Không có sản phẩm nào
                </td>
              </tr>
            ) : items.map(p => (
              <tr key={p.id} style={{ borderTop: "1px solid var(--border)" }}>
                {/* Tên + ảnh */}
                <td style={{ padding: "12px 20px" }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
                    {p.mainImage?.secureUrl ? (
                      <img
                        src={p.mainImage.secureUrl}
                        alt={p.name}
                        style={{ width: 46, height: 46, objectFit: "cover",
                          borderRadius: 10, border: "1px solid var(--border)", flexShrink: 0 }}
                        onError={e => { e.target.style.display = "none"; }}
                      />
                    ) : (
                      <div style={{ width: 46, height: 46, borderRadius: 10,
                        background: "var(--bg)", border: "1px solid var(--border)",
                        display: "flex", alignItems: "center", justifyContent: "center",
                        fontSize: 20, flexShrink: 0 }}>📦</div>
                    )}
                    <div>
                      <div style={{ fontWeight: 600, color: "var(--text)" }}>{p.name}</div>
                      <div style={{ fontSize: 11, color: "var(--muted)", marginTop: 2 }}>/{p.slug}</div>
                    </div>
                  </div>
                </td>
                {/* Danh mục */}
                <td style={{ padding: "12px 16px", color: "var(--muted)", fontSize: 12 }}>
                  {categories.find(c => c.id === p.categoryId)?.name || "—"}
                </td>
                {/* Giá bán */}
                <td style={{ padding: "12px 16px", textAlign: "right", fontWeight: 700, color: "#6366f1" }}>
                  {p.isContactPrice ? (
                    <span style={{ fontSize: 11, background: "#ede9fe", color: "#7c3aed",
                      padding: "2px 8px", borderRadius: 99, fontWeight: 600 }}>Liên hệ</span>
                  ) : fmt(p.price)}
                </td>
                {/* Giá gốc */}
                <td style={{ padding: "12px 16px", textAlign: "right",
                  color: "var(--muted)", textDecoration: "line-through", fontSize: 12 }}>
                  {p.originalPrice ? fmt(p.originalPrice) : "—"}
                </td>
                {/* Tồn kho */}
                <td style={{ padding: "12px 16px", textAlign: "center" }}>
                  <Badge status={p.isOutOfStock ? "out_of_stock" : "active"} />
                </td>
                {/* Hiển thị */}
                <td style={{ padding: "12px 16px", textAlign: "center" }}>
                  <Badge status={p.status ? "active" : "inactive"} />
                </td>
                {/* Thao tác */}
                <td style={{ padding: "12px 20px" }}>
                  <div style={{ display: "flex", gap: 8, justifyContent: "center" }}>
                    <button onClick={() => openEdit(p)}
                      style={{ background: "var(--bg)", border: "1px solid var(--border)",
                        borderRadius: 8, padding: "6px 10px", cursor: "pointer", color: "#6366f1" }}>
                      <Icon name="edit" size={13} />
                    </button>
                    <button onClick={() => handleDelete(p.id)}
                      style={{ background: "#fee2e2", border: "1px solid #fca5a5",
                        borderRadius: 8, padding: "6px 10px", cursor: "pointer", color: "#dc2626" }}>
                      <Icon name="trash" size={13} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* ── Pagination ── */}
      {totalPages > 1 && (
        <div style={{ display: "flex", justifyContent: "flex-end", gap: 8, marginTop: 16 }}>
          {Array.from({ length: totalPages }, (_, i) => i + 1).map(p => (
            <button key={p} onClick={() => setParams(prev => ({ ...prev, page: p }))}
              style={{
                width: 34, height: 34, borderRadius: 8, border: "1px solid var(--border)",
                background: params.page === p ? "#6366f1" : "var(--card)",
                color:      params.page === p ? "#fff"    : "var(--text)",
                cursor: "pointer", fontWeight: 600, fontSize: 13,
              }}>{p}</button>
          ))}
        </div>
      )}

      {/* ── Modal thêm / sửa ── */}
      {modal && (
        <Modal
          title={modal === "add" ? "Thêm sản phẩm" : `Sửa: ${selected?.name}`}
          onClose={closeModal}
        >
          {/* Tên */}
          <Field label="Tên sản phẩm *">
            <Input
              value={form.name}
              onChange={v => setForm(f => ({ ...f, name: v }))}
              placeholder="Nhập tên sản phẩm..."
            />
          </Field>

          {/* Mô tả */}
          <Field label="Mô tả">
            <textarea
              value={form.description}
              onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
              placeholder="Nhập mô tả..."
              rows={3}
              style={{
                width: "100%", padding: "10px 12px", borderRadius: 10,
                border: "1px solid var(--border)", background: "var(--bg)",
                color: "var(--text)", fontSize: 13, fontFamily: "inherit",
                resize: "vertical", boxSizing: "border-box", outline: "none",
              }}
            />
          </Field>

          {/* Danh mục + SKU */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <Field label="Danh mục">
              <Select
                value={form.categoryId}
                onChange={v => setForm(f => ({ ...f, categoryId: +v }))}
                options={[
                  { value: "", label: "— Chọn danh mục —" },
                  ...categories.map(c => ({ value: c.id, label: c.name })),
                ]}
              />
            </Field>
            <Field label="SKU">
              <Input
                value={form.sku}
                onChange={v => setForm(f => ({ ...f, sku: v }))}
                placeholder="VD: SP-001"
              />
            </Field>
          </div>

          {/* Giá bán + Giá gốc */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <Field label="Giá bán (₫)">
              <Input
                type="number"
                value={form.price}
                onChange={v => setForm(f => ({ ...f, price: v }))}
                placeholder="0"
              />
            </Field>
            <Field label="Giá gốc (₫)">
              <Input
                type="number"
                value={form.originalPrice}
                onChange={v => setForm(f => ({ ...f, originalPrice: v }))}
                placeholder="0"
              />
            </Field>
          </div>

          {/* Checkboxes */}
          <div style={{ display: "flex", gap: 20, marginBottom: 18 }}>
            {[
              { key: "isContactPrice", label: "Giá liên hệ" },
              { key: "isOutOfStock",   label: "Hết hàng"    },
              { key: "status",         label: "Hiển thị"    },
            ].map(({ key, label }) => (
              <label key={key}
                style={{ display: "flex", alignItems: "center", gap: 6,
                  fontSize: 13, cursor: "pointer", color: "var(--text)" }}>
                <input
                  type="checkbox"
                  checked={!!form[key]}
                  onChange={e => setForm(f => ({ ...f, [key]: e.target.checked }))}
                  style={{ accentColor: "#6366f1", width: 14, height: 14 }}
                />
                {label}
              </label>
            ))}
          </div>

          {/* ── Ảnh sản phẩm (multiple) ── */}
          <Field label="Ảnh sản phẩm">
            <label style={{
              display: "inline-flex", alignItems: "center", gap: 8,
              padding: "9px 16px", border: "1px dashed var(--border)",
              borderRadius: 10, cursor: "pointer", fontSize: 13,
              color: "var(--muted)", transition: "border-color .15s",
            }}
              onMouseEnter={e => e.currentTarget.style.borderColor = "#6366f1"}
              onMouseLeave={e => e.currentTarget.style.borderColor = "var(--border)"}
            >
              📎 Chọn ảnh (nhiều file)
              <input
                type="file"
                accept="image/*"
                multiple
                style={{ display: "none" }}
                onChange={handleFileChange}
              />
            </label>

            {/* Preview grid */}
            {form.previewUrls.length > 0 && (
              <div style={{ display: "flex", flexWrap: "wrap", gap: 8, marginTop: 10 }}>
                {form.previewUrls.map((url, i) => (
                  <div key={i} style={{ position: "relative" }}>
                    <img src={url} alt={`preview-${i}`}
                      style={{ width: 72, height: 72, objectFit: "cover",
                        borderRadius: 10, border: "1px solid var(--border)", display: "block" }}
                    />
                    <button onClick={() => removeFile(i)} style={{
                      position: "absolute", top: -6, right: -6,
                      width: 20, height: 20, borderRadius: "50%",
                      background: "#dc2626", color: "#fff", border: "2px solid var(--card)",
                      cursor: "pointer", fontSize: 11, lineHeight: 1,
                      display: "flex", alignItems: "center", justifyContent: "center",
                      padding: 0,
                    }}>×</button>
                    {i === 0 && (
                      <span style={{
                        position: "absolute", bottom: 0, left: 0, right: 0,
                        background: "rgba(99,102,241,.85)", color: "#fff",
                        fontSize: 9, fontWeight: 700, textAlign: "center",
                        borderRadius: "0 0 8px 8px", padding: "2px 0",
                      }}>CHÍNH</span>
                    )}
                  </div>
                ))}
              </div>
            )}

            <p style={{ fontSize: 11, color: "var(--muted)", marginTop: 6, marginBottom: 0 }}>
              Ảnh đầu tiên sẽ là ảnh chính. Hỗ trợ JPG, PNG, WEBP.
            </p>
          </Field>

          {/* ── Size / Price variants ── */}
          <Field label="Phân loại theo size (tuỳ chọn)">
            {form.sizePrices.map((sp, i) => (
              <div key={i} style={{ display: "flex", gap: 8, marginBottom: 8, alignItems: "center" }}>
                <input
                  placeholder="Label (VD: S, M, XL, 1kg...)"
                  value={sp.label}
                  onChange={e => updateSizePrice(i, "label", e.target.value)}
                  style={{
                    flex: 1, height: 38, padding: "0 12px", borderRadius: 10,
                    border: "1px solid var(--border)", background: "var(--bg)",
                    color: "var(--text)", fontSize: 13, fontFamily: "inherit", outline: "none",
                  }}
                />
                <input
                  placeholder="Giá (₫)"
                  type="number"
                  value={sp.price}
                  onChange={e => updateSizePrice(i, "price", e.target.value)}
                  style={{
                    width: 130, height: 38, padding: "0 12px", borderRadius: 10,
                    border: "1px solid var(--border)", background: "var(--bg)",
                    color: "var(--text)", fontSize: 13, fontFamily: "inherit", outline: "none",
                  }}
                />
                <button onClick={() => removeSizePrice(i)} style={{
                  background: "#fee2e2", border: "1px solid #fca5a5",
                  borderRadius: 8, width: 34, height: 34, cursor: "pointer",
                  color: "#dc2626", fontSize: 16, display: "flex",
                  alignItems: "center", justifyContent: "center", flexShrink: 0,
                }}>×</button>
              </div>
            ))}
            <button onClick={addSizePrice} style={{
              width: "100%", background: "none", border: "1px dashed var(--border)",
              borderRadius: 10, padding: "8px 0", cursor: "pointer",
              color: "var(--muted)", fontSize: 12, fontWeight: 600,
              marginTop: form.sizePrices.length > 0 ? 4 : 0,
              fontFamily: "inherit",
            }}>+ Thêm size</button>
          </Field>

          {/* Actions */}
          <div style={{ display: "flex", gap: 10, justifyContent: "flex-end", marginTop: 8 }}>
            <Btn variant="secondary" onClick={closeModal}>Huỷ</Btn>
            <Btn onClick={handleSave} disabled={saving}>
              {saving ? "Đang lưu..." : "Lưu sản phẩm"}
            </Btn>
          </div>
        </Modal>
      )}
    </div>
  );
}