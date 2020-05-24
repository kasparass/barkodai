using System.Threading.Tasks;

namespace Barkodai.Models
{
    public class Order
    {
        public static async Task<int> create(int user_id, int cart_id)
        {
            return await DB.doAction(async (cmd) =>
            {
                cmd.CommandText = "INSERT INTO orders (customer_id, address, cart_id) VALUES (@user_id, @address, @cart_id);";
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@cart_id", cart_id);
                cmd.Parameters.AddWithValue("@address", "");
                await cmd.ExecuteNonQueryAsync();
                return (int)cmd.LastInsertedId;
            });
        }
    }
}