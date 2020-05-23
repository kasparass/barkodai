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
        public double min_price { get; set; }
        public string barcode { get; set; }
        public ShopItem[] shop_items { get; set; }
        public Rating averageRating { get; set; }
    }
}
