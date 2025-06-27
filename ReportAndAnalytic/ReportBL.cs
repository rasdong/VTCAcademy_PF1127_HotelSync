using System;
using System.Data;

namespace HotelManagementSystem.ReportAndAnalytic
{
    public class ReportBL
    {
        private readonly ReportDAL _reportDAL;

        public ReportBL()
        {
            _reportDAL = new ReportDAL();
        }

        // Báo cáo doanh thu theo ngày
        public DataTable GetDailyRevenueReport(string dateInput)
        {
            try
            {
                if (string.IsNullOrEmpty(dateInput))
                    throw new ArgumentException("Ngày báo cáo không được để trống.");

                if (!DateTime.TryParseExact(dateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime reportDate))
                    throw new ArgumentException("Định dạng ngày không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy.");

                return _reportDAL.GetDailyRevenueReport(reportDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo doanh thu ngày: {ex.Message}");
            }
        }

        // Báo cáo doanh thu theo tháng
        public DataTable GetMonthlyRevenueReport(string monthInput)
        {
            try
            {
                if (string.IsNullOrEmpty(monthInput))
                    throw new ArgumentException("Tháng báo cáo không được để trống.");

                if (!DateTime.TryParseExact($"01/{monthInput}", "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime reportMonth))
                    throw new ArgumentException("Định dạng tháng không hợp lệ. Vui lòng nhập theo định dạng MM/yyyy.");

                return _reportDAL.GetMonthlyRevenueReport(reportMonth.Year, reportMonth.Month);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo doanh thu tháng: {ex.Message}");
            }
        }

        // Thống kê tỷ lệ lấp đầy phòng
        public DataTable GetOccupancyReport(string startDateInput, string endDateInput)
        {
            try
            {
                if (string.IsNullOrEmpty(startDateInput) || string.IsNullOrEmpty(endDateInput))
                    throw new ArgumentException("Ngày bắt đầu và kết thúc không được để trống.");

                if (!DateTime.TryParseExact(startDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime startDate))
                    throw new ArgumentException("Định dạng ngày bắt đầu không hợp lệ.");

                if (!DateTime.TryParseExact(endDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime endDate))
                    throw new ArgumentException("Định dạng ngày kết thúc không hợp lệ.");

                if (startDate > endDate)
                    throw new ArgumentException("Ngày bắt đầu không thể sau ngày kết thúc.");

                if ((endDate - startDate).TotalDays > 365)
                    throw new ArgumentException("Khoảng thời gian báo cáo không được vượt quá 1 năm.");

                return _reportDAL.GetOccupancyReport(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo tỷ lệ lấp đầy: {ex.Message}");
            }
        }

        // Báo cáo tình trạng phòng
        public DataTable GetRoomStatusReport()
        {
            try
            {
                return _reportDAL.GetRoomStatusReport();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo tình trạng phòng: {ex.Message}");
            }
        }

        // Báo cáo khách hàng theo quốc tịch
        public DataTable GetCustomerNationalityReport()
        {
            try
            {
                return _reportDAL.GetCustomerNationalityReport();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo khách hàng: {ex.Message}");
            }
        }

        // Báo cáo khách hàng VIP
        public DataTable GetVIPCustomerReport(string minBookingsInput = "5")
        {
            try
            {
                if (!int.TryParse(minBookingsInput, out int minBookings) || minBookings < 1)
                    minBookings = 5; // Default value

                return _reportDAL.GetVIPCustomerReport(minBookings);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo khách hàng VIP: {ex.Message}");
            }
        }

        // Báo cáo dịch vụ phổ biến
        public DataTable GetPopularServicesReport()
        {
            try
            {
                return _reportDAL.GetPopularServicesReport();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo dịch vụ: {ex.Message}");
            }
        }

        // Thống kê booking theo trạng thái
        public DataTable GetBookingStatusStatistics()
        {
            try
            {
                return _reportDAL.GetBookingStatusStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo thống kê booking: {ex.Message}");
            }
        }

        // Phân tích xu hướng đặt phòng
        public DataTable GetBookingTrendAnalysis(string monthsInput = "12")
        {
            try
            {
                if (!int.TryParse(monthsInput, out int months) || months < 1 || months > 60)
                    months = 12; // Default 12 months

                return _reportDAL.GetBookingTrendAnalysis(months);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi phân tích xu hướng: {ex.Message}");
            }
        }

        // Dashboard summary
        public DataTable GetDashboardSummary()
        {
            try
            {
                return _reportDAL.GetDashboardSummary();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy thông tin tổng quan: {ex.Message}");
            }
        }

        // Utility method để format số tiền
        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0") + " VND";
        }

        // Utility method để format phần trăm
        public static string FormatPercentage(decimal percentage)
        {
            return percentage.ToString("F1") + "%";
        }

        // Utility method để format ngày tháng
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }
    }
}
