using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class Rating
    {
        public int id { get; set; }
        public int item_id { get; set; }
        public int user_id { get; set; }
        public float price { get; set; }
        public float quality { get; set; }
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
