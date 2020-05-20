using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanItemsController : ControllerBase
    {
        private readonly FlightPlanContext _context;

        public FlightPlanItemsController(FlightPlanContext context)
        {
            _context = context;
        }

        // GET: api/FlightPlanItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlanItem>>> GetFlightPlanItems()
        {
            return await _context.FlightPlanItems.ToListAsync();
        }

        // GET: api/FlightPlanItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlanItem>> GetFlightPlanItem(long id)
        {
            var flightPlanItem = await _context.FlightPlanItems.FindAsync(id);

            if (flightPlanItem == null)
            {
                return NotFound();
            }

            return flightPlanItem;
        }

        // PUT: api/FlightPlanItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlanItem(long id, FlightPlanItem flightPlanItem)
        {
            if (id != flightPlanItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(flightPlanItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightPlanItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlightPlanItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlanItem>> PostFlightPlanItem(FlightPlanItem flightPlanItem)
        {
            _context.FlightPlanItems.Add(flightPlanItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlightPlanItem), new { id = flightPlanItem.Id }, flightPlanItem);

            // return CreatedAtAction("GetFlightPlanItem", new { id = flightPlanItem.Id }, flightPlanItem);
        }

        // DELETE: api/FlightPlanItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlanItem>> DeleteFlightPlanItem(long id)
        {
            var flightPlanItem = await _context.FlightPlanItems.FindAsync(id);
            if (flightPlanItem == null)
            {
                return NotFound();
            }

            _context.FlightPlanItems.Remove(flightPlanItem);
            await _context.SaveChangesAsync();

            return flightPlanItem;
        }

        private bool FlightPlanItemExists(long id)
        {
            return _context.FlightPlanItems.Any(e => e.Id == id);
        }
    }
}
