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

        public static async Task<T> doInTrasaction<T>(Func<MySqlCommand, Task<T>> action)
        {
            using (MySqlConnection conn = newConnection())
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();

                var transaction = await conn.BeginTransactionAsync();
                cmd.Transaction = transaction;
                cmd.Connection = conn;

                try
                {
                    T result = await action.Invoke(cmd);
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }

        public static async Task doInTrasaction(Func<MySqlCommand, Task> action)
        {
            using (MySqlConnection conn = newConnection())
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();

                var transaction = await conn.BeginTransactionAsync();
                cmd.Transaction = transaction;
                cmd.Connection = conn;

                try
                {
                    await action.Invoke(cmd);
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }
    }
}
