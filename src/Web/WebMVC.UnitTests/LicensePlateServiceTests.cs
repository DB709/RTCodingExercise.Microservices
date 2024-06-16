using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using Xunit;
using RTCodingExercise.Microservices.Controllers;
using Moq;
using Moq.Protected;
using System.Text.Json;
using System.Threading;
using WebMVC.Services;
using System.Linq;

namespace WebMVC.UnitTests
{
    public class LicensePlateServiceTests
    {
        private readonly LicensePlateService _licensePlateService;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly List<Plate> _expectedPlates;

        public LicensePlateServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _licensePlateService = new LicensePlateService(_httpClientFactoryMock.Object);

            _expectedPlates = new List<Plate>
            {
                new() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M },
                new() {  Id = Guid.NewGuid(), Registration = "MX93 XTY", Letters = "MX", Numbers = 93, PurchasePrice = 570.93M, SalePrice = 624.00M },
                new() {  Id = Guid.NewGuid(), Registration = "LK32 XTY", Letters = "LK", Numbers = 32, PurchasePrice = 989.57M, SalePrice = 1245.00M }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfPlates()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_expectedPlates))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var actualPlates = await _licensePlateService.GetPlatesAsync();

            // Assert
            Assert.NotNull(actualPlates);
            Assert.Equal(_expectedPlates.Count, actualPlates.ToList().Count);
        }

        [Fact]
        public async Task AddLicensePlate_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var plateModel = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };

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
    }
}