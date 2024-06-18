namespace Catalog.API.Data
{
    public interface ISaleRepository
    {
        IQueryable<Sale> GetAll();
        Task AddSaleAsync(Sale sale);
    }
}
