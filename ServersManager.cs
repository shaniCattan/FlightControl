using FlightControlWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace FlightControlWeb.Models
{
	public class ServersManager : IServerManager
	{
		public static ConcurrentDictionary<string, string> servers;

		public ServersManager(ConcurrentDictionary<string, string> servers1) => servers = servers1;

		public void AddServer(Server newServer)
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

		public ConcurrentDictionary<string, string> GetServers()
		{
			return servers;
		}

		public bool DeleteServer(string id)
		{
			if (servers.ContainsKey(id))
			{
				servers.TryRemove(id, out string value);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
