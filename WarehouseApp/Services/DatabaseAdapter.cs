using Npgsql;
using System;
using System.Collections.Generic;
using WarehouseApp.Models;

namespace WarehouseApp.Services
{
    public class DatabaseAdapter
    {
        private string connectionString;

        public DatabaseAdapter()
        {
            connectionString = "Host=localhost;Database=WarehouseApp;Username=postgres;Password=ybrjif2006";
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public User AuthenticateUser(string login, string password)
        {
            using (var conn = GetConnection())
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

        // ПОМЕЩЕНИЯ
        public List<Warehouse> GetAllWarehouses()
        {
            var warehouses = new List<Warehouse>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM warehouses ORDER BY name";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        warehouses.Add(new Warehouse
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Address = reader.GetString(2),
                            SpecialConditions = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return warehouses;
        }

        public bool AddWarehouse(Warehouse warehouse)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO warehouses (name, address, special_conditions, created_at)
                                VALUES (@name, @address, @conditions, @createdAt)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", warehouse.Name);
                    cmd.Parameters.AddWithValue("@address", warehouse.Address);
                    cmd.Parameters.AddWithValue("@conditions", (object)warehouse.SpecialConditions ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateWarehouse(Warehouse warehouse)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE warehouses SET name = @name, address = @address, special_conditions = @conditions
                                WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", warehouse.Id);
                    cmd.Parameters.AddWithValue("@name", warehouse.Name);
                    cmd.Parameters.AddWithValue("@address", warehouse.Address);
                    cmd.Parameters.AddWithValue("@conditions", (object)warehouse.SpecialConditions ?? DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteWarehouse(int warehouseId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM warehouses WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", warehouseId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ЗАЯВКИ НА АРЕНДУ
        public List<RentHistory> GetPendingRequests()
        {
            var requests = new List<RentHistory>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT rh.*, w.name as warehouse_name, u.full_name as user_name
                                FROM renthistory rh
                                JOIN warehouses w ON rh.warehouse_id = w.id
                                JOIN users u ON rh.user_id = u.id
                                WHERE rh.status = 'pending'
                                ORDER BY rh.request_date";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requests.Add(new RentHistory
                        {
                            Id = reader.GetInt32(0),
                            WarehouseId = reader.GetInt32(1),
                            WarehouseName = reader.GetString(10),
                            UserId = reader.GetInt32(2),
                            UserName = reader.GetString(11),
                            Status = reader.GetString(3),
                            RequestDate = reader.GetDateTime(4),
                            ApprovedDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            ApprovedBy = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                            SpecialConditions = reader.IsDBNull(7) ? null : reader.GetString(7),
                            RentStartDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                            RentEndDate = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                        });
                    }
                }
            }
            return requests;
        }

        public List<RentHistory> GetApprovedRents()
        {
            var rents = new List<RentHistory>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT rh.*, w.name as warehouse_name, u.full_name as user_name,
                                        a.full_name as approved_by_name
                                FROM renthistory rh
                                JOIN warehouses w ON rh.warehouse_id = w.id
                                JOIN users u ON rh.user_id = u.id
                                LEFT JOIN users a ON rh.approved_by = a.id
                                WHERE rh.status = 'approved'
                                ORDER BY rh.approved_date DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rents.Add(new RentHistory
                        {
                            Id = reader.GetInt32(0),
                            WarehouseId = reader.GetInt32(1),
                            WarehouseName = reader.GetString(10),
                            UserId = reader.GetInt32(2),
                            UserName = reader.GetString(11),
                            Status = reader.GetString(3),
                            RequestDate = reader.GetDateTime(4),
                            ApprovedDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            ApprovedBy = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                            ApprovedByName = reader.IsDBNull(12) ? null : reader.GetString(12),
                            SpecialConditions = reader.IsDBNull(7) ? null : reader.GetString(7),
                            RentStartDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                            RentEndDate = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                        });
                    }
                }
            }
            return rents;
        }

        public bool AddRentRequest(int warehouseId, int userId, string specialConditions)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO renthistory (warehouse_id, user_id, status, request_date, special_conditions)
                                VALUES (@warehouseId, @userId, 'pending', @requestDate, @conditions)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@requestDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@conditions", (object)specialConditions ?? DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ApproveRentRequest(int rentHistoryId, int adminId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE renthistory 
                                SET status = 'approved', approved_by = @adminId, approved_date = @approvedDate
                                WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", rentHistoryId);
                    cmd.Parameters.AddWithValue("@adminId", adminId);
                    cmd.Parameters.AddWithValue("@approvedDate", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool RejectRentRequest(int rentHistoryId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE renthistory SET status = 'rejected' WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", rentHistoryId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ПОСЕЩЕНИЯ
        public List<Visit> GetAllVisits()
        {
            var visits = new List<Visit>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT v.*, w.name as warehouse_name, u.full_name as user_name
                                FROM visits v
                                JOIN renthistory rh ON v.renthistory_id = rh.id
                                JOIN warehouses w ON rh.warehouse_id = w.id
                                JOIN users u ON rh.user_id = u.id
                                ORDER BY v.visit_date DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        visits.Add(new Visit
                        {
                            Id = reader.GetInt32(0),
                            RentHistoryId = reader.GetInt32(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            Company = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CarNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
                            VisitDate = reader.GetDateTime(6),
                            WarehouseName = reader.GetString(7),
                            UserName = reader.GetString(8)
                        });
                    }
                }
            }
            return visits;
        }

        public bool AddVisit(int rentHistoryId, string firstName, string lastName, string company, string carNumber)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO visits (renthistory_id, first_name, last_name, company, car_number, visit_date)
                                VALUES (@rentHistoryId, @firstName, @lastName, @company, @carNumber, @visitDate)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@rentHistoryId", rentHistoryId);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@company", (object)company ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@carNumber", (object)carNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@visitDate", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteVisit(int visitId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM visits WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", visitId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<RentHistory> GetActiveRents()
        {
            var rents = new List<RentHistory>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT rh.*, w.name as warehouse_name
                                FROM renthistory rh
                                JOIN warehouses w ON rh.warehouse_id = w.id
                                WHERE rh.status = 'approved'
                                ORDER BY w.name";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rents.Add(new RentHistory
                        {
                            Id = reader.GetInt32(0),
                            WarehouseId = reader.GetInt32(1),
                            WarehouseName = reader.GetString(10),
                            Status = reader.GetString(3)
                        });
                    }
                }
            }
            return rents;
        }
    }
}