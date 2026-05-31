using System;
using System.Collections.Generic;
using Npgsql;
using WarehouseApp.Models;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Services
{
    public class RentService : IRentService
    {
        private readonly string _connectionString;

        public RentService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<RentHistory> GetPendingRequests()
        {
            var requests = new List<RentHistory>();
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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

        public List<RentHistory> GetActiveRents()
        {
            var rents = new List<RentHistory>();
            using (var conn = new NpgsqlConnection(_connectionString))
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

        public bool AddRentRequest(int warehouseId, int userId, string specialConditions)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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

        public List<RentHistory> GetUserRentRequests(int userId)
        {
            var requests = new List<RentHistory>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT rh.*, w.name as warehouse_name, u.full_name as user_name,
                                a.full_name as approved_by_name
                        FROM renthistory rh
                        JOIN warehouses w ON rh.warehouse_id = w.id
                        JOIN users u ON rh.user_id = u.id
                        LEFT JOIN users a ON rh.approved_by = a.id
                        WHERE rh.user_id = @userId
                        ORDER BY rh.request_date DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
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
                                ApprovedByName = reader.IsDBNull(12) ? null : reader.GetString(12),
                                SpecialConditions = reader.IsDBNull(7) ? null : reader.GetString(7),
                                RentStartDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                RentEndDate = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }
            return requests;
        }
    }
}