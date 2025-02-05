using System.Collections.Generic;
namespace AirportDemo.Models
{
    public class FlightResponse
    {
        public List<Flight> Flights { get; set; } = new List<Flight>();
    }
}
