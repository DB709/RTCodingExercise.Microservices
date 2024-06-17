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
using WebMVC.Models;
using System.Linq;

namespace WebMVC.UnitTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<ILicensePlateService> _mockLicensePlateService;
        private readonly List<Plate> _expectedPlates;

        public HomeControllerTests()
        {
            _mockLicensePlateService = new Mock<ILicensePlateService>();

            _controller = new HomeController(_mockLicensePlateService.Object);

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
            var expectedPlateList = new PlateListModel 
            { 
                Plates = _expectedPlates, 
                CurrentPage = 1, 
                PageSize = 20, 
                TotalPlates = 3
            };

            // Arrange
            _mockLicensePlateService.Setup(x => x.GetPlatesAsync(1)).ReturnsAsync(expectedPlateList);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PlateListModel>(viewResult.Model);
            Assert.Equal(_expectedPlates.Count, model.Plates.Count());
        }

        [Fact]
        public async Task AddLicensePlate_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var plateModel = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };

            _mockLicensePlateService.Setup(x => x.AddLicensePlate(plateModel));

            // Act
            var result = await _controller.AddLicensePlate(plateModel);

            // Assert
            _mockLicensePlateService.Verify(x => x.AddLicensePlate(plateModel), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}