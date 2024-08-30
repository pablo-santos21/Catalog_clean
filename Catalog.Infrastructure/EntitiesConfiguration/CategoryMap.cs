using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(c => c.Id);

        //Properties
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .UseMySqlIdentityColumn();

        builder.Property(c => c.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR")
                .HasMaxLength(200);

        builder.Property(c => c.Description)
                .HasColumnType("VARCHAR")
                .HasMaxLength(200);

        builder.Property(c => c.Slug)
                .HasColumnType("VARCHAR")
                .HasMaxLength(200);

        //Index
        builder.HasIndex(c => c.Name, "IX_Category_Name")
            .IsUnique();

        builder.HasIndex(c => c.Slug, "IX_Category_Slug")
            .IsUnique();

        // Relationship
        builder.HasMany(c => c.Products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
