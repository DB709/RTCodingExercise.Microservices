using System.Text;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class LicensePlateService : ILicensePlateService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;

        private const string _odataPlateUrl = "odata/Plate";
        private const string _odataSaleUrl = "odata/Sale";

        public LicensePlateService(IHttpClientFactory httpClientFactory, JsonSerializerOptions options, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = options;
            _configuration = configuration;
        }

        public async Task<PlateListModel> GetPlatesAsync(int page, SortOrder saleSortOrder, string searchText)
        {
            var odataOptions = SetupOdataFilters(page, saleSortOrder, searchText);

            var response = await _httpClient.GetAsync($"{_configuration["CatalogAPIBaseUrl"]}{_odataPlateUrl}{odataOptions}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var plates = JsonSerializer.Deserialize<PlateListModel>(content, _options);

            if (plates?.Plates == null || !plates.Plates.Any())
            {
                return new PlateListModel();
            }
            
            plates.PageSize = int.Parse(_configuration["PageSize"]);
            plates.CurrentPage = page;

            response = await _httpClient.GetAsync($"{_configuration["CatalogAPIBaseUrl"]}{_odataSaleUrl}");
            var revenue = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(revenue);
            var root = jsonDocument.RootElement;
            var value = root.GetProperty("value").GetDecimal();

            plates.TotalRevenue = value;
            return plates;
        }

        public async Task AddLicensePlate(Plate model)
        {
            var plate = JsonSerializer.Serialize(model);
            var response = await _httpClient.PostAsync($"{_configuration["CatalogAPIBaseUrl"]}{_odataPlateUrl}", new StringContent(plate, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task AddPlateSale(Plate model, string discountCode)
        {
            var saleModel = new Sale
            {
                Plate = model,
                Id = Guid.NewGuid(),
                SaleDate = DateTime.Now,
                FinalSalePrice = FinalPrice(model.SalePrice, discountCode)
            };

            var sale = JsonSerializer.Serialize(saleModel);
            var response = await _httpClient.PostAsync($"{_configuration["CatalogAPIBaseUrl"]}{_odataSaleUrl}", new StringContent(sale, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateReservedStatus(Plate model)
        {
            model.Reserved = !model.Reserved;
            var plate = JsonSerializer.Serialize(model);
            var response = await _httpClient.PatchAsync($"{_configuration["CatalogAPIBaseUrl"]}{_odataPlateUrl}", new StringContent(plate, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        private string SetupOdataFilters(int page, SortOrder saleSortOrder, string searchText)
        {
            int skip = (page - 1) * int.Parse(_configuration["PageSize"]);

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
                    odataOptions += $"&$filter=Numbers eq {numberSearch} and Reserved eq false";

                if (!searchText.Any(x => char.IsDigit(x)))
                    odataOptions += $"&$filter=Letters eq '{searchText}' and Reserved eq false";
            }

            return odataOptions;
        }

        private decimal FinalPrice(decimal salePrice, string discountCode)
        {
            var finalPrice = salePrice * decimal.Parse(_configuration["VatMultiplier"]);

            switch (discountCode)
            {
                case "DISCOUNT":
                    return finalPrice -= 25M;
                case "PERCENTOFF":
                    return finalPrice * 0.85M;
                default:
                    return finalPrice;
            }
        }
    }
}
