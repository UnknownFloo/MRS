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
            var connection = dbManager.GetConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = @"SELECT * From users WHERE username = @username AND password = @password";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return true;
                    // Console.WriteLine($"User: {reader["username"]}, pass: {reader["password"]}, id: {reader["id"]}");
                }
            }

            return false;
        }

        public static async Task handleLogin(HttpListenerRequest req, HttpListenerResponse resp)
        {
            using (var body = req.InputStream) // here we have data
            using (var reader = new StreamReader(body, req.ContentEncoding))
            {
                try
                {
                    string json = reader.ReadToEnd();
                    JsonObject? user = System.Text.Json.JsonSerializer.Deserialize<JsonObject>(json);
                    Console.WriteLine(user);

                    if (user == null || !user.ContainsKey("username") || !user.ContainsKey("password"))
                    {
                        await Error.Handle400(req, resp, "Missing username or password");
                        return;
                    }

                    bool authenticated = Login.Authenticate(user?["username"]?.ToString() ?? "", user?["password"]?.ToString() ?? "");
                    Console.WriteLine($"Username: {user?["username"]}, Password: {user?["password"]}, Authenticated: {authenticated}");

                    if (authenticated)
                    {
                        string generatedToken = $"{user?["username"]}_mrsToken";

                        dynamic responseData = new { message = "Login successful", token = generatedToken};

                        byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseData));
                        resp.StatusCode = 200;
                        resp.ContentType = "application/json";
                        resp.ContentEncoding = System.Text.Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                        await resp.OutputStream.WriteAsync(data);
                    }
                    else
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>Login Failed</h1><p>Invalid username or password!</p></body></html>");
                        resp.StatusCode = 200;
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = System.Text.Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                        await resp.OutputStream.WriteAsync(data);
                    }

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