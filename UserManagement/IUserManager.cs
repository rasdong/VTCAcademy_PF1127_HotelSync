
using System.Collections.Generic;

namespace HotelManagementSystem
{
    public interface IUserManager
    {
        User? Login(string username, string password);
        bool Register(string username, string password, string role);
        List<User> GetUsers();
    }
}