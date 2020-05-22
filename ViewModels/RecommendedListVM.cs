using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public struct RecommendedListVM
    {
        public RecommendedList recommendedList { get; set; }
        public BlockList blockList { get; set; }
        public IEnumerable<Item> items { get; set; }
        public List<Rating> ratings { get; set; }
    }
}