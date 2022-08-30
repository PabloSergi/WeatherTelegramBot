using System;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using Telegram.Bot.Types;
using WeatherTelegramBot.BotHandlers;
using User = WeatherTelegramBot.BotHandlers.User;
using Newtonsoft.Json;
using System.Linq;

namespace NpgsqlDb
{
    public class UsersDB
    {
        public static void SaveUserData(List<User> userBase)
        {

            string sql = "TRUNCATE usersbase; insert into usersbase(userid, chatid, chatopened, lang, messagearray) values(:userid, :chatid, :chatopened, :lang, :messagearray)";

            

            for (int i = 0; i < userBase.Count; i++)
            {

                Console.WriteLine($"Add User N_{i}.");

                NpgsqlConnection conn = UsersUtilsDB.GetDBConnection();
                conn.Open();

                Console.WriteLine("Connection to base Opened.");


                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("userid",
                      NpgsqlDbType.Bigint));
                    cmd.Parameters.Add(new NpgsqlParameter("chatid",
                      NpgsqlDbType.Bigint));
                    cmd.Parameters.Add(new NpgsqlParameter("chatopened",
                      NpgsqlDbType.Boolean));
                    cmd.Parameters.Add(new NpgsqlParameter("lang",
                      NpgsqlDbType.Text));
                    cmd.Parameters.Add(new NpgsqlParameter("messagearray",
                       NpgsqlDbType.Array | NpgsqlDbType.Integer));

                    cmd.Parameters[0].Value = userBase[i].UserId;
                    cmd.Parameters[1].Value = userBase[i].ChatId;
                    cmd.Parameters[2].Value = userBase[i].ChatOpened;
                    cmd.Parameters[3].Value = userBase[i].Lang;
                    cmd.Parameters[4].Value = userBase[i].messageStorage;

                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("User added.");
            
                conn.Close();

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


        public static List<User> UploadUsersData()
        {
            List<User> userBase = new List<User>();

            NpgsqlConnection conn = UsersUtilsDB.GetDBConnection();
            {
                conn.Open();

                string sql = "select * from usersbase";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User tempUser = new User((long)reader[0]);
                            
                            tempUser.ChatId = (long)reader[1];
                            tempUser.ChatOpened = (bool)reader[2];
                            tempUser.Lang = (string)reader[3];
                            tempUser.messageStorage = ((int[])reader[4]).ToList();
                            userBase.Add(tempUser);
                        }
                    }
                }
                conn.Close();

                return userBase;
            }
        }
    }
}

