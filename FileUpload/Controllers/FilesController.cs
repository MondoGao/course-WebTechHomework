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

        // 对文件实体进行包装的类，过滤不需要返回的属性
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
            // 过滤不需要返回的属性
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

            // 根据现有信息创建文件实体
            var file = new File { FileName = uploadFile.FileName, Type = uploadFile.ContentType, UploadDate = nowStamp };

            if (uploadFile.Length > 0)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    // 读取上传文件计算 MD5
                    await uploadFile.CopyToAsync(stream);
                    stream.Position = 0;
                    var md5Arr = MD5.Create().ComputeHash(stream);
                  
                    StringBuilder sBuilder = new StringBuilder();
                    
                    for (int i = 0; i < md5Arr.Length; i++)
                    {
                        // x2 代表转为 16 进制长度为 2 的 string
                        sBuilder.Append(md5Arr[i].ToString("x2"));
                    }
                    
                    file.MD5 = sBuilder.ToString();

                    // 将文件内容转为字节数组并写入实体
                    file.FileContent = stream.ToArray();

                    var extension = "";

                    // 处理特定的文档预览
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
                        // 进行预览图片的生成并写入到实体中
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

                            // 删除临时生成的图片
                            System.IO.File.Delete(resultImage);
                        }
                    }
                }

                // 将生成的实体添加到 Model 中并与数据库同步
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

            // 查找对应 id 的文件
            var file = await _context.File.SingleOrDefaultAsync(m => m.ID == id);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(new FileReturnInfo(file));
        }

        // 查找对应 id 的文件并返回文件的内容
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

        // 查找对应 id 的文件并返回文件的预览图片
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

        // 删除对应 id 的文件
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