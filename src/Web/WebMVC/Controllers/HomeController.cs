using System.Text.Json;
using RTCodingExercise.Microservices.Models;
using System.Diagnostics;
using WebMVC.Services;

namespace RTCodingExercise.Microservices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILicensePlateService _licensePlateService;

        public HomeController(ILicensePlateService licensePlateService)
        {
            _licensePlateService = licensePlateService;
        }

        public async Task<IActionResult> Index(string searchText, int page = 1, SortOrder salePriceOrder = SortOrder.Unspecified)
        {
            return View(await _licensePlateService.GetPlatesAsync(page, salePriceOrder, searchText));
        }

        [HttpPost]
        public async Task<IActionResult> AddLicensePlate(Plate model)
        {
            if (!ModelState.IsValid)
            {
                return Error();
            }

            await _licensePlateService.AddLicensePlate(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateReservedStatus(Plate model)
        {
            await _licensePlateService.UpdateReservedStatus(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddPlateSale(Plate model)
        {
            await _licensePlateService.AddPlateSale(model);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}