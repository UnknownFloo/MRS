using System.Net;

namespace MRS.auth
{
    public class Register
    {
        public static Task handleRegister(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return Task.CompletedTask;
        }
    }
}