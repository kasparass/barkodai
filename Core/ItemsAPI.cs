using Barkodai.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Core
{
    public class ItemsAPI
    {
        public static async Task<Item[]> getItems()
        {
            await Task.Delay(new Random().Next(300, 800));
            return JsonConvert.DeserializeObject<Item[]>(File.ReadAllText("ItemsData.json"));
        }

        public static async Task<Item[]> getItems(IEnumerable<int> ids)
        {
            return (await getItems()).Where(i => ids.Contains(i.id)).ToArray();
        }
    }
}
