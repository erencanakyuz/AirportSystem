using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirportDemo.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly HttpClient _httpClient;

        public MaintenanceController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET http://localhost:5000/Maintenance
        [HttpGet]
        public IActionResult Index()
        {
            // Views/Maintenance/Index.cshtml döndürüyoruz
            return View();
        }

        // GET http://localhost:5000/api/airport-status
        [HttpGet]
        [Route("api/airport-status")]
        public async Task<IActionResult> GetAirportStatus()
        {
            const string apiUrl = "https://www.flightstats.com/v2/api/airport/IST?rqid=4guirc8bmfy";

            try
            {
                // 1) HttpRequestMessage oluþturun
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                // 2) User-Agent ekleyin
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0");

                // 3) HTTP isteðini gönderin
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // 4) JSON verisini çekin
                var jsonData = await response.Content.ReadAsStringAsync();
                return Content(jsonData, "application/json");
            }
            catch
            {
                return StatusCode(500, "Error fetching data");
            }
        }
    }
}
