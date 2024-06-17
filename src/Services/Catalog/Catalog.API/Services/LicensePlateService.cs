
using Microsoft.AspNetCore.OData.Query;

namespace Catalog.API.Services
{
    public class LicensePlateService : ILicensePlateService
    {
        public readonly ILicensePlateRepository _licensePlateRepository;

        public LicensePlateService(ILicensePlateRepository licensePlateRepository)
        {
            _licensePlateRepository = licensePlateRepository;
        }

        public IQueryable<Plate> GetAll() =>  _licensePlateRepository.GetAll();

        public async Task AddLicensePlate(Plate plate) => await _licensePlateRepository.AddLicensePlateAsync(plate);
    }
}
