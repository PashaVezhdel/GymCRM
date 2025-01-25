using System.Windows;

namespace GymCRM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            authorization authWindow = new authorization();
            authWindow.Show();
            this.Close();
        }
    }
}
