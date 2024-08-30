using Catalog.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class RegisterMap : IEntityTypeConfiguration<RegisterDTO>
{
    public void Configure(EntityTypeBuilder<RegisterDTO> builder)
    {
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(70);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);

        //INDEX
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.UserName)
            .IsUnique();
    }
}
