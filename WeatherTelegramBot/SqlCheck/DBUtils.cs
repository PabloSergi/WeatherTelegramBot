using MySql.Data.MySqlClient;
using Npgsql;

namespace NpgsqlDb
{
    class DBUtils
    {
        public static NpgsqlConnection GetDBConnection()
        {

            /*/ string host = "host.docker.internal";
             int port = 5432;
             string database = "postgres";
             string username = "postgres";
             string password = "051333"; /*/

            string host = "127.0.0.1";
            int port = 5432;
            string database = "WeatherCities";
            string username = "postgres";
            string password = "admin"; 

            return SqlUtils.GetDBConnection(host, port, database, username, password);
        }
      /*  public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "WeatherDB";
            string username = "root";
            string password = "Aa051333";

            return MySqlUtils.GetDBConnection(host, port, database, username, password);
        } */
    }
}