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
        private const int MIN_DELAY = 0; // private const int MIN_DELAY = 50;
        private const int MAX_DELAY = 0; // private const int MAX_DELAY = 400;
        public static async Task<Item[]> getItems()
        {
            await Task.Delay(new Random().Next(MIN_DELAY, MAX_DELAY));
            Item[] items = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText("ItemsData.json"));

            for (int i = 0; i < items.Length; i++)
            {
                items[i].id = i;
                items[i].min_price = items[i].shop_items == null || items[i].shop_items.Length == 0 ? 0 : items[i].shop_items.Min(s => s.price);
            }

            return items;
        }

        public static async Task<Item> getItem(int id)
        {
            return (await getItems()).First(i => i.id == id);
        }

        public static async Task<Item[]> getItems(IEnumerable<int> ids)
        {
            return (await getItems()).Where(i => ids.Contains(i.id)).ToArray();
        }

        public static async Task<ShopItem[]> getShops(int item_id)
        {
            return (await getItem(item_id))?.shop_items;
        }
    }
}
