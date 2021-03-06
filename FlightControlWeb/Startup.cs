using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using System.Collections.Concurrent;

namespace FlightControlWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services.AddControllers();

            services.AddSingleton<ConcurrentDictionary<string, string>>(); //servers
            services.AddSingleton<ConcurrentDictionary<string, string>>(); //externalActiveFlights
            services.AddSingleton<ConcurrentDictionary<string, FlightPlan>>(); //flightPlansDictionary
            services.AddSingleton<IFlightsManager, FlightsManager>();
            services.AddSingleton<IFlightPlanManager, FlightPlanManager>();
            services.AddSingleton<IServerManager, ServersManager>();


            services.AddMemoryCache();

            services.AddHttpClient("api", client => client.DefaultRequestHeaders.Add("Accept", "application/json"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}