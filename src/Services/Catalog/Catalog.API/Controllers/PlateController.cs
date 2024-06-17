using Catalog.API.Services;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Catalog.API.Controllers
{
    public class PlateController : ODataController
    {
        private readonly ILicensePlateService _licensePlatesService;
        private readonly ILogger<PlateController> _logger;

        public PlateController(ILicensePlateService platesService, ILogger<PlateController> logger)
        {
            _licensePlatesService = platesService;
            _logger = logger;
        }

        [EnableQuery(PageSize = 20)]
        public IQueryable<Plate> Get()
        {
            return _licensePlatesService.GetAll();
        }

        public async Task<IActionResult> Post([FromBody] Plate plate)
        {
            try
            {
                if (!ModelState.IsValid)
                { 
                    _logger.LogError($"Error add new license plate requested plate is not valid.");
                    return BadRequest(ModelState);
                }

                await _licensePlatesService.AddLicensePlate(plate);

                _logger.LogInformation($"Successfully added license plate {plate.Registration}.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error add new license plate - {ex.Message}.");
                return BadRequest(ex);
            }
        }
    }
}
