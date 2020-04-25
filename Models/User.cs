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

        public static User current
        {
            get
            {
                // For testing, create a fake user
                return new User
                {
                    id = 99,
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
        }
    }
}
