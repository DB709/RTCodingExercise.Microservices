using Catalog.API.Data;
using Catalog.API.Services;
using Catalog.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class LicensePlateServiceTests
    {
        private readonly Mock<ILicensePlateRepository> _mockLicensePlateRepository;
        private readonly ILicensePlateService _licensePlateService;
        private readonly Mock<ISaleRepository> _mockSaleRepository;

        public LicensePlateServiceTests() 
        {
            _mockLicensePlateRepository = new Mock<ILicensePlateRepository>();
            _mockSaleRepository = new Mock<ISaleRepository>();

            _licensePlateService = new LicensePlateService(_mockLicensePlateRepository.Object, _mockSaleRepository.Object);
        }
        [Fact]
        public void GetAllPlates_ReturnsListOfPlates()
        {
            // Arrange
            var expectedPlates = new List<Plate>
            {
                new() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M },
                new() {  Id = Guid.NewGuid(), Registration = "MX93 XTY", Letters = "MX", Numbers = 93, PurchasePrice = 570.93M, SalePrice = 624.00M },
                new() {  Id = Guid.NewGuid(), Registration = "LK32 XTY", Letters = "LK", Numbers = 32, PurchasePrice = 989.57M, SalePrice = 1245.00M }

            }.AsQueryable();

            _mockLicensePlateRepository.Setup(x => x.GetAll()).Returns(expectedPlates);

            // Act
            var actualPlates = _licensePlateService.GetAll();

            // Assert
            Assert.NotNull(actualPlates);
            Assert.Equal(expectedPlates.ToList().Count, actualPlates.ToList().Count);
            Assert.Equal(expectedPlates, actualPlates);
        }

        [Fact]
        public async Task AddPlate_IsSuccessful()
        {
            // Arrange
            var newPlate = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };

            _mockLicensePlateRepository.Setup(x => x.AddLicensePlateAsync(newPlate));

            // Act
            await _licensePlateService.AddLicensePlate(newPlate);

            // Assert
            _mockLicensePlateRepository.Verify(x => x.AddLicensePlateAsync(newPlate), Times.Once());
        }

        [Fact]
        public async Task UpdatePlate_IsSuccessful()
        {
            // Arrange
            var updatedPlate = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };

            _mockLicensePlateRepository.Setup(x => x.UpdateLicensePlateAsync(updatedPlate));

            // Act
            await _licensePlateService.UpdateLicensePlate(updatedPlate);

            // Assert
            _mockLicensePlateRepository.Verify(x => x.UpdateLicensePlateAsync(updatedPlate), Times.Once());
        }
    }
}