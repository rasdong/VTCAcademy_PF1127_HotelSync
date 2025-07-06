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
                // Sử dụng stored procedure để lấy danh sách người dùng với ORDER BY UserID ASC
                using (var command = new MySqlCommand("getAllUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(users);
                    }
                }
            }
            return users;
        }

        public int AddUser(string username, string password, int roleId, string email, string fullName, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("addUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_Username", username);
                    command.Parameters.AddWithValue("p_Password", password);
                    command.Parameters.AddWithValue("p_RoleID", roleId);
                    command.Parameters.AddWithValue("p_Email", email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("p_FullName", fullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                    command.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                    
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateUser(int userId, string username, int roleId, string email, string fullName, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("updateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_UserID", userId);
                    command.Parameters.AddWithValue("p_Username", username);
                    command.Parameters.AddWithValue("p_RoleID", roleId);
                    command.Parameters.AddWithValue("p_Email", email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("p_FullName", fullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("p_IsActive", true); // Default to active
                    command.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                    command.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int userId, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("deleteUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_UserID", userId);
                    command.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                    command.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ChangePassword(int userId, string newPassword, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("changePassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_UserID", userId);
                    command.Parameters.AddWithValue("p_NewPassword", newPassword);
                    command.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                    command.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ToggleUserStatus(int userId, bool isActive, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("toggleUserStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_UserID", userId);
                    command.Parameters.AddWithValue("p_IsActive", isActive);
                    command.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                    command.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool CheckUserExists(string username)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("checkUserExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_Username", username);
                    
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public int GetRoleId(string roleName)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("getRoleId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_RoleName", roleName);
                    
                    var result = command.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                        throw new ArgumentException($"Vai trò '{roleName}' không tồn tại");
                    return Convert.ToInt32(result);
                }
            }
        }

        public (bool success, int userId, string role) Login(string username, string password)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand("authenticateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_Username", username);
                    command.Parameters.AddWithValue("p_Password", password);
                    
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

        public bool UserExists(int userId)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE UserID = @UserId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public void UpdateUserStatus(int userId, bool isActive, int updatedBy, string updatedByUsername)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Users 
                    SET IsActive = @IsActive, 
                        UpdatedAt = NOW(),
                        UpdatedBy = @UpdatedBy
                    WHERE UserID = @UserId";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsActive", isActive);
                    command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new Exception("Không thể cập nhật trạng thái người dùng.");
                }
            }
        }
    }
}