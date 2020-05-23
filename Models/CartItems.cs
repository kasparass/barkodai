using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class CartItems
    {
        public int id { get; set; }
        public int cart_id { get; set; }
        public int shop_item_id { get; set; }
        public int amount { get; set; }

        public CartItems()
        {

        }

    }
}
