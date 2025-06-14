using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HotelManagementSystem
{
    public class UserManager : IUserManager
    {
        private readonly DatabaseHelper dbHelper;

        public UserManager()
        {
            dbHelper = new DatabaseHelper();
        }

        public User? Login(string username, string password)
        {
            try
            {
                string query = "SELECT u.UserID, u.Username, u.Password, r.RoleName " +
                              "FROM Users u JOIN Roles r ON u.RoleID = r.RoleID " +
                              "WHERE u.Username = @Username AND u.Password = @Password";
                var parameters = new[]
                {
                    new MySqlParameter("@Username", username),
                    new MySqlParameter("@Password", password) 
                };
                using (MySqlConnection conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parameters);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User(
                                    reader.GetInt32("UserID"),
                                    reader.GetString("Username"),
                                    reader.GetString("Password"),
                                    reader.GetString("RoleName")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đăng nhập: " + ex.Message);
            }
            return null;
        }

        public bool Register(string username, string password, string role)
        {
            try
            {
                
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                var checkParam = new[] { new MySqlParameter("@Username", username) };
                if ((long)dbHelper.ExecuteScalar(checkQuery, checkParam) > 0)
                    return false;

                
                string roleQuery = "SELECT RoleID FROM Roles WHERE RoleName = @RoleName";
                var roleParam = new[] { new MySqlParameter("@RoleName", role) };
                object roleIdObj = dbHelper.ExecuteScalar(roleQuery, roleParam);
                if (roleIdObj == null)
                    return false;
                int roleId = Convert.ToInt32(roleIdObj);

                
                string insertQuery = "INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername) " +
                                    "VALUES (@Username, @Password, @RoleID, 1, 'system')";
                var insertParams = new[]
                {
                    new MySqlParameter("@Username", username),
                    new MySqlParameter("@Password", password), 
                    new MySqlParameter("@RoleID", roleId)
                };
                dbHelper.ExecuteNonQuery(insertQuery, insertParams);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đăng ký: " + ex.Message);
            }
        }

        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            try
            {
                string query = "SELECT u.UserID, u.Username, u.Password, r.RoleName " +
                              "FROM Users u JOIN Roles r ON u.RoleID = r.RoleID";
                using (MySqlConnection conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User(
                                    reader.GetInt32("UserID"),
                                    reader.GetString("Username"),
                                    reader.GetString("Password"),
                                    reader.GetString("RoleName")
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách người dùng: " + ex.Message);
            }
            return users;
        }
    }
}