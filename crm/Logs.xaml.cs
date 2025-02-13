using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using GymCRM;

namespace crm
{
    public partial class Logs : Window
    {
        private DatabaseConnection _dbConnection;

        public Logs()
        {
            InitializeComponent();
            _dbConnection = new DatabaseConnection();
            LoadUserLogs();
        }

        private void LoadUserLogs()
        {
            if (_dbConnection.Connect())
            {
                string connectionString = DatabaseConfig.ConnectionString;
                string query = "SELECT * FROM user_logs";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        logsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка при підключенні до бази даних: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.");
            }
        }
    }
}
