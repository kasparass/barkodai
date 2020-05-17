using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class Item
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image_address { get; set; }
        public string category { get; set; }
        public Shop[] shops { get; set; }
    }
}
