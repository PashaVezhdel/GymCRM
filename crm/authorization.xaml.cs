using System;
using System.Windows;
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
                            MessageBox.Show($"Вхід успішний! Ваша роль: {role}", "GymCRM", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Відкриваємо MainWindow перед закриттям поточного вікна
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();

                            // Закриваємо вікно авторизації після того, як MainWindow відкрито
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
