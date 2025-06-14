using MySql.Data.MySqlClient;
using System;

namespace HotelManagementSystem
{
    public class StaticReportDataProvider : IReportDataProvider
    {
        private readonly DatabaseHelper dbHelper;

        public StaticReportDataProvider()
        {
            dbHelper = new DatabaseHelper();
        }

        public string[,] GetReportData()
        {
            string[,] reportData = new string[5, 2];
            try
            {
                
                reportData[0, 0] = "Tổng đặt phòng";
                object? totalBookings = dbHelper.ExecuteScalar("SELECT COUNT(*) FROM Bookings WHERE Status = 'Active'");
                reportData[0, 1] = totalBookings != null ? totalBookings.ToString() : "0";

                
                reportData[1, 0] = "Đã thanh toán";
                object? paidCount = dbHelper.ExecuteScalar("SELECT COUNT(*) FROM ServiceUsage WHERE PaymentStatus = 'Paid'");
                reportData[1, 1] = paidCount != null ? paidCount.ToString() : "0";

                
                reportData[2, 0] = "Chưa thanh toán";
                object? unpaidCount = dbHelper.ExecuteScalar("SELECT COUNT(*) FROM ServiceUsage WHERE PaymentStatus = 'Unpaid'");
                reportData[2, 1] = unpaidCount != null ? unpaidCount.ToString() : "0";

                
                reportData[3, 0] = "Doanh thu (VND)";
                object? revenue = dbHelper.ExecuteScalar("SELECT COALESCE(SUM(TotalPrice), 0) FROM ServiceUsage WHERE PaymentStatus = 'Paid'");
                reportData[3, 1] = revenue != null ? string.Format("{0:N0}", revenue) : "0";

                
                reportData[4, 0] = "Phòng trống";
                object? availableRooms = dbHelper.ExecuteScalar("SELECT COUNT(*) FROM Rooms WHERE Status = 'Available'");
                reportData[4, 1] = availableRooms != null ? availableRooms.ToString() : "0";
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu báo cáo: " + ex.Message);
            }
            return reportData;
        }
    }
}