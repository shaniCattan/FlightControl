using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
	public class InitialLocation
	{
/*		[IgnoreDataMember]
		public string id { get; set; }*/

		[JsonPropertyName("longitude")]
		public double Longitude { get; set; }

		[JsonPropertyName("latitude")]
		public double Latitude { get; set; }

		[JsonPropertyName("date_time")]
		public string Date_Time { get; set; }
	}
}
