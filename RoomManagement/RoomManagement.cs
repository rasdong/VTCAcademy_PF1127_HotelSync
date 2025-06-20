using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace HotelManagementSystem
{
    public class RoomManagement
    {
        // Lấy danh sách tất cả phòng
        public List<Room> GetAllRooms()
        {
            var rooms = new List<Room>();
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("viewAllRooms", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32("RoomID"),
                            RoomNumber = reader.GetString("RoomNumber"),
                            RoomType = reader.GetString("RoomType"),
                            Price = reader.GetDecimal("Price"),
                            Status = reader.GetString("Status"),
                            Amenities = reader.GetString("Amenities"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader.GetInt32("UpdatedBy"),
                            UpdatedByUsername = reader["UpdatedByUsername"] == DBNull.Value ? null : reader.GetString("UpdatedByUsername")
                        });
                    }
                }
            }
            return rooms;
        }

        // Thêm phòng mới
        public void AddRoom(string roomNumber, string roomType, decimal price, string amenities, int updatedBy, string updatedByUsername)
        {
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("addRoom", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_RoomNumber", roomNumber);
                cmd.Parameters.AddWithValue("p_RoomType", roomType);
                cmd.Parameters.AddWithValue("p_Price", price);
                cmd.Parameters.AddWithValue("p_Amenities", amenities);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
        }

        // Xóa phòng (chỉ khi phòng trống)
        public void DeleteRoom(int roomId, int updatedBy, string updatedByUsername)
        {
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("deleteRoom", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_RoomID", roomId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
        }

        // Sửa thông tin phòng
        public void UpdateRoom(int roomId, string roomNumber, string roomType, decimal price, string status, string amenities, int updatedBy, string updatedByUsername)
        {
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("updateRoom", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_RoomID", roomId);
                cmd.Parameters.AddWithValue("p_RoomNumber", roomNumber);
                cmd.Parameters.AddWithValue("p_RoomType", roomType);
                cmd.Parameters.AddWithValue("p_Price", price);
                cmd.Parameters.AddWithValue("p_Status", status);
                cmd.Parameters.AddWithValue("p_Amenities", amenities);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();
            }
        }

        // Tìm kiếm phòng theo trạng thái, loại, giá
        public List<Room> SearchRooms(string? status, string? roomType, decimal? minPrice, decimal? maxPrice)
        {
            var rooms = new List<Room>();
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("searchRooms", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_RoomType", (object?)roomType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_Status", (object?)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MinPrice", (object?)minPrice ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_MaxPrice", (object?)maxPrice ?? DBNull.Value);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32("RoomID"),
                            RoomNumber = reader.GetString("RoomNumber"),
                            RoomType = reader.GetString("RoomType"),
                            Price = reader.GetDecimal("Price"),
                            Status = reader.GetString("Status"),
                            Amenities = reader.GetString("Amenities"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader.GetInt32("UpdatedBy"),
                            UpdatedByUsername = reader["UpdatedByUsername"] == DBNull.Value ? null : reader.GetString("UpdatedByUsername")
                        });
                    }
                }
            }
            return rooms;
        }

        // Kiểm tra tình trạng phòng theo thời gian thực
        public List<Room> CheckRoomAvailability(DateTime checkIn, DateTime checkOut)
        {
            var rooms = new List<Room>();
            using (var conn = DataHelper.Instance.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("findAvailableRooms", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_CheckInDate", checkIn);
                cmd.Parameters.AddWithValue("p_CheckOutDate", checkOut);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32("RoomID"),
                            RoomNumber = reader.GetString("RoomNumber"),
                            RoomType = reader.GetString("RoomType"),
                            Price = reader.GetDecimal("Price"),
                            Status = reader.GetString("Status")
                        });
                    }
                }
            }
            return rooms;
        }
    }
}
