using System.Net;

using System.Text.Json;
using System.Text.Json.Nodes;

using MySql.Data.MySqlClient;

using MRS.db;
using MRS.Status;
using MRS.auth;


namespace MRS.Leaderboard
{
    public class Leaderboard
    {

        public static async Task HandleLeaderboard(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string? token = req.Headers.Get("Authorization");
            if (token == null || !TokenValidation.ValidateToken(token))
            {
                await Error.Handle401(req, resp, "Invalid or missing token");
                return;
            }

            var DBManager = new DBManager();
            MySqlDataReader dbReader = DBManager.Query(@"SELECT username, karma From users ORDER BY karma DESC");

            var leaderboard = new List<object>();
            while (dbReader.Read())
            {
                leaderboard.Add(new
                {
                    username = dbReader["username"],
                    karma = dbReader["karma"]
                });
            }
            DBManager.CloseConnection();
            await Success.Handle200(req, resp, JsonSerializer.Serialize(leaderboard));
            return;
        }
    }
}