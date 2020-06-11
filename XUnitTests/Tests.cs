using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace XUnitFlightsControllersTest
{
	public class FlightsControllersTest
	{
		/*		[Fact]
				public async Task GetActiveFlights_ExternalsExist_ReturnFlightsList()
				{
					//Arrange
					var server = new ServersController().Post(new Server("TS123", "http://rony7.atwebpages.com"));
					var controller = new FlightsController();
					var dictionary = new Dictionary<string, StringValues>();
					dictionary.Add("relative_to", new StringValues("2020-12-27T00:01:30Z"));
					dictionary.Add("showtoc", new StringValues("0"));

					controller.Request.Query = new QueryCollection(dictionary);

					//Act
					var result = await controller.GetActiveFlights("2020-12-27T00:01:30Z");

					//Assert
					Assert.IsAssignableFrom<List<Flights>>(result);
				}*/

		private readonly ConcurrentDictionary<string, string> externalFlightsStub =
			new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, string> serversStub =
			new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, FlightPlan> flightPlansStub =
			new ConcurrentDictionary<string, FlightPlan>();

		/*		[Fact]
				public void Get_OnlyExternalExists_ReturnsFlightPlan()
				{
					var controller = new FlightPlanController(flightPlansStub.Object);
					FlightsController.externalActiveFlights.TryAdd("AB345", "http://rony7.atwebpages.com");

					var result = controller.Get("AB345");

					Assert.IsAssignableFrom<Flights>(result);
				}*/

		/*[Fact]
		public void Get_InternalsExist_ReturnsFlightPlan()
		{
			var mockController = new Mock<IFlightsController>();
			var list = new List<Flights>()
			{
				new Flights()
				{
					Flight_ID = "AB3445",
					Date_Time = "2020-12-27T00:01:30Z",
					Longitude = 34.234,
					Latitude = 36.321,
					Company_Name = "ELAL",
					Is_External = false,
					Passengers = 300
				},
				new Flights()
				{
					Flight_ID = "JCI525",
					Date_Time = "2020-12-27T00:01:30Z",
					Longitude = 37.312,
					Latitude = 31.234,
					Company_Name = "Delta",
					Is_External = false,
					Passengers = 123
				}
			};
			
			mockController.Setup(a => a.GetActiveFlights("2020-12-27T00:01:30Z")).ReturnsAsync(list);

			//Act
			var mockFlightPlanController = new FlightPlanController(flightPlansStub.Object);
			var flights = mockFlightPlanController.Get("AB3445");

			//Assert
			((OkObjectResult)flights.Result).Value.Should().BeSameAs(list);
			//Assert.IsAssignableFrom<List<Flights>>(result);
		}*/
		/*		[Fact]
				public void Get_InternalsExist_ReturnsFlightPlan()
				{
					var mockFlightPlanController = new Mock<IFlightPlanController>();
					var mockFlightsController = new Mock<IFlightsController>();
					var mockFlightsManager = new Mock<IFlightsManager>();
					var testPlan = new FlightPlan(200, "ELAL",
						new InitialLocation()
						{
							Latitude = 34.234,
							Longitude = 21.234,
							Date_Time = "2020-12-27T00:01:30Z"
						}, new List<Segment>() { new Segment() {
							Latitude = 32.234,
							Longitude = 26.234,
							Timespan_Seconds = 650} });

					mockFlightPlanController.SetupGet(a => a.Post(testPlan));
					var mockFlightPlanController = new FlightPlanController(flightPlansStub.Object);

					//Act
					var flights = mockFlightPlanController.Get("AB3445");
					var testFlightsController = new FlightsController(externalFlightsStub.Object,
						flightPlansStub.Object);

					//Assert
					//Assert.IsAssignableFrom<FlightPlan>(flights);
				}*/

		[Fact]
		public void Get_flightPlanExists_ReturnsFlightPlan()
		{
			var mockFlightPlanController = new FlightPlanController
				(new ConcurrentDictionary<string, FlightPlan>());
			var testPlan = new FlightPlan(200, "ELAL",
				new InitialLocation()
				{
					Latitude = 34.234,
					Longitude = 21.234,
					Date_Time = "2020-12-27T00:01:30Z"
				}, new List<Segment>() { new Segment() {
					Latitude = 32.234,
					Longitude = 26.234,
					Timespan_Seconds = 650} });

			var response = mockFlightPlanController.Post(testPlan);
			var okResult = response as OkObjectResult;
			var flightRes = (FlightPlan)okResult.Value;
			Assert.Same(flightRes, testPlan);
		}
		/*		[Fact]
				public void Get_InternalsExist_ReturnsFlightPlan()
				{
					var myContext = new Mock<HttpContext>();
					myContext.SetupGet(x => x.Request.QueryString).Returns(
						new QueryString("?relative_to=2020-12-27T00:01:30Z"));
					var myControllerContext = new ControllerContext() { HttpContext = myContext.Object};
					var controller = new FlightsController(externalFlightsStub, flightPlansStub)
					{
						ControllerContext = myControllerContext
					};
					var result = controller.GetActiveFlights("2020-12-27T00:02:30Z");
				}*/
	}
}
