using System.Text;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class LicensePlateService : ILicensePlateService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        private const decimal _vatMultiplier = 1.2M;
        private const string _odataBaseUrl = "odata/Plate";
        private const int _pageSize = 20;

        public LicensePlateService(IHttpClientFactory httpClientFactory, JsonSerializerOptions options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = options;
        }

        public async Task<PlateListModel> GetPlatesAsync(int page, SortOrder saleSortOrder, string searchText)
        {
            var odataOptions = SetupOdataFilters(page, saleSortOrder, searchText);

            var response = await _httpClient.GetAsync($"http://catalog-api:80/{_odataBaseUrl}{odataOptions}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var plates = JsonSerializer.Deserialize<PlateListModel>(content, _options);

            if (plates?.Plates == null || !plates.Plates.Any())
            {
                return new PlateListModel();
            }
            
            plates.Plates = plates.Plates.Select(x => { x.SalePrice = x.SalePrice * _vatMultiplier; return x; }).ToList();
            plates.PageSize = _pageSize;
            plates.CurrentPage = page;

            return plates;
        }

        public async Task AddLicensePlate(Plate model)
        {
            var plate = JsonSerializer.Serialize(model);
            var response = await _httpClient.PostAsync($"http://catalog-api:80/{_odataBaseUrl}", new StringContent(plate, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateReservedStatus(Plate model)
        {
            model.Reserved = !model.Reserved;
            var plate = JsonSerializer.Serialize(model);
            var response = await _httpClient.PatchAsync($"http://catalog-api:80/{_odataBaseUrl}", new StringContent(plate, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        private static string SetupOdataFilters(int page, SortOrder saleSortOrder, string searchText)
        {
            int skip = (page - 1) * _pageSize;

            var odataOptions = $"?$count=true&$skip={skip}";

            if (saleSortOrder != SortOrder.Unspecified)
            {
                if (saleSortOrder == SortOrder.Ascending)
                    odataOptions += "&$orderby=SalePrice asc";

                if (saleSortOrder == SortOrder.Descending)
                    odataOptions += "&$orderby=SalePrice desc";
            }

            if (!string.IsNullOrWhiteSpace(searchText)) 
            {
                if (int.TryParse(searchText, out var numberSearch))
                    odataOptions += $"&$filter=Numbers eq {numberSearch}";

                if (!searchText.Any(x => char.IsDigit(x)))
                    odataOptions += $"&$filter=Letters eq '{searchText}'";
            }


            return odataOptions;
        }
    }
}
