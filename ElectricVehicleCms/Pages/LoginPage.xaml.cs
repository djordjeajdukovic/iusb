
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ElectricVehicleCms.Models;

namespace ElectricVehicleCms.Pages
{
    public partial class LoginPage : Page
    {
        private readonly MainWindow _mainWindow;

        public LoginPage()
        {
            InitializeComponent();

            _mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginErrorLabel.Content = string.Empty;

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            User foundUser = _mainWindow.Users.FirstOrDefault(user => user.Username == username && user.Password == password);

            if (foundUser == null)
            {
                LoginErrorLabel.Content = "Wrong username or password.";
                return;
            }

            _mainWindow.CurrentUser = foundUser;
            _mainWindow.NavigateToVehiclesPage();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Close();
        }
    }
}
