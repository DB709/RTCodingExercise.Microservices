using Catalog.Domain;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using Xunit;
using Moq;
using Moq.Protected;
using System.Text.Json;
using System.Threading;
using WebMVC.Services;
using System.Linq;
using WebMVC.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WebMVC.UnitTests
{
    public class LicensePlateServiceTests
    {
        private readonly LicensePlateService _licensePlateService;
        private readonly JsonSerializerOptions _options;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly List<Plate> _expectedPlates;

        public LicensePlateServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockConfiguration = new Mock<IConfiguration>();
            _options = new JsonSerializerOptions();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _licensePlateService = new LicensePlateService(_mockHttpClientFactory.Object, _options, _mockConfiguration.Object);

            _expectedPlates = new List<Plate>
            {
                new() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 1245.00M },
                new() {  Id = Guid.NewGuid(), Registration = "MX93 XTY", Letters = "MX", Numbers = 93, PurchasePrice = 570.93M, SalePrice = 125.00M },
                new() {  Id = Guid.NewGuid(), Registration = "LK32 XTY", Letters = "LK", Numbers = 32, PurchasePrice = 989.57M, SalePrice =  624.00M }
            };
        }

        [Fact]
        public async Task GetPlatesAsync_ReturnsResult_WithAListOfPlates()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new PlateListModel { Plates = _expectedPlates }))
            };

            var totalRevenueResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { value = 12.34M }))
            };

            _mockConfiguration.Setup(x => x["CatalogAPIBaseUrl"]).Returns("https://mockcatalog/");
            _mockConfiguration.Setup(x => x["PageSize"]).Returns("20");
            var plateUri = new Uri("https://mockcatalog/odata/Plate?$count=true&$skip=0");
            var saleUri = new Uri("https://mockcatalog/odata/Sale");
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == plateUri), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            _mockHttpMessageHandler
             .Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == saleUri), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(totalRevenueResponse);

            // Act
            var actualPlates = await _licensePlateService.GetPlatesAsync(1, SortOrder.Unspecified, string.Empty);

            // Assert
            Assert.NotNull(actualPlates);
            Assert.Equal(_expectedPlates.Count, actualPlates.Plates.ToList().Count);
        }

        [Fact]
        public async Task AddLicensePlate_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var plateModel = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };

            _mockConfiguration.Setup(x => x["CatalogAPIBaseUrl"]).Returns("https://mockcatalog/");
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            // Act
            await _licensePlateService.AddLicensePlate(plateModel);

            // Assert
            _mockHttpMessageHandler
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateReservedStatus_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var plateModel = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };

            _mockConfiguration.Setup(x => x["CatalogAPIBaseUrl"]).Returns("https://mockcatalog/");
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            // Act
            await _licensePlateService.UpdateReservedStatus(plateModel);

            // Assert
            _mockHttpMessageHandler
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }
    }
}