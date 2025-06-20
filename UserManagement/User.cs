using System;

namespace HotelManagementSystem.UserManagement
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User(int userId, string username, string password, string role)
        {
            UserID = userId;
            Username = username;
            Password = password;
            Role = role;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
