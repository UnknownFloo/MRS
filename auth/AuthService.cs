using System.Net;

using MRS.Status;

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
                await Error.Handle500(req, resp, "No subpages found"); 
            }
            var route = (req.HttpMethod, "/" + subpages?[^1] ?? "");
            // Console.WriteLine($"Routing to: {route}"); // Debugging Information
            if (routes.TryGetValue(route, out var handler))
            {
                await handler(req, resp);
            }
            else
            {
                await Error.Handle404(req, resp, "Route not found");
            }
        }

    }

}