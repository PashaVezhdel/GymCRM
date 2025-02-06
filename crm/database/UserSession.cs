namespace crm
{
    internal class UserSession
    {
        private static string _username;
        private static string _role;

        public static void SetUsername(string username)
        {
            _username = username;
        }

        public static string GetUsername()
        {
            return _username;
        }

        public static void SetUserRole(string role)
        {
            _role = role;
        }

        public static string GetUserRole()
        {
            return _role;
        }

        public static bool IsAdmin()
        {
            return _role == "ADMIN";
        }
    }
}
