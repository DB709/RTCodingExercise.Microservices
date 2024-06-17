using WebMVC.Models;

namespace WebMVC.Services
{
    public interface ILicensePlateService
    {
        Task<PlateListModel> GetPlatesAsync(int page, SortOrder saleSortOrder, string searchText);
        Task AddLicensePlate(Plate model);
    }
}
