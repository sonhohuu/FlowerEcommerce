namespace FlowerEcommerce.Domain.Entities;
public class FileAttachment : ModificationAuditedEntity
{
    // ── Cloudinary fields ─────────────────────────────────
    public string PublicId { get; set; } = string.Empty;
    public string SecureUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public long Bytes { get; set; }

    // ── App metadata ──────────────────────────────────────
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public string? AltText { get; set; }
}
