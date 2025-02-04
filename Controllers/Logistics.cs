using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
    public class Logistics
    {
        [Key]
        public int Id { get; set; }
        public string CargoType { get; set; } = null!;
        public int Weight { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
