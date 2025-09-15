using System.Net;

namespace MRS.auth
{
    public class AuthService
    {
        public delegate Task RouteHandler(HttpListenerRequest req, HttpListenerResponse resp);
        private static Dictionary<(string method, string path), RouteHandler> routes = new()
            {
                { ("POST", "/login"),  Login.handleLogin },
                { ("POST", "/register"),  Register.handleRegister }
            };

        public static async Task handleAuthService(HttpListenerRequest req, HttpListenerResponse resp)
        {
            Console.WriteLine($"AuthService received request: {req.HttpMethod} | {req.Url?.AbsolutePath}");
            string[] subpages = req.Url?.AbsolutePath.ToString().Split("/") ?? [];

            //Console.WriteLine($"Subpages: {string.Join(", ", subpages)}"); // Debugging Information
            if (subpages == null)
            {
                await MainClass.Handle404(req, resp, "No subpages found"); // Change to 500 Internal Server Error
            }
            var route = (req.HttpMethod, "/" + subpages?[^1] ?? "");
            // Console.WriteLine($"Routing to: {route}"); // Debugging Information
            if (routes.TryGetValue(route, out var handler))
            {
                await handler(req, resp);
            }
            else
            {
                await MainClass.Handle404(req, resp, "Route not found");
            }

            byte[] data = System.Text.Encoding.UTF8.GetBytes("AuthService is running");
            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            await resp.OutputStream.WriteAsync(data);

        }

    }

}