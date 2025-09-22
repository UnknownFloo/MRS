using System.Net;

using System.Text.Json;
using System.Text.Json.Nodes;

using MySql.Data.MySqlClient;

using MRS.db;
using MRS.Status;

namespace MRS.auth
{
    public class Register
    {
        public static async Task handleRegister(HttpListenerRequest req, HttpListenerResponse resp)
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