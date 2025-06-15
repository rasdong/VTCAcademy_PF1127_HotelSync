using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HotelManagementSystem
{
    public class CustomerManagement
    {
        public void AddCustomer(string name, string idCard, string phone, string email, string nationality)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality)
                             VALUES (@name, @idCard, @phone, @email, @nationality)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@idCard", idCard);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@nationality", nationality);
            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Khách hàng đã được thêm.");
        }

        public void UpdateCustomer(int customerId, string name, string idCard, string phone, string email, string nationality)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"UPDATE Customers SET Name=@name, IDCard=@idCard, Phone=@phone,
                             Email=@email, Nationality=@nationality WHERE CustomerID=@customerId";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@idCard", idCard);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@nationality", nationality);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "✔ Cập nhật thành công." : "✘ Không tìm thấy khách hàng.");
        }

        public void DeleteCustomer(int customerId)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();

            string checkBooking = "SELECT COUNT(*) FROM Bookings WHERE CustomerID = @id";
            using var checkCmd = new MySqlCommand(checkBooking, conn);
            checkCmd.Parameters.AddWithValue("@id", customerId);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Console.WriteLine("✘ Không thể xóa khách có đặt phòng.");
                return;
            }

            string query = "DELETE FROM Customers WHERE CustomerID = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "✔ Đã xóa khách hàng." : "✘ Không tìm thấy khách hàng.");
        }

        public void SearchCustomer(string keyword)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"SELECT * FROM Customers WHERE Name LIKE @kw OR IDCard LIKE @kw OR Phone LIKE @kw";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"[{reader["CustomerID"]}] {reader["Name"]} | {reader["IDCard"]} | {reader["Phone"]} | {reader["Email"]} | {reader["Nationality"]}");
            }
        }

        public void ViewCustomerHistory(int customerId)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"SELECT b.BookingID, r.RoomNumber, b.CheckInDate, b.CheckOutDate, b.Status 
                             FROM Bookings b JOIN Rooms r ON b.RoomID = r.RoomID 
                             WHERE b.CustomerID = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Booking: {reader["BookingID"]}, Room: {reader["RoomNumber"]}, {reader["CheckInDate"]} to {reader["CheckOutDate"]}, Status: {reader["Status"]}");
            }
        }
    }
}
