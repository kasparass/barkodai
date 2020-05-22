using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barkodai.Core;

namespace Barkodai.Models 
{
    public class RecommendedList
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public IList<Item> items { get; set; }
        public User user { get; set; }

        public RecommendedList()
        {
            items = new List<Item>();
        }

        public RecommendedList(User owner) : this()
        {
            user = owner;
            user_id = owner.id;
        }

        public async static Task<RecommendedList> getList(User user)
        {
            return await DB.doInTrasaction(async cmd =>
            {
                // Initialize list as empty list
                RecommendedList list = new RecommendedList(user)
                {
                    id = -1,
                };

                // Select list
                cmd.CommandText = "SELECT id FROM recommended_lists WHERE user_id = @id;";
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
                    cmd.CommandText = "SELECT item_id FROM recommended_list_items WHERE recommended_list_id = @id;";
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
                cmd.CommandText = "SELECT id FROM recommended_lists WHERE user_id = @id;";
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
                    cmd.CommandText = "INSERT INTO recommended_lists(user_id) VALUES(@id);";
                    await cmd.ExecuteNonQueryAsync();
                    listId = (int)cmd.LastInsertedId;
                }

                // Insert items into list
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO recommended_list_items(recommended_list_id, item_id) VALUES(@listID, @itemID);";
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
                cmd.CommandText = "SELECT id FROM recommended_lists WHERE user_id = @id;";
                cmd.Parameters.AddWithValue("@id", user.id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        listId = reader.GetInt32(reader.GetOrdinal("id"));
                    }
                }

                // If user has a list, remove items from it
                if (listId != -1)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM recommended_list_items WHERE recommended_list_id = @listID AND item_id = @itemID;";
                    cmd.Parameters.AddWithValue("listID", listId);
                    cmd.Parameters.AddWithValue("itemID", itemId);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

    }
}