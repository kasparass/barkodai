﻿using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public struct ItemInformationList
    {
        public IEnumerable<Item> items { get; set; }
        public HashSet<string> hiddenCategories { get; set; }
        public string success_message { get; set; }
        
        public int cart_count { get; set; }
    }
}
