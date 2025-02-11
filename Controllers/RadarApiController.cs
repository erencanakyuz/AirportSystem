using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AirportDemo.Controllers
{
    [Route("api/radar")]
    [ApiController]
    public class RadarApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RadarApiController> _logger;

        public RadarApiController(IHttpClientFactory httpClientFactory, ILogger<RadarApiController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// ICAO -> IATA dönüþüm tablosu.
        /// Ör. "THY" -> "TK", "PGT" -> "PC", "SXS" -> "XQ" vb.
        /// </summary>
        private static readonly Dictionary<string, string> icaoToIata = new Dictionary<string, string>
        {
            {"THY", "TK"},   // Turkish Airlines
            {"PGT", "PC"},   // Pegasus
            {"SXS", "XQ"},   // SunExpress
            {"KKK", "KK"},   // AtlasGlobal (Eski örnek)
            // Buraya ihtiyacýnýz olan baþka kodlar varsa ekleyebilirsiniz.
        };

        /// <summary>
        /// "THY2457" gibi bir ICAO kodunu alýp "TK2457" þeklinde çevirir.
        /// Eðer sözlükte yoksa orijinal callsign'i döndürür.
        /// </summary>
        private string ConvertCallsignIcaoToIata(string callsign)
        {
            if (string.IsNullOrWhiteSpace(callsign))
                return callsign;

            // Büyük harfe çevirip trim uygulayalým
            callsign = callsign.Trim().ToUpper();

            // En az 3 karakter (ör: "THY" prefix'i)
            if (callsign.Length < 3)
                return callsign;

            // Ýlk 3 harf ICAO prefix (örn. "THY")
            var icaoPrefix = callsign.Substring(0, 3);

            // Geri kalan kýsým uçuþ numarasý veya özel suffix (örn. "2457", "2SW", vb.)
            var suffix = callsign.Length > 3
                ? callsign.Substring(3)
                : string.Empty;

            // Sözlükten eþleþme aranýr
            if (icaoToIata.TryGetValue(icaoPrefix, out var iataPrefix))
            {
                // Ör: "THY" -> "TK", suffix = "2457", sonuç "TK2457"
                return iataPrefix + suffix;
            }

            // Eþleþme yoksa orijinalini döndür
            return callsign;
        }

        [HttpGet("flights")]
        public async Task<IActionResult> GetFlights()
        {
            // Define the geographic bounding box
            double lamin = 39.4732, lamax = 43.0768;
            double lomin = 26.3981, lomax = 31.1039;
            string url = $"https://opensky-network.org/api/states/all?lamin={lamin}&lomin={lomin}&lamax={lamax}&lomax={lomax}";

            using var client = _httpClientFactory.CreateClient();

            // OpenSky credentials (replace with valid ones)
            string openSkyUser = "UserTest1";
            string openSkyPassword = "2@LhapEtPxSL4pn";
            var encoded = Convert.ToBase64String(
                Encoding.GetEncoding("ISO-8859-1").GetBytes($"{openSkyUser}:{openSkyPassword}")
            );
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // Log the raw JSON content
                _logger.LogInformation("Received flight data: {Content}", content);

                var json = JObject.Parse(content);
                if (json["states"] is JArray states)
                {
                    _logger.LogInformation("Number of flight states received: {Count}", states.Count);

                    foreach (var stateToken in states)
                    {
                        if (stateToken is JArray state && state.Count > 1)
                        {
                            // state[1] => callsign
                            var originalCallsign = state[1]?.ToString();

                            // Dönüþtürülmüþ deðer
                            var convertedCallsign = ConvertCallsignIcaoToIata(originalCallsign);

                            // Güncelle
                            state[1] = convertedCallsign;

                            _logger.LogInformation(
                                "Original callsign: {Original}, Converted: {Converted}",
                                originalCallsign, convertedCallsign
                            );
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No valid 'states' array found in the JSON.");
                }

                // Güncellenmiþ JSON'u döndürüyoruz
                return Content(json.ToString(), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Flight data unavailable. Exception details: {Message}", ex.Message);
                return new JsonResult(new { error = "Flight data unavailable.", details = ex.Message });
            }
        }
    }
}
