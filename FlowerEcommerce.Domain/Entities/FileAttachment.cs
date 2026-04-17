using FlowerEcommerce.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class FileAttachment : CreationAuditedEntity
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string? ThumbnailPath { get; set; }
        public string? ContentType { get; set; }
        public long Size { get; set; }
        public string? PublicUrl { get; set; } = null!;
    }
}
