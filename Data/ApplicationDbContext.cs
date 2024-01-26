
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    //dotnet ef migrations add Init: Runs a migration
    //dotnet ef database update: Commits and updates the migrations in the actual database, reflecting the changes you have done

    //Usamos IdentityDbContext<AppUser> en vez de DbContext, ya que IdentityDbContext contiene entidades como IdentityRole e IdentityUser, y queremos usar el tipo de usuario que generamos (AppUser)

    //IdentityRole representa los roles que existen en mi aplicacion
    //IdentityUser representa los usuarios que existen en mi aplicacion

    //IdentityDbContext es una subclase de DbContext, por lo que tiene todas las funcionalidades de DbContext + las funcionalidades especificas de IdentityDbContext, esa es la razon por la que puedo usar los mismos metodos que con DbContext y hacen lo mismo
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        //Tables
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Comment> Comments { get; set; }

        //Las entidades de entidad de IdentityDbContext (IdentityRole,IdentityUser, etc.) se configuran sobrescribiendo OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Heredando la configuracion del modelo base usando base.OnModelCreating(builder)
            base.OnModelCreating(builder);
            //Añadiendo la funcionalidad adicional
            //Definiendo los roles que tendre en mi aplicacion
            List<IdentityRole> roles = new List<IdentityRole> {

                new IdentityRole{
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole{
                    Name = "User",
                    NormalizedName = "USER"
                },
            };

            //Inserta los datos que tengan cada campo de los IdentityRole(Name y NormalizedName), siendo cada IdentityRole un registro
            //.HasData inserta los datos al aplicarse las migraciones, y no se puede modificar mediante el DbContext, es decir, que estos registros no podran ser eliminados usando los metodos .Update o .Delete de EF Core, solo podran ser cambiados cambiando el metodo .HasData y aplicando una nueva migracion
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}