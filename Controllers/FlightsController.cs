using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirportDemo.Models;    // Flight ve FlightResponse modellerinin bulunduğu namespace
using AirportDemo.Data;      // ApplicationDbContext

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
        // API’den uçuş verilerini çekip sıralama uyguladıktan sonra view’a gönderiyoruz.
        public async Task<IActionResult> Index(string sortColumn, string sortDirection)
        {
            // API çağrısından gelen liste; null ise boş listeye çeviriyoruz.
            List<Flight> flights = await GetFlightsFromApi() ?? new List<Flight>();

            // Sıralama parametreleri yoksa varsayılan değer ataması
            if (string.IsNullOrEmpty(sortColumn))
            {
                sortColumn = "FlightNumber";
                sortDirection = "asc";
            }
            else
            {
                // Her tıklamada sıralama yönünü tersine çeviriyoruz
                sortDirection = sortDirection == "asc" ? "desc" : "asc";
            }
            ViewData["SortColumn"] = sortColumn;
            ViewData["SortDirection"] = sortDirection;

            // Sıralama işlemi (modeldeki property isimleri API'den gelenle uyumlu olmalı)
            flights = sortColumn switch
            {
                "DepartureTime" => sortDirection == "asc"
                                    ? flights.OrderBy(f => f.DepartureTime).ToList()
                                    : flights.OrderByDescending(f => f.DepartureTime).ToList(),
                "Destination" => sortDirection == "asc"
                                    ? flights.OrderBy(f => f.Destination).ToList()
                                    : flights.OrderByDescending(f => f.Destination).ToList(),
                "DepartureLocation" => sortDirection == "asc"
                                    ? flights.OrderBy(f => f.DepartureLocation).ToList()
                                    : flights.OrderByDescending(f => f.DepartureLocation).ToList(),
                "FlightNumber" => sortDirection == "asc"
                                    ? flights.OrderBy(f => f.FlightNumber).ToList()
                                    : flights.OrderByDescending(f => f.FlightNumber).ToList(),
                "Status" => sortDirection == "asc"
                                    ? flights.OrderBy(f => f.Status).ToList()
                                    : flights.OrderByDescending(f => f.Status).ToList(),
                _ => flights.OrderBy(f => f.FlightNumber).ToList()
            };

            return View(flights);
        }

        // API’ye POST isteği gönderip uçuş listesini döndüren metot.
        private async Task<List<Flight>> GetFlightsFromApi()
        {
            List<Flight> flights = new List<Flight>();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Gerekli başlıkları ekliyoruz:
                    httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.istairport.com/ucuslar/ucus-bilgileri/gelen-ucuslar/");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                    httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");

                    // API’nin beklediği form verileri:
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

                    // API isteğini yapıyoruz
                    var response = await httpClient.PostAsync("https://www.istairport.com/umbraco/api/FlightInfo/GetFlightStatusBoard", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine("API Yanıtı: " + jsonString);

                        // Önce direkt liste olarak deserialize etmeyi deniyoruz.
                        try
                        {
                            flights = JsonSerializer.Deserialize<List<Flight>>(jsonString, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? new List<Flight>();
                        }
                        catch (JsonException)
                        {
                            // Eğer API yanıtı bir wrapper nesne içeriyorsa:
                            var flightResponse = JsonSerializer.Deserialize<FlightResponse>(jsonString, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            flights = flightResponse?.Flights ?? new List<Flight>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapabilirsiniz.
                System.Diagnostics.Debug.WriteLine("API çağrısı hatası: " + ex.Message);
            }

            return flights;
        }

        // Aşağıdaki CRUD işlemleri (Create, Edit, Delete, Details vb.) yerel veritabanı (_context) ile çalışıyor.
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

        public IActionResult GenerateRandomData()
        {
            var randomFlights = new[]
            {
                new Flight { FlightNumber = "TK100", Airline = "Turkish Airlines", DepartureLocation = "Istanbul", Destination = "London", DepartureTime = DateTime.Now.AddHours(3), Status = "Scheduled" },
                new Flight { FlightNumber = "BA200", Airline = "British Airways", DepartureLocation = "Paris", Destination = "New York", DepartureTime = DateTime.Now.AddHours(5), Status = "In Air" },
                new Flight { FlightNumber = "LH300", Airline = "Lufthansa", DepartureLocation = "Frankfurt", Destination = "Berlin", DepartureTime = DateTime.Now.AddHours(2), Status = "Delayed" },
                new Flight { FlightNumber = "QR400", Airline = "Qatar Airways", DepartureLocation = "Doha", Destination = "Doha", DepartureTime = DateTime.Now.AddHours(6), Status = "Scheduled" },
                new Flight { FlightNumber = "EK500", Airline = "Emirates", DepartureLocation = "Dubai", Destination = "Dubai", DepartureTime = DateTime.Now.AddHours(4), Status = "In Air" }
            };

            _context.Flights.AddRange(randomFlights);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
