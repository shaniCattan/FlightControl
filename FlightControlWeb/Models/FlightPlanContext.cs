using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Models
{
    public class FlightPlanContext : DbContext
    {
        public FlightPlanContext(DbContextOptions<FlightPlanContext> options) : base(options)
        {

        }

        public DbSet<FlightPlanItem> FlightPlanItems { get; set; }

        public DbSet<FlightControlWeb.Models.ServerItem> ServerItem { get; set; }
    }
}