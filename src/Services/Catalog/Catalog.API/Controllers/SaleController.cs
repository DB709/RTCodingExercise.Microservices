using Catalog.API.Services;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Catalog.API.Controllers
{
    public class SaleController : ODataController
    {
        private readonly ILicensePlateService _licensePlatesService;
        private readonly ILogger<PlateController> _logger;

        public SaleController(ILicensePlateService platesService, ILogger<PlateController> logger)
        {
            _licensePlatesService = platesService;
            _logger = logger;
        }

        public async Task<ActionResult<decimal>> Get()
        {
            return await _licensePlatesService.GetSalesTotal();
        }

        public async Task<IActionResult> Post([FromBody] Sale sale)
        {
            try
            {
                if (!ModelState.IsValid)
                { 
                    _logger.LogError($"Error add new license plate requested plate is not valid.");
                    return BadRequest(ModelState);
                }

                await _licensePlatesService.MakeLicensePlateSale(sale);

                _logger.LogInformation($"Successfully added license plate {sale.Plate.Registration}.");
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
