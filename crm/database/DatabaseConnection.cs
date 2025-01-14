using MySql.Data.MySqlClient;
using System;

namespace GymCRM
{
    public class DatabaseConnection
    {
        public bool Connect()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Підключення успішне!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
                return false;
            }
        }
    }
}
