using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace FileUpload.Controllers
{
    [Produces("application/json")]
    [Route("api/Files")]
    public class FilesController : Controller
    {
        private readonly FileUploadContext _context;
        private readonly IHostingEnvironment _env;

        public FilesController(FileUploadContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Files
        [HttpGet]
        public IEnumerable<File> GetFile()
        {
            return _context.File;
        }

        [HttpPost("Test")]
        public async Task<IActionResult> Test([FromForm]IFormFile file)
        {
            var nowStamp = DateTime.Now;
            var rootPath = _env.ContentRootPath;

            var fileName = file.FileName;

            var fileIns = new File { FileName = fileName, Type = file.ContentType, UploadDate = nowStamp };

            if (file.Length > 0)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    var md5Arr = MD5.Create().ComputeHash(stream);

                    fileIns.MD5 = Convert.ToBase64String(md5Arr);
                    fileIns.FileContent = stream.ToArray();
                }

                await _context.File.AddAsync(fileIns);
                _context.SaveChanges();

                return Ok(fileIns);
            }
            else
            {
                return NoContent();
            }
        }

        // GET: api/Files/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var file = await _context.File.SingleOrDefaultAsync(m => m.ID == id);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        // PUT: api/Files/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFile([FromRoute] int id, [FromBody] File file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != file.ID)
            {
                return BadRequest();
            }

            _context.Entry(file).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
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

        // POST: api/Files
        [HttpPost]
        public async Task<IActionResult> PostFile([FromBody] File file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.File.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id = file.ID }, file);
        }

        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var file = await _context.File.SingleOrDefaultAsync(m => m.ID == id);
            if (file == null)
            {
                return NotFound();
            }

            _context.File.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(file);
        }

        private bool FileExists(int id)
        {
            return _context.File.Any(e => e.ID == id);
        }
    }
}