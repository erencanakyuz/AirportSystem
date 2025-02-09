// RadarApiController.cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        [HttpGet]
        public async Task<IActionResult> GetFlights()
        {
            // Wide bounding box around Istanbul
            double lamin = 39.4732;
            double lamax = 43.0768;
            double lomin = 26.3981;
            double lomax = 31.1039;

            string url = $"https://opensky-network.org/api/states/all?lamin={lamin}&lomin={lomin}&lamax={lamax}&lomax={lomax}";

            using var client = _httpClientFactory.CreateClient();

            // Your OpenSky credentials
            string openSkyUser = "UserTest1";
            string openSkyPassword = "2@LhapEtPxSL4pn";

            var encoded = Convert.ToBase64String(
                Encoding.GetEncoding("ISO-8859-1").GetBytes($"{openSkyUser}:{openSkyPassword}")
            );
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", encoded);

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { error = "Flight data unavailable.", details = ex.Message });
            }
        }
    }
}
