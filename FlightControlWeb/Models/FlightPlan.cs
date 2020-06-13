using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public FlightPlan() { }
        public FlightPlan(int pass, string name, InitialLocation initLoc, List<Segment> segs)
        {
            Passengers = pass;
            Company_Name = name;
            Initial_Location = initLoc;
            Segments = segs;
        }
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; }

        [JsonPropertyName("initial_location")]
        public InitialLocation Initial_Location { get; set; }

        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; }
    }
}
