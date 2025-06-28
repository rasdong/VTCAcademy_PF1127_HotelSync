using System;
using System.Data;

namespace HotelManagementSystem
{
    public class ServiceManagementUI : BaseUI
    {
        private readonly ServiceBLL _serviceBLL = new ServiceBLL();

        public ServiceManagementUI(string? username, string? role, int? userId) 
            : base(username, role, userId)
        {
        }

        public void ShowServiceManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Dịch Vụ");
                SetupBox(80, 20);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Thêm dịch vụ mới");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Cập nhật thông tin dịch vụ");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Xóa dịch vụ");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Xem danh sách dịch vụ");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Tìm kiếm dịch vụ theo loại");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Ghi nhận yêu cầu dịch vụ");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("7. Quay lại");
                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 18);
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
                            ShowAddService();
                            break;
                        case "2":
                            ShowUpdateService();
                            break;
                        case "3":
                            ShowDeleteService();
                            break;
                        case "4":
                            ShowServiceList();
                            break;
                        case "5":
                            ShowSearchServicesByType();
                            break;
                        case "6":
                            ShowAddServiceUsage();
                            break;
                        case "7":
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

        private void ShowAddService()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Thêm Dịch Vụ Mới");
            SetupBox(80, 18);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên dịch vụ: ");
            string? serviceName = ReadInputWithEscape(x + 14, y + 4);
            if (serviceName == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Loại dịch vụ (Food/Laundry/Spa/Other): ");
            string? type = ReadInputWithEscape(x + 39, y + 6);
            if (type == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Giá dịch vụ (VND): ");
            string? price = ReadInputWithEscape(x + 19, y + 8);
            if (price == null) return;

            try
            {
                _serviceBLL.AddService(serviceName, type, price, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Thêm dịch vụ thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowUpdateService()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Cập Nhật Dịch Vụ");
            SetupBox(80, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID dịch vụ: ");
            string? serviceIdInput = ReadInputWithEscape(x + 13, y + 4);
            if (serviceIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Tên dịch vụ: ");
            string? serviceName = ReadInputWithEscape(x + 14, y + 6);
            if (serviceName == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Loại dịch vụ (Food/Laundry/Spa/Other): ");
            string? type = ReadInputWithEscape(x + 39, y + 8);
            if (type == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Giá dịch vụ (VND): ");
            string? price = ReadInputWithEscape(x + 19, y + 10);
            if (price == null) return;

            try
            {
                _serviceBLL.UpdateService(serviceIdInput, serviceName, type, price, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Cập nhật dịch vụ thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowDeleteService()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Xóa Dịch Vụ");
            SetupBox(60, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID dịch vụ: ");
            string? serviceIdInput = ReadInputWithEscape(x + 13, y + 4);
            if (serviceIdInput == null) return;

            try
            {
                if (!_serviceBLL.CanDeleteService(serviceIdInput))
                {
                    ShowErrorMessage("Không thể xóa dịch vụ vì đã có khách hàng sử dụng! Nhấn phím bất kỳ để quay lại...");
                    Console.ReadKey();
                    return;
                }

                _serviceBLL.DeleteService(serviceIdInput, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Xóa dịch vụ thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowServiceList()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Dịch Vụ");
            SetupBox(100, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH DỊCH VỤ ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            DataTable services;
            try
            {
                services = _serviceBLL.GetAllServices();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Tên dịch vụ", "Loại", "Giá (VND)", "Ngày tạo" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length + 2;
            }

            Console.SetCursorPosition(x + 2, y + 6);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
            {
                Console.Write(headers[col].PadRight(columnWidths[col]));
            }
            Console.WriteLine();

            Console.SetCursorPosition(x + 2, y + 7);
            Console.WriteLine(new string('─', width - 4));
            Console.ResetColor();

            for (int i = 0; i < services.Rows.Count; i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                Console.Write((services.Rows[i]["ServiceID"]?.ToString() ?? "").PadRight(columnWidths[0]));
                Console.Write((services.Rows[i]["ServiceName"]?.ToString() ?? "").PadRight(columnWidths[1]));
                Console.Write((services.Rows[i]["Type"]?.ToString() ?? "").PadRight(columnWidths[2]));
                Console.Write(Convert.ToDecimal(services.Rows[i]["Price"]).ToString("N0").PadRight(columnWidths[3]));
                Console.Write(Convert.ToDateTime(services.Rows[i]["CreatedAt"]).ToString("yyyy-MM-dd"));

                if (i < services.Rows.Count - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (services.Rows.Count - 1) * 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowSearchServicesByType()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tìm Kiếm Dịch Vụ");
            SetupBox(80, 15);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Loại dịch vụ (Food/Laundry/Spa/Other): ");
            string? type = ReadInputWithEscape(x + 39, y + 4);
            if (type == null) return;

            try
            {
                DataTable serviceResult = _serviceBLL.SearchServicesByType(type);
                
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- KẾT QUẢ TÌM KIẾM DỊCH VỤ LOẠI {type.ToUpper()} ---");
                Console.ResetColor();

                if (serviceResult.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 7);
                    Console.Write("Không tìm thấy dịch vụ nào!");
                }
                else
                {
                    Console.SetCursorPosition(x + 2, y + 7);
                    Console.Write($"Tìm thấy {serviceResult.Rows.Count} dịch vụ:");
                    
                    for (int i = 0; i < serviceResult.Rows.Count; i++)
                    {
                        Console.SetCursorPosition(x + 2, y + 8 + i);
                        Console.Write($"ID: {serviceResult.Rows[i]["ServiceID"]}, Tên: {serviceResult.Rows[i]["ServiceName"]}, Giá: {Convert.ToDecimal(serviceResult.Rows[i]["Price"]):N0} VND");
                    }
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

        private void ShowAddServiceUsage()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Ghi Nhận Yêu Cầu Dịch Vụ");
            SetupBox(80, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 15, y + 4);
            if (bookingIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("ID dịch vụ: ");
            string? serviceIdInput = ReadInputWithEscape(x + 13, y + 6);
            if (serviceIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("ID khách hàng: ");
            string? customerIdInput = ReadInputWithEscape(x + 16, y + 8);
            if (customerIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Số lượng: ");
            string? quantityInput = ReadInputWithEscape(x + 11, y + 10);
            if (quantityInput == null) return;

            try
            {
                _serviceBLL.AddServiceUsage(bookingIdInput, serviceIdInput, customerIdInput, quantityInput, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Ghi nhận yêu cầu dịch vụ thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowServiceUsageByBooking()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Xem Dịch Vụ Theo Đặt Phòng");
            SetupBox(100, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 15, y + 4);
            if (bookingIdInput == null) return;

            try
            {
                DataTable serviceUsage = _serviceBLL.GetServiceUsageByBooking(bookingIdInput);
                
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- DỊCH VỤ CỦA ĐẶT PHÒNG {bookingIdInput} ---");
                Console.ResetColor();

                if (serviceUsage.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 7);
                    Console.Write("Đặt phòng này chưa sử dụng dịch vụ nào!");
                }
                else
                {
                    Console.SetCursorPosition(x + 2, y + 7);
                    Console.Write("Tên dịch vụ".PadRight(20) + "Loại".PadRight(12) + "SL".PadRight(5) + "Đơn giá".PadRight(12) + "Tổng tiền".PadRight(12) + "Trạng thái");
                    Console.SetCursorPosition(x + 2, y + 8);
                    Console.Write(new string('─', width - 4));
                    
                    for (int i = 0; i < serviceUsage.Rows.Count; i++)
                    {
                        Console.SetCursorPosition(x + 2, y + 9 + i);
                        string serviceName = serviceUsage.Rows[i]["ServiceName"].ToString() ?? "";
                        string type = serviceUsage.Rows[i]["Type"].ToString() ?? "";
                        string quantity = serviceUsage.Rows[i]["Quantity"].ToString() ?? "";
                        string unitPrice = Convert.ToDecimal(serviceUsage.Rows[i]["UnitPrice"]).ToString("N0");
                        string totalPrice = Convert.ToDecimal(serviceUsage.Rows[i]["TotalPrice"]).ToString("N0");
                        string status = serviceUsage.Rows[i]["PaymentStatus"].ToString() ?? "";
                        
                        Console.Write(serviceName.PadRight(20) + type.PadRight(12) + quantity.PadRight(5) + unitPrice.PadRight(12) + totalPrice.PadRight(12) + status);
                    }
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

        private void ShowCalculateServiceCost()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tính Phí Dịch Vụ");
            SetupBox(80, 12);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 15, y + 4);
            if (bookingIdInput == null) return;

            try
            {
                decimal totalCost = _serviceBLL.GetTotalServiceCostByBooking(bookingIdInput);
                
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- TỔNG PHÍ DỊCH VỤ ĐẶT PHÒNG {bookingIdInput} ---");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write($"Tổng phí dịch vụ: {totalCost:N0} VND");

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
    }
}
