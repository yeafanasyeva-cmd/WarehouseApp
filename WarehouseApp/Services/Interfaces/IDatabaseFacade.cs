using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseApp.Services.Interfaces
{
    public interface IDatabaseFacade
    {
        IAuthService Auth { get; }
        IWarehouseService Warehouses { get; }
        IRentService Rents { get; }
        IVisitService Visits { get; }
    }
}