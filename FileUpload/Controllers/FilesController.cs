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

        public class FileReturnInfo
        {
            public int id;
            public string fileName;
            public string uploadDate;
            public string type;
            public string md5;
            public bool hasPreviewImage;

            public FileReturnInfo(File file)
            {
                id = file.ID;
                fileName = file.FileName;
                uploadDate = file.UploadDate.ToString("G");
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

            if (uploadFile == null)
            {
                return NoContent();
            }

            var file = new File { FileName = uploadFile.FileName, Type = uploadFile.ContentType, UploadDate = nowStamp };

            if (uploadFile.Length > 0)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    await uploadFile.CopyToAsync(stream);
                    stream.Position = 0;
                    var md5Arr = MD5.Create().ComputeHash(stream);
                  
                    StringBuilder sBuilder = new StringBuilder();
                    
                    for (int i = 0; i < md5Arr.Length; i++)
                    {
                        sBuilder.Append(md5Arr[i].ToString("x2"));
                    }
                    
                    file.MD5 = sBuilder.ToString();
                    file.FileContent = stream.ToArray();

                    var extension = "";

                    switch (file.Type)
                    {
                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                            extension = ".docx";
                            break;
                        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            extension = ".xlsx";
                            break;
                        case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                            extension = ".pptx";
                            break;
                        case "application/msword":
                            extension = ".doc";
                            break;
                        case "application/vnd.ms-excel":
                            extension = ".xls";
                            break;
                        case "application/vnd.ms-powerpoint":
                            extension = ".ppt";
                            break;
                        case "application/pdf":
                            extension = ".pdf";
                            break;
                    }

                    if (extension != "")
                    {
                        var resultImage = CovertFile.Covert(rootPath, file.MD5, extension, stream);
                        if (System.IO.File.Exists(resultImage))
                        {
                            using (var previewStream = new System.IO.FileStream(resultImage, System.IO.FileMode.Open))
                            {
                                await stream.FlushAsync();
                                stream.Position = 0;
                                await previewStream.CopyToAsync(stream);
                                file.PreviewImage = stream.ToArray();
                            }
                        }

                        System.IO.File.Delete(resultImage);
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

            return Ok(new FileReturnInfo(file));
        }

        private bool FileExists(int id)
        {
            return _context.File.Any(e => e.ID == id);
        }
    }
}