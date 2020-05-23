using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public class CartItemsListVM
    {
        public IEnumerable<CartItems> items { get; set; }
        public string success_message { get; set; }

    }
}
