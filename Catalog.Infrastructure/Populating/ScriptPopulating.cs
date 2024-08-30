using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Populating;

internal static class ScriptPopulating
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TypeEvent>().HasData(
           new TypeEvent { Id = 1, Name = "Feira" },
           new TypeEvent { Id = 2, Name = "Palesta" },
           new TypeEvent { Id = 3, Name = "Workshop" },
           new TypeEvent { Id = 4, Name = "Reunião" }
        );

        modelBuilder.Entity<CategoryBlog>().HasData(
            new CategoryBlog { Id = 1, Name = "Dicas", Description = "Dica", Slug = "dica" },
            new CategoryBlog { Id = 2, Name = "Receitas", Description = "Receita", Slug = "receitas" },
            new CategoryBlog { Id = 3, Name = "Testes", Description = "Teste", Slug = "teste" }
        );


        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Comida", Description = "Comida", Slug = "comida" },
            new Category { Id = 2, Name = "Bebida", Description = "Bebida", Slug = "bebida" },
            new Category { Id = 3, Name = "Roupa", Description = "Roupa", Slug = "roupa" }

        );

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "837ae170-ff3b-4521-bafd-be66148527dc", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "f01e92a6-cea2-41bf-8c82-ff17832b3920", Name = "SuperUser", NormalizedName = "SUPERUSER" },
            new IdentityRole { Id = "eed2168c-6772-4192-8d78-03fe1c760d93", Name = "Customer", NormalizedName = "CUSTOMER" },
            new IdentityRole { Id = "7f6b07de-3acc-4905-98e8-00b7faa3164d", Name = "Seller", NormalizedName = "SELLER" }
        );
    }
}
