using System;
using MySql.Data.MySqlClient;

namespace crm
{
    public class UserLogger
    {
        private readonly string _connectionString;

        public UserLogger(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void LogUserActivity(string username, string action)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO user_logs (username, action, timestamp) VALUES (@username, @action, @timestamp)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
