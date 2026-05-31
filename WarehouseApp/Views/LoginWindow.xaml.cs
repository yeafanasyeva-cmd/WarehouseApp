using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Views
{
    public partial class LoginWindow : Window
    {
        private IDatabaseFacade _facade;

        public LoginWindow()
        {
            InitializeComponent();
            _facade = new DatabaseFacade();
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

            User user = _facade.Auth.AuthenticateUser(login, password);

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
                AdminMainWindow adminWindow = new AdminMainWindow(user, _facade);
                adminWindow.Show();
            }
            else
            {
                UserMainWindow userWindow = new UserMainWindow(user, _facade);
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