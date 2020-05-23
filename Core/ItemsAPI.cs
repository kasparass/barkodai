using Barkodai.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

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
            Shop[] shops = JsonConvert.DeserializeObject<Shop[]>(File.ReadAllText("ShopData.json"));

            for (int i = 0; i < items.Length; i++)
            {
                items[i].id = i;
            }

            for(int i = 0; i < shops.Length; i++)
            {
                shops[i].id = i+1;
            }

            foreach(Item item in items)
            {
                if(item.shop_items != null)
                {
                    foreach (ShopItem shopItem in item.shop_items)
                    {
                        shopItem.item = item;
                        shopItem.shop = shops.First(s => s.id == shopItem.shop_id);
                    }
                }
            }

            foreach(Item item in items)
            {
                item.min_price = item.shop_items == null || item.shop_items.Length == 0 ? 0 : item.shop_items.Min(s => s.price);
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

        public static async Task<(Item, string)> getItemFromPhoto(Stream imageStream)
        {
            await Task.Delay(50);
            if (new Random().NextDouble() < 0.5)
            {
                return (null, "Unable to find item from the image.");
            }

            Item[] items = await getItems();
            return (items[new Random().Next(0, items.Length)], "Success!");
        }
    }
}
