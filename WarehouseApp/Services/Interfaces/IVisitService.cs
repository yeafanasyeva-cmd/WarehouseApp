using System.Collections.Generic;
using WarehouseApp.Models;

namespace WarehouseApp.Services.Interfaces
{
    public interface IVisitService
    {
        List<Visit> GetAllVisits();
        bool AddVisit(int rentHistoryId, string firstName, string lastName, string company, string carNumber);
        bool DeleteVisit(int visitId);
    }
}