using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirportDemo.Models
{
	public class FlightApiResponse
	{
		[JsonPropertyName("status")]
		public bool Status { get; set; }

		[JsonPropertyName("message")]
		public int Message { get; set; }

		// JSON deserialize sýrasýnda deðer atanacak; 
		// "default!" operatörü derleyiciye bu property'sinin null olmayacaðýnýn garantisini verir.
		[JsonPropertyName("result")]
		public FlightResult Result { get; set; } = default!;
	}

	public class FlightResult
	{
		[JsonPropertyName("data")]
		public FlightData Data { get; set; } = default!;
	}

	public class FlightData
	{
		[JsonPropertyName("header")]
		public object Header { get; set; } = new object();

		[JsonPropertyName("flights")]
		public List<FlightDto> Flights { get; set; } = new List<FlightDto>();
	}

	public class FlightDto
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("flightNature")]
		public int FlightNature { get; set; }

		[JsonPropertyName("flightNumber")]
		public string FlightNumber { get; set; } = string.Empty;

		[JsonPropertyName("airlineCode")]
		public string AirlineCode { get; set; } = string.Empty;

		[JsonPropertyName("airlineName")]
		public string AirlineName { get; set; } = string.Empty;

		[JsonPropertyName("airlineLogo")]
		public string AirlineLogo { get; set; } = string.Empty;

		[JsonPropertyName("isInternational")]
		public int IsInternational { get; set; }

		[JsonPropertyName("fromCityCode")]
		public string FromCityCode { get; set; } = string.Empty;

		[JsonPropertyName("fromCityName")]
		public string FromCityName { get; set; } = string.Empty;

		[JsonPropertyName("toCityCode")]
		public string ToCityCode { get; set; } = string.Empty;

		[JsonPropertyName("toCityName")]
		public string ToCityName { get; set; } = string.Empty;

		[JsonPropertyName("scheduledDatetime")]
		public DateTime ScheduledDatetime { get; set; }

		[JsonPropertyName("estimatedDatetime")]
		public DateTime EstimatedDatetime { get; set; }

		[JsonPropertyName("gate")]
		public string Gate { get; set; } = string.Empty;

		[JsonPropertyName("stopOversCode")]
		public string StopOversCode { get; set; } = string.Empty;

		[JsonPropertyName("stopOversName")]
		public string StopOversName { get; set; } = string.Empty;

		[JsonPropertyName("carousel")]
		public string Carousel { get; set; } = string.Empty;

		[JsonPropertyName("codeshare")]
		public List<string> Codeshare { get; set; } = new List<string>();

		[JsonPropertyName("airlineCodeList")]
		public List<string> AirlineCodeList { get; set; } = new List<string>();

		[JsonPropertyName("landsideRemarkCode")]
		public string LandsideRemarkCode { get; set; } = string.Empty;

		[JsonPropertyName("landsideRemark")]
		public string LandsideRemark { get; set; } = string.Empty;

		[JsonPropertyName("airsideRemarkCode")]
		public string AirsideRemarkCode { get; set; } = string.Empty;

		[JsonPropertyName("airsideRemark")]
		public string AirsideRemark { get; set; } = string.Empty;

		[JsonPropertyName("counter")]
		public string Counter { get; set; } = string.Empty;

		[JsonPropertyName("remarkCode")]
		public string RemarkCode { get; set; } = string.Empty;

		[JsonPropertyName("remarkColorCode")]
		public string RemarkColorCode { get; set; } = string.Empty;

		[JsonPropertyName("remark")]
		public string Remark { get; set; } = string.Empty;

		[JsonPropertyName("baggageRemark")]
		public string BaggageRemark { get; set; } = string.Empty;

		[JsonPropertyName("baggageRemarkCode")]
		public string BaggageRemarkCode { get; set; } = string.Empty;

		[JsonPropertyName("baggageRemarkColorCode")]
		public string BaggageRemarkColorCode { get; set; } = string.Empty;
	}
}
