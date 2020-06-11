using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FlightControlWeb.Controllers;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models

{
    public class Flights
    {
        [JsonPropertyName("flight_id")]
        public string Flight_ID { get; set; }

        [Range(-180,180)]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [Range(-90,90)]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
            
        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; }

        [JsonPropertyName("date_time")]
        public string Date_Time { get; set; }

        bool isExternal = false;
        [JsonPropertyName("is_external")]
        public bool Is_External { get; set; }
    }
}
