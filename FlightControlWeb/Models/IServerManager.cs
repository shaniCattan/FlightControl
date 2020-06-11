using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public interface IServerManager
	{
		public void AddServer(Server newServer);
		public ConcurrentDictionary<string, string> GetServers();
		public bool DeleteServer(string id);
	}
}
