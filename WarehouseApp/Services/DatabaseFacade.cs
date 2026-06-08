using System;
using System.IO;
using System.Text.Json;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Services
{
    public class DatabaseFacade : IDatabaseFacade
    {
        private string _connectionString;
        public IAuthService Auth { get; private set; }
        public IWarehouseService Warehouses { get; private set; }
        public IRentService Rents { get; private set; }
        public IVisitService Visits { get; private set; }

        public DatabaseFacade()
        {
            LoadConnectionString();
            InitializeServices();
        }

        private void LoadConnectionString()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            if (!File.Exists(configPath))
            {
                throw new Exception("Файл конфигурации не найден");
            }

            try
            {
                var json = File.ReadAllText(configPath);
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                var database = root.GetProperty("Database");

                var host = database.GetProperty("Host").GetString();
                var port = database.GetProperty("Port").GetInt32();
                var dbName = database.GetProperty("Database").GetString();
                var username = database.GetProperty("Username").GetString();
                var password = database.GetProperty("Password").GetString();

                _connectionString = $"Host={host};Port={port};Database={dbName};Username={username};Password={password}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка чтения конфигурации: {ex.Message}");
            }
        }

        private void InitializeServices()
        {
            Auth = new AuthService(_connectionString);
            Warehouses = new WarehouseService(_connectionString);
            Rents = new RentService(_connectionString);
            Visits = new VisitService(_connectionString);
        }
    }
}