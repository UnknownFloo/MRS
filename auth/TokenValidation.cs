using System.Net;


using MySql.Data.MySqlClient;

using MRS.db;

namespace MRS.auth
{
    public class TokenValidation
    {
        public static bool ValidateToken(string token)
        {
            string? TokenValue = token.Split(" ")?[1];
            string? user = TokenValue?.Split("_")?[0];
            string? tokenSuffix = TokenValue?.Split("_")?[1];

            var DBManager = new DBManager();
            MySqlDataReader dbReader = DBManager.Query(@"SELECT * From users WHERE username = @username", ["@username", user ?? ""]);

            if (dbReader == null) return false;

            if (dbReader.Read() && tokenSuffix == "mrsToken")
            {
                DBManager.CloseConnection();
                return true;
            }

            DBManager.CloseConnection();
            return false;
        }
    }
}