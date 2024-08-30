using Catalog.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class LoginMap : IEntityTypeConfiguration<LoginDTO>
{
    public void Configure(EntityTypeBuilder<LoginDTO> builder)
    {
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(50);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
    }
}
