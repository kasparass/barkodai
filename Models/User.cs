using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class User
    {
        public int id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public bool is_admin { get; set; }
        public bool is_blocked { get; set; }
        public bool is_worker { get; set; }

        private static User testUser;

        public static User current
        {
            get
            {
                if (testUser != null) return testUser;

                return new User
                {
                    id = 1,
                    email = "helloItsMe@gmail.com",
                    password = "this is totally hashed btw",
                    first_name = "Andrius",
                    last_name = "Urbelis",
                    phone = "+37066942042",
                    is_admin = false,
                    is_blocked = false,
                    is_worker = false
                };
            }
            set
            {
                testUser = value;
            }
        }

        public static async Task<User> changeTestUser(int id)
        {
            User.current = await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "SELECT * FROM users WHERE id = @id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if(reader.Read())
                    {
                        return new User
                        {
                            id = id,
                            email = reader.GetString(reader.GetOrdinal("email")),
                            password = reader.GetString(reader.GetOrdinal("password")),
                            first_name = reader.GetString(reader.GetOrdinal("first_name")),
                            last_name = reader.GetString(reader.GetOrdinal("last_name")),
                            phone = reader.GetString(reader.GetOrdinal("phone")),
                            is_admin = reader.GetBoolean(reader.GetOrdinal("is_admin")),
                            is_blocked = reader.GetBoolean(reader.GetOrdinal("is_worker")),
                            is_worker = reader.GetBoolean(reader.GetOrdinal("is_blocked"))
                        };
                    }
                    throw new Exception("Unable to change test user - user not found. Requested ID: " + id);
                    
                }
            });

            return User.current;
        }
        
        public static async Task<int> getCartCount()
        {
            return await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "SELECT COUNT(*) AS count FROM carts INNER JOIN cart_items ci on carts.id = ci.cart_id WHERE carts.user_id = @id;";
                cmd.Parameters.AddWithValue("@id", current.id);

                int count = 0;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(reader.GetOrdinal("count"));
                    }
                }
                return count;
            });
        }
    }
}
