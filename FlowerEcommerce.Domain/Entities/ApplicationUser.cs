using FlowerEcommerce.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace FlowerEcommerce.Domain.Entities
{
    public class ApplicationUser : IdentityUser<ulong>, IDeletionAuditedEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;

        public string FullName
        {
            get
            {
                var fullName = $"{LastName} {FirstName}";
                if (string.IsNullOrWhiteSpace(fullName)) fullName = UserName ?? Email ?? string.Empty;

                return fullName.Trim();
            }
        }

        public IList<Order> Orders { get; set; } = [];
        
        public IList<ProductRating> Ratings { get; set; } = [];

        #region Auditing Properties

        public ApplicationUser? Creator { get; set; }
        public ApplicationUser? LastModifier { get; set; }
        public ulong? CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ulong? LastModifierId { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        public bool IsDeleted => DeletedAt != null;
        public ulong? DeleterId { get; set; }
        public DateTime? DeletedAt { get; set; }

        #endregion
    }
}
