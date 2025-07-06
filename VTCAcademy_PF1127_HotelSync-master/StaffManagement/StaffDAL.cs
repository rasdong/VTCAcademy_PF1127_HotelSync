using MySql.Data.MySqlClient;
using System;
using System.Data;
using HotelManagementSystem;

namespace HotelManagementSystem
{
    public class StaffDAL
    {
        // 1. Thêm nhân viên mới
        public void AddStaff(string name, string role, string phone, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Gọi stored procedure addStaff
                MySqlCommand cmd = new MySqlCommand("addStaff", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_Name", name);
                cmd.Parameters.AddWithValue("p_Role", role);
                cmd.Parameters.AddWithValue("p_Phone", phone);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Thêm nhân viên mới: {name}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi thêm nhân viên: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 2. Cập nhật thông tin nhân viên
        public void UpdateStaff(int staffId, string name, string role, string phone, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra nhân viên tồn tại
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Staff WHERE StaffID = @StaffID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@StaffID", staffId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Nhân viên không tồn tại.");

                // Gọi stored procedure updateStaff
                MySqlCommand cmd = new MySqlCommand("updateStaff", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_StaffID", staffId);
                cmd.Parameters.AddWithValue("p_Name", name);
                cmd.Parameters.AddWithValue("p_Role", role);
                cmd.Parameters.AddWithValue("p_Phone", phone);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Cập nhật nhân viên ID {staffId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi cập nhật nhân viên: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 3. Xóa nhân viên
        public void DeleteStaff(int staffId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra nhân viên tồn tại
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Staff WHERE StaffID = @StaffID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@StaffID", staffId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Nhân viên không tồn tại.");

                // Gọi stored procedure deleteStaff
                MySqlCommand cmd = new MySqlCommand("deleteStaff", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_StaffID", staffId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Xóa nhân viên ID {staffId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 4. Xem danh sách nhân viên
        public DataTable GetAllStaff()
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("getAllStaff", conn)
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
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}");
            }
        }

        // 5. Tìm kiếm nhân viên theo vai trò
        public DataTable SearchStaffByRole(string role)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT StaffID, Name, Role, Phone, CreatedAt, UpdatedAt FROM Staff WHERE Role = @Role", conn);
                    cmd.Parameters.AddWithValue("@Role", role);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm nhân viên: {ex.Message}");
            }
        }

        // 6. Gán nhiệm vụ cho nhân viên
        public void AssignStaffToTask(int staffId, string taskType, int? roomId, int? bookingId, int assignedBy, string assignedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra nhân viên tồn tại
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Staff WHERE StaffID = @StaffID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@StaffID", staffId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Nhân viên không tồn tại.");

                // Gọi stored procedure assignStaffToTask
                MySqlCommand cmd = new MySqlCommand("assignStaffToTask", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_StaffID", staffId);
                cmd.Parameters.AddWithValue("p_TaskType", taskType);
                cmd.Parameters.AddWithValue("p_RoomID", roomId.HasValue ? (object)roomId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("p_BookingID", bookingId.HasValue ? (object)bookingId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("p_AssignedBy", assignedBy);
                cmd.Parameters.AddWithValue("p_AssignedByUsername", assignedByUsername);
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi gán nhiệm vụ cho nhân viên: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}