using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightControlWeb.Controllers
{
	[Route("api/[controller]")]
	public class FlightPlanController : Controller
	{
		private IFlightPlanManager manager = new FlightPlanManager();
		public static Dictionary<string, FlightPlan> plansDict = new Dictionary<string, FlightPlan>();

		// GET: api/<controller>
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public async Task<Object> Get(string id)
		{
			//if it's in our dictionary return it, otherwise look for it in external servers
			FlightPlan flightPlan;
			if (plansDict.TryGetValue(id, out flightPlan))
			{
				return flightPlan;
			} else
			{
				if (FlightsController.externalActiveFlights.TryGetValue(id, out string serverUrl))
				{
					string exPlan = await new FlightsManager().
						getExternalFlights(serverUrl+"/api/FlightPlan/"+id);
					return JsonConvert.DeserializeObject<FlightPlan>(exPlan);
				} else
				{
					return BadRequest("No flight plan of id no. " + id + " was found");
				}
			}
		}

		// POST api/<controller>
		[HttpPost]
		public ActionResult<string> Post([FromBody]FlightPlan flightPlan)
		{
			if (flightPlan != null)
			{
				manager.AddPlan(flightPlan, plansDict);
				return Ok("Flight plan added successfully");
			}
			return BadRequest("Flight plan was not added to server. " +
				"Possibly one or more of the fields in the flight plan is incorrect");
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public void Delete(string id)
		{
		}
	}
}
