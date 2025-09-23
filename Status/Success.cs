using System.Net;

namespace MRS.Status
{
    public class Success
    {
        public static async Task Handle200(HttpListenerRequest req, HttpListenerResponse resp, string message)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            resp.StatusCode = 200;
            resp.ContentType = "application/json";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data);
        }
    }
}