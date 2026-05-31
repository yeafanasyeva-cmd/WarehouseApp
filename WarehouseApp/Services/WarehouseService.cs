using System;
using System.Collections.Generic;
using Npgsql;
using WarehouseApp.Models;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly string _connectionString;

        public WarehouseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Warehouse> GetAllWarehouses()
        {
            var warehouses = new List<Warehouse>();
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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
            using (var conn = new NpgsqlConnection(_connectionString))
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
    }
}