using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public class BlockListVM
    {
        public BlockList blockList { get; set; }
        public IEnumerable<Item> items { get; set; }
        public HashSet<string> hiddenCategories { get; set; }
    }
}
