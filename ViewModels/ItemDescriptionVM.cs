using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public struct ItemDescriptionVM
    {
        public Item item { get; set; }
        public bool showShops { get; set; }
        public Rating userRating { get; set; }
        public int userId { get; set; }
    }
}
