using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntitiesConfiguration;

public class ScheduledEventMap : IEntityTypeConfiguration<ScheduledEvent>
{
    public void Configure(EntityTypeBuilder<ScheduledEvent> builder)
    {
        builder.ToTable("ScheduledEvent");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .UseMySqlIdentityColumn()
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Title)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(155);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(1000);

        builder.Property(s => s.Local)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(200);

        builder.Property(s => s.City)
            .IsRequired()
            .HasColumnType("VARCHAR")
            .HasMaxLength(50);

        builder.Property(s => s.State)
            .HasColumnType("int")
            .HasConversion(
            s => s.ToString(),
            s => (States)Enum.Parse(typeof(States), s))
            .HasColumnType("varchar")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(s => s.Neighborhood)
            .HasColumnType("VARCHAR")
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .HasColumnType("datetime");

        builder.Property(s => s.UpdateAt)
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .HasColumnType("datetime");

        builder.Property(s => s.EventDate)
            .HasColumnType("datetime");

        builder.Property(s => s.Occurred)
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(s => s.Slug)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

        //index
        builder.HasIndex(s => s.Slug, "IX_Event_Slug")
            .IsUnique();

        builder.HasIndex(s => s.Title, "IX_Event_Title")
            .IsUnique();

        //Relationship
        builder.HasOne(s => s.TypeEvent)
            .WithMany(t => t.ScheduledEvents)
            .HasForeignKey(s => s.TypeEventId)
            .HasConstraintName("FK_Event_Type")
            .OnDelete(DeleteBehavior.Restrict);

    }
}
