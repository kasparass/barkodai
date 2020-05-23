using Newtonsoft.Json;
using System;

namespace Barkodai.Models
{
    public class ShopItem
    {
        public int id { get; set; }
        public double price { get; set; }
        public int shop_id { get; set; }

        [JsonIgnore]
        public Item item { get; set; }
        [JsonIgnore]
        public Shop shop { get; set; }
    }
}