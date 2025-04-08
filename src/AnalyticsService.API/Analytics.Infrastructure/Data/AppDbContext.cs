using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
        }

        // Define DbSets for your entities here
        // public DbSet<YourEntity> YourEntities { get; set; }

        // Example method to save changes
        public void SaveChanges()
        {
            // Logic to save changes to the database
        }
    }
}
