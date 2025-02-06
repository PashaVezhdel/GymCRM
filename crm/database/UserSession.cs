namespace crm
{
    internal class UserSession
    {
        private static string role;

        public static void SetUserRole(string userRole)
        {
            role = userRole;
        }

        public static bool IsAdmin()
        {
            return role == "ADMIN";
        }

        public static string GetUserRole()
        {
            return role;
        }
    }
}
