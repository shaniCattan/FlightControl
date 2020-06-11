using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public interface IFlightsManager
	{
		public List<Flights> GetActiveInternals(string relativeTo);
		public Task<List<Flights>> GetExternalInternal(string relativeTo);
		public bool IsValidDateTime(string datetimeCheck);
		public bool DeleteFlight(string id);
	}
}
