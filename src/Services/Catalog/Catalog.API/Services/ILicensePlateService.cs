using Microsoft.AspNetCore.OData.Query;

namespace Catalog.API.Services
{
    public interface ILicensePlateService
    {
        IQueryable<Plate> GetAll();

        Task AddLicensePlate(Plate plate);

        Task UpdateLicensePlate(Plate plate);

        Task<decimal> GetSalesTotal();

        Task MakeLicensePlateSale(Sale sale);
    }
}
