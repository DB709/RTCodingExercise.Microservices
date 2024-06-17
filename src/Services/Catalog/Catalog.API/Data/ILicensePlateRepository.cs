namespace Catalog.API.Data
{
    public interface ILicensePlateRepository
    {
        IQueryable<Plate> GetAll();

        Task AddLicensePlateAsync(Plate plate);
    }
}
