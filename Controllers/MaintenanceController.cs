using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

        // GET: http://localhost:5000/Maintenance
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: http://localhost:5000/api/airport-status
        [HttpGet]
        [Route("api/airport-status")]
        public async Task<IActionResult> GetAirportStatus()
        {
            const string apiUrl = "https://www.flightstats.com/v2/api/airport/IST?rqid=4guirc8bmfy";
            try
            {
                // Tek seferlik HttpRequestMessage
                using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                // User-Agent baþlýðýný ekle
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible;)");

                // Ýstek yap
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonData = await response.Content.ReadAsStringAsync();
                return Content(jsonData, "application/json");
            }
            catch (HttpRequestException ex)
            {
                // Hata loglama vb.
                return StatusCode(500, $"Error fetching flight status data: {ex.Message}");
            }
        }

        // GET: http://localhost:5000/api/waiting-times
        [HttpGet]
        [Route("api/waiting-times")]
        public async Task<IActionResult> GetWaitingTimes()
        {
            const string waitingTimesUrl = "https://www.istairport.com/umbraco/api/Checkpoint/GetWaitingTimes?culture=tr-TR";
            try
            {
                // Tek seferlik HttpRequestMessage
                using var request = new HttpRequestMessage(HttpMethod.Get, waitingTimesUrl);

                // Baþlýklarý ekle (UserAgent, Referrer, Accept, X-Requested-With vb.)
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                                                  "(KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

                request.Headers.Referrer = new Uri("https://www.istairport.com/ucuslar/ucus-bilgileri/gelen-ucuslar/");

                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonData = await response.Content.ReadAsStringAsync();
                return Content(jsonData, "application/json");
            }
            catch (HttpRequestException ex)
            {
                // Hata loglama vb.
                return StatusCode(500, $"Error fetching waiting times data: {ex.Message}");
            }
        }
    }
}
