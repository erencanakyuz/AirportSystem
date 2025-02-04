using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
	public class Maintenance
	{
		[Key]
		public int Id { get; set; }
		public string Aircraft { get; set; } = null!;
		public string Description { get; set; } = null!;
		public DateTime ScheduledDate { get; set; }
	}
}
