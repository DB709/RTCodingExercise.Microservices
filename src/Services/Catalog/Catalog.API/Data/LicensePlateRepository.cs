namespace Catalog.API.Data
{
    public class LicensePlateRepository : ILicensePlateRepository
    {
        private readonly ApplicationDbContext _context;

        public LicensePlateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Plate> GetAll() => _context.Plates.AsQueryable();

        public async Task AddLicensePlateAsync(Plate plate)
        {
            _context.Plates.Add(plate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLicensePlateAsync(Plate plate)
        {
            _context.Plates.Update(plate);
            await _context.SaveChangesAsync();
        }
    }
}
