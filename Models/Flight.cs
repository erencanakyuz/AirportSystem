using System;
using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        public string FlightNumber { get; set; } = null!;

        [Required]
        public string DepartureLocation { get; set; } = null!;

        [Required]
        public string Destination { get; set; } = null!;

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public string Status { get; set; } = "On Time";

        [Required]
        public string Airline { get; set; } = null!;
    }
}
