using codesphere_api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace codesphere_api.Persistence
{


    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Menu> Menus => Set<Menu>();

        public DbSet<Permission> Permissions => Set<Permission>();

        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureProduct(modelBuilder);
            ConfigureApplicationUser(modelBuilder);
            ConfigureApplicationRole(modelBuilder);
            ConfigureMenu(modelBuilder);
            ConfigurePermission(modelBuilder);
            ConfigureRolePermission(modelBuilder);
        }

        private static void ConfigureProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("ProductId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(250);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);
            });
        }

        private static void ConfigureApplicationUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.Id)
                    .HasColumnName("UserId");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);
            });
        }

        private static void ConfigureApplicationRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");

                entity.Property(e => e.Id)
                    .HasColumnName("RoleId");

                entity.Property(e => e.Description)
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.Property(e => e.IsSystem)
                    .IsRequired();

                entity.HasMany(e => e.RolePermissions)
                    .WithOne(e => e.Role)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureMenu(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menus");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("MenuId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Route)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                // Menu -> Parent Menu
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Code)
                    .IsUnique();
            });
        }

        private static void ConfigurePermission(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("PermissionId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasOne(e => e.Menu)
                    .WithMany(e => e.Permissions)
                    .HasForeignKey(e => e.MenuId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.HasIndex(e => new
                {
                    e.MenuId,
                    e.Code
                })
                .IsUnique();
            });
        }

        private static void ConfigureRolePermission(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");

                entity.HasKey(e => new
                {
                    e.RoleId,
                    e.PermissionId
                });

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
