using System.Net;

namespace MRS.Status
{
    public class Error
    {
        public static async Task Handle400(HttpListenerRequest req, HttpListenerResponse resp, string message)
        {
            resp.StatusCode = 400;
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>400 Bad Request</h1><p>{message}</p></body></html>");
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data);
        }

        public static async Task Handle500(HttpListenerRequest req, HttpListenerResponse resp, string error)
        {
            resp.StatusCode = 500;
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>500 Internal Server Error</h1><p>{error}</p></body></html>");
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data);
        }

        public static async Task Handle404(HttpListenerRequest req, HttpListenerResponse resp, string error)
        {
            resp.StatusCode = 404;
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"<html><body><h1>404 Not Found</h1><p>{error}</p></body></html>");
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data);
        }
    }
}