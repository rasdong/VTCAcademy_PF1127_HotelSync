
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace HotelManagementSystem
{
    public class DatabaseReportDataProvider : IReportDataProvider
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=hotel_management;Uid=root;Pwd=123321;";

        public string[,] GetReportData()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var reportData = new System.Collections.Generic.List<string[]>();

                
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM Bookings", connection))
                {
                    int totalBookings = Convert.ToInt32(command.ExecuteScalar());
                    reportData.Add(new[] { "Tổng đặt phòng", totalBookings.ToString() });
                }

                
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM Bookings WHERE Status = 'Paid'", connection))
                {
                    int paidBookings = Convert.ToInt32(command.ExecuteScalar());
                    reportData.Add(new[] { "Đã thanh toán", paidBookings.ToString() });
                }

                
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM Bookings WHERE Status = 'Unpaid'", connection))
                {
                    int unpaidBookings = Convert.ToInt32(command.ExecuteScalar());
                    reportData.Add(new[] { "Chưa thanh toán", unpaidBookings.ToString() });
                }

                
                using (var command = new MySqlCommand(
                    "SELECT SUM(TotalAmount) FROM Bookings WHERE Status = 'Paid'", connection))
                {
                    object result = command.ExecuteScalar();
                    decimal revenue = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    reportData.Add(new[] { "Doanh thu (VND)", $"{revenue:N0}" });
                }

                
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM Rooms WHERE Status = 'Available'", connection))
                {
                    int availableRooms = Convert.ToInt32(command.ExecuteScalar());
                    reportData.Add(new[] { "Phòng trống", availableRooms.ToString() });
                }

                
                string[,] reportArray = new string[reportData.Count, 2];
                for (int i = 0; i < reportData.Count; i++)
                {
                    reportArray[i, 0] = reportData[i][0];
                    reportArray[i, 1] = reportData[i][1];
                }

                return reportArray;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu báo cáo: {ex.Message}");
                return new string[,] { { "Lỗi", "Không thể lấy dữ liệu" } };
            }
        }
    }
}