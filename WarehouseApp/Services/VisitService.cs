using System;
using System.Collections.Generic;
using Npgsql;
using WarehouseApp.Models;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Services
{
    public class VisitService : IVisitService
    {
        private readonly string _connectionString;

        public VisitService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Visit> GetAllVisits()
        {
            var visits = new List<Visit>();
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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
    }
}