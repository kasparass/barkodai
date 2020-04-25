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

        public async static Task<BlockList> getList(User user)
        {
            // add some items for testing 
            var items = await ItemsAPI.getItems();
            return new BlockList { items = items.Take(2).ToList() };

            using (MySqlConnection conn = DB.newConnection())
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();

                MySqlTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.Connection = conn;

                try
                {
                    // Initialize list as empty list
                    BlockList list = new BlockList
                    {
                        id = -1,
                        user = user,
                        user_id = user.id,
                        categories = new List<string>(),
                        items = new List<Item>()
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
                            {
                                itemIds.Add(reader.GetInt32(reader.GetOrdinal("item_id")));
                            }
                        }

                        list.items = (await ItemsAPI.getItems(itemIds)).ToList();
                    }

                    transaction.Commit();

                    return list;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
    }
}
