using System.Data.Entity;
using GoSharpRest.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoSharpRest.Models
{
    public class GoSharpRestContext : IdentityDbContext<ApplicationUser>
    {
    
        public GoSharpRestContext() : base("name=GoSharpRestContext")
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public static GoSharpRestContext Create()
        {
            return new GoSharpRestContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasRequired(p => p.ProjectManager)
                .WithMany(u => u.ManagedProjects)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Developers)
                .WithMany(u => u.AssignedProjects)
                .Map(cs =>
                {
                    cs.MapLeftKey("ProjectRefId");
                    cs.MapRightKey("ManagerRefId");
                    cs.ToTable("ProjectsToDevelopers");
                });

            modelBuilder.Entity<Project>()
                .HasRequired(p => p.Order)
                .WithOptional(order => order.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(user => user.AssignedOrders);

            modelBuilder.Entity<WorkItem>().HasRequired(w => w.AssignedDeveloper).WithMany(u => u.AssignedTasks);
        }

        public DbSet<SiteTemplate> SiteTemplates { get; set; }
        public DbSet<ShoppingCart> Carts { get; set; }
        public DbSet<CartRecord> CartRecords { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
    }
}
