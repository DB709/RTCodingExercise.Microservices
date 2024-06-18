
using Catalog.Domain;
using Microsoft.AspNetCore.OData.Query;

namespace Catalog.API.Services
{
    public class LicensePlateService : ILicensePlateService
    {
        public readonly ILicensePlateRepository _licensePlateRepository;
        public readonly ISaleRepository _saleRepository;

        public LicensePlateService(ILicensePlateRepository licensePlateRepository, ISaleRepository saleRepository)
        {
            _licensePlateRepository = licensePlateRepository;
            _saleRepository = saleRepository;
        }

        public IQueryable<Plate> GetAll() =>  _licensePlateRepository.GetAll();

        public async Task AddLicensePlate(Plate plate) => await _licensePlateRepository.AddLicensePlateAsync(plate);

        public async Task UpdateLicensePlate(Plate plate) => await _licensePlateRepository.UpdateLicensePlateAsync(plate);

        public async Task<decimal> GetSalesTotal()
        {
            var sales = await _saleRepository.GetAll().Select(x => x.FinalSalePrice).ToListAsync();

            return sales.Sum();
        }

        public async Task MakeLicensePlateSale(Sale sale)
        {
            sale.Plate.IsSold = true;
            await _licensePlateRepository.UpdateLicensePlateAsync(sale.Plate);
            await _saleRepository.AddSaleAsync(sale);
        }
    }
}
