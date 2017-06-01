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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Linq;


namespace FileUpload.Controllers
{
    [Produces("application/json")]
    [Route("api/Files")]
    public class FilesController : Controller
    {
        private readonly FileUploadContext _context;
        private readonly IHostingEnvironment _env;

        private class FileReturnInfo
        {
            public int id;
            public string fileName;
            public DateTime uploadDate;
            public string type;
            public string md5;
            public bool hasPreviewImage;

            public FileReturnInfo(File file)
            {
                id = file.ID;
                fileName = file.FileName;
                uploadDate = file.UploadDate;
                type = file.Type;
                md5 = file.MD5;
                hasPreviewImage = file.PreviewImage != null;
            }
        }

        public FilesController(FileUploadContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Files
        [HttpGet]
        public IActionResult GetFile()
        {
            List<FileReturnInfo> emitedList = new List<FileReturnInfo> { };
            foreach (var file in _context.File.ToList())
            {
                emitedList.Add(new FileReturnInfo(file));
            }
            return Ok(emitedList);
        }

        // POST: api/Files
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm]IFormFile uploadFile)
        {
            var nowStamp = DateTime.Now;
            var rootPath = _env.ContentRootPath;

            var file = new File { FileName = uploadFile.FileName, Type = uploadFile.ContentType, UploadDate = nowStamp };

            if (uploadFile.Length > 0)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    await uploadFile.CopyToAsync(stream);
                    stream.Position = 0;
                    var md5Arr = MD5.Create().ComputeHash(stream);

                    file.MD5 = Convert.ToBase64String(md5Arr);
                    file.FileContent = stream.ToArray();

                    if (file.Type == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                    {
                        var resultImage = CovertFile.Covert(rootPath, file.MD5, ".docx", stream);

                        using (var previewStream = new System.IO.FileStream(resultImage, System.IO.FileMode.Open))
                        {
                            await stream.FlushAsync();
                            stream.Position = 0;
                            await previewStream.CopyToAsync(stream);
                            file.PreviewImage = stream.ToArray();
                        }
                    }
                }

                await _context.File.AddAsync(file);
                _context.SaveChanges();

                return Ok(new FileReturnInfo(file));
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

            return Ok(new FileReturnInfo(file));
        }

        // GET: api/Files/5/Download
        [HttpGet("{id}/Download")]
        public async Task<IActionResult> Download([FromRoute] int id)
        {
            var file = await _context.File.SingleOrDefaultAsync(m => m.ID == id);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.FileContent, file.Type, file.FileName);
        }

        // GET: api/Files/5/Preview
        [HttpGet("{id}/Preview")]
        public async Task<IActionResult> Preview([FromRoute] int id)
        {
            var file = await _context.File.SingleOrDefaultAsync(m => m.ID == id);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.PreviewImage, "image/png", $"{file.MD5}.png");
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