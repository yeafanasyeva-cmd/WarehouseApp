using System.Collections.Generic;
using WarehouseApp.Models;

namespace WarehouseApp.Services.Interfaces
{
    public interface IWarehouseService
    {
        List<Warehouse> GetAllWarehouses();
        bool AddWarehouse(Warehouse warehouse);
        bool UpdateWarehouse(Warehouse warehouse);
        bool DeleteWarehouse(int warehouseId);
    }
}