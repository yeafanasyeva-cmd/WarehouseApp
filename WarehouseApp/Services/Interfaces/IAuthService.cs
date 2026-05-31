using WarehouseApp.Models;

namespace WarehouseApp.Services.Interfaces
{
    public interface IAuthService
    {
        User AuthenticateUser(string login, string password);
    }
}