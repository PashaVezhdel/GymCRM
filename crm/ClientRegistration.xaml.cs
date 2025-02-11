using System;
using System.Windows;
using GymCRM;
using MySql.Data.MySqlClient;

namespace crm
{
    public partial class ClientRegistration : Window
    {
        public ClientRegistration()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                    {
                        connection.Open();

                        string getIdQuery = "SELECT MIN(t1.id + 1) AS next_id " +
                                            "FROM clients t1 " +
                                            "LEFT JOIN clients t2 ON t1.id + 1 = t2.id " +
                                            "WHERE t2.id IS NULL";

                        int nextId = 1; 
                        using (MySqlCommand getIdCommand = new MySqlCommand(getIdQuery, connection))
                        {
                            object result = getIdCommand.ExecuteScalar();
                            if (result != DBNull.Value && result != null)
                            {
                                nextId = Convert.ToInt32(result);
                            }
                        }

                        string query = "INSERT INTO clients (id, full_name, date_of_birth, phone_number, last_payment_date, subscription_end_date, comments, balance) " +
                                       "VALUES (@id, @fullName, @dateOfBirth, @phoneNumber, @lastPaymentDate, @subscriptionEndDate, @comments, @balance)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", nextId);
                            command.Parameters.AddWithValue("@fullName", FullNameTextBox.Text);
                            command.Parameters.AddWithValue("@dateOfBirth", DateOfBirthTextBox.Text);
                            command.Parameters.AddWithValue("@phoneNumber", PhoneNumberTextBox.Text);
                            command.Parameters.AddWithValue("@lastPaymentDate", LastPaymentDateTextBox.Text);
                            command.Parameters.AddWithValue("@subscriptionEndDate", SubscriptionEndDateTextBox.Text);
                            command.Parameters.AddWithValue("@comments", CommentTextBox.Text);

                            string balanceText = BalanceTextBox.Text.Replace(',', '.');
                            decimal balance;
                            if (decimal.TryParse(balanceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out balance))
                            {
                                command.Parameters.AddWithValue("@balance", balance);
                            }
                            else
                            {
                                MessageBox.Show("Будь ласка, введіть коректно баланс.");
                                return;
                            }

                            command.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Клієнта успішно зареєстровано!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка під час реєстрації: " + ex.Message);
                }
            }
        }


        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DateOfBirthTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastPaymentDateTextBox.Text) ||
                string.IsNullOrWhiteSpace(SubscriptionEndDateTextBox.Text) ||
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
            if (!DateTime.TryParseExact(LastPaymentDateTextBox.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out lastPaymentDate))
            {
                MessageBox.Show("Будь ласка, введіть правильну дату останнього платежу (формат: РРРР/ММ/ДД).");
                return false;
            }

            DateTime subscriptionEndDate;
            if (!DateTime.TryParseExact(SubscriptionEndDateTextBox.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out subscriptionEndDate))
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

            string balanceText = BalanceTextBox.Text.Replace(',', '.');  
            decimal balance;
            if (!decimal.TryParse(balanceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out balance))
            {
                MessageBox.Show("Будь ласка, введіть коректно баланс.");
                return false;
            }

            return true;
        }
    }
}
