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
            await Task.Delay(new Random().Next(50, 400));
            Item[] items = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText("ItemsData.json"));

            for(int i = 0; i < items.Length; i++)
            {
                items[i].id = i;
            }

            return items;
        }

        public static async Task<Item[]> getItems(IEnumerable<int> ids)
        {
            return (await getItems()).Where(i => ids.Contains(i.id)).ToArray();
        }
    }
}
