using System;
using System.Windows;
using crm;
using MySql.Data.MySqlClient;

namespace GymCRM
{
    public partial class authorization : Window
    {
        private readonly DatabaseConnection _dbConnection;

        public authorization()
        {
            InitializeComponent();
            _dbConnection = new DatabaseConnection();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT role FROM users WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string role = result.ToString();
                            UserSession.SetUsername(username);
                            UserSession.SetUserRole(role);
                            new UserLogger(DatabaseConfig.ConnectionString).LogUserActivity(username, "LOGIN");

                            new MainWindow().Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Невірний логін або пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при підключенні до бази даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
