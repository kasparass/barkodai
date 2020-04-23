using Barkodai.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Barkodai.Core
{
    public class ItemsAPIJSONMock : IItemsAPI
    {
        public async Task<Item[]> getItems()
        {
            await Task.Delay(new Random().Next(300, 800));
            return JsonConvert.DeserializeObject<Item[]>(File.ReadAllText("ItemsData.json"));
        }
    }
}
