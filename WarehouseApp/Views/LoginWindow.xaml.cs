using System;
using System.IO;
using System.Windows;
using WarehouseApp.Models;
using WarehouseApp.Services;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Views
{
    public partial class LoginWindow : Window
    {
        private IDatabaseFacade _facade;
        private string _configPath;

        public LoginWindow()
        {
            InitializeComponent();
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            InitializeDatabaseConnection();
        }

        private void InitializeDatabaseConnection()
        {
            if (!File.Exists(_configPath))
            {
                OpenSetupWindow();
                return;
            }

            try
            {
                _facade = new DatabaseFacade();

                try
                {
                    var warehouses = _facade.Warehouses.GetAllWarehouses();
                }
                catch
                {
                    OpenSetupWindow();
                }
            }
            catch
            {
                OpenSetupWindow();
            }
        }

        private void OpenSetupWindow()
        {
            var setupWindow = new SetupWindow();
            if (setupWindow.ShowDialog() == true && setupWindow.IsConnected)
            {
                try
                {
                    _facade = new DatabaseFacade();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании подключения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
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

            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Если ошибка связана с БД - предлагаем перенастроить подключение
                if (ex.Message.Contains("connection") || ex.Message.Contains("базой"))
                {
                    var result = MessageBox.Show("Потеряно соединение с базой данных.\n" +
                        "Открыть настройки подключения?", "Ошибка",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        OpenSetupWindow();
                    }
                }
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_facade);
            if (registerWindow.ShowDialog() == true)
            {
                txtLogin.Focus();
            }
        }
    }
}