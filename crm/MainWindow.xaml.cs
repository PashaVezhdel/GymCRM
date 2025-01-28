using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;

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
        }

        private void LoadClientsData()
        {
            string connectionString = DatabaseConfig.ConnectionString;
            string query = "SELECT * FROM Clients"; 

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                    dataAdapter.Fill(Clients); 

                    ClientsDataGrid.ItemsSource = Clients.DefaultView; 
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
                DateOfBirthTextBox.Text = SelectedClient["date_of_birth"].ToString();
                PhoneNumberTextBox.Text = SelectedClient["phone_number"].ToString();
                LastPaymentTextBox.Text = SelectedClient["last_payment_date"].ToString();
                SubscriptionEndTextBox.Text = SelectedClient["subscription_end_date"].ToString();
                CreationDateTextBox.Text = SelectedClient["created_at"].ToString();
                BalanceTextBox.Text = SelectedClient["balance"].ToString();
                CommentTextBox.Text = SelectedClient["comments"].ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            authorization authWindow = new authorization();
            authWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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

        private void UpdateClientsList()
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            /* Перевірка даних перед виконанням збереження
            if (!ValidateInput())
            {
                MessageBox.Show("Заповніть всі поля!");
                return;
            } */

            try
            {
                string connectionString = DatabaseConfig.ConnectionString;
                string query = "UPDATE Clients SET full_name = @full_name, date_of_birth = @date_of_birth, " +
                               "phone_number = @phone_number, last_payment_date = @last_payment_date, " +
                               "subscription_end_date = @subscription_end_date, balance = @balance " +
                               "WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@full_name", FullNameTextBox.Text);
                    command.Parameters.AddWithValue("@date_of_birth", DateOfBirthTextBox.Text);
                    command.Parameters.AddWithValue("@phone_number", PhoneNumberTextBox.Text);
                    command.Parameters.AddWithValue("@last_payment_date", LastPaymentTextBox.Text);
                    command.Parameters.AddWithValue("@subscription_end_date", SubscriptionEndTextBox.Text);
                    command.Parameters.AddWithValue("@balance", BalanceTextBox.Text);
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

    }
}
