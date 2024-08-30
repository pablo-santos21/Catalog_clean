using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.EntitiesConfiguration;
using Catalog.Infrastructure.Identity;
using Catalog.Infrastructure.Populating;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Catalog.Infrastructure.Context;

public class ApplicationDbContext : IdentityDbContext<IdentityApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ScheduledEvent> Events { get; set; }
    public DbSet<PostBlog> PostsBlog { get; set; }
    public DbSet<CategoryBlog> CategoriesBlog { get; set; }
    public DbSet<TypeEvent> TypesEvent { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new ProductMap());
        modelBuilder.ApplyConfiguration(new CategoryBlogMap());
        modelBuilder.ApplyConfiguration(new PostMap());
        modelBuilder.ApplyConfiguration(new ScheduledEventMap());
        modelBuilder.ApplyConfiguration(new TypeEventMap());

        modelBuilder.Seed();

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITimestampedEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
                entry.Entity.UpdateAt = DateTime.Now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateAt = DateTime.Now;
                entry.Property(x => x.CreatedAt).IsModified = false;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
