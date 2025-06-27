using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem.ReportAndAnalytic
{
    public class ReportDAL
    {
        private readonly string _connectionString;

        public ReportDAL()
        {
            _connectionString = "Server=localhost;Database=hotel_management;Uid=root;Pwd=123321;";
        }

        // Báo cáo doanh thu theo ngày
        public DataTable GetDailyRevenueReport(DateTime reportDate)
        {
            string query = @"
                SELECT 
                    DATE(i.IssueDate) as ReportDate,
                    COUNT(DISTINCT i.BookingID) as TotalBookings,
                    SUM(i.TotalAmount) as BookingRevenue,
                    COUNT(DISTINCT su.UsageID) as ServiceUsages,
                    IFNULL(SUM(su.TotalPrice), 0) as ServiceRevenue,
                    (SUM(i.TotalAmount) + IFNULL(SUM(su.TotalPrice), 0)) as TotalRevenue
                FROM Invoices i
                LEFT JOIN ServiceUsage su ON DATE(su.Date) = DATE(i.IssueDate)
                WHERE DATE(i.IssueDate) = @ReportDate
                GROUP BY DATE(i.IssueDate)";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportDate", reportDate.Date);
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Báo cáo doanh thu theo tháng
        public DataTable GetMonthlyRevenueReport(int year, int month)
        {
            string query = @"
                SELECT 
                    WEEK(i.IssueDate) - WEEK(DATE_SUB(i.IssueDate, INTERVAL DAYOFMONTH(i.IssueDate) - 1 DAY)) + 1 as WeekNumber,
                    COUNT(DISTINCT i.BookingID) as TotalBookings,
                    SUM(i.TotalAmount) as BookingRevenue,
                    IFNULL(SUM(su.TotalPrice), 0) as ServiceRevenue,
                    (SUM(i.TotalAmount) + IFNULL(SUM(su.TotalPrice), 0)) as TotalRevenue
                FROM Invoices i
                LEFT JOIN ServiceUsage su ON YEAR(su.Date) = YEAR(i.IssueDate) 
                    AND MONTH(su.Date) = MONTH(i.IssueDate)
                    AND WEEK(su.Date) = WEEK(i.IssueDate)
                WHERE YEAR(i.IssueDate) = @Year AND MONTH(i.IssueDate) = @Month
                GROUP BY WeekNumber
                ORDER BY WeekNumber";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Month", month);
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Thống kê tỷ lệ lấp đầy phòng
        public DataTable GetOccupancyReport(DateTime startDate, DateTime endDate)
        {
            string query = @"
                SELECT 
                    DATE(b.CheckInDate) as Date,
                    COUNT(DISTINCT r.RoomID) as TotalRooms,
                    COUNT(DISTINCT CASE WHEN b.Status = 'Active' THEN b.RoomID END) as OccupiedRooms,
                    ROUND((COUNT(DISTINCT CASE WHEN b.Status = 'Active' THEN b.RoomID END) * 100.0 / COUNT(DISTINCT r.RoomID)), 2) as OccupancyRate
                FROM Rooms r
                LEFT JOIN Bookings b ON r.RoomID = b.RoomID 
                    AND DATE(b.CheckInDate) BETWEEN @StartDate AND @EndDate
                WHERE DATE(b.CheckInDate) BETWEEN @StartDate AND @EndDate
                GROUP BY DATE(b.CheckInDate)
                ORDER BY Date";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Báo cáo tình trạng phòng
        public DataTable GetRoomStatusReport()
        {
            string query = @"
                SELECT 
                    r.Status,
                    r.RoomType,
                    COUNT(*) as RoomCount,
                    ROUND((COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Rooms)), 2) as Percentage
                FROM Rooms r
                GROUP BY r.Status, r.RoomType
                ORDER BY r.Status, r.RoomType";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Báo cáo khách hàng theo quốc tịch
        public DataTable GetCustomerNationalityReport()
        {
            string query = @"
                SELECT 
                    c.Nationality,
                    COUNT(DISTINCT c.CustomerID) as CustomerCount,
                    COUNT(b.BookingID) as TotalBookings,
                    ROUND(AVG(DATEDIFF(b.CheckOutDate, b.CheckInDate)), 1) as AvgStayDuration,
                    ROUND((COUNT(DISTINCT c.CustomerID) * 100.0 / (SELECT COUNT(*) FROM Customers)), 2) as Percentage
                FROM Customers c
                LEFT JOIN Bookings b ON c.CustomerID = b.CustomerID
                GROUP BY c.Nationality
                ORDER BY CustomerCount DESC";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Báo cáo khách hàng VIP (theo số lần đặt phòng)
        public DataTable GetVIPCustomerReport(int minBookings = 5)
        {
            string query = @"
                SELECT 
                    c.Name,
                    c.Nationality,
                    COUNT(b.BookingID) as TotalBookings,
                    SUM(i.TotalAmount) as TotalSpent,
                    MAX(b.CheckInDate) as LastVisit,
                    CASE 
                        WHEN COUNT(b.BookingID) >= 20 THEN 'Platinum'
                        WHEN COUNT(b.BookingID) >= 10 THEN 'Gold'
                        WHEN COUNT(b.BookingID) >= 5 THEN 'Silver'
                        ELSE 'Bronze'
                    END as VIPLevel
                FROM Customers c
                INNER JOIN Bookings b ON c.CustomerID = b.CustomerID
                LEFT JOIN Invoices i ON b.BookingID = i.BookingID
                GROUP BY c.CustomerID, c.Name, c.Nationality
                HAVING COUNT(b.BookingID) >= @MinBookings
                ORDER BY TotalBookings DESC, TotalSpent DESC
                LIMIT 20";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MinBookings", minBookings);
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Báo cáo dịch vụ phổ biến
        public DataTable GetPopularServicesReport()
        {
            string query = @"
                SELECT 
                    s.ServiceName,
                    s.Type,
                    s.Price as UnitPrice,
                    COUNT(su.UsageID) as UsageCount,
                    SUM(su.Quantity) as TotalQuantity,
                    SUM(su.TotalPrice) as TotalRevenue,
                    ROUND((COUNT(su.UsageID) * 100.0 / (SELECT COUNT(*) FROM ServiceUsage)), 2) as UsagePercentage
                FROM Services s
                INNER JOIN ServiceUsage su ON s.ServiceID = su.ServiceID
                GROUP BY s.ServiceID, s.ServiceName, s.Type, s.Price
                ORDER BY TotalRevenue DESC, UsageCount DESC
                LIMIT 15";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Thống kê booking theo trạng thái
        public DataTable GetBookingStatusStatistics()
        {
            string query = @"
                SELECT 
                    Status,
                    COUNT(*) as BookingCount,
                    ROUND((COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Bookings)), 2) as Percentage,
                    AVG(DATEDIFF(CheckOutDate, CheckInDate)) as AvgDuration
                FROM Bookings
                GROUP BY Status
                ORDER BY BookingCount DESC";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Phân tích xu hướng đặt phòng theo tháng
        public DataTable GetBookingTrendAnalysis(int months = 12)
        {
            string query = @"
                SELECT 
                    YEAR(CheckInDate) as Year,
                    MONTH(CheckInDate) as Month,
                    MONTHNAME(CheckInDate) as MonthName,
                    COUNT(*) as BookingCount,
                    COUNT(DISTINCT CustomerID) as UniqueCustomers,
                    AVG(DATEDIFF(CheckOutDate, CheckInDate)) as AvgStayDuration
                FROM Bookings
                WHERE CheckInDate >= DATE_SUB(CURDATE(), INTERVAL @Months MONTH)
                GROUP BY YEAR(CheckInDate), MONTH(CheckInDate)
                ORDER BY Year DESC, Month DESC";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Months", months);
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Dashboard summary
        public DataTable GetDashboardSummary()
        {
            string query = @"
                SELECT 
                    'TotalRooms' as Metric, COUNT(*) as Value FROM Rooms
                UNION ALL
                SELECT 
                    'AvailableRooms' as Metric, COUNT(*) as Value FROM Rooms WHERE Status = 'Available'
                UNION ALL
                SELECT 
                    'OccupiedRooms' as Metric, COUNT(*) as Value FROM Rooms WHERE Status = 'Occupied'
                UNION ALL
                SELECT 
                    'TotalCustomers' as Metric, COUNT(*) as Value FROM Customers
                UNION ALL
                SELECT 
                    'ActiveBookings' as Metric, COUNT(*) as Value FROM Bookings WHERE Status = 'Active'
                UNION ALL
                SELECT 
                    'TotalStaff' as Metric, COUNT(*) as Value FROM Staff
                UNION ALL
                SELECT 
                    'TodayRevenue' as Metric, IFNULL(SUM(TotalAmount), 0) as Value 
                    FROM Invoices WHERE DATE(IssueDate) = CURDATE()
                UNION ALL
                SELECT 
                    'MonthlyRevenue' as Metric, IFNULL(SUM(TotalAmount), 0) as Value 
                    FROM Invoices WHERE YEAR(IssueDate) = YEAR(CURDATE()) AND MONTH(IssueDate) = MONTH(CURDATE())";

            using (var connection = new MySqlConnection(_connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
    }
}
