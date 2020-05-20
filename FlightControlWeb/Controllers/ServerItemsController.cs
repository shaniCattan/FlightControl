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
    public class ServerItemsController : ControllerBase
    {
        private readonly ServerContext _context;

        public ServerItemsController(ServerContext context)
        {
            _context = context;
        }

        // GET: api/ServerItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerItem>>> GetServerItems()
        {
            return await _context.ServerItems.ToListAsync();
        }

        // GET: api/ServerItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServerItem>> GetServerItem(long id)
        {
            var serverItem = await _context.ServerItems.FindAsync(id);

            if (serverItem == null)
            {
                return NotFound();
            }

            return serverItem;
        }

        // PUT: api/ServerItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServerItem(long id, ServerItem serverItem)
        {
            if (id != serverItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(serverItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerItemExists(id))
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

        // POST: api/ServerItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ServerItem>> PostServerItem(ServerItem serverItem)
        {
            _context.ServerItems.Add(serverItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServerItem), new { id = serverItem.Id }, serverItem);

           // return CreatedAtAction("GetServerItem", new { id = serverItem.Id }, serverItem);
        }

        // DELETE: api/ServerItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServerItem>> DeleteServerItem(long id)
        {
            var serverItem = await _context.ServerItems.FindAsync(id);
            if (serverItem == null)
            {
                return NotFound();
            }

            _context.ServerItems.Remove(serverItem);
            await _context.SaveChangesAsync();

            return serverItem;
        }

        private bool ServerItemExists(long id)
        {
            return _context.ServerItems.Any(e => e.Id == id);
        }
    }
}
