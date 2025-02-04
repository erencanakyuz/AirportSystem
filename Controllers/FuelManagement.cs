using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
	public class FuelManagement
	{
		[Key]
		public int Id { get; set; }
		public string FuelType { get; set; } = null!;
		public int Quantity { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
