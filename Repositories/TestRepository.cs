using System.Collections.Generic;
using Barkodai.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Barkodai.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly string conString;

        public TestRepository(IConfiguration configuration)
        {
            conString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<MyTest> GetTests()
        {
            List<MyTest> result = new List<MyTest>();
            using (MySqlConnection con = new MySqlConnection(conString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM test_table", con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new MyTest
                    {
                        Cs = reader.GetBoolean("cs"),
                        Dabar = reader.GetString("dabar")
                    });
                }
            }
            return result;
        }
    }
}
