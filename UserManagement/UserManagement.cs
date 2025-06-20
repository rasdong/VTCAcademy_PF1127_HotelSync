using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagementSystem.UserManagement
{
    public class UserManagement
    {
        private List<User> users = new List<User>();

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public bool RemoveUser(int userId)
        {
            var user = users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                users.Remove(user);
                return true;
            }
            return false;
        }

        public User? GetUserById(int userId)
        {
            return users.FirstOrDefault(u => u.UserID == userId);
        }

        public User? GetUserByUsername(string username)
        {
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public List<User> GetAllUsers()
        {
            return new List<User>(users);
        }

        public bool UpdateUser(User updatedUser)
        {
            var user = users.FirstOrDefault(u => u.UserID == updatedUser.UserID);
            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Role = updatedUser.Role;
                user.UpdatedAt = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}
