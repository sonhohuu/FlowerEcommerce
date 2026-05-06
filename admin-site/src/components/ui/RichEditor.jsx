// components/ui/RichEditor.jsx
import { useEditor, EditorContent } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import { useEffect } from "react";

const toolbarBtn = (active, onClick, label) => (
  <button
    type="button"
    onClick={onClick}
    style={{
      padding: "4px 10px",
      borderRadius: 6,
      border: "1px solid var(--border)",
      background: active ? "#6366f1" : "var(--bg)",
      color: active ? "#fff" : "var(--text)",
      cursor: "pointer",
      fontSize: 13,
      fontWeight: 600,
    }}
  >
    {label}
  </button>
);

export default function RichEditor({ value, onChange }) {
  const editor = useEditor({
    extensions: [StarterKit],
    content: value || "",
    onUpdate: ({ editor }) => {
      // Trả về HTML string — lưu thẳng vào DB
      onChange(editor.getHTML());
    },
  });

  // Sync khi value thay đổi từ bên ngoài (ví dụ: load edit)
  useEffect(() => {
    if (editor && value !== editor.getHTML()) {
      editor.commands.setContent(value || "");
    }
  }, [value]);

  if (!editor) return null;

  return (
    <div style={{
      border: "1px solid var(--border)",
      borderRadius: 10,
      overflow: "hidden",
      background: "var(--bg)",
    }}>
      {/* Toolbar */}
      <div style={{
        display: "flex", gap: 6, padding: "8px 10px",
        borderBottom: "1px solid var(--border)",
        background: "var(--card)", flexWrap: "wrap",
      }}>
        {toolbarBtn(editor.isActive("bold"), () => editor.chain().focus().toggleBold().run(), "B")}
        {toolbarBtn(editor.isActive("italic"), () => editor.chain().focus().toggleItalic().run(), "I")}
        {toolbarBtn(editor.isActive("bulletList"), () => editor.chain().focus().toggleBulletList().run(), "• List")}
        {toolbarBtn(editor.isActive("orderedList"), () => editor.chain().focus().toggleOrderedList().run(), "1. List")}
        {toolbarBtn(false, () => editor.chain().focus().setHardBreak().run(), "↵ BR")}
      </div>

      {/* Editor area */}
      <EditorContent
        editor={editor}
        style={{ padding: "10px 12px", minHeight: 120, fontSize: 13, color: "var(--text)" }}
      />
    </div>
  );
}