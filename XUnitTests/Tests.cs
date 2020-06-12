using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace XUnitFlightsControllersTest
{
	public class FlightsControllersTest
	{
		private readonly ConcurrentDictionary<string, string> externalFlightsStub =
			new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, string> serversStub =
			new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, FlightPlan> flightPlansStub =
			new ConcurrentDictionary<string, FlightPlan>();

		[Fact]
		public void Get_flightPlanExists_ReturnsFlightPlan()
		{
			var mockFlightPlanManager = new FlightPlanManager(flightPlansStub, externalFlightsStub);
			var mockFlightPlanController = new FlightPlanController(mockFlightPlanManager);
			var testPlan = new FlightPlan(200, "ELAL",
				new InitialLocation()
				{
					Latitude = 34.234,
					Longitude = 21.234,
					Date_Time = "2020-12-27T00:01:30Z"
				}, new List<Segment>() { new Segment() {
					Latitude = 32.234,
					Longitude = 26.234,
					Timespan_Seconds = 650}
				});

			var response = mockFlightPlanController.Post(testPlan);
			var okResult = response as OkObjectResult;
			var flightRes = (FlightPlan)okResult.Value;
			Assert.Same(flightRes, testPlan);
		}
	}
}
