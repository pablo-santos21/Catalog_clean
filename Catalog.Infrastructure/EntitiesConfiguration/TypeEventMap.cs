using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class TypeEventMap : IEntityTypeConfiguration<TypeEvent>
{
    public void Configure(EntityTypeBuilder<TypeEvent> builder)
    {
        builder.ToTable("TypeEvent");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .UseMySqlIdentityColumn();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(80);

        // index
        builder.HasIndex(t => t.Name, "IX_Type_Name")
            .IsUnique();

        //Relationship
        builder.HasMany(t => t.ScheduledEvents)
            .WithOne(s => s.TypeEvent)
            .HasForeignKey(s => s.TypeEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
