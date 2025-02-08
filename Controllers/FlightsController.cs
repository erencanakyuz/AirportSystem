using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AirportDemo.Controllers
{
    public class RadarController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RadarController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Returns the radar view (see below)
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFlights()
        {
            double lamin = 40.8;
            double lamax = 41.1;
            double lomin = 28.7;
            double lomax = 28.9;

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
