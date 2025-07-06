using System;
using System.Data;
using VTCAcademy_PF1127_HotelSync.CustomerManagement;

namespace HotelManagementSystem
{
    public class CustomerManagementUI : BaseUI
    {
        private readonly CustomerBL _customerBL = new CustomerBL();

        public CustomerManagementUI(string? username, string? role, int? userId) 
            : base(username, role, userId)
        {
        }

        public void ShowCustomerManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Khách Hàng");
                SetupBox(80, 18);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Thêm khách hàng mới");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Cập nhật thông tin khách hàng");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Xóa khách hàng");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Xem danh sách khách hàng");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Tìm kiếm khách hàng");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Quay lại");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
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
                            ShowAddCustomer();
                            break;
                        case "2":
                            ShowUpdateCustomer();
                            break;
                        case "3":
                            ShowDeleteCustomer();
                            break;
                        case "4":
                            ShowCustomerList();
                            break;
                        case "5":
                            ShowSearchCustomer();
                            break;
                        case "6":
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

        private void ShowAddCustomer()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Thêm Khách Hàng Mới");
            SetupBox(80, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Họ và tên: ");
            string? fullName = ReadInputWithEscape(x + 13, y + 5);
            if (fullName == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Email: ");
            string? email = ReadInputWithEscape(x + 9, y + 7);
            if (email == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("CMND/CCCD: ");
            string? idCard = ReadInputWithEscape(x + 13, y + 9);
            if (idCard == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write("Số điện thoại: ");
            string? phone = ReadInputWithEscape(x + 16, y + 11);
            if (phone == null) return;

            Console.SetCursorPosition(x + 2, y + 13);
            Console.Write("Quốc tịch: ");
            string? nationality = ReadInputWithEscape(x + 12, y + 13);
            if (nationality == null) return;

            try
            {
                var result = _customerBL.AddCustomer(fullName, idCard, phone, email, nationality, currentUserId, currentUsername ?? "Unknown");
                if (result.Success)
                {
                    ShowSuccessMessage($"Thêm khách hàng thành công! ID: {result.CustomerId}");
                }
                else
                {
                    ShowErrorMessage($"Thêm khách hàng thất bại: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
            }
            Console.ReadKey();
        }

        private void ShowUpdateCustomer()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Cập Nhật Thông Tin Khách Hàng");
            SetupBox(80, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID khách hàng: ");
            string? customerIdStr = ReadInputWithEscape(x + 16, y + 5);
            if (customerIdStr == null) return;

            if (!int.TryParse(customerIdStr, out int customerId))
            {
                ShowErrorMessage("ID khách hàng không hợp lệ!");
                Console.ReadKey();
                return;
            }

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Họ và tên mới: ");
            string? fullName = ReadInputWithEscape(x + 17, y + 7);
            if (fullName == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Email mới: ");
            string? email = ReadInputWithEscape(x + 13, y + 9);
            if (email == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write("Số điện thoại mới: ");
            string? phone = ReadInputWithEscape(x + 20, y + 11);
            if (phone == null) return;

            Console.SetCursorPosition(x + 2, y + 13);
            Console.Write("Quốc tịch mới: ");
            string? nationality = ReadInputWithEscape(x + 16, y + 13);
            if (nationality == null) return;

            try
            {
                var result = _customerBL.UpdateCustomer(customerId, fullName, phone, email, nationality, currentUserId, currentUsername ?? "Unknown");
                if (result.Success)
                {
                    ShowSuccessMessage("Cập nhật thông tin khách hàng thành công!");
                }
                else
                {
                    ShowErrorMessage($"Cập nhật thông tin khách hàng thất bại: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
            }
            Console.ReadKey();
        }

        private void ShowDeleteCustomer()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Xóa Khách Hàng");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID khách hàng cần xóa: ");
            string? customerIdStr = ReadInputWithEscape(x + 24, y + 5);
            if (customerIdStr == null) return;

            if (!int.TryParse(customerIdStr, out int customerId))
            {
                ShowErrorMessage("ID khách hàng không hợp lệ!");
                Console.ReadKey();
                return;
            }

            Console.SetCursorPosition(x + 2, y + 7);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Bạn có chắc chắn muốn xóa khách hàng này? (y/n): ");
            string? confirm = ReadInputWithEscape(x + 49, y + 7);
            if (confirm == null || confirm.ToLower() != "y") return;

            try
            {
                var result = _customerBL.DeleteCustomer(customerId);
                if (result.Success)
                {
                    ShowSuccessMessage("Xóa khách hàng thành công!");
                }
                else
                {
                    ShowErrorMessage($"Xóa khách hàng thất bại: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
            }
            Console.ReadKey();
        }

        private void ShowCustomerList()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Khách Hàng");
            SetupBox(120, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH KHÁCH HÀNG ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            DataTable customers;
            try
            {
                customers = _customerBL.GetAllCustomers();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            if (customers.Rows.Count == 0)
            {
                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Không có khách hàng nào trong hệ thống.");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 8);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Tên", "CMND/CCCD", "SĐT", "Email", "Quốc tịch", "Ngày tạo" };
            int[] columnWidths = new[] { 5, 20, 15, 12, 25, 12, 12 };

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

            int maxRowsToShow = Math.Min(customers.Rows.Count, 10); // Giới hạn hiển thị 10 dòng
            for (int i = 0; i < maxRowsToShow; i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                
                string id = (customers.Rows[i]["CustomerID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                string name = (customers.Rows[i]["Name"]?.ToString() ?? "").PadRight(columnWidths[1]);
                string idCard = (customers.Rows[i]["IDCard"]?.ToString() ?? "").PadRight(columnWidths[2]);
                string phone = (customers.Rows[i]["Phone"]?.ToString() ?? "").PadRight(columnWidths[3]);
                string email = (customers.Rows[i]["Email"]?.ToString() ?? "").PadRight(columnWidths[4]);
                string nationality = (customers.Rows[i]["Nationality"]?.ToString() ?? "").PadRight(columnWidths[5]);
                string createdAt = customers.Rows[i]["CreatedAt"] != DBNull.Value ? 
                    Convert.ToDateTime(customers.Rows[i]["CreatedAt"]).ToString("yyyy-MM-dd") : "";

                // Cắt ngắn nếu quá dài
                if (name.Length > columnWidths[1]) name = name.Substring(0, columnWidths[1] - 3) + "...";
                if (email.Length > columnWidths[4]) email = email.Substring(0, columnWidths[4] - 3) + "...";

                Console.Write(id);
                Console.Write(name);
                Console.Write(idCard);
                Console.Write(phone);
                Console.Write(email);
                Console.Write(nationality);
                Console.Write(createdAt);

                if (i < maxRowsToShow - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            if (customers.Rows.Count > 10)
            {
                Console.SetCursorPosition(x + 2, y + 8 + maxRowsToShow * 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Hiển thị {maxRowsToShow}/{customers.Rows.Count} khách hàng. Sử dụng tìm kiếm để xem chi tiết.");
                Console.ResetColor();
            }

            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowSearchCustomer()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tìm Kiếm Khách Hàng");
            SetupBox(80, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("1. Tìm theo tên");
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("2. Tìm theo CMND/CCCD");
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("3. Tìm theo số điện thoại");
            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("4. Tìm kiếm tổng hợp");
            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("0. Quay lại");

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write("Lựa chọn: ");
            string? choice = ReadInputWithEscape(x + 12, y + 11);
            if (choice == null) return;

            try
            {
                DataTable customers = new DataTable();
                string searchTerm = "";

                switch (choice)
                {
                    case "1":
                        Console.SetCursorPosition(x + 2, y + 13);
                        Console.Write("Nhập tên cần tìm: ");
                        searchTerm = ReadInputWithEscape(x + 20, y + 13) ?? "";
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            customers = _customerBL.SearchCustomersByName(searchTerm);
                        }
                        break;
                    case "2":
                        Console.SetCursorPosition(x + 2, y + 13);
                        Console.Write("Nhập CMND/CCCD: ");
                        searchTerm = ReadInputWithEscape(x + 18, y + 13) ?? "";
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            customers = _customerBL.SearchCustomers(idCard: searchTerm);
                        }
                        break;
                    case "3":
                        Console.SetCursorPosition(x + 2, y + 13);
                        Console.Write("Nhập số điện thoại: ");
                        searchTerm = ReadInputWithEscape(x + 21, y + 13) ?? "";
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            customers = _customerBL.SearchCustomers(phone: searchTerm);
                        }
                        break;
                    case "4":
                        Console.Clear();
                        DrawHeader("Tìm Kiếm Tổng Hợp");
                        Console.WriteLine("Nhập thông tin tìm kiếm (bỏ trống nếu không cần):");
                        
                        Console.Write("Tên: ");
                        string? name = Console.ReadLine();
                        
                        Console.Write("CMND/CCCD: ");
                        string? idCard = Console.ReadLine();
                        
                        Console.Write("Số điện thoại: ");
                        string? phone = Console.ReadLine();
                        
                        customers = _customerBL.SearchCustomers(name, idCard, phone);
                        break;
                    case "0":
                        return;
                    default:
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        return;
                }

                // Hiển thị kết quả
                if (string.IsNullOrEmpty(searchTerm) && choice != "4")
                {
                    ShowErrorMessage("Vui lòng nhập từ khóa tìm kiếm!");
                    Console.ReadKey();
                    return;
                }
                
                ShowSearchResults(customers, choice == "4" ? "Tìm kiếm tổng hợp" : $"Tìm kiếm: {searchTerm}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
            }
            
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowSearchResults(DataTable customers, string searchTitle)
        {
            Console.Clear();
            DrawHeader("Kết Quả Tìm Kiếm Khách Hàng");
            SetupBox(120, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"--- {searchTitle.ToUpper()} ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            if (customers.Rows.Count == 0)
            {
                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Không tìm thấy khách hàng nào với tiêu chí này.");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Tên", "CMND/CCCD", "SĐT", "Email", "Quốc tịch", "Ngày tạo" };
            int[] columnWidths = new[] { 5, 20, 15, 12, 25, 12, 12 };

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

            int maxRowsToShow = Math.Min(customers.Rows.Count, 10);
            for (int i = 0; i < maxRowsToShow; i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                
                string id = (customers.Rows[i]["CustomerID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                string name = (customers.Rows[i]["Name"]?.ToString() ?? "").PadRight(columnWidths[1]);
                string idCard = (customers.Rows[i]["IDCard"]?.ToString() ?? "").PadRight(columnWidths[2]);
                string phone = (customers.Rows[i]["Phone"]?.ToString() ?? "").PadRight(columnWidths[3]);
                string email = (customers.Rows[i]["Email"]?.ToString() ?? "").PadRight(columnWidths[4]);
                string nationality = (customers.Rows[i]["Nationality"]?.ToString() ?? "").PadRight(columnWidths[5]);
                string createdAt = customers.Rows[i]["CreatedAt"] != DBNull.Value ? 
                    Convert.ToDateTime(customers.Rows[i]["CreatedAt"]).ToString("yyyy-MM-dd") : "";

                // Cắt ngắn nếu quá dài
                if (name.Length > columnWidths[1]) name = name.Substring(0, columnWidths[1] - 3) + "...";
                if (email.Length > columnWidths[4]) email = email.Substring(0, columnWidths[4] - 3) + "...";

                Console.Write(id);
                Console.Write(name);
                Console.Write(idCard);
                Console.Write(phone);
                Console.Write(email);
                Console.Write(nationality);
                Console.Write(createdAt);

                if (i < maxRowsToShow - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            if (customers.Rows.Count > 10)
            {
                Console.SetCursorPosition(x + 2, y + 8 + maxRowsToShow * 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Hiển thị {maxRowsToShow}/{customers.Rows.Count} kết quả tìm kiếm.");
                Console.ResetColor();
            }

            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }
    }
}
