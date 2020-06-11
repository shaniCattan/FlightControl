using FlightControlWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace FlightControlWeb.Models
{
	public class ServersManager
	{
		public ServersManager() { }

		public void AddServer(Server newServer, ConcurrentDictionary<string, string> servers)
		{
			if (!servers.ContainsKey(newServer.Server_ID)) //check if id exists
			{
				if (!servers.Any(ser => ser.Value.Equals(newServer.Server_URL,
					StringComparison.CurrentCultureIgnoreCase))) //check if URL exists
				{
					servers.TryAdd(newServer.Server_ID, newServer.Server_URL);
				} else
				{
					throw new ArgumentException("Server URL is already in the list");
				}
			} else
			{
				throw new ArgumentException("ID is already in use");
			}
		}

		public bool DeleteServer(string id)
		{
			if (ServersController.servers.ContainsKey(id))
			{
				ServersController.servers.TryRemove(id, out string value);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
