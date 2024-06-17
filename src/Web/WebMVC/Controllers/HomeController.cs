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

        public async Task<IActionResult> Index(int page = 1)
        {
            return View(await _licensePlateService.GetPlatesAsync(page));
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