using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

using MRS.Status;

namespace MRS.auth
{
    public class Login
    {
        private static readonly dynamic[] users = [
            new { id = 1 , username = "admin", password = "password" },
            new { id = 2 , username = "user", password = "1234" }
        ];

        private static int GenerateToken(string userName)
        {
            dynamic? user = Array.Find(users, u => u.username == userName);
            return ((user?.id + user?.username?.ToString().Length) * 420) ?? 0;
        }

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
                        int generatedToken = GenerateToken(user?["username"]?.ToString() ?? "");

                        if(generatedToken == 0)
                        {
                            await Error.Handle500(req, resp, "Token generation failed");
                            return;
                        }

                        dynamic responseData = new { message = "Login successful", token = generatedToken.ToString() + "_mrsToken"};

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