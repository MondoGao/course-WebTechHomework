using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class File
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
        public string MD5 { get; set; }
        public byte[] PreviewImage { get; set; }
        public byte[] FileContent { get; set; }

        public Order Order { get; set; }
    }
}
