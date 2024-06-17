using WebMVC.Models;

namespace WebMVC.Services
{
    public interface ILicensePlateService
    {
        Task<PlateListModel> GetPlatesAsync(int page, SortOrder saleSortOrder);
        Task AddLicensePlate(Plate model);
    }
}
