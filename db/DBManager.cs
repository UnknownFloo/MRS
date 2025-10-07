using System;
using System.Net;
using MySql.Data.MySqlClient;
using Mysqlx.Connection;


namespace MRS.db
{
    public class DBManager
    {
        readonly MySqlConnection myConnection = new MySqlConnection("server=127.0.0.1;port=3306;uid=mrs_user;pwd=mrs_password;database=mrs");

        public MySqlDataReader Query(string sql, params string[] parameter)
        {
            Console.WriteLine($"Executing SQL: {sql} with parameters: {string.Join(", ", parameter)}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = GetConnection();
            cmd.CommandText = sql;

            for (int i = 0; i < parameter.Length; i += 2)
            {
                cmd.Parameters.AddWithValue(parameter[i], parameter[i + 1]);
            }

            return cmd.ExecuteReader();
        }

        public MySqlConnection GetConnection()
        {
            try
            {
                myConnection.Open();
                Console.WriteLine("Database connection established.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
            }
            return myConnection;
        }

        public void CloseConnection()
        {
            if (myConnection.State == System.Data.ConnectionState.Open)
            {
                myConnection.Close();
                Console.WriteLine("Database connection closed.");
            }
        }
    }
}