using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class PostMap : IEntityTypeConfiguration<PostBlog>
{
    public void Configure(EntityTypeBuilder<PostBlog> builder)
    {
        builder.ToTable("PostBlog");

        builder.HasKey(p => p.Id);

        //propriedades
        builder.Property(p => p.Id)
            .UseMySqlIdentityColumn()
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(255);

        builder.Property(p => p.SubTitle)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(255);

        builder.Property(p => p.Context)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(3000);

        builder.Property(p => p.IsApproved)
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(p => p.IsDeleted)
            .HasColumnType("bit")
            .HasDefaultValue(false);

        builder.Property(p => p.ImagePost)
            .HasColumnType("VARCHAR")
            .HasMaxLength(1000);

        builder.Property(p => p.Slug)
                .HasColumnType("VARCHAR")
                .HasMaxLength(255);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .HasColumnType("datetime");

        builder.Property(p => p.UpdateAt)
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .HasColumnType("datetime");

       
        //index
        builder.HasIndex(p => p.Title, "IX_Post_Name")
            .IsUnique();

        builder.HasIndex(p => p.Slug, "IX_Post_Slug")
            .IsUnique();

        builder.HasIndex(p => p.UserID, "IX_User_UserID");


        //Relationship

        builder.HasOne(p => p.CategoryBlog)
            .WithMany(c => c.PostsBlog)
            .HasForeignKey(p => p.CategoryBlogId)
            .HasConstraintName("FK_Post_Category")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserID)
            .HasConstraintName("FK_Post_User")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
