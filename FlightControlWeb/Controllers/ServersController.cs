using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightControlWeb.Controllers
{
	[Route("api/[controller]")]
	public class ServersController : Controller
	{
		private ServersManager srvManager = new ServersManager();
		public static Dictionary<string, string> servers = new Dictionary<string, string>();

		// GET: api/<controller>
		[HttpGet]
		public Dictionary<string,string> Get()
		{
			return servers;
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		[HttpPost]
		public ActionResult<string> Post([FromBody]Server newServer)
		{
			try
			{
				srvManager.AddServer(newServer, servers);
				return Ok("Server added successfully");
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
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
			if (srvManager.DeleteServer(id))
			{
				return Ok("Server no. " + id + " deleted successfully");
			}
			else
			{
				return BadRequest("No external server of ID no." + id + 
					" exists in this server's list");
			}
		}
	}
}
