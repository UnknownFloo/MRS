using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

using MRS.Status;

namespace MRS.auth
{
    public class Login
    {
        private static readonly dynamic[] users = [
            new { username = "admin", password = "password" },
            new { username = "user", password = "1234" }
        ];

        public static bool Authenticate(string username, string password)
        {
            dynamic? user = Array.Find(users, u => u.username == username && u.password == password);

            return user != null;
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

                    if (user == null || !user.ContainsKey("username") || !user.ContainsKey("password"))
                    {
                        await Error.Handle400(req, resp, "Missing username or password");
                        return;
                    }

                    bool authenticated = Login.Authenticate(user?["username"]?.ToString() ?? "", user?["password"]?.ToString() ?? "");
                    Console.WriteLine($"Username: {user?["username"]}, Password: {user?["password"]}, Authenticated: {authenticated}");

                    if (authenticated)
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>Login Successful</h1><p>Welcome, {user?["username"]}!</p></body></html>");
                        resp.StatusCode = 200;
                        resp.ContentType = "text/html";
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