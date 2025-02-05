using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirportDemo.Models;
using AirportDemo.Data;
using System.Security.Authentication;

namespace AirportDemo.Controllers
{
    [Authorize]
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index(string sortColumn, string sortDirection)
        {
            List<Flight> flights = await GetFlightsFromApi() ?? new List<Flight>();

            // Default sorting parameters
            if (string.IsNullOrEmpty(sortColumn))
            {
                sortColumn = "FlightNumber";
                sortDirection = "asc";
            }
            else
            {
                sortDirection = sortDirection == "asc" ? "desc" : "asc";
            }
            ViewData["SortColumn"] = sortColumn;
            ViewData["SortDirection"] = sortDirection;

            flights = sortColumn switch
            {
                "DepartureTime" => sortDirection == "asc" ? flights.OrderBy(f => f.DepartureTime).ToList() : flights.OrderByDescending(f => f.DepartureTime).ToList(),
                "Destination" => sortDirection == "asc" ? flights.OrderBy(f => f.Destination).ToList() : flights.OrderByDescending(f => f.Destination).ToList(),
                "DepartureLocation" => sortDirection == "asc" ? flights.OrderBy(f => f.DepartureLocation).ToList() : flights.OrderByDescending(f => f.DepartureLocation).ToList(),
                "FlightNumber" => sortDirection == "asc" ? flights.OrderBy(f => f.FlightNumber).ToList() : flights.OrderByDescending(f => f.FlightNumber).ToList(),
                "Status" => sortDirection == "asc" ? flights.OrderBy(f => f.Status).ToList() : flights.OrderByDescending(f => f.Status).ToList(),
                _ => flights.OrderBy(f => f.FlightNumber).ToList()
            };

            Console.WriteLine("Total flights to be displayed: " + flights.Count);
            return View(flights);
        }

        // New Action: Returns latest flights as JSON
        [HttpGet]
        public async Task<IActionResult> LatestFlights()
        {
            List<Flight> flights = await GetFlightsFromApi() ?? new List<Flight>();
            return Json(flights);
        }

        // New Action: Adds a fake flight for testing and returns it as JSON.
        [HttpPost]
        public IActionResult AddFakeFlight()
        {
            // Create a fake flight object with random values.
            var rnd = new Random();
            var flight = new Flight
            {
                FlightNumber = "FAKE" + rnd.Next(100, 999),
                Airline = "Fake Airline",
                DepartureLocation = "Test City",
                Destination = "Demo City",
                DepartureTime = DateTime.Now,
                Status = "New"
            };

            _context.Flights.Add(flight);
            _context.SaveChanges();

            // Return the new flight as JSON.
            return Json(flight);
        }

        // API call to get flights from external API (or local simulation)
        private async Task<List<Flight>> GetFlightsFromApi()
        {
            List<Flight> flights = new List<Flight>();

            try
            {
                var handler = new HttpClientHandler();
                // For testing only—disable certificate validation.
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                // Uncomment the next line if you need to force TLS 1.2:
                // handler.SslProtocols = SslProtocols.Tls12;

                using (var httpClient = new HttpClient(new LoggingHandler() { InnerHandler = handler }))
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
                    httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.istairport.com/ucuslar/ucus-bilgileri/gelen-ucuslar/");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                    httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");

                    var formData = new Dictionary<string, string>
                    {
                        { "nature", "0" },
                        { "searchTerm", "changeflight" },
                        { "pageSize", "20" },
                        { "isInternational", "0" },
                        { "date", "" },
                        { "endDate", "" },
                        { "culture", "tr" },
                        { "clickedButton", "" }
                    };

                    var content = new FormUrlEncodedContent(formData);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                    {
                        CharSet = "UTF-8"
                    };

                    var apiUrl = "https://www.istairport.com/umbraco/api/FlightInfo/GetFlightStatusBoard";
                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("API JSON Response: " + jsonString);

                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var apiResponse = JsonSerializer.Deserialize<FlightApiResponse>(jsonString, options);

                        if (apiResponse?.Result?.Data?.Flights != null)
                        {
                            Console.WriteLine("Number of flights in API response: " + apiResponse.Result.Data.Flights.Count);
                            flights = apiResponse.Result.Data.Flights.Select(f => new Flight
                            {
                                Id = f.Id,
                                FlightNumber = f.FlightNumber,
                                Airline = f.AirlineName,
                                DepartureLocation = f.FromCityName,
                                Destination = f.ToCityName,
                                DepartureTime = f.ScheduledDatetime,
                                Status = f.Remark
                            }).ToList();
                            Console.WriteLine("Number of flights after mapping: " + flights.Count);
                        }
                        else
                        {
                            Console.WriteLine("apiResponse.Result.Data.Flights is null or empty.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("API request failed with status code: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("API call error: " + ex.ToString());
            }

            return flights;
        }

        // CRUD işlemleri (Create, Edit, Delete, Details vb.) burada devam ediyor...
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(flight);
        }

        public IActionResult Details(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        public IActionResult Edit(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpPost]
        public IActionResult Edit(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Update(flight);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(flight);
        }

        public IActionResult Delete(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteAll()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        
    }
}
