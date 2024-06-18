namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Plate> Plates { get; set; }
        public DbSet<Sale> Sales { get; set; }
    }
}
