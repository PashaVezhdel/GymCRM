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
    }
}
