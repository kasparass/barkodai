using Barkodai.Core;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class BlockList
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public IList<Item> items { get; set; }
        public IList<string> categories { get; set; }
        public User user { get; set; }

        public BlockList()
        {
            items = new List<Item>();
            categories = new List<string>();
        }

        public BlockList(User owner) : this()
        {
            user = owner;
            user_id = owner.id;
        }

        public async static Task<BlockList> getList(User user)
        {
            return await DB.doInTrasaction(async cmd =>
            {
                // Initialize list as empty list
                BlockList list = new BlockList(user)
                {
                    id = -1,
                };

                // Select list
                cmd.CommandText = "SELECT id FROM blocked_lists WHERE user_id = @id;";
                cmd.Parameters.AddWithValue("@id", user.id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        list.id = reader.GetInt32(reader.GetOrdinal("id"));
                    }
                }

                // If a list was found, try to search for it's contents
                if (list.id != -1)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT item_id FROM blocked_list_items WHERE blocked_list_id = @id;";
                    cmd.Parameters.AddWithValue("@id", list.id);

                    List<int> itemIds = new List<int>();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                            itemIds.Add(reader.GetInt32(reader.GetOrdinal("item_id")));
                    }

                    list.items = (await ItemsAPI.getItems(itemIds)).ToList();
                }

                return list;

            });
        }

        public static async Task addItem(User user, int itemId)
        {
            await DB.doInTrasaction(async cmd =>
            {
                // Check if user has a list
                int listId = -1;
                cmd.CommandText = "SELECT id FROM blocked_lists WHERE user_id = @id;";
                cmd.Parameters.AddWithValue("@id", user.id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        listId = reader.GetInt32(reader.GetOrdinal("id"));
                    }
                }

                // If user has no list, create it
                if (listId == -1)
                {
                    cmd.CommandText = "INSERT INTO blocked_lists(user_id) VALUES(@id);";
                    await cmd.ExecuteNonQueryAsync();
                    listId = (int)cmd.LastInsertedId;
                }

                // Insert item into list
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO blocked_list_items(blocked_list_id, item_id) VALUES(@listID, @itemID);";
                cmd.Parameters.AddWithValue("itemID", itemId);
                cmd.Parameters.AddWithValue("listID", listId);
                await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task deleteItem(User user, int itemId)
        {
            await DB.doInTrasaction(async cmd =>
            {
                // Check if user has a list
                int listId = -1;
                cmd.CommandText = "SELECT id FROM blocked_lists WHERE user_id = @id;";
                cmd.Parameters.AddWithValue("@id", user.id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        listId = reader.GetInt32(reader.GetOrdinal("id"));
                    }
                }

                // If user has a list, remove item from it
                if (listId != -1)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM blocked_list_items WHERE blocked_list_id = @listID AND item_id = @itemID;";
                    cmd.Parameters.AddWithValue("itemID", itemId);
                    cmd.Parameters.AddWithValue("listID", listId);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }
    }
}
