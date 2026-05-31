using Npgsql;
using WarehouseApp.Models;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User AuthenticateUser(string login, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE login = @login AND password = @password";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                Password = reader.GetString(2),
                                Role = reader.GetString(3),
                                FullName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Company = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool IsLoginExists(string login)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM users WHERE login = @login";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool RegisterUser(string login, string password, string fullName, string company, string role)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO users (login, password, role, full_name, company)
                                VALUES (@login, @password, @role, @fullName, @company)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@fullName", (object)fullName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@company", (object)company ?? DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}