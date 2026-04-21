using FlowerEcommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerEcommerce.Infrastructure.Persistence.Database.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
