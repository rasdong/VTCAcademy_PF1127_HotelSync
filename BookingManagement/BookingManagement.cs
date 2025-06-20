using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace HotelManagementSystem
{
    public class BookingManagement
    {
        // Lấy danh sách tất cả đặt phòng
        public List<Booking> GetAllBookings()
        {
            var bookings = new List<Booking>();
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("viewAllBookings", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookings.Add(new Booking
                        {
                            BookingID = reader.GetInt32("BookingID"),
                            CustomerID = reader.GetInt32("CustomerID"),
                            RoomID = reader.GetInt32("RoomID"),
                            CheckInDate = reader.GetDateTime("CheckInDate"),
                            CheckOutDate = reader.GetDateTime("CheckOutDate"),
                            Status = reader.GetString("Status"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader.GetInt32("UpdatedBy"),
                            UpdatedByUsername = reader["UpdatedByUsername"] == DBNull.Value ? null : reader.GetString("UpdatedByUsername")
                        });
                    }
                }
            }
            return bookings;
        }

        // Thêm đặt phòng mới
        public void AddBooking(int customerId, int roomId, DateTime checkIn, DateTime checkOut, int updatedBy, string updatedByUsername)
        {
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("addBookingWithTransaction", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_CustomerID", customerId);
                cmd.Parameters.AddWithValue("p_RoomID", roomId);
                cmd.Parameters.AddWithValue("p_CheckInDate", checkIn);
                cmd.Parameters.AddWithValue("p_CheckOutDate", checkOut);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
        }

        // Sửa đặt phòng
        public void UpdateBooking(int bookingId, int roomId, DateTime checkIn, DateTime checkOut, string status, int updatedBy, string updatedByUsername)
        {
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("updateBookingStatus", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                cmd.Parameters.AddWithValue("p_Status", status);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
        }

        // Xóa đặt phòng (hủy)
        public void CancelBooking(int bookingId, string reason, int updatedBy, string updatedByUsername)
        {
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("cancelBooking", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                cmd.Parameters.AddWithValue("p_CancellationReason", reason);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
        }

        // Tìm kiếm đặt phòng theo trạng thái
        public List<Booking> SearchBookings(string? status)
        {
            var bookings = new List<Booking>();
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("viewAllBookings", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(status) || reader.GetString("Status") == status)
                        {
                            bookings.Add(new Booking
                            {
                                BookingID = reader.GetInt32("BookingID"),
                                CustomerID = reader.GetInt32("CustomerID"),
                                RoomID = reader.GetInt32("RoomID"),
                                CheckInDate = reader.GetDateTime("CheckInDate"),
                                CheckOutDate = reader.GetDateTime("CheckOutDate"),
                                Status = reader.GetString("Status"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.GetDateTime("UpdatedAt"),
                                UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader.GetInt32("UpdatedBy"),
                                UpdatedByUsername = reader["UpdatedByUsername"] == DBNull.Value ? null : reader.GetString("UpdatedByUsername")
                            });
                        }
                    }
                }
            }
            return bookings;
        }
    }
}
