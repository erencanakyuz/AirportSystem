using System;
using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FlightNumber { get; set; } = null!;

        [Required]
        public string Destination { get; set; } = null!;
        public string DepartureLocation { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
    }
}
