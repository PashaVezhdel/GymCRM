using System;
using System.Windows;

namespace GymCRM
{
    public partial class authorization : Window
    {
        public authorization() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Створюємо екземпляр класу для підключення
            DatabaseConnection dbConnection = new DatabaseConnection();

            // Викликаємо метод підключення
            bool isConnected = dbConnection.Connect();

            // Виводимо повідомлення про результат
            if (isConnected)
            {
                MessageBox.Show("Connection successful!");
            }
            else
            {
                MessageBox.Show("Connection failed!");
            }
        }
    }
}
