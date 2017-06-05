using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime GenerateTime { get; set; }
        public string Password { get; set; }

        public ICollection<File> Files { get; set; }
    }
}
