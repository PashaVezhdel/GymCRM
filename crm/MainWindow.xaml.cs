using MySql.Data.MySqlClient;
using System;
using System.Data;
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
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            authorization authWindow = new authorization();
            authWindow.Show();
            this.Close();
        }
    }
}
