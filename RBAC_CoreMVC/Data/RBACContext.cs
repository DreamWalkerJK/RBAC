using Microsoft.EntityFrameworkCore;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Data
{
    public class RBACContext : DbContext
    {
        public RBACContext (DbContextOptions<RBACContext> options)
            : base(options)
        {
        }

        
        public DbSet<Department> Departments { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<RoleMenu>()
                .HasKey(rm => new { rm.RoleId, rm.MenuId });

            builder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // PostgerSQL 使用Guid
            //builder.HasPostgresExtension("uuid-ossp");

            base.OnModelCreating(builder);
                
        }

    }
}
