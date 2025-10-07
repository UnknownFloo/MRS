using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

using MRS.auth;
using MRS.db;
using MRS.Status;
using MySql.Data.MySqlClient;

namespace MRS
{

    class MainClass
    {

        public static HttpListener listener = new HttpListener();

        public static string url = "http://localhost:8080/";

        public delegate Task RouteHandler(HttpListenerRequest req, HttpListenerResponse resp);

        private static readonly Dictionary<(string method, string path), RouteHandler> routes = new()
        {
            { ("GET", "/"), HandleHome },

            { ("POST", "/api/auth/login"), Login.HandleLogin },
            { ("POST", "/api/auth/register"), Register.handleRegister },

            { ("GET", "/api/leaderboard"), Leaderboard.Leaderboard.HandleLeaderboard },
        };

        public static async Task HandleHome(HttpListenerRequest req, HttpListenerResponse resp)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes("Welcome to the home page!");
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data);
        }

        public static async Task HandleIncomingConnections()
        {

            while (true)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if (req.Url?.AbsolutePath == "/favicon.ico") continue;

                Console.WriteLine("Request: {0} | {1}", req.HttpMethod, req.Url);

                var route = (req.HttpMethod, req.Url?.AbsolutePath ?? "");

                if (routes.TryGetValue(route, out var handler))
                {
                    await handler(req, resp);
                }
                else
                {
                    await Error.Handle404(req, resp, "Route not found");
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
