using System;
using HotelManagementSystem.InvoiceManagement;

namespace HotelManagementSystem.ConsoleUI
{
    public class InvoiceManagementUI : BaseUI
    {
        private readonly InvoiceBL _invoiceBL;

        public InvoiceManagementUI(string? username = null, string? role = null, int? userId = null) : base(username, role, userId)
        {
            _invoiceBL = new InvoiceBL();
        }

        public void ShowInvoiceManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Hóa Đơn");
                SetupBox(80, 24);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Tạo hóa đơn mới");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Tạo hóa đơn tự động");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Xem tất cả hóa đơn");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Xem chi tiết hóa đơn");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Tìm kiếm hóa đơn");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Cập nhật trạng thái thanh toán");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("7. In hóa đơn");
                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 18);
                Console.Write("8. Tính toán số tiền booking");
                Console.SetCursorPosition(x + 2, y + 19);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 20);
                Console.Write("9. Quay lại");
                Console.SetCursorPosition(x + 2, y + 21);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 22);
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
                            CreateNewInvoice();
                            break;
                        case "2":
                            CreateAutoInvoice();
                            break;
                        case "3":
                            ViewAllInvoices();
                            break;
                        case "4":
                            ViewInvoiceDetails();
                            break;
                        case "5":
                            SearchInvoices();
                            break;
                        case "6":
                            UpdatePaymentStatus();
                            break;
                        case "7":
                            PrintInvoice();
                            break;
                        case "8":
                            CalculateInvoiceAmount();
                            break;
                        case "9":
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

        private void CreateNewInvoice()
        {
            Console.Clear();
            DrawHeader("Tạo Hóa Đơn Mới");
            SetupBox(80, 20);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Booking: ");
            string? bookingId = ReadInputWithEscape(x + 14, y + 5);
            if (bookingId == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("ID Khách hàng: ");
            string? customerId = ReadInputWithEscape(x + 17, y + 7);
            if (customerId == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Tổng số tiền: ");
            string? totalAmount = ReadInputWithEscape(x + 16, y + 9);
            if (totalAmount == null) return;

            try
            {
                if (int.TryParse(bookingId, out int bookingIdInt) && 
                    int.TryParse(customerId, out int customerIdInt) &&
                    decimal.TryParse(totalAmount, out decimal totalAmountDecimal))
                {
                    _invoiceBL.CreateInvoice(bookingId, customerId, totalAmount.ToString(), currentUserId ?? 1, currentUsername ?? "System");
                    ShowSuccessMessage("Tạo hóa đơn thành công! Nhấn phím bất kỳ để quay lại...");
                }
                else
                {
                    ShowErrorMessage("Dữ liệu đầu vào không hợp lệ!");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void CreateAutoInvoice()
        {
            Console.Clear();
            DrawHeader("Tạo Hóa Đơn Tự Động");
            SetupBox(70, 14);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Booking: ");
            string? bookingId = ReadInputWithEscape(x + 14, y + 5);
            if (bookingId == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("ID Khách hàng: ");
            string? customerId = ReadInputWithEscape(x + 17, y + 7);
            if (customerId == null) return;

            try
            {
                if (int.TryParse(bookingId, out int bookingIdInt) && 
                    int.TryParse(customerId, out int customerIdInt))
                {
                    _invoiceBL.CreateInvoiceWithAutoCalculation(bookingId, customerId, currentUserId ?? 1, currentUsername ?? "System");
                    ShowSuccessMessage("Tạo hóa đơn tự động thành công! Nhấn phím bất kỳ để quay lại...");
                }
                else
                {
                    ShowErrorMessage("ID không hợp lệ!");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ViewAllInvoices()
        {
            Console.Clear();
            DrawHeader("Danh Sách Tất Cả Hóa Đơn");
            
            try
            {
                _invoiceBL.ViewAllInvoices();
                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ViewInvoiceDetails()
        {
            Console.Clear();
            DrawHeader("Chi Tiết Hóa Đơn");
            SetupBox(70, 12);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 5);
            if (invoiceId == null) return;

            try
            {
                if (int.TryParse(invoiceId, out int invoiceIdInt))
                {
                    _invoiceBL.ViewInvoiceDetails(invoiceId);
                }
                else
                {
                    ShowErrorMessage("ID hóa đơn không hợp lệ!");
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

        private void SearchInvoices()
        {
            Console.Clear();
            DrawHeader("Tìm Kiếm Hóa Đơn");
            SetupBox(80, 18);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Tên khách hàng (để trống nếu không lọc): ");
            string? customerName = ReadInputWithEscape(x + 42, y + 5);
            if (customerName == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Từ ngày (dd/MM/yyyy, để trống nếu không lọc): ");
            string? fromDate = ReadInputWithEscape(x + 46, y + 7);
            if (fromDate == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Đến ngày (dd/MM/yyyy, để trống nếu không lọc): ");
            string? toDate = ReadInputWithEscape(x + 47, y + 9);
            if (toDate == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write("Trạng thái (Pending/Paid/Cancelled, để trống nếu không lọc): ");
            string? paymentStatus = ReadInputWithEscape(x + 62, y + 11);
            if (paymentStatus == null) return;

            try
            {
                _invoiceBL.SearchInvoices(customerName, fromDate, toDate, paymentStatus);
                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void UpdatePaymentStatus()
        {
            Console.Clear();
            DrawHeader("Cập Nhật Trạng Thái Thanh Toán");
            SetupBox(70, 16);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 5);
            if (invoiceId == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Trạng thái mới (Pending/Paid/Cancelled): ");
            string? paymentStatus = ReadInputWithEscape(x + 43, y + 7);
            if (paymentStatus == null) return;

            try
            {
                if (int.TryParse(invoiceId, out int invoiceIdInt))
                {
                    _invoiceBL.UpdatePaymentStatus(invoiceId, paymentStatus, currentUserId ?? 1, currentUsername ?? "System");
                    ShowSuccessMessage("Cập nhật trạng thái thành công! Nhấn phím bất kỳ để quay lại...");
                }
                else
                {
                    ShowErrorMessage("ID hóa đơn không hợp lệ!");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void PrintInvoice()
        {
            Console.Clear();
            DrawHeader("In Hóa Đơn");
            SetupBox(70, 12);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 5);
            if (invoiceId == null) return;

            try
            {
                if (int.TryParse(invoiceId, out int invoiceIdInt))
                {
                    _invoiceBL.PrintInvoice(invoiceId);
                }
                else
                {
                    ShowErrorMessage("ID hóa đơn không hợp lệ!");
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

        private void CalculateInvoiceAmount()
        {
            Console.Clear();
            DrawHeader("Tính Toán Số Tiền Booking");
            SetupBox(70, 12);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Booking: ");
            string? bookingId = ReadInputWithEscape(x + 14, y + 5);
            if (bookingId == null) return;

            try
            {
                if (int.TryParse(bookingId, out int bookingIdInt))
                {
                    _invoiceBL.CalculateInvoiceAmount(bookingId);
                    
                    Console.SetCursorPosition(x + 2, y + 7);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"Số tiền đã được tính toán và hiển thị");
                    Console.ResetColor();
                }
                else
                {
                    ShowErrorMessage("ID booking không hợp lệ!");
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

        private void DeleteInvoice()
        {
            Console.Clear();
            DrawHeader("Xóa Hóa Đơn (Admin Only)");
            SetupBox(70, 14);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 5);
            if (invoiceId == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("CẢNH BÁO: Thao tác này không thể hoàn tác!");
            Console.ResetColor();

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Xác nhận xóa (y/n): ");
            string? confirm = ReadInputWithEscape(x + 22, y + 9);
            if (confirm == null || confirm.ToLower() != "y") return;

            try
            {
                if (int.TryParse(invoiceId, out int invoiceIdInt))
                {
                    _invoiceBL.DeleteInvoice(invoiceId, currentUserId ?? 1, currentUsername ?? "System");
                    ShowSuccessMessage("Xóa hóa đơn thành công! Nhấn phím bất kỳ để quay lại...");
                }
                else
                {
                    ShowErrorMessage("ID hóa đơn không hợp lệ!");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
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

        private void ShowMenuOptions(string[] options)
        {
            int startY = y + 4;
            for (int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(x + 2, startY + i);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(options[i]);
                Console.ResetColor();
            }
        }

        private string? GetUserChoice()
        {
            Console.SetCursorPosition(x + 2, y + height - 4);
            Console.Write("Lựa chọn của bạn: ");
            return ReadInputWithEscape(x + 20, y + height - 4);
        }
    }
}
