using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Diagnostics;

namespace FileUpload
{
    // 文档预览生成类
    public class CovertFile
    {
        // 生成预览函数
        // rootPath - 网站根目录
        // fileName - 要储存的临时文件名
        // extension - 文件原扩展名，用于外部程序识别
        // stream - 包含上传的文件内容的流
        public static string Covert(string rootPath, string fileName, string extension, MemoryStream stream)
        {
            // 预览图生成目录
            var outDir = Path.Combine(rootPath, "TempFiles");
            // 上传文件临时保存路径
            var tmpFileName = Path.Combine(Path.GetTempPath(), $"{fileName}{extension}");
            // 预览图保存路径
            var resultFileName = Path.Combine(outDir, $"{fileName}.png");

            // 创建临时文件并将上传文件写入其中
            using (var fileStream = new FileStream(tmpFileName, FileMode.Create))
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
            }

            // 创建外部进程，调用 LibreOffice 命令行对文档第一页生成预览
            var p = new Process();
             
            p.StartInfo.FileName = "D:\\Program Files\\LibreOffice 5\\program\\soffice.exe";
            p.StartInfo.Arguments = $"--headless --convert-to png {tmpFileName} --outdir {outDir}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;

            p.Start();

            // 等待转码完成
            p.WaitForExit();

            // 删除临时文件
            File.Delete(tmpFileName);

            // 返回预览图路径
            return resultFileName;
        }
    }
}