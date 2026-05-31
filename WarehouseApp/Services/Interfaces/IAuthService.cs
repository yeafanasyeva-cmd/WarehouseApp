using WarehouseApp.Models;

namespace WarehouseApp.Services.Interfaces
{
    public interface IAuthService
    {
        User AuthenticateUser(string login, string password);
        bool RegisterUser(string login, string password, string fullName, string company, string role);
        bool IsLoginExists(string login);
    }
}