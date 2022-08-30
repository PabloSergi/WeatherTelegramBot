using System;
using MySql.Data.MySqlClient;
using Npgsql;

namespace NpgsqlDb
{
    public class DBCheck
    {

        public static bool CheckCity(string cityName)
        {

            string sql = "SELECT COUNT(*) FROM city WHERE name ILIKE @a";

            Console.WriteLine("Getting Connection ...");
            
            NpgsqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                Console.WriteLine("Connection Opened.");
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@a", cityName);
                    var result = Convert.ToInt32(cmd.ExecuteScalar());
                    if (result > 0)
                    {
                        Console.WriteLine("City Checked. Returning TRUE.");
                        conn.Close();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("City Checked. Returning FALSE.");
                        conn.Close();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Base down");
                conn.Close();
                return false;
            }

            


            


           /* using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@a", cityName);
                var result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result > 0)
                {
                    Console.WriteLine("City Checked. Returning TRUE.");
                    return true;
                }
                else
                {
                    Console.WriteLine("City Checked. Returning FALSE.");
                    return false;
                }
            } */
        }
    }
}

