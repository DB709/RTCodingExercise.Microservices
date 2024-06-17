using MassTransit.Riders;
using MassTransit;

namespace Catalog.API.Data
{
    public class PlateConsumer : IConsumer<Plate>
    {
        private readonly ILicensePlateRepository _repository;

        public PlateConsumer(ILicensePlateRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Plate> context)
        {
            await _repository.AddLicensePlateAsync(context.Message);
        }
    }
}
