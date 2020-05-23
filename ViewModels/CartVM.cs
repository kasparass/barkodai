using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.ViewModels
{
    public struct CartVM
    {
        public Cart cart { get; set; }
        public string message { get; set; }

        // error, success etc.
        public string message_type { get; set; }
    }
}
