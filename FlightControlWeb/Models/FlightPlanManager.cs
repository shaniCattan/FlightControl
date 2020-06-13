using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightPlanManager : IFlightPlanManager
	{
		public ConcurrentDictionary<string, FlightPlan> PlansDict { get; set; }
		public ConcurrentDictionary<string, string> ExternalActiveFlights { get; set; }

		public FlightPlanManager(ConcurrentDictionary<string, FlightPlan> plansDict1,
			ConcurrentDictionary<string, string> externals)
		{
			PlansDict = plansDict1;
			ExternalActiveFlights = externals;
		}

		public static List<string> usedIDs = new List<string>();
		public string NewID()
		{
			StringBuilder tempPass = new StringBuilder();
			Random random = new Random();
			string password;
			//create new password which consists of two upper case letters and 4 ints
			//if it's already in use, create a new one and override the old one
			do
			{
				for (int i = 0; i < 2; i++)
				{
					char c = Convert.ToChar(random.Next(65, 90));
					tempPass.Append(c);
				}
				password = tempPass.Append(random.Next(1000, 9999)).ToString();
			} while (usedIDs.Contains(password));

			usedIDs.Add(password);
			return password;
		}

		public void AddPlan(FlightPlan flightPlan)
		{
			PlansDict.TryAdd(NewID(), flightPlan);
		}
	}
}
