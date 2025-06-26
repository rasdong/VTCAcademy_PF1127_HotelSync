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
            MySqlTransaction transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra phòng trống
                MySqlCommand checkCmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM Bookings WHERE RoomID = @RoomID AND Status = 'Active' " +
                    "AND (@CheckInDate < CheckOutDate AND @CheckOutDate > CheckInDate)", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@RoomID", roomId);
                checkCmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                checkCmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                    throw new Exception("Phòng đã được đặt trong khoảng thời gian này.");

                // Kiểm tra khách hàng tồn tại và lấy CustomerID
                checkCmd = new MySqlCommand("SELECT CustomerID FROM Customers WHERE IDCard = @IDCard", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@IDCard", IDCard);
                object customerIdResult = checkCmd.ExecuteScalar();
                if (customerIdResult == null)
                    throw new Exception("Khách hàng không tồn tại.");
                int customerId = Convert.ToInt32(customerIdResult);

                // Kiểm tra phòng tồn tại
                checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Rooms WHERE RoomID = @RoomID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@RoomID", roomId);
                count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Phòng không tồn tại.");

                // Chèn bản ghi vào Bookings
                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, UpdatedBy, UpdatedByUsername) " +
                    "VALUES (@CustomerID, @IDCard, @RoomID, @CheckInDate, @CheckOutDate, @UpdatedBy, @UpdatedByUsername); " +
                    "SELECT LAST_INSERT_ID();", conn)
                {
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@IDCard", IDCard);
                cmd.Parameters.AddWithValue("@RoomID", roomId);
                cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                int bookingId = Convert.ToInt32(cmd.ExecuteScalar());

                transaction.Commit();
                return bookingId;
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
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
            MySqlTransaction transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

              
                MySqlCommand checkCmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM Bookings WHERE BookingID = @BookingID AND Status = 'Active'", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@BookingID", bookingId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Đặt phòng không tồn tại hoặc không hợp lệ để hủy.");

                
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
            catch (Exception ex) 
            {
                transaction?.Rollback();
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
            MySqlTransaction transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();
        
                MySqlCommand checkCmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM Bookings WHERE BookingID = @BookingID AND Status = 'Active' AND CheckInDate <= NOW()", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@BookingID", BookingId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Đặt phòng không tồn tại hoặc không hợp lệ để check-in.");
        
                MySqlCommand cmd = new MySqlCommand("checkInWithTransaction", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_BookingID", BookingId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery(); 
        
                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
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
             MySqlTransaction transaction = null;
             try
             {
                 conn.Open();
                 transaction = conn.BeginTransaction();
         
                 
                 MySqlCommand checkCmd = new MySqlCommand(
                     "SELECT COUNT(*) FROM Bookings WHERE BookingID = @BookingID AND Status = 'Active'", conn)
                 {
                     Transaction = transaction
                 };
                 checkCmd.Parameters.AddWithValue("@BookingID", bookingId);
                 int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                 if (count == 0)
                     throw new Exception("Đặt phòng không tồn tại hoặc không hợp lệ để check-out.");
         
                 
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
                 transaction?.Rollback();
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
                MySqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                   
                    MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Bookings WHERE BookingID = @BookingID AND Status = 'Active'", conn)
                    {
                        Transaction = transaction
                    };
                    checkCmd.Parameters.AddWithValue("@BookingID", bookingId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count == 0)
                        throw new Exception("Đặt phòng không tồn tại hoặc không hợp lệ để gia hạn.");

                    
                    MySqlCommand cmd = new MySqlCommand("extendBookingWithTransaction", conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Transaction = transaction
                    };
                    cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                    cmd.Parameters.AddWithValue("p_NewCheckOutDate", newEndDate);
                    cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                    cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
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
                        cmd.Parameters.AddWithValue("p_RoomID", (object)roomId ?? DBNull.Value);
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
