using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Globalization;
using crm;

namespace GymCRM
{
    public partial class MainWindow : Window
    {
        public DataTable Clients { get; set; }
        public DataRow SelectedClient { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Clients = new DataTable(); 
            LoadClientsData();
            DataContext = this;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string username = UserSession.GetUsername();

            if (!string.IsNullOrEmpty(username))
            {
                new UserLogger(DatabaseConfig.ConnectionString).LogUserActivity(username, "LOGOUT");
            }
        }

        public void LoadClientsData()
        {
            string connectionString = DatabaseConfig.ConnectionString;
            string query = "SELECT * FROM clients"; // Оновлюємо весь список клієнтів

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                    DataTable clientsDataTable = new DataTable();
                    dataAdapter.Fill(clientsDataTable);

                    // Оновлюємо ItemsSource DataGrid
                    ClientsDataGrid.ItemsSource = clientsDataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка підключення до бази даних: " + ex.Message);
                }
            }
        }


        private void ClientsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is DataRowView rowView)
            {
                SelectedClient = rowView.Row;

                FullNameTextBox.Text = SelectedClient["full_name"].ToString();

                if (SelectedClient["date_of_birth"] != DBNull.Value)
                    DateOfBirthTextBox.Text = ((DateTime)SelectedClient["date_of_birth"]).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                if (SelectedClient["last_payment_date"] != DBNull.Value)
                    LastPaymentTextBox.Text = ((DateTime)SelectedClient["last_payment_date"]).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                if (SelectedClient["subscription_end_date"] != DBNull.Value)
                    SubscriptionEndTextBox.Text = ((DateTime)SelectedClient["subscription_end_date"]).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                CreationDateTextBox.Text = SelectedClient["created_at"].ToString();
                BalanceTextBox.Text = Convert.ToDecimal(SelectedClient["balance"]).ToString("F2", CultureInfo.InvariantCulture);
                if (SelectedClient["phone_number"] != DBNull.Value)
                    PhoneNumberTextBox.Text = SelectedClient["phone_number"].ToString();

                CommentTextBox.Text = SelectedClient["comments"].ToString();
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            string username = UserSession.GetUsername();

            if (!string.IsNullOrEmpty(username))
            {
                new UserLogger(DatabaseConfig.ConnectionString).LogUserActivity(username, "LOGOUT");
            }

            new authorization().Show();
            this.Close();
        }


        private void Delete(object sender, RoutedEventArgs e)
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Будь ласка, виберіть клієнта для видалення.");
                return;
            }

            var result = MessageBox.Show("Ви впевнені, що хочете видалити цього клієнта?", "Підтвердження", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                string connectionString = DatabaseConfig.ConnectionString;
                string query = "DELETE FROM Clients WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", SelectedClient["id"]);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Клієнт успішно видалений.");

                    UpdateClientsList();
                }

                FullNameTextBox.Clear();
                DateOfBirthTextBox.Clear();
                PhoneNumberTextBox.Clear();
                LastPaymentTextBox.Clear();
                SubscriptionEndTextBox.Clear();
                BalanceTextBox.Clear();
                CommentTextBox.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при видаленні клієнта: " + ex.Message);
            }
        }

        public void UpdateClientsList()
        {
            string connectionString = DatabaseConfig.ConnectionString;
            string query = "SELECT * FROM Clients";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    ClientsDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при оновленні списку клієнтів: " + ex.Message);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Заповніть всі поля коректно!");
                return;
            }

            try
            {
                string connectionString = DatabaseConfig.ConnectionString;
                string query = "UPDATE Clients SET full_name = @full_name, date_of_birth = @date_of_birth, " +
                               "phone_number = @phone_number, last_payment_date = @last_payment_date, " +
                               "subscription_end_date = @subscription_end_date, balance = @balance, " +
                               "comments = @comments " + 
                               "WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@full_name", FullNameTextBox.Text);
                    command.Parameters.AddWithValue("@date_of_birth", DateOfBirthTextBox.Text);  // Формат: yyyy-MM-dd
                    command.Parameters.AddWithValue("@phone_number", PhoneNumberTextBox.Text);
                    command.Parameters.AddWithValue("@last_payment_date", LastPaymentTextBox.Text);  // Формат: yyyy-MM-dd
                    command.Parameters.AddWithValue("@subscription_end_date", SubscriptionEndTextBox.Text);  // Формат: yyyy-MM-dd
                    command.Parameters.AddWithValue("@balance", BalanceTextBox.Text);
                    command.Parameters.AddWithValue("@comments", CommentTextBox.Text);  
                    command.Parameters.AddWithValue("@id", SelectedClient["id"]);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Зміни успішно збережено.");
                    UpdateClientsList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при збереженні змін: " + ex.Message);
            }
        }


        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DateOfBirthTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastPaymentTextBox.Text) ||
                string.IsNullOrWhiteSpace(SubscriptionEndTextBox.Text) ||
                string.IsNullOrWhiteSpace(BalanceTextBox.Text))
            {
                MessageBox.Show("Будь ласка, заповніть всі поля!");
                return false; 
            }


            DateTime dateOfBirth;
            if (!DateTime.TryParseExact(DateOfBirthTextBox.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out dateOfBirth))
            {
                MessageBox.Show("Будь ласка, введіть правильну дату народження (формат: РРРР/ММ/ДД).");
                return false;
            }

            DateTime lastPaymentDate;
            if (!DateTime.TryParseExact(LastPaymentTextBox.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out lastPaymentDate))
            {
                MessageBox.Show("Будь ласка, введіть правильну дату останнього платежу (формат: РРРР/ММ/ДД).");
                return false;
            }

            DateTime subscriptionEndDate;
            if (!DateTime.TryParseExact(SubscriptionEndTextBox.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out subscriptionEndDate))
            {
                MessageBox.Show("Будь ласка, введіть правильну дату закінчення підписки (формат: РРРР/ММ/ДД).");
                return false;
            }

            string phonePattern = @"^\+380\d{9}$";  
            if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneNumberTextBox.Text, phonePattern))
            {
                MessageBox.Show("Будь ласка, введіть правильний номер телефону (формат: +380XXXXXXXXX).");
                return false;
            }

            decimal balance;
            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            if (!decimal.TryParse(BalanceTextBox.Text, System.Globalization.NumberStyles.Any, cultureInfo, out balance))
            {
                MessageBox.Show("Будь ласка, введіть коректно баланс.");
                return false;
            }

            return true;  
        }

        public void UpdateList(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateClientsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при оновленні списку клієнтів: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Будь ласка, введіть номер телефону або ПІБ для пошуку.");
                return;
            }

            var databaseConnection = new DatabaseConnection();
            if (!databaseConnection.Connect())
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.");
                return;
            }

            string query = "SELECT * FROM clients WHERE full_name LIKE @searchText OR phone_number LIKE @searchText";

            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                var reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Клієнтів не знайдено.");
                }
                else
                {
                    ClientsDataGrid.ItemsSource = dt.DefaultView;
                }
            }
        }

        private void Button_Registration(object sender, RoutedEventArgs e)
        {
            ClientRegistration registrationWindow = new ClientRegistration(this);
            registrationWindow.Show();

        }

        private void Statistics(object sender, RoutedEventArgs e)
        {
            Statistics statisticsWindow = new Statistics();
            statisticsWindow.Show();
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsAdmin()) 
            {
                AdminPanel adminPanelWindow = new AdminPanel();
                adminPanelWindow.Show();
            }
            else
            {
                MessageBox.Show("У вас немає прав доступу до панелі адміністратора.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
