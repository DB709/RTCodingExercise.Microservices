namespace Catalog.API.Data
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;

        public SaleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Sale> GetAll() => _context.Sales.AsQueryable();

        public async Task AddSaleAsync(Sale sale)
        {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
        }
    }
}
