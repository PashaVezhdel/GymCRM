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
            LoadStatistics(); 
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

        private void LoadStatistics()
        {
            MySqlConnection connection;

            if (Connect(out connection))
            {
                try
                {
                    TotalClients.Text = GetTotalClients(connection).ToString();
                    TotalBalance.Text = GetTotalBalance(connection).ToString("C");
                    AverageBalance.Text = GetAverageBalance(connection).ToString("C");
                    ZeroBalanceCount.Text = GetZeroBalanceCount(connection).ToString();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
            MessageBox.Show("Статистика успішно завантажена!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
