﻿using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;

namespace FlightControlWeb.Controllers
{
	[Route("api/[controller]")]
	public class ServersController : Controller
	{
		private IServerManager srvManager;

		public ServersController(IServerManager sm) => srvManager = sm;

		// GET: api/<controller>
		[HttpGet]
		public ActionResult<ConcurrentDictionary<string,string>> Get()
		{
			try
			{
				return Ok(srvManager.GetServers());
			} catch (Exception e)
			{
				return BadRequest(e.Message);
			}
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
				srvManager.AddServer(newServer);
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
				return BadRequest("No external server of ID no. " + id + 
					" exists in this server's list");
			}
		}
	}
}
