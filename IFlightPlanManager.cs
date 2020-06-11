using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public interface IFlightPlanManager
	{
		public ConcurrentDictionary<string, FlightPlan> PlansDict { get; set; }
		public ConcurrentDictionary<string, string> ExternalActiveFlights { get; set; }
		public void AddPlan(FlightPlan flightPlan);
	}
}
