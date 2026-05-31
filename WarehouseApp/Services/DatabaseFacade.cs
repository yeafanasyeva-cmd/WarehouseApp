using WarehouseApp.Services.Interfaces;

namespace WarehouseApp.Services
{
    public class DatabaseFacade : IDatabaseFacade
    {
        private readonly string _connectionString = "Host=localhost;Database=WarehouseApp;Username=postgres;Password=ybrjif2006";

        public IAuthService Auth { get; private set; }
        public IWarehouseService Warehouses { get; private set; }
        public IRentService Rents { get; private set; }
        public IVisitService Visits { get; private set; }

        public DatabaseFacade()
        {
            Auth = new AuthService(_connectionString);
            Warehouses = new WarehouseService(_connectionString);
            Rents = new RentService(_connectionString);
            Visits = new VisitService(_connectionString);
        }

        public DatabaseFacade(
            IAuthService auth,
            IWarehouseService warehouses,
            IRentService rents,
            IVisitService visits)
        {
            Auth = auth;
            Warehouses = warehouses;
            Rents = rents;
            Visits = visits;
        }
    }
}