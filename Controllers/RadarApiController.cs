using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AirportDemo.Controllers
{
    public class RadarApiController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RadarApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFlights()
        {
            // Bounding box for Istanbul Airport (IST) area - Adjusted to cover a bit wider area
            // You might need to fine-tune these values based on the area you want to cover.
            double lamin = 40.7;  // Minimum latitude
            double lamax = 41.4;  // Maximum latitude
            double lomin = 28.5;  // Minimum longitude
            double lomax = 29.2;  // Maximum longitude

            string url = $"https://opensky-network.org/api/states/all?lamin={lamin}&lomin={lomin}&lamax={lamax}&lomax={lomax}";

            using var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { error = "Flight data unavailable. Please try again later.", details = ex.Message });
            }
        }
    }
}