using System;
using System.Data;
using HotelManagementSystem.ReportAndAnalytic;

namespace HotelManagementSystem.ConsoleUI
{
    public class ReportAndAnalyticsUI : BaseUI
    {
        private readonly ReportBL _reportBL;

        public ReportAndAnalyticsUI(string? username = null, string? role = null, int? userId = null) : base(username, role, userId)
        {
            _reportBL = new ReportBL();
        }

        public void ShowReportAndAnalytics()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Báo Cáo & Phân Tích");
                SetupBox(80, 22);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                if (!string.IsNullOrEmpty(currentUsername))
                {
                    Console.Write($" | Người dùng: {currentUsername}");
                    if (!string.IsNullOrEmpty(currentRole))
                    {
                        Console.Write($" ({currentRole})");
                    }
                }
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Báo cáo doanh thu theo ngày");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Báo cáo doanh thu theo tuần");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Báo cáo doanh thu theo tháng");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Báo cáo tỷ lệ lấp đầy phòng");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Báo cáo khách hàng theo quốc tịch");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Báo cáo khách hàng VIP");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("7. Báo cáo dịch vụ phổ biến");
                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 18);
                Console.Write("8. Quay lại");
                Console.SetCursorPosition(x + 2, y + 19);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 20);
                Console.Write("0. Thoát");

                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.Write("Lựa chọn của bạn: ");
                string? choice = ReadInputWithEscape(x + 20, y + height - 2);
                if (choice == null)
                    return;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            DailyRevenueReport();
                            break;
                        case "2":
                            WeeklyRevenueReport();
                            break;
                        case "3":
                            MonthlyRevenueReport();
                            break;
                        case "4":
                            OccupancyRateReport();
                            break;
                        case "5":
                            CustomerNationalityReport();
                            break;
                        case "6":
                            VIPCustomerReport();
                            break;
                        case "7":
                            PopularServicesReport();
                            break;
                        case "8":
                            return;
                        case "0":
                            Console.Clear();
                            Environment.Exit(0);
                            break;
                        default:
                            ShowErrorMessage("Lựa chọn không hợp lệ! Nhấn phím bất kỳ để thử lại...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Lỗi: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        private void DailyRevenueReport()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Báo Cáo Doanh Thu Theo Ngày");
            SetupBox(80, 18);

            ShowDateTimeInfo();
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Ngày báo cáo (dd/MM/yyyy): ");
            string? reportDate = ReadInputWithEscape(x + 28, y + 6);
            if (reportDate == null) return;

            try
            {
                var reportData = _reportBL.GetDailyRevenueReport(reportDate);
                
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kết Quả Báo Cáo Doanh Thu");
                SetupBox(100, 25);

                ShowDateTimeInfo();
                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 5);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- BÁO CÁO DOANH THU NGÀY {reportDate} ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 5);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- BÁO CÁO DOANH THU NGÀY {reportDate} ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write(new string('─', width - 4));

                if (reportData.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 8);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Không có dữ liệu cho ngày này.");
                    Console.ResetColor();
                }
                else
                {
                    var row = reportData.Rows[0];
                    
                    Console.SetCursorPosition(x + 2, y + 8);
                    Console.Write("1. Doanh thu từ đặt phòng:");
                    Console.SetCursorPosition(x + 4, y + 9);
                    Console.Write($"- Số booking: {row["TotalBookings"]}");
                    Console.SetCursorPosition(x + 4, y + 10);
                    Console.Write($"- Tổng tiền: {ReportBL.FormatCurrency(Convert.ToDecimal(row["BookingRevenue"]))}");
                    
                    Console.SetCursorPosition(x + 2, y + 12);
                    Console.Write("2. Doanh thu từ dịch vụ:");
                    Console.SetCursorPosition(x + 4, y + 13);
                    Console.Write($"- Số dịch vụ sử dụng: {row["ServiceUsages"]}");
                    Console.SetCursorPosition(x + 4, y + 14);
                    Console.Write($"- Tổng tiền: {ReportBL.FormatCurrency(Convert.ToDecimal(row["ServiceRevenue"]))}");
                    
                    Console.SetCursorPosition(x + 2, y + 16);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"3. Tổng doanh thu: {ReportBL.FormatCurrency(Convert.ToDecimal(row["TotalRevenue"]))}");
                    Console.ResetColor();
                }

                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void MonthlyRevenueReport()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Báo Cáo Doanh Thu Theo Tháng");
            SetupBox(80, 18);

            ShowDateTimeInfo();
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Tháng (MM/yyyy): ");
            string? reportMonth = ReadInputWithEscape(x + 18, y + 6);
            if (reportMonth == null) return;

            try
            {
                var reportData = _reportBL.GetMonthlyRevenueReport(reportMonth);
                
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kết Quả Báo Cáo Doanh Thu");
                SetupBox(100, 25);

                ShowDateTimeInfo();
                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 5);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- BÁO CÁO DOANH THU THÁNG {reportMonth} ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write(new string('─', width - 4));

                if (reportData.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 8);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Không có dữ liệu cho tháng này.");
                    Console.ResetColor();
                }
                else
                {
                    decimal totalRevenue = 0;
                    int totalBookings = 0;

                    Console.SetCursorPosition(x + 2, y + 8);
                    Console.Write("Doanh thu theo tuần:");
                    
                    int currentLine = 9;
                    foreach (DataRow row in reportData.Rows)
                    {
                        Console.SetCursorPosition(x + 4, y + currentLine);
                        Console.Write($"- Tuần {row["WeekNumber"]}: {ReportBL.FormatCurrency(Convert.ToDecimal(row["TotalRevenue"]))} ({row["TotalBookings"]} booking)");
                        totalRevenue += Convert.ToDecimal(row["TotalRevenue"]);
                        totalBookings += Convert.ToInt32(row["TotalBookings"]);
                        currentLine++;
                    }
                    
                    Console.SetCursorPosition(x + 2, y + currentLine + 1);
                    Console.Write("Tổng quan tháng:");
                    Console.SetCursorPosition(x + 4, y + currentLine + 2);
                    Console.Write($"- Tổng số booking: {totalBookings}");
                    Console.SetCursorPosition(x + 4, y + currentLine + 3);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"- Tổng doanh thu: {ReportBL.FormatCurrency(totalRevenue)}");
                    Console.ResetColor();
                }

                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }



        private void VIPCustomerReport()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Báo Cáo Khách Hàng VIP");
            SetupBox(100, 25);
            
            ShowDateTimeInfo();
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- BÁO CÁO KHÁCH HÀNG VIP ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write(new string('─', width - 4));
            
            try
            {
                var reportData = _reportBL.GetVIPCustomerReport();
                
                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("Tiêu chí VIP: >= 5 lần đặt phòng");
                
                if (reportData.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 10);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Không có khách hàng VIP nào.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Top khách hàng VIP:");
                    Console.WriteLine("Tên khách hàng\t\tQuốc tịch\t\tSố booking\tTổng chi tiêu\t\t\tLoại thành viên");
                    Console.WriteLine(new string('-', 100));
                    
                    int rank = 1;
                    foreach (DataRow row in reportData.Rows)
                    {
                        var totalSpent = row["TotalSpent"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalSpent"]);
                        Console.WriteLine($"{rank,2}. {row["Name"],-20}\t{row["Nationality"],-12}\t{row["TotalBookings"],-10}\t{ReportBL.FormatCurrency(totalSpent),-20}\t{row["VIPLevel"]}");
                        rank++;
                    }
                    
                    Console.WriteLine();
                    Console.WriteLine($"Tổng số khách VIP: {reportData.Rows.Count}");
                }

                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }







        private void PopularServicesReport()
        {
            Console.Clear();
            DrawHeader("Báo Cáo Dịch Vụ Phổ Biến");
            
            try
            {
                var reportData = _reportBL.GetPopularServicesReport();
                
                Console.WriteLine("=== BÁO CÁO DỊCH VỤ PHỔ BIẾN ===");
                Console.WriteLine();
                
                if (reportData.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu dịch vụ.");
                }
                else
                {
                    Console.WriteLine("Top dịch vụ được sử dụng nhiều nhất:");
                    Console.WriteLine("Rank\tTên dịch vụ\t\t\tLoại\t\tSố lượt\tDoanh thu\t\tTỷ lệ");
                    Console.WriteLine(new string('-', 90));
                    
                    int rank = 1;
                    foreach (DataRow row in reportData.Rows)
                    {
                        Console.WriteLine($"{rank,2}\t{row["ServiceName"],-25}\t{row["Type"],-10}\t{row["UsageCount"],-8}\t{ReportBL.FormatCurrency(Convert.ToDecimal(row["TotalRevenue"])),-15}\t{ReportBL.FormatPercentage(Convert.ToDecimal(row["UsagePercentage"]))}");
                        rank++;
                    }
                    
                    // Tính tổng doanh thu
                    decimal totalRevenue = 0;
                    foreach (DataRow row in reportData.Rows)
                    {
                        totalRevenue += Convert.ToDecimal(row["TotalRevenue"]);
                    }
                    
                    Console.WriteLine();
                    Console.WriteLine($"Tổng doanh thu dịch vụ: {ReportBL.FormatCurrency(totalRevenue)}");
                }

                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        // Thêm báo cáo tỷ lệ lấp đầy phòng
        private void OccupancyRateReport()
        {
            Console.Clear();
            DrawHeader("Báo Cáo Tỷ Lệ Lấp Đầy Phòng");
            SetupBox(80, 18);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Ngày bắt đầu (dd/MM/yyyy): ");
            string? startDate = ReadInputWithEscape(x + 28, y + 5);
            if (startDate == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Ngày kết thúc (dd/MM/yyyy): ");
            string? endDate = ReadInputWithEscape(x + 29, y + 7);
            if (endDate == null) return;

            try
            {
                var reportData = _reportBL.GetOccupancyReport(startDate, endDate);
                
                Console.Clear();
                Console.WriteLine($"=== BÁO CÁO TỶ LỆ LẤP ĐẦY PHÒNG ===");
                Console.WriteLine($"Từ ngày: {startDate} đến ngày: {endDate}");
                Console.WriteLine();

                if (reportData.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu cho khoảng thời gian này.");
                }
                else
                {
                    Console.WriteLine("Ngày\t\tTổng phòng\tPhòng đã đặt\tTỷ lệ lấp đầy");
                    Console.WriteLine(new string('-', 60));
                    
                    decimal totalOccupancyRate = 0;
                    int dayCount = 0;
                    
                    foreach (DataRow row in reportData.Rows)
                    {
                        var occupancyRate = row["OccupancyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["OccupancyRate"]);
                        Console.WriteLine($"{ReportBL.FormatDate(Convert.ToDateTime(row["Date"]))}\t{row["TotalRooms"]}\t\t{row["OccupiedRooms"]}\t\t{ReportBL.FormatPercentage(occupancyRate)}");
                        totalOccupancyRate += occupancyRate;
                        dayCount++;
                    }
                    
                    if (dayCount > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Tỷ lệ lấp đầy trung bình: {ReportBL.FormatPercentage(totalOccupancyRate / dayCount)}");
                    }
                }

                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        // Thêm báo cáo khách hàng theo quốc tịch
        private void CustomerNationalityReport()
        {
            Console.Clear();
            DrawHeader("Báo Cáo Khách Hàng Theo Quốc Tịch");
            
            try
            {
                var reportData = _reportBL.GetCustomerNationalityReport();
                
                Console.WriteLine("=== BÁO CÁO KHÁCH HÀNG THEO QUỐC TỊCH ===");
                Console.WriteLine();
                
                if (reportData.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu khách hàng.");
                }
                else
                {
                    Console.WriteLine("Quốc tịch\t\tSố khách hàng\tSố booking\tThời gian lưu trú TB\tTỷ lệ");
                    Console.WriteLine(new string('-', 85));
                    
                    foreach (DataRow row in reportData.Rows)
                    {
                        var avgDuration = row["AvgStayDuration"] == DBNull.Value ? 0 : Convert.ToDecimal(row["AvgStayDuration"]);
                        Console.WriteLine($"{row["Nationality"],-15}\t{row["CustomerCount"],-12}\t{row["TotalBookings"],-10}\t{avgDuration:F1} ngày\t\t\t{ReportBL.FormatPercentage(Convert.ToDecimal(row["Percentage"]))}");
                    }
                }

                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void WeeklyRevenueReport()
        {
            Console.Clear();
            DrawHeader("Báo Cáo Doanh Thu Theo Tuần");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Nhập ngày đầu tuần (dd/MM/yyyy): ");
            string? weekInput = ReadInputWithEscape(x + 35, y + 5);
            if (weekInput == null) return;

            try
            {
                DataTable weeklyRevenue = _reportBL.GetWeeklyRevenueReport(weekInput);
                Console.Clear();
                DrawHeader("Báo Cáo Doanh Thu Theo Tuần");
                
                if (weeklyRevenue.Rows.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nKhông có dữ liệu doanh thu cho tuần này.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n--- BÁO CÁO DOANH THU THEO TUẦN ---");
                    Console.ResetColor();
                    
                    foreach (DataRow row in weeklyRevenue.Rows)
                    {
                        Console.WriteLine($"Tuần: {row["WeekStart"]} - {row["WeekEnd"]}");
                        Console.WriteLine($"Số booking: {row["TotalBookings"]}");
                        Console.WriteLine($"Doanh thu phòng: {row["BookingRevenue"]:N0} VND");
                        Console.WriteLine($"Doanh thu dịch vụ: {row["ServiceRevenue"]:N0} VND");
                        Console.WriteLine($"Tổng doanh thu: {row["TotalRevenue"]:N0} VND");
                        Console.WriteLine(new string('─', 50));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
            }
            
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowDateTimeInfo()
        {
            Console.SetCursorPosition(x + 2, y + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            if (!string.IsNullOrEmpty(currentUsername))
            {
                Console.Write($" | Người dùng: {currentUsername}");
                if (!string.IsNullOrEmpty(currentRole))
                {
                    Console.Write($" ({currentRole})");
                }
            }
            Console.ResetColor();
        }
    }
}
