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
    public class FlightItemsController : ControllerBase
    {
        private readonly FlightContext _context;

        public FlightItemsController(FlightContext context)
        {
            _context = context;
        }

        // GET: api/FlightItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightItem>>> GetFlightItems()
        {
            return await _context.FlightItems.ToListAsync();
        }

        // GET: api/FlightItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightItem>> GetFlightItem(long id)
        {
            var flightItem = await _context.FlightItems.FindAsync(id);

            if (flightItem == null)
            {
                return NotFound();
            }

            return flightItem;
        }

        // PUT: api/FlightItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightItem(long id, FlightItem flightItem)
        {
            if (id != flightItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(flightItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightItemExists(id))
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

        // POST: api/FlightItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightItem>> PostFlightItem(FlightItem flightItem)
        {
            _context.FlightItems.Add(flightItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlightItem), new { id = flightItem.Id }, flightItem);
            //     return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/FlightItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightItem>> DeleteFlightItem(long id)
        {
            var flightItem = await _context.FlightItems.FindAsync(id);
            if (flightItem == null)
            {
                return NotFound();
            }

            _context.FlightItems.Remove(flightItem);
            await _context.SaveChangesAsync();

            return flightItem;
        }

        private bool FlightItemExists(long id)
        {
            return _context.FlightItems.Any(e => e.Id == id);
        }
    }
}
