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

            try
            {
                DataTable customers = _customerBL.GetAllCustomers();
                
                if (customers.Rows.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nKhông có khách hàng nào trong hệ thống.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n--- DANH SÁCH KHÁCH HÀNG ---");
                    Console.ResetColor();
                    
                    foreach (DataRow row in customers.Rows)
                    {
                        Console.WriteLine($"ID: {row["customer_id"]} | Tên: {row["name"]} | Email: {row["email"]} | SĐT: {row["phone"]}");
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

        private void ShowSearchCustomer()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tìm Kiếm Khách Hàng");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Nhập từ khóa tìm kiếm (tên, email, hoặc số điện thoại): ");
            string? keyword = ReadInputWithEscape(x + 56, y + 5);
            if (keyword == null) return;

            try
            {
                DataTable customers = _customerBL.SearchCustomersByName(keyword);
                Console.Clear();
                DrawHeader("Kết Quả Tìm Kiếm Khách Hàng");
                
                if (customers.Rows.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nKhông tìm thấy khách hàng nào với từ khóa này.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n--- KẾT QUẢ TÌM KIẾM ---");
                    Console.ResetColor();
                    
                    foreach (DataRow row in customers.Rows)
                    {
                        Console.WriteLine($"ID: {row["customer_id"]} | Tên: {row["name"]} | Email: {row["email"]} | SĐT: {row["phone"]}");
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
    }
}
