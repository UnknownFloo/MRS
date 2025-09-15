using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MRS
{

    class MainClass
    {

    public static HttpListener listener = new HttpListener();

    public static string url = "http://localhost:8080/";

    public delegate Task RouteHandler(HttpListenerRequest req, HttpListenerResponse resp);

        private static Dictionary<(string method, string path), RouteHandler> routes = new()
        {
            { ("POST", "/auth/login"), HandleLogin },
            { ("GET", "/"), HandleHome }
        };

        public static void Handle404(HttpListenerRequest req, HttpListenerResponse resp, string error)
        {
            resp.StatusCode = 404;
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>404 Not Found</h1><p>{error}</p></body></html>");
            resp.ContentLength64 = data.LongLength;
            resp.OutputStream.Write(data, 0, data.Length);
            resp.Close();
        }
        
        public static async Task HandleHome(HttpListenerRequest req, HttpListenerResponse resp)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes("Welcome to the home page!");
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
        }

        public static async Task HandleLogin(HttpListenerRequest req, HttpListenerResponse resp)
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
                        Handle404(req, resp, "Missing username or password");
                    }

                    bool authenticated = Login.Authenticate(user["username"]?.ToString() ?? "", user["password"]?.ToString() ?? "");
                    Console.WriteLine($"Username: {user?["username"]}, Password: {user?["password"]}, Authenticated: {authenticated}");

                    if (authenticated)
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>Login Successful</h1><p>Welcome, {user?["username"]}!</p></body></html>");
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = System.Text.Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    }
                    else
                    {
                        Handle404(req, resp, "Invalid username or password");
                    }

                }
                catch (JsonException ex)
                {
                    Handle404(req, resp, "Invalid JSON");
                    Console.WriteLine("Error parsing JSON: " + ex.Message);
                }
            }
        }

        public static async Task HandleIncomingConnections()
        {

            while (true)
            {

                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if (req.Url?.AbsolutePath == "/favicon.ico") continue;

                byte[] data = System.Text.Encoding.UTF8.GetBytes($"You have requested: {req.Url?.AbsolutePath}");
                resp.ContentType = "text/html";

                Console.WriteLine("Request: {0} {1}", req.HttpMethod, req.Url);

                var route = (req.HttpMethod, req.Url?.AbsolutePath ?? "");
                if(routes.TryGetValue(route, out var handler))
                {
                    await handler(req, resp);
                }
                else
                {
                    Handle404(req, resp, "Route not found");
                }

                resp.Close();
            }
        }

        public static void Main(string[] args)
        {
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }    
}
