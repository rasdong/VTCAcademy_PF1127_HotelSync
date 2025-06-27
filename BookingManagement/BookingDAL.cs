using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace HotelManagementSystem
{
    public class BookingDAL
    {
        // 1. Đặt phòng
        public int CreateBooking(string IDCard, int roomId, DateTime checkInDate, DateTime checkOutDate, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            try
            {
                conn.Open();

                // Gọi stored procedure createBooking
                MySqlCommand cmd = new MySqlCommand("createBooking", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_IDCard", IDCard);
                cmd.Parameters.AddWithValue("p_RoomID", roomId);
                cmd.Parameters.AddWithValue("p_CheckInDate", checkInDate);
                cmd.Parameters.AddWithValue("p_CheckOutDate", checkOutDate);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                
                // Output parameter
                MySqlParameter bookingIdParam = new MySqlParameter("p_BookingID", MySqlDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(bookingIdParam);

                cmd.ExecuteNonQuery();

                int bookingId = Convert.ToInt32(bookingIdParam.Value);
                return bookingId;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi tạo đặt phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 2. Hủy đặt phòng
        public void CancelBooking(int bookingId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            try
            {
                conn.Open();

                // Gọi stored procedure cancelBooking
                MySqlCommand cmd = new MySqlCommand("cancelBooking", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi hủy đặt phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 3. Check-in
        public void CheckIn(int BookingId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            try
            {
                conn.Open();

                // Gọi stored procedure checkIn
                MySqlCommand cmd = new MySqlCommand("checkIn", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_BookingID", BookingId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi check-in: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 4. Check-out
        public void CheckOut(int bookingId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            try
            {
                conn.Open();

                // Gọi stored procedure checkOut
                MySqlCommand cmd = new MySqlCommand("checkOut", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi check-out: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 5. Gia hạn đặt phòng
        public void ExtendBooking(int bookingId, DateTime newEndDate, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            try
            {
                conn.Open();

                // Gọi stored procedure extendBooking
                MySqlCommand cmd = new MySqlCommand("extendBooking", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                cmd.Parameters.AddWithValue("p_NewEndDate", newEndDate);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi gia hạn đặt phòng: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

            // 6. Xem lịch sử đặt phòng
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
                        cmd.Parameters.AddWithValue("p_RoomID", roomId.HasValue ? (object)roomId.Value : DBNull.Value);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                    return dt;
                }
                catch (MySqlException ex)
                {
                    throw new Exception($"Lỗi khi lấy lịch sử đặt phòng: {ex.Message}");
                }
            }
        }
    }
