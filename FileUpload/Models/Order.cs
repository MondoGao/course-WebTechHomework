using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    // 生成的订单资源类
    public class Order
    {
        public int ID { get; set; }
        public DateTime GenerateTime { get; set; }
        // 生成的密码
        public string Password { get; set; }

        // 与文件的一对多关系
        public ICollection<File> Files { get; set; }
    }
}
