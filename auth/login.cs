using System.Text.Json.Nodes;

namespace MRS
{
    class Login
    {
        private static readonly dynamic[] users = [
            new { username = "admin", password = "password" },
            new { username = "user", password = "1234" }
        ];

        public static bool Authenticate(string username, string password)
        {
            dynamic? user = Array.Find(users, u => u.username == username && u.password == password);
            
            return user != null;
        }
    }
}