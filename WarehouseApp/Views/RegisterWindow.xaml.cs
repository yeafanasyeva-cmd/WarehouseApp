using System.Windows;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Views
{
    public partial class RegisterWindow : Window
    {
        private IDatabaseFacade _facade;

        public RegisterWindow(IDatabaseFacade facade)
        {
            InitializeComponent();
            _facade = facade;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string fullName = txtFullName.Text.Trim();
            string company = txtCompany.Text.Trim();

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Clear();
                txtConfirmPassword.Clear();
                return;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Введите ФИО", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_facade.Auth.IsLoginExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Clear();
                txtLogin.Focus();
                return;
            }

            bool result = _facade.Auth.RegisterUser(login, password, fullName, company, "user");

            if (result)
            {
                MessageBox.Show("Регистрация успешно завершена!\nТеперь вы можете войти в систему.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации. Попробуйте ещё раз.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}