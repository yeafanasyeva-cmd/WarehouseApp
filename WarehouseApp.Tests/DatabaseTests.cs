using System;
using System.Linq;
using Xunit;
using WarehouseApp.Models;
using WarehouseApp.Services;
using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Tests
{
    public class DatabaseTests : IDisposable
    {
        private IDatabaseFacade _facade;
        private int _testWarehouseId;

        public DatabaseTests()
        {
            _facade = new DatabaseFacade();
        }

        public void Dispose()
        {
            if (_testWarehouseId > 0)
            {
                _facade.Warehouses.DeleteWarehouse(_testWarehouseId);
            }
        }

        // ==================== ТЕСТ 1 ====================
        [Fact]
        public void Test_AdminLogin_ValidCredentials_ReturnsAdminUser()
        {
            User user = _facade.Auth.AuthenticateUser("vasya", "123456");

            Assert.NotNull(user);
            Assert.Equal("admin", user.Role);
            Assert.Equal("Василий Петров", user.FullName);
        }

        // ==================== ТЕСТ 2 ====================
        [Fact]
        public void Test_UserLogin_ValidCredentials_ReturnsUser()
        {
            User user = _facade.Auth.AuthenticateUser("nastya", "123456");

            Assert.NotNull(user);
            Assert.Equal("user", user.Role);
        }

        // ==================== ТЕСТ 3 ====================
        [Fact]
        public void Test_Login_InvalidPassword_ReturnsNull()
        {
            User user = _facade.Auth.AuthenticateUser("vasya", "wrong_password");

            Assert.Null(user);
        }

        // ==================== ТЕСТ 4 ====================
        [Fact]
        public void Test_GetAllWarehouses_ReturnsList()
        {
            var warehouses = _facade.Warehouses.GetAllWarehouses();

            Assert.NotNull(warehouses);
            Assert.NotEmpty(warehouses);
        }

        // ==================== ТЕСТ 5 ====================
        [Fact]
        public void Test_AddWarehouse_ThenGet_Success()
        {
            var newWarehouse = new Warehouse
            {
                Name = "Тестовый склад XUnit",
                Address = "г. Тест, ул. Тестовая, 99",
                SpecialConditions = "Тестовые условия"
            };

            bool addResult = _facade.Warehouses.AddWarehouse(newWarehouse);
            Assert.True(addResult);

            var warehouses = _facade.Warehouses.GetAllWarehouses();
            var added = warehouses.FirstOrDefault(w => w.Name == "Тестовый склад XUnit");

            Assert.NotNull(added);
            Assert.Equal("г. Тест, ул. Тестовая, 99", added.Address);

            _testWarehouseId = added.Id;
        }

        // ==================== ТЕСТ 6 ====================
        [Fact]
        public void Test_AddRentRequest_ReturnsTrue()
        {
            var warehouses = _facade.Warehouses.GetAllWarehouses();
            if (warehouses.Count == 0)
            {
                return; // нет помещений - тест пропускаем
            }

            int warehouseId = warehouses.First().Id;
            int userId = 2;

            bool result = _facade.Rents.AddRentRequest(warehouseId, userId, "Тестовая заявка от xUnit");
            Assert.True(result);
        }

        // ==================== ТЕСТ 7 ====================
        [Fact]
        public void Test_ApproveRentRequest_ChangesStatus()
        {
            var warehouses = _facade.Warehouses.GetAllWarehouses();
            if (warehouses.Count == 0)
            {
                return;
            }

            int warehouseId = warehouses.First().Id;
            int userId = 2;
            int adminId = 1;

            string uniqueCondition = $"Тест на одобрение {DateTime.Now.Ticks}";
            _facade.Rents.AddRentRequest(warehouseId, userId, uniqueCondition);

            var pendingRequests = _facade.Rents.GetPendingRequests();
            var request = pendingRequests.FirstOrDefault(r => r.SpecialConditions == uniqueCondition);

            if (request == null)
            {
                return;
            }

            bool approveResult = _facade.Rents.ApproveRentRequest(request.Id, adminId);
            Assert.True(approveResult);
        }

        // ==================== ТЕСТ 8 ====================
        [Fact]
        public void Test_AddAndDeleteVisit_Success()
        {
            var activeRents = _facade.Rents.GetActiveRents();

            if (activeRents.Count == 0)
            {
                var warehouses = _facade.Warehouses.GetAllWarehouses();
                if (warehouses.Count == 0)
                {
                    return;
                }

                int warehouseId = warehouses.First().Id;
                int userId = 2;
                int adminId = 1;

                string uniqueCondition = $"Для теста посещений {DateTime.Now.Ticks}";
                _facade.Rents.AddRentRequest(warehouseId, userId, uniqueCondition);

                var pending = _facade.Rents.GetPendingRequests();
                var newRequest = pending.FirstOrDefault(r => r.SpecialConditions == uniqueCondition);

                if (newRequest != null)
                {
                    _facade.Rents.ApproveRentRequest(newRequest.Id, adminId);
                }

                activeRents = _facade.Rents.GetActiveRents();
                if (activeRents.Count == 0)
                {
                    return;
                }
            }

            int rentId = activeRents.First().Id;
            string uniqueId = DateTime.Now.Ticks.ToString();
            string firstName = $"Тест{uniqueId}";
            string lastName = $"Тестов{uniqueId}";

            bool addResult = _facade.Visits.AddVisit(rentId, firstName, lastName, "ООО ТестXUnit", "Т999ТТ");
            Assert.True(addResult);

            var visits = _facade.Visits.GetAllVisits();
            var added = visits.FirstOrDefault(v => v.FirstName == firstName && v.LastName == lastName);
            Assert.NotNull(added);

            bool deleteResult = _facade.Visits.DeleteVisit(added.Id);
            Assert.True(deleteResult);
        }
    }
}