using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.Json;

namespace HotelManagementSystem
{
    public class RoomDAL
    {
        // Kiểm tra quyền người dùng
        public bool CheckUserPermission(int updatedBy, string requiredPermission)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT r.Permissions FROM Users u JOIN Roles r ON u.RoleID = r.RoleID WHERE u.UserID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@UserID", updatedBy);
                    string permissionsJson = cmd.ExecuteScalar()?.ToString();
                    if (string.IsNullOrEmpty(permissionsJson))
                        return false;

                    var permissions = JsonSerializer.Deserialize<string[]>(permissionsJson);
                    return permissions != null && permissions.Contains(requiredPermission);
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        // 1. Thêm phòng mới
        public int AddRoom(string roomNumber, string roomType, decimal price, string amenities, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("addRoomWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
        
                        cmd.Parameters.AddWithValue("p_RoomNumber", roomNumber);
                        cmd.Parameters.AddWithValue("p_RoomType", roomType);
                        cmd.Parameters.AddWithValue("p_Price", price);
                        cmd.Parameters.AddWithValue("p_Amenities", amenities);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
        
                        var outputParam = new MySqlParameter("p_RoomID", MySqlDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);
        
                        cmd.ExecuteNonQuery();
                        int roomId = Convert.ToInt32(outputParam.Value);
        
                        transaction.Commit();
                        return roomId;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
        }

        // 2. Xóa phòng
        public void DeleteRoom(int roomId, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra quyền trước khi xóa
                        if (!CheckUserPermission(updatedBy, "manage_rooms"))
                        {
                            throw new Exception("Người dùng không có quyền xóa phòng.");
                        }

                        MySqlCommand cmd = new MySqlCommand("deleteRoomWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_RoomID", roomId);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        // Truyền trực tiếp thông báo lỗi từ SQL
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
        }

        // 3. Sửa thông tin phòng
        public void UpdateRoom(int roomId, string roomNumber, string roomType, decimal price, string amenities, string status, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("updateRoomWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_RoomID", roomId);
                        cmd.Parameters.AddWithValue("p_RoomNumber", roomNumber);
                        cmd.Parameters.AddWithValue("p_RoomType", roomType);
                        cmd.Parameters.AddWithValue("p_Price", price);
                        cmd.Parameters.AddWithValue("p_Amenities", amenities);
                        cmd.Parameters.AddWithValue("p_Status", status);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
        }

        // 4. Dọn phòng
        public void CleanRoom(int roomId, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("cleanRoomWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_RoomID", roomId);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
        }

        // 5. Xem danh sách phòng
        public DataTable GetAllRooms()
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("getAllRooms", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // 6. Tìm kiếm phòng
        public DataTable SearchRooms(string status, string roomType, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("searchRooms", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("p_Status", (object)status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_RoomType", (object)roomType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MinPrice", (object)minPrice ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MaxPrice", (object)maxPrice ?? DBNull.Value);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // 7. Kiểm tra tình trạng phòng
        public DataTable CheckRoomAvailability(DateTime startDate, DateTime endDate)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("checkRoomAvailability", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("p_StartDate", startDate);
                    cmd.Parameters.AddWithValue("p_EndDate", endDate);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}