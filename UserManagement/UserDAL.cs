using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class UserDAL
    {
        public DataTable GetAllUsers()
        {
            DataTable users = new DataTable();
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT u.UserID, u.Username, r.RoleName, 
                           u.CreatedAt, u.UpdatedAt 
                    FROM Users u 
                    JOIN Roles r ON u.RoleID = r.RoleID 
                    ORDER BY u.CreatedAt DESC";
                    
                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(users);
                }
            }
            return users;
        }

        public int AddUser(string username, string password, int roleId, string email, string fullName, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername) 
                    VALUES (@username, @password, @roleId, @updatedBy, @updatedByUsername);
                    SELECT LAST_INSERT_ID();";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@roleId", roleId);
                    command.Parameters.AddWithValue("@updatedBy", updatedBy);
                    command.Parameters.AddWithValue("@updatedByUsername", updatedByUsername);
                    
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateUser(int userId, string username, int roleId, string email, string fullName, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Users 
                    SET Username = @username, RoleID = @roleId,
                        UpdatedBy = @updatedBy, UpdatedByUsername = @updatedByUsername, UpdatedAt = NOW()
                    WHERE UserID = @userId";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@roleId", roleId);
                    command.Parameters.AddWithValue("@updatedBy", updatedBy);
                    command.Parameters.AddWithValue("@updatedByUsername", updatedByUsername);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int userId, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE UserID = @userId";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ChangePassword(int userId, string newPassword, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Users 
                    SET Password = @password, UpdatedBy = @updatedBy, UpdatedByUsername = @updatedByUsername, UpdatedAt = NOW()
                    WHERE UserID = @userId";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@password", newPassword);
                    command.Parameters.AddWithValue("@updatedBy", updatedBy);
                    command.Parameters.AddWithValue("@updatedByUsername", updatedByUsername);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ToggleUserStatus(int userId, bool isActive, int updatedBy, string updatedByUsername)
        {
            // Ghi chú: Bảng Users hiện tại không có cột IsActive
            // Method này tạm thời không thực hiện gì
            // TODO: Cần thêm cột IsActive vào bảng Users hoặc sử dụng cách khác để quản lý trạng thái
            throw new NotImplementedException("Chức năng bật/tắt trạng thái user chưa được hỗ trợ do bảng Users không có cột IsActive");
        }

        public bool CheckUserExists(string username)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public int GetRoleId(string roleName)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = "SELECT RoleID FROM Roles WHERE RoleName = @roleName";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roleName", roleName);
                    var result = command.ExecuteScalar();
                    if (result == null)
                        throw new ArgumentException($"Vai trò '{roleName}' không tồn tại");
                    return (int)result;
                }
            }
        }

        public (bool success, int userId, string role) Login(string username, string password)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT u.UserID, u.Username, r.RoleName 
                    FROM Users u 
                    JOIN Roles r ON u.RoleID = r.RoleID 
                    WHERE u.Username = @username AND u.Password = @password";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32("UserID");
                            string role = reader.GetString("RoleName");
                            return (true, userId, role);
                        }
                    }
                }
            }
            return (false, 0, "");
        }
    }
}