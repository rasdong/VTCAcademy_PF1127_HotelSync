using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.Json;

namespace HotelManagementSystem
{
    public class BookingDAL
    {
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
                catch
                {
                    return false;
                }
            }
        }

        public int CreateBooking(string IDCard, int roomId, DateTime checkInDate, DateTime checkOutDate, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("createBookingWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_IDCard", IDCard);
                        cmd.Parameters.AddWithValue("p_RoomID", roomId);
                        cmd.Parameters.AddWithValue("p_CheckInDate", checkInDate);
                        cmd.Parameters.AddWithValue("p_CheckOutDate", checkOutDate);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.Parameters.Add(new MySqlParameter("p_BookingID", MySqlDbType.Int32) { Direction = ParameterDirection.Output });
                        cmd.ExecuteNonQuery();
                        int bookingId = Convert.ToInt32(cmd.Parameters["p_BookingID"].Value);

                        transaction.Commit();
                        return bookingId;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        public void CancelBooking(int bookingId, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("cancelBookingWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        public void CheckIn(int bookingId, string IDCard, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("checkInWithIDVerification", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                        cmd.Parameters.AddWithValue("p_IDCard", IDCard);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        public void CheckOut(int bookingId, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("checkOutWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        public void ExtendBooking(int bookingId, DateTime newCheckOutDate, int updatedBy, string updatedByUsername)
        {
            using (MySqlConnection conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("extendBookingWithTransaction", conn)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaction
                        };
                        cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                        cmd.Parameters.AddWithValue("p_NewCheckOutDate", newCheckOutDate);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                        cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        public DataTable GetBookingHistory(int customerId, int? roomId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("getBookingHistory", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("p_CustomerID", customerId);
                    cmd.Parameters.AddWithValue("p_RoomID", (object)roomId ?? DBNull.Value);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                if (ex.Message.Contains("Khách hàng không tồn tại"))
                    throw new Exception("Khách hàng không tồn tại.");
                if (ex.Message.Contains("Phòng không tồn tại"))
                    throw new Exception("Phòng không tồn tại.");
                throw new Exception($"Lỗi khi lấy lịch sử đặt phòng: {ex.Message}");
            }
        }

        public DataTable CheckBookingExists(int bookingId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("checkBookingExists", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GetAllBookings()
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("getAllBookings", conn)
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
                throw new Exception($"Lỗi khi lấy danh sách đặt phòng: {ex.Message}");
            }
        }
        
    }
}