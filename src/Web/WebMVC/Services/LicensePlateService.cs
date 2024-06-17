using System.Text;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class LicensePlateService : ILicensePlateService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        private const decimal VatMultiplier = 1.2M;
        private const string odataBaseUrl = "odata/Plate";

        public LicensePlateService(IHttpClientFactory httpClientFactory, JsonSerializerOptions options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = options;
        }

        public async Task<PlateListModel> GetPlatesAsync(int page)
        {
            int pageSize = 20;
            int skip = (page - 1) * pageSize;

            var odataOptions = $"?$count=true&$skip={skip}";

            var response = await _httpClient.GetAsync($"http://catalog-api:80/{odataBaseUrl}{odataOptions}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var plates = JsonSerializer.Deserialize<PlateListModel>(content, _options);

            if (plates?.Plates == null || !plates.Plates.Any())
            {
                return new PlateListModel();
            }
            
            plates.Plates = plates.Plates.Select(x => { x.SalePrice = x.SalePrice * VatMultiplier; return x; }).ToList();
            plates.PageSize = pageSize;
            plates.CurrentPage = page;

            return plates;
        }

        public async Task AddLicensePlate(Plate model)
        {
            var plate = JsonSerializer.Serialize(model);
            var response = await _httpClient.PostAsync($"http://catalog-api:80/{odataBaseUrl}", new StringContent(plate, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
