using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai
{
    public class DB
    {
        public static string connectionString { get; set; }

        public static MySqlConnection newConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
