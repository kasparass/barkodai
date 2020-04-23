using Barkodai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Core
{
    public interface IItemsAPI
    {
        Task<Item[]> getItems();
    }
}
