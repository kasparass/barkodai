using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class Rating
    {
        public int id { get; set; }
        [BindProperty]
        public int item_id { get; set; }
        [BindProperty]
        public int user_id { get; set; }
        [BindProperty]
        public float price { get; set; }
        [BindProperty]
        public float quality { get; set; }
        [BindProperty]
        public float use { get; set; }

        public int ratingCount { get; set; }

        public Rating()
        {
            ratingCount = 1;
        }

        public static async Task<List<Rating>> getRatings(int item_id)
        {
            return await DB.doAction(async (cmd) =>
            {
                List<Rating> ratings = new List<Rating>();

                cmd.CommandText = "SELECT * FROM ratings WHERE item_id = @item_id;";
                cmd.Parameters.AddWithValue("@item_id", item_id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        ratings.Add(new Rating
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            item_id = reader.GetInt32(reader.GetOrdinal("item_id")),
                            user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                            use = reader.GetFloat(reader.GetOrdinal("use")),
                            quality = reader.GetFloat(reader.GetOrdinal("quality")),
                            price = reader.GetFloat(reader.GetOrdinal("price"))
                        });
                    }
                }

                return ratings;
            });
        }

        public static async Task<Rating> getRating(int user_id, int item_id)
        {
            return await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "SELECT * FROM ratings WHERE item_id = @item_id AND user_id = @user_id;";
                cmd.Parameters.AddWithValue("@item_id", item_id);
                cmd.Parameters.AddWithValue("@user_id", user_id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new Rating
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            item_id = reader.GetInt32(reader.GetOrdinal("item_id")),
                            user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                            use = reader.GetFloat(reader.GetOrdinal("use")),
                            quality = reader.GetFloat(reader.GetOrdinal("quality")),
                            price = reader.GetFloat(reader.GetOrdinal("price"))
                        };
                    }
                    return null;
                }
            });
        }

        public static async Task create(Rating rating)
        {
            await DB.doInTrasaction(async cmd =>
            {
                cmd.Parameters.AddWithValue("@price", rating.price);
                cmd.Parameters.AddWithValue("@quality", rating.quality);
                cmd.Parameters.AddWithValue("@use", rating.use);
                cmd.Parameters.AddWithValue("@user_id", rating.user_id);
                cmd.Parameters.AddWithValue("@item_id", rating.item_id);

                // Delete any previous ratings
                cmd.CommandText = "DELETE FROM ratings WHERE item_id = @item_id AND user_id = @user_Id;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "INSERT INTO ratings(price, quality, `use`, user_id, item_id) " +
                "VALUES(@price, @quality, @use, @user_id, @item_id);";

                await cmd.ExecuteNonQueryAsync();
            });
        }

        public static async Task<Rating> getAverageRating(int item_id)
        {
            List<Rating> ratings = await getRatings(item_id);
            Rating avg = new Rating { ratingCount = ratings.Count, item_id = -1, user_id = -1 };
            if (ratings.Count > 0)
            {
                avg.use = ratings.Average(r => r.use);
                avg.quality = ratings.Average(r => r.quality);
                avg.price = ratings.Average(r => r.price);
            }
            return avg;
        }
    }
}
