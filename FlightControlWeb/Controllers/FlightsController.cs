using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightControlWeb.Controllers
{
	[Route("api/[controller]")]
	public class FlightsController : Controller
	{
		private IFlightsManager flightsManager;

		public FlightsController(IFlightsManager flightsManager1) => flightsManager = flightsManager1;

		[HttpGet]
		public async Task<Object> GetActiveFlights([FromQuery(Name = "relative_to")] string relativeTo)
		{
			if (!flightsManager.IsValidDateTime(relativeTo))
			{
				return BadRequest("relative_to format should be yyyy-MM-ddTHH:mm:ssZ");
			}

			string request = Request.QueryString.Value;
			bool isExternal = request.Contains("sync_all");
			List<Flights> actives = new List<Flights>();
			if (!isExternal)
			{
				actives = flightsManager.GetActiveInternals(relativeTo);
			}
			else
			{
				try
				{
					actives = await flightsManager.GetExternalInternal(relativeTo);
				} catch (Exception e)
				{
					return BadRequest(e.Message);
				}
			}
			return Ok(actives);
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		[HttpPost]
		public void Post([FromBody]string value)
		{
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public ActionResult<string> Delete(string id)
		{
			if (flightsManager.DeleteFlight(id))
			{
				return Ok("Flight no. " + id + " deleted successfully");
			}
			else
			{
				return BadRequest("Id does not exist in server");
			}
		}
	}
}
