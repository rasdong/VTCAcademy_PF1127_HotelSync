using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class ReportingAndAnalytics
    {
        private readonly string connectionString;
        public ReportingAndAnalytics(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Báo cáo doanh thu theo ngày/tuần/tháng
        public decimal GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            decimal total = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT SUM(TotalAmount) FROM Invoices WHERE IssueDate BETWEEN @start AND @end AND PaymentStatus = 'Paid'";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                        total = Convert.ToDecimal(result);
                }
            }
            return total;
        }

        // Thống kê tỷ lệ lấp đầy phòng
        public double GetRoomOccupancyRate(DateTime startDate, DateTime endDate)
        {
            double occupancyRate = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string totalRoomsQuery = "SELECT COUNT(*) FROM Rooms";
                int totalRooms = Convert.ToInt32(new MySqlCommand(totalRoomsQuery, connection).ExecuteScalar());
                if (totalRooms == 0) return 0;

                string occupiedRoomDaysQuery = @"SELECT SUM(DATEDIFF(LEAST(CheckOutDate, @end), GREATEST(CheckInDate, @start)) + 1) FROM Bookings WHERE Status != 'Cancelled' AND CheckInDate <= @end AND CheckOutDate >= @start";
                using (var cmd = new MySqlCommand(occupiedRoomDaysQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    var occupiedRoomDays = cmd.ExecuteScalar();
                    int totalDays = (endDate - startDate).Days + 1;
                    if (occupiedRoomDays != DBNull.Value && occupiedRoomDays != null && totalDays > 0)
                    {
                        occupancyRate = Convert.ToDouble(occupiedRoomDays) / (totalRooms * totalDays) * 100.0;
                    }
                }
            }
            return occupancyRate;
        }

        // Báo cáo khách hàng theo quốc tịch và tần suất quay lại
        public List<(string Nationality, int CustomerCount, int RepeatCustomers)> GetCustomerReport(DateTime startDate, DateTime endDate)
        {
            var result = new List<(string, int, int)>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT c.Nationality, COUNT(DISTINCT c.CustomerID) AS CustomerCount, SUM(CASE WHEN b.BookingID IS NOT NULL THEN 1 ELSE 0 END) AS RepeatCustomers FROM Customers c LEFT JOIN Bookings b ON c.CustomerID = b.CustomerID AND b.CreatedAt BETWEEN @start AND @end GROUP BY c.Nationality";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nationality = reader.GetString("Nationality");
                            int customerCount = reader.GetInt32("CustomerCount");
                            int repeat = reader.GetInt32("RepeatCustomers");
                            result.Add((nationality, customerCount, repeat));
                        }
                    }
                }
            }
            return result;
        }

        // Báo cáo dịch vụ: dịch vụ sử dụng nhiều nhất, doanh thu cao nhất
        public (string ServiceName, int TimesUsed, decimal TotalRevenue) GetTopService(DateTime startDate, DateTime endDate)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT s.ServiceName, COUNT(su.UsageID) AS TimesUsed, SUM(su.TotalPrice) AS TotalRevenue FROM Services s LEFT JOIN ServiceUsage su ON s.ServiceID = su.ServiceID AND su.Date BETWEEN @start AND @end GROUP BY s.ServiceID, s.ServiceName ORDER BY TotalRevenue DESC, TimesUsed DESC LIMIT 1";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader.GetString("ServiceName");
                            int times = reader.IsDBNull(reader.GetOrdinal("TimesUsed")) ? 0 : reader.GetInt32("TimesUsed");
                            decimal revenue = reader.IsDBNull(reader.GetOrdinal("TotalRevenue")) ? 0 : reader.GetDecimal("TotalRevenue");
                            return (name, times, revenue);
                        }
                    }
                }
            }
            return ("", 0, 0);
        }
    }
}
