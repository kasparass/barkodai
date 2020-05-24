using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barkodai.Core;
using Barkodai.Models;

namespace Barkodai.Models
{
    public class Cart
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public bool ordered { get; set; }
        public List<ShopItem> items { get; set; }

        public Cart()
        {
            this.id = -1;
        }

        public static async Task<Cart> getCart(int user_id)
        {
            return await DB.doInTrasaction(async (cmd) =>
            {
                Cart cart = new Cart();
                cmd.CommandText = "SELECT * FROM carts WHERE user_id = @user_id AND ordered = FALSE LIMIT 1;";
                cmd.Parameters.AddWithValue("@user_id", user_id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        cart.id = reader.GetInt32(reader.GetOrdinal("id"));
                        cart.user_id = reader.GetInt32(reader.GetOrdinal("user_id"));
                    }
                }

                //---------------------
                // Fetch cart items

                // Fetch all possible shop items
                IEnumerable<ShopItem> shopItems = (await ItemsAPI.getItems()).SelectMany(i => i.shop_items);

                // Fetch shop item ids in the cart
                List<int> ids = new List<int>();
                cmd.CommandText = "SELECT * FROM cart_items WHERE cart_id = @cart_id;";
                cmd.Parameters.AddWithValue("@cart_id", cart.id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt32(reader.GetOrdinal("shop_item_id")));
                    }
                }

                // Transform shop item ids to shop items
                cart.items = ids.Select(i => shopItems.First(si => si.id == i)).ToList();

                return cart;
            });
        }

        public static async Task<bool> isItemInCart(int cart_id, int shop_item_id)
        {
            return await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "SELECT c.id FROM cart_items INNER JOIN carts c on cart_items.cart_id = c.id WHERE shop_item_id = @sid AND c.id = @cid AND c.ordered = FALSE";
                cmd.Parameters.AddWithValue("@cid", cart_id);
                cmd.Parameters.AddWithValue("@sid", shop_item_id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    return reader.Read();
                }
            });
        }

        public static async Task<int> create(int user_id)
        {
            return await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "INSERT INTO carts (user_id) VALUES (@user_id);";
                cmd.Parameters.AddWithValue("@user_id", user_id);
                await cmd.ExecuteNonQueryAsync();
                return (int)cmd.LastInsertedId;
            });
        }

        public static async Task addItemToCart(int cart_id, int shop_item_id)
        {
            await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "INSERT INTO cart_items (cart_id, shop_item_id) VALUES (@cart_id, @shop_item_id);";
                cmd.Parameters.AddWithValue("@cart_id", cart_id);
                cmd.Parameters.AddWithValue("@shop_item_id", shop_item_id);
                await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task deleteFromCart(int user_id, int shop_item_id)
        {
            Cart cart = await getCart(user_id);
            await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "DELETE FROM cart_items WHERE cart_id = @cart_id AND shop_item_id = @shop_item_id;";
                cmd.Parameters.AddWithValue("@cart_id", cart.id);
                cmd.Parameters.AddWithValue("@shop_item_id", shop_item_id);
                await cmd.ExecuteNonQueryAsync();
            });
        }
        
        public static async Task changeToOrdered(int cart_id)
        {
            await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "UPDATE carts SET ordered = TRUE WHERE id = @cart_id;";
                cmd.Parameters.AddWithValue("@cart_id", cart_id);
                await cmd.ExecuteNonQueryAsync();
            });
        }
    }
}
