using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    // 上传的文件资源类
    public class File
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
        public string MD5 { get; set; }
        // 预览图片，若不是可生成预览的文档则为空
        public byte[] PreviewImage { get; set; }
        // 文件内容主体
        public byte[] FileContent { get; set; }

        // 每个文件属于一个订单
        public Order Order { get; set; }
    }
}
