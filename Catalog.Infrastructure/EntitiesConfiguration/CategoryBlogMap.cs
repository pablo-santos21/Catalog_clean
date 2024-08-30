using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class CategoryBlogMap : IEntityTypeConfiguration<CategoryBlog>
{
    public void Configure(EntityTypeBuilder<CategoryBlog> builder)
    {
        builder.ToTable("CategoryBlog");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .UseMySqlIdentityColumn()
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(80);

        builder.Property(c => c.Description)
            .HasColumnType("VARCHAR")
            .HasMaxLength(200);

        builder.Property(c => c.Slug)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

        //index
        builder.HasIndex(c => c.Name, "IX_CategoryBlog_Name")
            .IsUnique();

        builder.HasIndex(c => c.Slug, "IX_CategoryBlog_Slug")
            .IsUnique();

        //Relationship
        builder.HasMany(c => c.PostsBlog)
               .WithOne(p => p.CategoryBlog)
               .HasForeignKey(p => p.CategoryBlogId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
