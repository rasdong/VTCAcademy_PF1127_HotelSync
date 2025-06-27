using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace HotelManagementSystem
{
    public class RoomDAL
    {
        // 1. Thêm phòng mới (đã có trong mã gốc, chỉ thêm UpdatedBy, UpdatedByUsername)
        public void AddRoom(string roomNumber, string roomType, decimal price, string amenities, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra RoomNumber không trùng
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Rooms WHERE RoomNumber = @RoomNumber", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                    throw new Exception("Số phòng đã tồn tại.");

                // Gọi stored procedure addRoom
                MySqlCommand cmd = new MySqlCommand("addRoom", conn)
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
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Thêm phòng mới: {roomNumber}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi thêm phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 2. Xóa phòng
        public void DeleteRoom(int roomId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Gọi stored procedure deleteRoom
                MySqlCommand cmd = new MySqlCommand("deleteRoom", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_RoomID", roomId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Xóa phòng ID {roomId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi xóa phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 3. Sửa thông tin phòng
        public void UpdateRoom(int roomId, string roomNumber, string roomType, decimal price, string amenities, string status, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra RoomNumber không trùng (trừ phòng hiện tại)
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Rooms WHERE RoomNumber = @RoomNumber AND RoomID != @RoomID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                checkCmd.Parameters.AddWithValue("@RoomID", roomId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                    throw new Exception("Số phòng đã tồn tại.");

                // Gọi stored procedure updateRoom
                MySqlCommand cmd = new MySqlCommand("updateRoom", conn)
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

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Cập nhật phòng ID {roomId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi cập nhật phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 4. Dọn phòng
        public void CleanRoom(int roomId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();
        
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
                transaction?.Rollback();
                throw new Exception($"Lỗi khi dọn phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 5. Xem danh sách phòng (không cần transaction vì chỉ đọc)
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
                throw new Exception($"Lỗi khi lấy danh sách phòng: {ex.Message}");
            }
        }

        // 6. Tìm kiếm phòng (không cần transaction vì chỉ đọc)
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
                    cmd.Parameters.AddWithValue("p_Status", (object?)status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_RoomType", (object?)roomType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MinPrice", (object?)minPrice ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_MaxPrice", (object?)maxPrice ?? DBNull.Value);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm phòng: {ex.Message}");
            }
        }

        // 7. Kiểm tra tình trạng phòng (không cần transaction vì chỉ đọc)
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
                throw new Exception($"Lỗi khi kiểm tra tình trạng phòng: {ex.Message}");
            }
        }
    }
}