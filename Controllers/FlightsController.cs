using Microsoft.AspNetCore.Mvc;
using AirportDemo.Data;
using AirportDemo.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace AirportDemo.Controllers
{
    [Authorize] // Kullanıcı girişi gerektiren tüm işlemler
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        public IActionResult Index(string sortColumn, string sortDirection)
        {
            var flights = _context.Flights.AsQueryable();

            // Varsayılan sıralama yönü
            ViewData["SortDirection"] = "asc";
            ViewData["SortColumn"] = sortColumn;

            if (!string.IsNullOrEmpty(sortColumn))
            {
                sortDirection = (sortDirection == "asc") ? "desc" : "asc";
                ViewData["SortDirection"] = sortDirection;
            }

            // Sıralama işlemi
            flights = sortColumn switch
            {
                "DepartureTime" => sortDirection == "asc" ? flights.OrderByDescending(f => f.DepartureTime) : flights.OrderBy(f => f.DepartureTime),
                "Destination" => sortDirection == "asc" ? flights.OrderBy(f => f.Destination) : flights.OrderByDescending(f => f.Destination),
                "DepartureLocation" => sortDirection == "asc" ? flights.OrderBy(f => f.DepartureLocation) : flights.OrderByDescending(f => f.DepartureLocation),
                "FlightNumber" => sortDirection == "asc" ? flights.OrderBy(f => f.FlightNumber) : flights.OrderByDescending(f => f.FlightNumber),
                _ => flights.OrderBy(f => f.Id)
            };

            return View(flights.ToList());
        }

        [HttpGet]
        public JsonResult FilterFlights(string destination, string departureLocation, string flightNumber, DateTime? departureDate)
        {
            var flights = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(destination))
            {
                flights = flights.Where(f => f.Destination.ToLower().Contains(destination.ToLower()));
            }
            if (!string.IsNullOrEmpty(departureLocation))
            {
                flights = flights.Where(f => f.DepartureLocation.ToLower().Contains(departureLocation.ToLower()));
            }
            if (!string.IsNullOrEmpty(flightNumber))
            {
                flights = flights.Where(f => f.FlightNumber.ToLower().Contains(flightNumber.ToLower()));
            }
            if (departureDate.HasValue)
            {
                flights = flights.Where(f => f.DepartureTime.Date == departureDate.Value.Date);
            }

            return Json(flights.ToList());
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flights/Create
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

        // GET: Flights/Details/5
        public IActionResult Details(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // GET: Flights/Edit/5
        public IActionResult Edit(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flights/Edit/5
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

        // GET: Flights/Delete/5
        public IActionResult Delete(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flights/Delete/5
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
                new Flight { FlightNumber = "TK100", DepartureLocation = "Istanbul", Destination = "London", DepartureTime = DateTime.Now.AddHours(3) },
                new Flight { FlightNumber = "BA200", DepartureLocation = "Paris", Destination = "New York", DepartureTime = DateTime.Now.AddHours(5) },
                new Flight { FlightNumber = "LH300", DepartureLocation = "Frankfurt", Destination = "Berlin", DepartureTime = DateTime.Now.AddHours(2) },
                new Flight { FlightNumber = "QR400", DepartureLocation = "Doha", Destination = "Doha", DepartureTime = DateTime.Now.AddHours(6) },
                new Flight { FlightNumber = "EK500", DepartureLocation = "Dubai", Destination = "Dubai", DepartureTime = DateTime.Now.AddHours(4) }
            };

            _context.Flights.AddRange(randomFlights);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
