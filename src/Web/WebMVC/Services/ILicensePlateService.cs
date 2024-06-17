using WebMVC.Models;

namespace WebMVC.Services
{
    public interface ILicensePlateService
    {
        Task<PlateListModel> GetPlatesAsync(int page);
        Task AddLicensePlate(Plate model);
    }
}
