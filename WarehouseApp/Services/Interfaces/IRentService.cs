using System.Collections.Generic;
using WarehouseApp.Models;

namespace WarehouseApp.Services.Interfaces
{
    public interface IRentService
    {
        List<RentHistory> GetPendingRequests();
        List<RentHistory> GetApprovedRents();
        List<RentHistory> GetActiveRents();
        List<RentHistory> GetUserRentRequests(int userId);
        bool AddRentRequest(int warehouseId, int userId, string specialConditions);
        bool ApproveRentRequest(int rentHistoryId, int adminId);
        bool RejectRentRequest(int rentHistoryId);
    }
}