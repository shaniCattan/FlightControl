using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightsManager : IFlightsManager
	{
		public static ConcurrentDictionary<string, FlightPlan> plansDict;
		public static ConcurrentDictionary<string, string> externalActiveFlights;
		public static ConcurrentDictionary<string, string> servers;


		public FlightsManager(ConcurrentDictionary<string, FlightPlan> plans,
			ConcurrentDictionary<string, string> externals, ConcurrentDictionary<string, string> servers1)
		{
			plansDict = plans;
			externalActiveFlights = externals;
			servers = servers1;
		}

		private List<double> calcLongLat(KeyValuePair<string, FlightPlan> plan, DateTime initialDT,
			DateTime relativeToDT)
		{
			int cumulativeTimespan = 0, segIndex = 0;
			Segment currSeg = new Segment(), lastSeg = new Segment();
			DateTime segStartDT = initialDT, segEndDT = new DateTime();
			//calculate percentage of time flown in current segment
			//first determine in which segment we are according to relativeTo
			foreach (Segment seg in plan.Value.Segments)
			{
				segIndex++;
				cumulativeTimespan += seg.Timespan_Seconds;
				segEndDT = initialDT.AddSeconds(cumulativeTimespan);
				if ((DateTime.Compare(segEndDT, relativeToDT) >= 0))
				{
					//calculate beginning of current segments
					segStartDT = segEndDT.AddSeconds(-seg.Timespan_Seconds);
					currSeg = seg;
					break;
				}
			}
			//determine last segment
			if (segIndex > 1)
			{
				lastSeg = plan.Value.Segments[segIndex - 2];
			}
			//if we are in the first segment, then last segment == current segment
			else if (segIndex == 1)
			{
				lastSeg.Latitude = plan.Value.Initial_Location.Latitude;
				lastSeg.Longitude = plan.Value.Initial_Location.Longitude;
			}
			//calculate the timespan between the end of the last segment and relativeToDT
			TimeSpan progress = relativeToDT - segStartDT;
			TimeSpan segLength = segEndDT - segStartDT;
			double relativeProgress = progress / segLength;
			return new List<double>() {
				lastSeg.Latitude + relativeProgress*(currSeg.Latitude-lastSeg.Latitude),
				lastSeg.Longitude + relativeProgress*(currSeg.Longitude-lastSeg.Longitude),
			};
		}

		public List<Flights> GetActiveInternals(string relativeTo)
		{
			List<Flights> activeFlights = new List<Flights>();
			//create a DateTime object out of the relatieTo argument
			DateTime relativeToDT = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(relativeTo));

			//iterate through the flightPlans in our server, and if relativeTo's DateTime is 
			//between the plan's initial and final DateTime, add it to activeFlights array
			foreach (KeyValuePair<string, FlightPlan> plan in plansDict)
			{
				FlightPlan currentPlan = plan.Value;
				int totalFlightTime = 0;
				//create a DateTime object out of the flight plan's Date_Time string
				DateTime planInitialDT = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(
						currentPlan.Initial_Location.Date_Time));
				//calculate total flight time in seconds
				foreach (Segment seg in currentPlan.Segments)
				{
					totalFlightTime += seg.Timespan_Seconds;
				}
				//calculate final DateTime of flight plan
				DateTime planFinalDateTime = planInitialDT.AddSeconds(totalFlightTime);
				//if relativeTo's DateTime is between the plan's initial and final DateTime,
				//add it to activeFlights array
				if ((DateTime.Compare(relativeToDT, planInitialDT) >= 0) &&
					(DateTime.Compare(relativeToDT, planFinalDateTime) <= 0))
				{
					//determine Longitude and Latitude
					List<double> location = calcLongLat(plan, planInitialDT, relativeToDT);
					activeFlights.Add(new Flights()
					{
						Flight_ID = plan.Key,
						Latitude = location[0],
						Longitude = location[1],
						Passengers = currentPlan.Passengers,
						Company_Name = currentPlan.Company_Name,
						Date_Time = relativeTo,
						Is_External = false
					});
				}
			}
			return activeFlights;
		}

		public static async Task<string> getExternalFlights(string url)
		{
			WebRequest request = WebRequest.Create(url);
			request.Method = "GET";
			HttpWebResponse response = null;
			try
			{
				WebResponse answer = await request.GetResponseAsync();
				response = (HttpWebResponse)answer;
			} catch (Exception e)
			{
				throw e;
			}
			string strResult = null;
			using (Stream stream = response.GetResponseStream())
			{
				StreamReader sr = new StreamReader(stream);
				strResult = sr.ReadToEnd();
				sr.Close();
			}
			return strResult;
		}

		public void addToExternalsDict(List<Flights> externals, KeyValuePair<string, string> server)
		{
			foreach(var e in externals)
			{
				if (!externalActiveFlights.ContainsKey(e.Flight_ID))
				{
					externalActiveFlights.TryAdd(e.Flight_ID, server.Value);
				}
			}
		}

		public async Task<List<Flights>> GetExternalInternal(string relativeTo)
		{
			List<Flights> allActives = GetActiveInternals(relativeTo); //internal flights
			List<Flights> externals = new List<Flights>();
			string strResult = null;
			foreach (var server in servers)
			{
				string url = String.Format(server.Value + "/api/Flights?relative_to=" + relativeTo);
				try
				{
					strResult = await getExternalFlights(url);
				} catch
				{
					throw new Exception("Error in external server");
				}
				externals = JsonConvert.DeserializeObject<List<Flights>>(strResult);
				//add active flights from current server to allActives list which will be returned
				if (externals.Any())
				{
					externals = Validate(externals);
					allActives.AddRange(externals);
					addToExternalsDict(externals, server);
				}
			}
			return allActives;
		}

		public bool IsValidDateTime(string datetimeCheck)
		{
			string format = "yyyy-MM-ddTHH:mm:ssZ";
			DateTime dateTime;
			if (!DateTime.TryParseExact(datetimeCheck, format, CultureInfo.InvariantCulture,
				DateTimeStyles.None, out dateTime))
			{
				return false;
			}
			return true;
		}

		public List<Flights> Validate(List<Flights> externals)
		{
			foreach(var e in externals)
			{
				if ((e.Is_External.ToString() != "False") && (e.Is_External.ToString() != "True"))
				{
					externals.Remove(e);
				}
				else if (!int.TryParse(e.Passengers.ToString(), out int value))
				{
					externals.Remove(e);
				}
				else if (!IsValidDateTime(e.Date_Time.ToString()))
				{
					externals.Remove(e);
				}
			}
			return externals;
		}

		public bool DeleteFlight(string id)
		{
			if (plansDict.ContainsKey(id))
			{
				plansDict.TryRemove(id, out FlightPlan value);
				return true;
			}
			return false;
		}
	}
}
