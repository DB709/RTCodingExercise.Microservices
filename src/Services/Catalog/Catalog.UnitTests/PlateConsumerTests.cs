using Catalog.API.Data;
using Catalog.Domain;
using MassTransit.Testing;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlateConsumerTests
    {
        [Fact]
        public async Task Consume_ValidMessage_CallsSaveOrderAsync()
        {
            // Arrange
            var repository = new Mock<ILicensePlateRepository>();
            var plate = new Plate() { Id = Guid.NewGuid(), Registration = "LK93 XTY", Letters = "LK", Numbers = 93, PurchasePrice = 100.57M, SalePrice = 125.00M };
            var harness = new InMemoryTestHarness();
            var consumerHarness = harness.Consumer(() => new PlateConsumer(repository.Object));

            await harness.Start();

            try
            {
                // Act
                await harness.InputQueueSendEndpoint.Send(plate);

                // Assert
                Assert.True(await harness.Consumed.Any<Plate>());
                repository.Verify(r => r.AddLicensePlateAsync(It.IsAny<Plate>()), Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
