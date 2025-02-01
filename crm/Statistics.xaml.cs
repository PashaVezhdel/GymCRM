using GymCRM;
using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace crm
{
    public partial class Statistics : Window
    {
        public Statistics()
        {
            InitializeComponent();
        }

        private bool Connect(out MySqlConnection connection)
        {
            connection = new MySqlConnection(DatabaseConfig.ConnectionString);
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
                return false;
            }
        }

        private int GetTotalClients(MySqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM Clients";
            MySqlCommand command = new MySqlCommand(query, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private decimal GetTotalBalance(MySqlConnection connection)
        {
            string query = "SELECT SUM(Balance) FROM Clients";
            MySqlCommand command = new MySqlCommand(query, connection);
            return Convert.ToDecimal(command.ExecuteScalar());
        }

        private decimal GetAverageBalance(MySqlConnection connection)
        {
            string query = "SELECT AVG(Balance) FROM Clients";
            MySqlCommand command = new MySqlCommand(query, connection);
            return Convert.ToDecimal(command.ExecuteScalar());
        }

        private int GetZeroBalanceCount(MySqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM Clients WHERE Balance = 0";
            MySqlCommand command = new MySqlCommand(query, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection connection;

            if (Connect(out connection))
            {
                try
                {
                    int totalClients = GetTotalClients(connection);
                    decimal totalBalance = GetTotalBalance(connection);
                    decimal averageBalance = GetAverageBalance(connection);
                    int zeroBalanceCount = GetZeroBalanceCount(connection);

                    TotalClients.Text = totalClients.ToString();
                    TotalBalance.Text = totalBalance.ToString("C"); 
                    AverageBalance.Text = averageBalance.ToString("C");
                    ZeroBalanceCount.Text = zeroBalanceCount.ToString();

                    MessageBox.Show("Статистика успішно завантажена!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при отриманні даних: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
