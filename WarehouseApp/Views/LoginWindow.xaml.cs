using System.Windows;
using WarehouseApp.Services;
using WarehouseApp.Models;

namespace WarehouseApp.Views
{
    public partial class LoginWindow : Window
    {
        private DatabaseAdapter dbAdapter;

        public LoginWindow()
        {
            InitializeComponent();
            dbAdapter = new DatabaseAdapter();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            User user = dbAdapter.AuthenticateUser(login, password);

            if (user == null)
            {
                txtLogin.Clear();
                txtPassword.Clear();
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.Role == "admin")
            {
                AdminMainWindow adminWindow = new AdminMainWindow(user, dbAdapter);
                adminWindow.Show();
            }
            else
            {
                UserMainWindow userWindow = new UserMainWindow(user, dbAdapter);
                userWindow.Show();
            }

            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}