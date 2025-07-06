using System;
using System.Data;
using System.Linq;
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
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Tạo hóa đơn từ booking");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Xem danh sách hóa đơn");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Tìm kiếm hóa đơn");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Xem chi tiết hóa đơn");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Cập nhật trạng thái thanh toán");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. In hóa đơn");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("7. Lọc hóa đơn theo điều kiện");
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
                            CreateInvoiceFromBooking();
                            break;
                        case "2":
                            ViewAllInvoices();
                            break;
                        case "3":
                            SearchInvoices();
                            break;
                        case "4":
                            ViewInvoiceDetails();
                            break;
                        case "5":
                            UpdatePaymentStatus();
                            break;
                        case "6":
                            PrintInvoice();
                            break;
                        case "7":
                            FilterInvoices();
                            break;
                        case "8":
                            return;
                        case "0":
                            Environment.Exit(0);
                            break;
                        default:
                            ShowErrorMessage("Lựa chọn không hợp lệ!");
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

        // 1. Tạo hóa đơn từ booking
        private void CreateInvoiceFromBooking()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tạo Hóa Đơn Từ Booking");
            SetupBox(90, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Mã booking: ");
            string? bookingId = ReadInputWithEscape(x + 14, y + 5);
            if (bookingId == null) return;

            try
            {
                int invoiceId = _invoiceBL.CreateInvoice(bookingId, currentUserId ?? 1, currentUsername ?? "System");
                
                // Chỉ hiển thị thông báo thành công màu xanh
                WriteTextInBox($"Tạo hóa đơn thành công! Mã hóa đơn: {invoiceId}", y + 8, ConsoleColor.Green);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                // Chỉ hiển thị thông báo lỗi màu đỏ
                WriteTextInBox($"Lỗi khi tạo hóa đơn: {ex.Message}", y + 8, ConsoleColor.Red);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        // 7. Lọc hóa đơn theo điều kiện
        private void FilterInvoices()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Lọc Hóa Đơn");
            SetupBox(80, 18);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên khách hàng (Enter để bỏ qua): ");
            string? customerName = ReadInputWithEscape(x + 35, y + 4);
            if (customerName == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Từ ngày (dd/MM/yyyy) (Enter để bỏ qua): ");
            string? fromDate = ReadInputWithEscape(x + 40, y + 6);
            if (fromDate == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Đến ngày (dd/MM/yyyy) (Enter để bỏ qua): ");
            string? toDate = ReadInputWithEscape(x + 42, y + 8);
            if (toDate == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Trạng thái (Paid/Unpaid) (Enter để bỏ qua): ");
            string? paymentStatus = ReadInputWithEscape(x + 45, y + 10);
            if (paymentStatus == null) return;

            try
            {
                string? filterCustomerName = string.IsNullOrEmpty(customerName) ? null : customerName;
                DateTime? filterFromDate = string.IsNullOrEmpty(fromDate) ? null : DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                DateTime? filterToDate = string.IsNullOrEmpty(toDate) ? null : DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                string? filterPaymentStatus = string.IsNullOrEmpty(paymentStatus) ? null : paymentStatus;

                DataTable invoices = _invoiceBL.GetAllInvoicesForDisplay(filterCustomerName, filterFromDate, filterToDate, filterPaymentStatus);

                // Show filtered results similar to ViewAllInvoices
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kết Quả Lọc Hóa Đơn");
                SetupBox(120, 22);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--- KẾT QUẢ LỌC HÓA ĐƠN ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                if (invoices.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 6);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Không tìm thấy hóa đơn nào phù hợp với điều kiện lọc.");
                    Console.ResetColor();
                }
                else
                {
                    // Display table similar to ViewAllInvoices
                    string[] headers = new[] { "Mã HĐ", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái" };
                    int[] columnWidths = new int[headers.Length];

                    // Đặt độ rộng tối thiểu cho từng cột
                    columnWidths[0] = 8;  // Mã HĐ
                    columnWidths[1] = 12; // Mã Booking
                    columnWidths[2] = 25; // Khách hàng
                    columnWidths[3] = 15; // Tổng tiền
                    columnWidths[4] = 16; // Ngày phát hành
                    columnWidths[5] = 15; // Trạng thái

                    string[,] invoiceData = new string[invoices.Rows.Count, headers.Length];
                    for (int i = 0; i < invoices.Rows.Count; i++)
                    {
                        invoiceData[i, 0] = invoices.Rows[i]["InvoiceID"]?.ToString() ?? "";
                        invoiceData[i, 1] = invoices.Rows[i]["BookingID"]?.ToString() ?? "";
                        invoiceData[i, 2] = invoices.Rows[i]["CustomerName"]?.ToString() ?? "";
                        invoiceData[i, 3] = Convert.ToDecimal(invoices.Rows[i]["TotalAmount"]).ToString("N0");
                        invoiceData[i, 4] = Convert.ToDateTime(invoices.Rows[i]["IssueDate"]).ToString("dd/MM/yyyy");
                        invoiceData[i, 5] = invoices.Rows[i]["PaymentStatus"]?.ToString() ?? "";

                        // Cập nhật độ rộng cột nếu cần
                        for (int col = 0; col < headers.Length; col++)
                        {
                            int length = invoiceData[i, col].Length;
                            if (length > columnWidths[col])
                                columnWidths[col] = Math.Min(length, columnWidths[col] + 5);
                        }
                    }

                    Console.SetCursorPosition(x + 2, y + 6);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    for (int col = 0; col < headers.Length; col++)
                    {
                        Console.Write(headers[col].PadRight(columnWidths[col]));
                    }
                    Console.WriteLine();

                    Console.SetCursorPosition(x + 2, y + 7);
                    Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                    Console.ResetColor();

                    for (int i = 0; i < invoiceData.GetLength(0); i++)
                    {
                        Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                        for (int col = 0; col < invoiceData.GetLength(1); col++)
                        {
                            Console.Write(invoiceData[i, col].PadRight(columnWidths[col]));
                        }
                        Console.WriteLine();
                        if (i < invoiceData.GetLength(0) - 1)
                        {
                            Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                            Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                        }
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

        private void ViewAllInvoices()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Hóa Đơn");
            SetupBox(120, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH HÓA ĐƠN ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            DataTable invoices;
            try
            {
                invoices = _invoiceBL.GetAllInvoicesForDisplay();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            if (invoices.Rows.Count == 0)
            {
                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Không có hóa đơn nào trong hệ thống.");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 8);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "Mã HĐ", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái" };
            int[] columnWidths = new int[headers.Length];

            // Đặt độ rộng tối thiểu cho từng cột
            columnWidths[0] = 8;  // Mã HĐ
            columnWidths[1] = 12; // Mã Booking
            columnWidths[2] = 25; // Khách hàng
            columnWidths[3] = 15; // Tổng tiền
            columnWidths[4] = 16; // Ngày phát hành
            columnWidths[5] = 15; // Trạng thái

            string[,] invoiceData = new string[invoices.Rows.Count, headers.Length];
            for (int i = 0; i < invoices.Rows.Count; i++)
            {
                invoiceData[i, 0] = invoices.Rows[i]["InvoiceID"]?.ToString() ?? "";
                invoiceData[i, 1] = invoices.Rows[i]["BookingID"]?.ToString() ?? "";
                invoiceData[i, 2] = invoices.Rows[i]["CustomerName"]?.ToString() ?? "";
                invoiceData[i, 3] = Convert.ToDecimal(invoices.Rows[i]["TotalAmount"]).ToString("N0");
                invoiceData[i, 4] = Convert.ToDateTime(invoices.Rows[i]["IssueDate"]).ToString("dd/MM/yyyy");
                invoiceData[i, 5] = invoices.Rows[i]["PaymentStatus"]?.ToString() ?? "";

                // Cập nhật độ rộng cột nếu cần
                for (int col = 0; col < headers.Length; col++)
                {
                    int length = invoiceData[i, col].Length;
                    if (length > columnWidths[col])
                        columnWidths[col] = Math.Min(length, columnWidths[col] + 5); // Tăng độ rộng tối đa 5 ký tự
                }
            }

            // Hiển thị tiêu đề cột
            Console.SetCursorPosition(x + 2, y + 6);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
            {
                Console.Write(headers[col].PadRight(columnWidths[col]));
            }
            Console.WriteLine();

            Console.SetCursorPosition(x + 2, y + 7);
            Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
            Console.ResetColor();

            // Hiển thị dữ liệu
            for (int i = 0; i < invoiceData.GetLength(0); i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                for (int col = 0; col < invoiceData.GetLength(1); col++)
                {
                    Console.Write(invoiceData[i, col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
                if (i < invoiceData.GetLength(0) - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (invoiceData.GetLength(0) - 1) * 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ViewInvoiceDetails()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Chi Tiết Hóa Đơn");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Mã hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 4);
            if (invoiceId == null) return;

            try
            {
                var (invoiceData, serviceData) = _invoiceBL.GetInvoiceDetailsForDisplay(invoiceId);
                
                if (invoiceData.Rows.Count == 0)
                {
                    ShowErrorMessage("Không tìm thấy hóa đơn.");
                    Console.ReadKey();
                    return;
                }

                DataRow invoice = invoiceData.Rows[0];

                // Clear and show invoice details
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Chi Tiết Hóa Đơn");
                SetupBox(100, 28);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--- CHI TIẾT HÓA ĐƠN ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                // Invoice basic info
                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write($"Số hóa đơn: {invoice["InvoiceID"]}");
                Console.SetCursorPosition(x + 50, y + 6);
                Console.Write($"Ngày: {Convert.ToDateTime(invoice["IssueDate"]):dd/MM/yyyy}");

                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write($"Booking ID: {invoice["BookingID"]}");
                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write(new string('─', width - 4));

                // Customer info
                Console.SetCursorPosition(x + 2, y + 9);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("THÔNG TIN KHÁCH HÀNG:");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write($"Họ tên: {invoice["CustomerName"]}");
                Console.SetCursorPosition(x + 50, y + 10);
                Console.Write($"CMND/CCCD: {invoice["IDCard"]}");

                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write($"Email: {invoice["Email"] ?? "N/A"}");
                Console.SetCursorPosition(x + 50, y + 11);
                Console.Write($"Điện thoại: {invoice["Phone"] ?? "N/A"}");

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write(new string('─', width - 4));

                // Room info
                Console.SetCursorPosition(x + 2, y + 13);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("THÔNG TIN PHÒNG:");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write($"Số phòng: {invoice["RoomNumber"]} - Loại: {invoice["RoomType"]}");

                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write($"Check-in: {Convert.ToDateTime(invoice["CheckInDate"]):dd/MM/yyyy}");
                Console.SetCursorPosition(x + 50, y + 15);
                Console.Write($"Check-out: {Convert.ToDateTime(invoice["CheckOutDate"]):dd/MM/yyyy}");

                int nights = (Convert.ToDateTime(invoice["CheckOutDate"]) - Convert.ToDateTime(invoice["CheckInDate"])).Days;
                decimal roomTotal = nights * Convert.ToDecimal(invoice["RoomPrice"]);

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write($"Số đêm: {nights}");
                Console.SetCursorPosition(x + 50, y + 16);
                Console.Write($"Giá phòng/đêm: {Convert.ToDecimal(invoice["RoomPrice"]):N0} VND");

                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));

                // Payment details
                Console.SetCursorPosition(x + 2, y + 18);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("CHI TIẾT THANH TOÁN:");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 19);
                Console.Write($"Tiền phòng ({nights} đêm): {roomTotal:N0} VND");

                decimal serviceTotal = 0;
                int currentLine = y + 20;
                if (serviceData.Rows.Count > 0)
                {
                    Console.SetCursorPosition(x + 2, currentLine);
                    Console.Write("Dịch vụ bổ sung:");
                    currentLine++;
                    
                    foreach (DataRow service in serviceData.Rows)
                    {
                        decimal serviceAmount = Convert.ToDecimal(service["TotalPrice"]);
                        serviceTotal += serviceAmount;
                        Console.SetCursorPosition(x + 4, currentLine);
                        Console.Write($"- {service["ServiceName"]} ({service["Type"]}) x{service["Quantity"]}: {serviceAmount:N0} VND");
                        currentLine++;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x + 2, currentLine);
                    Console.Write("Dịch vụ bổ sung: Không có");
                    currentLine++;
                }

                Console.SetCursorPosition(x + 2, currentLine);
                Console.Write(new string('─', width - 4));
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"TỔNG CỘNG: {Convert.ToDecimal(invoice["TotalAmount"]):N0} VND");
                Console.ResetColor();
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine);
                Console.Write($"Trạng thái thanh toán: {invoice["PaymentStatus"]}");

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

        private void SearchInvoices()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tìm Kiếm Hóa Đơn");
            SetupBox(100, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Mã hóa đơn (Enter để bỏ qua): ");
            string? invoiceId = ReadInputWithEscape(x + 31, y + 4);
            if (invoiceId == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Tên khách hàng (Enter để bỏ qua): ");
            string? customerName = ReadInputWithEscape(x + 35, y + 6);
            if (customerName == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Từ ngày (dd/MM/yyyy) (Enter để bỏ qua): ");
            string? fromDate = ReadInputWithEscape(x + 40, y + 8);
            if (fromDate == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Đến ngày (dd/MM/yyyy) (Enter để bỏ qua): ");
            string? toDate = ReadInputWithEscape(x + 42, y + 10);
            if (toDate == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 12);
            Console.Write("Trạng thái (Paid/Unpaid) (Enter để bỏ qua): ");
            string? paymentStatus = ReadInputWithEscape(x + 45, y + 12);
            if (paymentStatus == null) return;

            try
            {
                string? searchInvoiceId = string.IsNullOrEmpty(invoiceId) ? null : invoiceId;
                string? searchCustomerName = string.IsNullOrEmpty(customerName) ? null : customerName;
                string? searchFromDate = string.IsNullOrEmpty(fromDate) ? null : fromDate;
                string? searchToDate = string.IsNullOrEmpty(toDate) ? null : toDate;
                string? searchPaymentStatus = string.IsNullOrEmpty(paymentStatus) ? null : paymentStatus;

                // Get search results from BL
                DataTable invoices = _invoiceBL.SearchInvoicesForDisplay(searchInvoiceId, searchCustomerName, searchFromDate, searchToDate, searchPaymentStatus);

                // Clear and show results
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kết Quả Tìm Kiếm Hóa Đơn");
                SetupBox(120, 22);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--- KẾT QUẢ TÌM KIẾM HÓA ĐƠN ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                if (invoices.Rows.Count == 0)
                {
                    Console.SetCursorPosition(x + 2, y + 6);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Không tìm thấy hóa đơn nào phù hợp với điều kiện tìm kiếm.");
                    Console.ResetColor();
                    Console.SetCursorPosition(x + 2, y + 8);
                    Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                    Console.ReadKey();
                    return;
                }

                string[] headers = new[] { "Mã HĐ", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái" };
                int[] columnWidths = new[] { 8, 12, 25, 15, 16, 15 };

                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Magenta;
                for (int col = 0; col < headers.Length; col++)
                {
                    Console.Write(headers[col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();

                Console.SetCursorPosition(x + 2, y + 7);
                Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                Console.ResetColor();

                int maxRowsToShow = Math.Min(invoices.Rows.Count, 8); // Giới hạn hiển thị 8 dòng
                for (int i = 0; i < maxRowsToShow; i++)
                {
                    Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                    
                    string id = (invoices.Rows[i]["InvoiceID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                    string bookingId = (invoices.Rows[i]["BookingID"]?.ToString() ?? "").PadRight(columnWidths[1]);
                    string customer = invoices.Rows[i]["CustomerName"]?.ToString() ?? "";
                    customer = customer.Length > 23 ? customer.Substring(0, 20) + "..." : customer;
                    customer = customer.PadRight(columnWidths[2]);
                    string totalAmount = Convert.ToDecimal(invoices.Rows[i]["TotalAmount"]).ToString("N0").PadRight(columnWidths[3]);
                    string issueDate = Convert.ToDateTime(invoices.Rows[i]["IssueDate"]).ToString("dd/MM/yyyy").PadRight(columnWidths[4]);
                    string status = (invoices.Rows[i]["PaymentStatus"]?.ToString() ?? "").PadRight(columnWidths[5]);

                    Console.Write(id);
                    Console.Write(bookingId);
                    Console.Write(customer);
                    Console.Write(totalAmount);
                    Console.Write(issueDate);
                    Console.Write(status);

                    if (i < maxRowsToShow - 1)
                    {
                        Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                        Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                    }
                }

                if (invoices.Rows.Count > 8)
                {
                    Console.SetCursorPosition(x + 2, y + 8 + maxRowsToShow * 2);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Hiển thị {maxRowsToShow}/{invoices.Rows.Count} hóa đơn.");
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

        private void UpdatePaymentStatus()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Cập Nhật Trạng Thái Thanh Toán");
            SetupBox(80, 18);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Mã hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 4);
            if (invoiceId == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Trạng thái mới (Paid/Unpaid): ");
            string? paymentStatus = ReadInputWithEscape(x + 32, y + 6);
            if (paymentStatus == null) return;

            try
            {
                _invoiceBL.UpdatePaymentStatus(invoiceId, paymentStatus, currentUserId ?? 0, currentUsername ?? "");
                Console.SetCursorPosition(x + 2, y + 8);
                ShowSuccessMessage("Cập nhật trạng thái thanh toán thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(x + 2, y + 8);
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void PrintInvoice()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - In Hóa Đơn");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Mã hóa đơn: ");
            string? invoiceId = ReadInputWithEscape(x + 14, y + 4);
            if (invoiceId == null) return;

            try
            {
                var (invoiceData, serviceData) = _invoiceBL.GetInvoiceDetailsForDisplay(invoiceId);
                
                if (invoiceData.Rows.Count == 0)
                {
                    ShowErrorMessage("Không tìm thấy hóa đơn để in.");
                    Console.ReadKey();
                    return;
                }

                DataRow invoice = invoiceData.Rows[0];

                // Clear and show invoice
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Chi Tiết Hóa Đơn");
                SetupBox(100, 30);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--- HÓA ĐƠN THANH TOÁN ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("KHÁCH SẠN VTC ACADEMY");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                // Invoice info
                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write($"Số hóa đơn: {invoice["InvoiceID"]}");
                Console.SetCursorPosition(x + 50, y + 8);
                Console.Write($"Ngày: {Convert.ToDateTime(invoice["IssueDate"]):dd/MM/yyyy}");

                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write($"Booking ID: {invoice["BookingID"]}");
                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write(new string('─', width - 4));

                // Customer info
                Console.SetCursorPosition(x + 2, y + 11);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("THÔNG TIN KHÁCH HÀNG:");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write($"Họ tên: {invoice["CustomerName"]}");
                Console.SetCursorPosition(x + 50, y + 12);
                Console.Write($"CMND/CCCD: {invoice["IDCard"]}");

                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write($"Email: {invoice["Email"] ?? "N/A"}");
                Console.SetCursorPosition(x + 50, y + 13);
                Console.Write($"Điện thoại: {invoice["Phone"] ?? "N/A"}");

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write(new string('─', width - 4));

                // Room info
                Console.SetCursorPosition(x + 2, y + 15);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("THÔNG TIN PHÒNG:");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write($"Số phòng: {invoice["RoomNumber"]} - Loại: {invoice["RoomType"]}");

                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write($"Check-in: {Convert.ToDateTime(invoice["CheckInDate"]):dd/MM/yyyy}");
                Console.SetCursorPosition(x + 50, y + 17);
                Console.Write($"Check-out: {Convert.ToDateTime(invoice["CheckOutDate"]):dd/MM/yyyy}");

                int nights = (Convert.ToDateTime(invoice["CheckOutDate"]) - Convert.ToDateTime(invoice["CheckInDate"])).Days;
                decimal roomTotal = nights * Convert.ToDecimal(invoice["RoomPrice"]);

                Console.SetCursorPosition(x + 2, y + 18);
                Console.Write($"Số đêm: {nights}");
                Console.SetCursorPosition(x + 50, y + 18);
                Console.Write($"Giá phòng/đêm: {Convert.ToDecimal(invoice["RoomPrice"]):N0} VND");

                Console.SetCursorPosition(x + 2, y + 19);
                Console.Write(new string('─', width - 4));

                // Payment details
                Console.SetCursorPosition(x + 2, y + 20);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("CHI TIẾT THANH TOÁN:");
                Console.ResetColor();

                Console.SetCursorPosition(x + 2, y + 21);
                Console.Write($"Tiền phòng ({nights} đêm): {roomTotal:N0} VND");

                decimal serviceTotal = 0;
                int currentLine = y + 22;
                if (serviceData.Rows.Count > 0)
                {
                    Console.SetCursorPosition(x + 2, currentLine);
                    Console.Write("Dịch vụ bổ sung:");
                    currentLine++;
                    
                    foreach (DataRow service in serviceData.Rows)
                    {
                        decimal serviceAmount = Convert.ToDecimal(service["TotalPrice"]);
                        serviceTotal += serviceAmount;
                        Console.SetCursorPosition(x + 4, currentLine);
                        Console.Write($"- {service["ServiceName"]} ({service["Type"]}) x{service["Quantity"]}: {serviceAmount:N0} VND");
                        currentLine++;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x + 2, currentLine);
                    Console.Write("Dịch vụ bổ sung: Không có");
                    currentLine++;
                }

                Console.SetCursorPosition(x + 2, currentLine);
                Console.Write(new string('─', width - 4));
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine);
                Console.Write($"Tổng tiền phòng: {roomTotal:N0} VND");
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine);
                Console.Write($"Tổng tiền dịch vụ: {serviceTotal:N0} VND");
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"TỔNG CỘNG: {Convert.ToDecimal(invoice["TotalAmount"]):N0} VND");
                Console.ResetColor();
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine);
                Console.Write($"Trạng thái thanh toán: {invoice["PaymentStatus"]}");
                currentLine++;

                Console.SetCursorPosition(x + 2, currentLine + 1);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Cảm ơn quý khách đã sử dụng dịch vụ!");
                Console.ResetColor();

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
