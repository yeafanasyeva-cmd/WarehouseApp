using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Npgsql;

namespace WarehouseApp.Views
{
    public partial class SetupWindow : Window
    {
        public string ConnectionString { get; private set; }
        public bool IsConnected { get; private set; }

        public SetupWindow()
        {
            InitializeComponent();

            btnTest.Click += BtnTest_Click;
            btnSave.Click += BtnSave_Click;
            btnExit.Click += BtnExit_Click;
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            var connString = GetConnectionString();

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    lblStatus.Text = "✓ Подключение успешно!";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                    btnSave.IsEnabled = true;

                    MessageBox.Show("Подключение к базе данных успешно!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Ошибка: {ex.Message}";
                lblStatus.Foreground = System.Windows.Media.Brushes.Red;
                btnSave.IsEnabled = false;

                MessageBox.Show($"Ошибка подключения:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сохраняем настройки
                SaveConfig();

                ConnectionString = GetConnectionString();
                IsConnected = true;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtHost.Text))
            {
                MessageBox.Show("Введите адрес сервера", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                MessageBox.Show("Введите имя базы данных", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtPort.Text, out _))
            {
                MessageBox.Show("Порт должен быть числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private string GetConnectionString()
        {
            return $"Host={txtHost.Text};" +
                   $"Port={txtPort.Text};" +
                   $"Database={txtDatabase.Text};" +
                   $"Username={txtUsername.Text};" +
                   $"Password={txtPassword.Password}";
        }

        private void SaveConfig()
        {
            var config = new
            {
                Database = new
                {
                    Host = txtHost.Text,
                    Port = int.Parse(txtPort.Text),
                    Database = txtDatabase.Text,
                    Username = txtUsername.Text,
                    Password = txtPassword.Password
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(config, options);

            var configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            File.WriteAllText(configPath, json);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            IsConnected = false;
            DialogResult = false;
            Close();
        }
    }
}