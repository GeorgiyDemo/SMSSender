using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace SMSTimetable
{
    public class DatabaseLogicClass
    {
        public static string connStr = "ДЕМ МОЖЕТ УЖЕ НАЙДЕТ СЕБЕ ТЯН?";

        public static string GetSQL(string sql)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            MySqlCommand command = new MySqlCommand(sql, conn);
            string name = command.ExecuteScalar().ToString();
            conn.Close();
            return name;
        }

        public static async Task<string> GetSQLAsync(string SQL)
        {
            string return_str = "";
            using (var conn = new MySqlConnection(connStr))
            {
                await conn.OpenAsync();
                /*
                // Insert some data
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                    cmd.Parameters.AddWithValue("p", "Hello world");
                    await cmd.ExecuteNonQueryAsync();
                }
                */

                using (var cmd = new MySqlCommand(SQL, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                        return_str += (reader.GetString(0));
            }
            return return_str;
        }

      
    }
}
