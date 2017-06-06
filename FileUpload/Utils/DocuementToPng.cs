using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Diagnostics;

namespace FileUpload
{
    public class CovertFile
    {
        public static string Covert(string rootPath, string fileName, string extension, MemoryStream stream)
        {
            var outDir = Path.Combine(rootPath, "TempFiles");
            var tmpFileName = Path.Combine(Path.GetTempPath(), $"{fileName}{extension}");
            var resultFileName = Path.Combine(outDir, $"{fileName}.png");

            using (var fileStream = new FileStream(tmpFileName, FileMode.Create))
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
            }

            var p = new Process();
             
            p.StartInfo.FileName = "D:\\Program Files\\LibreOffice 5\\program\\soffice.exe";
            p.StartInfo.Arguments = $"--headless --convert-to png {tmpFileName} --outdir {outDir}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;

            p.Start();

            p.WaitForExit();

            File.Delete(tmpFileName);

            return resultFileName;
        }
    }
}