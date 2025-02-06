using GymCRM;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace crm
{
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
            LoadUsersData(); 
        }

        private void LoadUsersData()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            if (dbConnection.Connect())  
            {
                string query = "SELECT * FROM users";
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    UsersDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.");
            }
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is DataRowView selectedRow)
            {
                UsernameTextBox.Text = selectedRow["username"].ToString();
                PasswordBox.Password = selectedRow["password"].ToString(); 
                RoleComboBox.SelectedItem = RoleComboBox.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == selectedRow["role"].ToString());
            }
            else
            {
                UsernameTextBox.Clear();
                PasswordBox.Clear();
                RoleComboBox.SelectedIndex = -1;
            }
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password) || RoleComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Будь ласка, заповніть всі поля.");
                return;
            }

            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string query = "INSERT INTO users (username, password, role) VALUES (@username, @password, @role)";

            DatabaseConnection dbConnection = new DatabaseConnection();
            if (dbConnection.Connect())
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@role", role);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Аккаунт успішно створений!");
                            LoadUsersData();
                        }
                        else
                        {
                            MessageBox.Show("Не вдалося створити аккаунт.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.");
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string username = selectedRow["username"].ToString();

                string query = "DELETE FROM users WHERE username = @username";

                DatabaseConnection dbConnection = new DatabaseConnection();
                if (dbConnection.Connect())
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                        {
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@username", username);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Аккаунт успішно видалений!");
                                LoadUsersData();
                            }
                            else
                            {
                                MessageBox.Show("Не вдалося видалити аккаунт.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Не вдалося підключитися до бази даних.");
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть користувача для видалення.");
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, виберіть користувача для редагування.");
                return;
            }

            DataRowView selectedRow = (DataRowView)UsersDataGrid.SelectedItem;
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Будь ласка, заповніть всі поля!");
                return;
            }

            string query = "UPDATE users SET username = @username, password = @password, role = @role WHERE username = @oldUsername";

            DatabaseConnection dbConnection = new DatabaseConnection();
            if (dbConnection.Connect())
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@oldUsername", selectedRow["username"].ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Зміни збережено!");
                            LoadUsersData();
                        }
                        else
                        {
                            MessageBox.Show("Не вдалося зберегти зміни.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.");
            }
        }

        private void MonitorDBButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            DatabaseConnection dbConnection = new DatabaseConnection();

            if (dbConnection.Connect())
            {
                DateTime endTime = DateTime.Now;
                TimeSpan responseTime = endTime - startTime;
                MessageBox.Show($"Підключення до бази даних успішне!\nЧас відгуку: {responseTime.TotalSeconds} секунд");
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.");
            }
        }

    }
}
