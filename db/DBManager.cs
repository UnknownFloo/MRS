using System;
using System.Net;
using MySql.Data.MySqlClient;


namespace MRS.db
{
    public class DBManager
    {
        readonly MySqlConnection myConnection = new MySqlConnection("server=127.0.0.1;port=3306;uid=root;pwd=;database=mrs");
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