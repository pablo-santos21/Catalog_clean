using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class ProductMap : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(p => p.Id);

        //Properties
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .UseMySqlIdentityColumn();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(200);

        builder.Property(p => p.Slug)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Stock)
            .HasColumnType("int");

        builder.Property(p => p.Image)
            .HasColumnType("JSON");

        builder.Property(p => p.IsActive)
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .HasColumnType("datetime");

        builder.Property(p => p.UpdateAt)
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .HasColumnType("datetime");

       

        //Index
        builder.HasIndex(p => p.Name, "IX_Product_Name")
            .IsUnique();

        builder.HasIndex(p => p.Slug, "IX_Product_Slug")
            .IsUnique();

        builder.HasIndex(p => p.UserID, "IX_User_UserID");

        //relationship
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("FK_Product_Category")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.UserID)
            .HasConstraintName("FK_Product_User")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
