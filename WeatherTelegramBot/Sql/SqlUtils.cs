using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Npgsql;

namespace NpgsqlDb
{
    class SqlUtils
    {
        public static NpgsqlConnection GetDBConnection(string host, int port, string database, string username, string password)
        {
            string connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password;

            NpgsqlConnection conn = new NpgsqlConnection(connString);

            return conn;
        }

      /*  public static MySqlConnection GetDBConnection(string host, int port, string database, string username, string password)
        {
            string connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password;

             MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        } */
    }
}