using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public struct ScanVM
    {
        public string error_message { get; set; }
        public Item item { get; set; }
    }
}
