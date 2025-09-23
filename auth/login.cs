using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

using MySql.Data.MySqlClient;

using MRS.db;
using MRS.Status;

namespace MRS.auth
{
    public class Login
    {
        public static bool Authenticate(string username, string password)
        {
            Console.WriteLine($"Authenticating user: {username} with password: {password}");

            DBManager dbManager = new DBManager();
            MySqlDataReader authReader = dbManager.Query(@"SELECT * From users WHERE username = @username AND password = @password", ["@username", username, "@password", password]);

            bool authenticated = authReader.Read();
            dbManager.CloseConnection();
            return authenticated;
        }

        public static async Task HandleLogin(HttpListenerRequest req, HttpListenerResponse resp)
        {
            using (var body = req.InputStream)
            using (var reader = new StreamReader(body, req.ContentEncoding))
            {
                try
                {
                    string json = reader.ReadToEnd();
                    JsonObject? user = JsonSerializer.Deserialize<JsonObject>(json);
                    Console.WriteLine(user);

                    if (user == null || !user.ContainsKey("username") || !user.ContainsKey("password"))
                    {
                        await Error.Handle400(req, resp, "Missing username or password");
                        return;
                    }

                    bool authenticated = Authenticate(user?["username"]?.ToString() ?? "", user?["password"]?.ToString() ?? "");
                    //Console.WriteLine($"Username: {user?["username"]}, Password: {user?["password"]}, Authenticated: {authenticated}");

                    dynamic responseData = new
                    {
                        message = authenticated ? "Login successful" : "Login failed",
                        token = authenticated ? $"{user?["username"]}_mrsToken" : null
                    };

                    await Success.Handle200(req, resp, JsonSerializer.Serialize(responseData));
                    return;
                }
                catch (JsonException ex)
                {
                    await Error.Handle400(req, resp, "Invalid JSON");
                    Console.WriteLine("Error parsing JSON: " + ex.Message);
                }
            }
        }
    }
}