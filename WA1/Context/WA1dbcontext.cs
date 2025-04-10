using Microsoft.EntityFrameworkCore;
using WA1.Entities;

namespace WA1.Context
{
    public class WA1dbcontext : DbContext
    {
        public WA1dbcontext(DbContextOptions<WA1dbcontext> options)
            : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Setting up relationships
            modelBuilder.Entity<Employee>()
               .HasOne(e => e.Department);  
        }
    }
    
}
