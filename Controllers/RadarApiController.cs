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
            // **START HERE: Modify these bounding box values**
            double lamin = 38.0;   // Minimum latitude (further South)
            double lamax = 45.0;   // Maximum latitude (further North)
            double lomin = 25.0;   // Minimum longitude (further West)
            double lomax = 35.0;   // Maximum longitude (further East)

            // **Original comment (still relevant):**
            // Burada lamin, lamax, lomin, lomax deðerlerini çok geniþ tuttuk.
            // Böylece IST yakýnýndaki tüm uçaklar (ve daha fazlasý) gelsin.
            string url = $"https://opensky-network.org/api/states/all?lamin=38.0&lomin=25.0&lamax=45.0&lomax=35.0";

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