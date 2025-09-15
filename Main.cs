using System;
using System.Net;
using System.Net.Sockets;

namespace MRS
{

    class MainClass
    {

    public static HttpListener listener = new HttpListener();
    public static string url = "http://localhost:8080/";

    public static async Task HandleIncomingConnections()
    {
        while (true)
        {
            
            HttpListenerContext ctx = await listener.GetContextAsync();
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            if (req.Url?.AbsolutePath == "/favicon.ico") continue;
        
            Console.WriteLine("Request: {0} {1}", req.HttpMethod, req.Url);

            byte[] data = System.Text.Encoding.UTF8.GetBytes($"You have requested: {req.Url?.AbsolutePath}");
            resp.ContentType = "text/html";


            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            await resp.OutputStream.WriteAsync(data, 0, data.Length);
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
