using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	interface IFlightPlanManager
	{
		public void AddPlan(FlightPlan flightPlan, Dictionary<string, FlightPlan> plansDict);

	}
}
