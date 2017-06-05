using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;

namespace FileUpload.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private readonly FileUploadContext _context;

        private class OrderReturnInfo
        {
            public int id;
            public string generateTime;
            public ICollection<File> files;

            public OrderReturnInfo(Order order)
            {
                id = order.ID;
                generateTime = order.GenerateTime.ToString("G");
                files = order.Files;
            }
        }

        public OrdersController(FileUploadContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetFile()
        {
            List<OrderReturnInfo> emitedList = new List<OrderReturnInfo>();
            foreach (var order in _context.Order.ToList())
            {
                emitedList.Add(new OrderReturnInfo(order));
            }
            return Ok(emitedList);
        }


        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id, [FromQuery] string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.ID == id);

            if (order == null)
            {
                return NotFound();
            }
            else if (order.Password != password)
            {
                return Forbid();
            }

            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] int[] files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateTime generateTime = DateTime.Now;
            string password = Guid.NewGuid().ToString().Split('-')[1];
            List<File> filesList = new List<File>();

            foreach (var fileId in files)
            {
                var file = await _context.File.SingleOrDefaultAsync(m => m.ID == fileId);
                if (file != null)
                {
                    filesList.Add(file);
                }
            }
            if (filesList.Count < 1)
            {
                return BadRequest();
            }

            Order order = new Order { GenerateTime = generateTime, Password = password, Files = filesList };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.ID }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.ID == id);
        }
    }
}