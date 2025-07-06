using System;
using System.Data;
using System.Linq;

namespace HotelManagementSystem.InvoiceManagement
{
    public class InvoiceBL
    {
        private InvoiceDAL _invoiceDAL;

        public InvoiceBL()
        {
            _invoiceDAL = new InvoiceDAL();
        }

        // 1. Tạo hóa đơn (tính tiền dựa trên số ngày lưu trú, giá phòng và dịch vụ)
        public void CreateInvoice(string bookingIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID booking phải là số nguyên dương.");

                int invoiceId = _invoiceDAL.CreateInvoiceWithCalculation(bookingId, updatedBy, updatedByUsername);
                if (invoiceId <= 0)
                    throw new Exception("Không thể tạo hóa đơn tự động.");

                Console.WriteLine($"Tạo hóa đơn thành công! Mã hóa đơn: {invoiceId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }

        // 2. Xem danh sách hóa đơn (có thể lọc theo khách hàng, ngày, trạng thái)
        // 2. Xem danh sách hóa đơn (có thể lọc theo khách hàng, ngày, trạng thái)
        public void ViewAllInvoices(string? customerFilter = null, DateTime? fromDate = null, DateTime? toDate = null, string? statusFilter = null)
        {
            try
            {
                DataTable invoices = _invoiceDAL.GetAllInvoices(customerFilter, fromDate, toDate, statusFilter);
                
                if (invoices.Rows.Count == 0)
                {
                    Console.WriteLine("Không có hóa đơn nào trong hệ thống.");
                    return;
                }

                Console.WriteLine("=== DANH SÁCH HÓA ĐƠN ===");
                Console.WriteLine();
                Console.WriteLine("{0,-10} {1,-15} {2,-25} {3,-20} {4,-15} {5,-20} {6,-15}",
                    "Mã HĐ", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái", "Phòng");
                Console.WriteLine(new string('-', 125));

                foreach (DataRow row in invoices.Rows)
                {
                    string customerName = row["CustomerName"]?.ToString() ?? "";
                    Console.WriteLine("{0,-10} {1,-15} {2,-25} {3,-20:C0} {4,-15:dd/MM/yyyy} {5,-20} {6,-15}",
                        row["InvoiceID"],
                        row["BookingID"],
                        customerName.Length > 24 ? customerName.Substring(0, 21) + "..." : customerName,
                        Convert.ToDecimal(row["TotalAmount"]),
                        Convert.ToDateTime(row["IssueDate"]),
                        row["PaymentStatus"],
                        row["RoomNumber"]);
                }
                string[] headers = new[] { "ID", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái" };
                int[] columnWidths = new[] { 8, 12, 25, 15, 15, 12 };

                Console.SetCursorPosition(2, 14);
                Console.ForegroundColor = ConsoleColor.Magenta;
                for (int col = 0; col < headers.Length; col++)
                {
                    Console.Write(headers[col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();

                Console.SetCursorPosition(2, 15);
                Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                Console.ResetColor();

                // Display invoice data
                int maxRows = Math.Min(invoices.Rows.Count, 10);
                for (int i = 0; i < maxRows; i++)
                {
                    DataRow row = invoices.Rows[i];
                    Console.SetCursorPosition(2, 16 + i * 2);
                    
                    string id = (row["InvoiceID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                    string bookingId = (row["BookingID"]?.ToString() ?? "").PadRight(columnWidths[1]);
                    string customerName = row["CustomerName"]?.ToString() ?? "";
                    customerName = customerName.Length > 23 ? customerName.Substring(0, 20) + "..." : customerName;
                    customerName = customerName.PadRight(columnWidths[2]);
                    string totalAmount = Convert.ToDecimal(row["TotalAmount"]).ToString("N0").PadRight(columnWidths[3]);
                    string issueDate = Convert.ToDateTime(row["IssueDate"]).ToString("dd/MM/yyyy").PadRight(columnWidths[4]);
                    string paymentStatus = (row["PaymentStatus"]?.ToString() ?? "").PadRight(columnWidths[5]);

                    Console.Write(id);
                    Console.Write(bookingId);
                    Console.Write(customerName);
                    Console.Write(totalAmount);
                    Console.Write(issueDate);
                    Console.Write(paymentStatus);

                    if (i < maxRows - 1)
                    {
                        Console.SetCursorPosition(2, 17 + i * 2);
                        Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                    }
                }

                if (invoices.Rows.Count > 10)
                {
                    Console.SetCursorPosition(2, 16 + maxRows * 2);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Hiển thị {maxRows}/{invoices.Rows.Count} hóa đơn. Sử dụng tìm kiếm để xem chi tiết.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xem danh sách hóa đơn: {ex.Message}");
            }
        }

        // 3. Tìm kiếm hóa đơn (theo mã hóa đơn, tên khách, khoảng thời gian)
        public void SearchInvoices(string? invoiceId = null, string? customerName = null, 
                                  string? fromDateInput = null, string? toDateInput = null, string? paymentStatus = null)
        {
            try
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;

                // Parse dates
                if (!string.IsNullOrEmpty(fromDateInput))
                {
                    if (!DateTime.TryParseExact(fromDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                        throw new ArgumentException("Ngày bắt đầu không đúng định dạng (dd/MM/yyyy).");
                    fromDate = parsedFromDate;
                }

                if (!string.IsNullOrEmpty(toDateInput))
                {
                    if (!DateTime.TryParseExact(toDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                        throw new ArgumentException("Ngày kết thúc không đúng định dạng (dd/MM/yyyy).");
                    toDate = parsedToDate;
                }

                // Validate payment status
                if (!string.IsNullOrEmpty(paymentStatus) && !IsValidPaymentStatus(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không hợp lệ. Chỉ chấp nhận: Paid, Unpaid");

                // Validate date range
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                    throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

                DataTable results = _invoiceDAL.SearchInvoices(invoiceId, customerName, fromDate, toDate, paymentStatus);
                
                if (results.Rows.Count == 0)
                {
                    Console.WriteLine("=== KẾT QUẢ TÌM KIẾM HÓA ĐƠN ===");
                    Console.WriteLine();
                    Console.WriteLine("Không tìm thấy hóa đơn nào phù hợp với điều kiện tìm kiếm.");
                    return;
                }

                Console.WriteLine($"=== TÌM THẤY {results.Rows.Count} HÓA ĐƠN ===");
                Console.WriteLine();
                Console.WriteLine("{0,-10} {1,-15} {2,-25} {3,-20} {4,-15} {5,-20}",
                    "Mã HĐ", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái");
                Console.WriteLine(new string('-', 105));

                // Display table headers
                string[] headers = new[] { "ID", "Mã Booking", "Khách hàng", "Tổng tiền", "Ngày phát hành", "Trạng thái" };
                int[] columnWidths = new[] { 8, 12, 25, 15, 15, 12 };

                Console.SetCursorPosition(2, 14);
                Console.ForegroundColor = ConsoleColor.Magenta;
                for (int col = 0; col < headers.Length; col++)
                {
                    Console.Write(headers[col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();

                Console.SetCursorPosition(2, 15);
                Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                Console.ResetColor();

                // Display search results
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    DataRow row = results.Rows[i];
                    Console.SetCursorPosition(2, 16 + i * 2);
                    
                    string id = (row["InvoiceID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                    string bookingId = (row["BookingID"]?.ToString() ?? "").PadRight(columnWidths[1]);
                    string customerName_display = row["CustomerName"]?.ToString() ?? "";
                    customerName_display = customerName_display.Length > 23 ? customerName_display.Substring(0, 20) + "..." : customerName_display;
                    customerName_display = customerName_display.PadRight(columnWidths[2]);
                    string totalAmount = Convert.ToDecimal(row["TotalAmount"]).ToString("N0").PadRight(columnWidths[3]);
                    string issueDate = Convert.ToDateTime(row["IssueDate"]).ToString("dd/MM/yyyy").PadRight(columnWidths[4]);
                    string paymentStatus_display = (row["PaymentStatus"]?.ToString() ?? "").PadRight(columnWidths[5]);

                    Console.Write(id);
                    Console.Write(bookingId);
                    Console.Write(customerName_display);
                    Console.Write(totalAmount);
                    Console.Write(issueDate);
                    Console.Write(paymentStatus_display);

                    if (i < results.Rows.Count - 1)
                    {
                        Console.SetCursorPosition(2, 17 + i * 2);
                        Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}");
            }
        }

        // 4. Cập nhật trạng thái hóa đơn (đã thanh toán/chưa thanh toán)
        public void UpdatePaymentStatus(string invoiceIdInput, string paymentStatus, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("Mã hóa đơn phải là số nguyên dương.");

                if (!IsValidPaymentStatus(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không hợp lệ. Chỉ chấp nhận: Paid, Unpaid");

                bool result = _invoiceDAL.UpdateInvoicePaymentStatus(invoiceId, paymentStatus, updatedBy, updatedByUsername);
                if (!result)
                    throw new Exception("Không thể cập nhật trạng thái thanh toán.");

                Console.WriteLine("Cập nhật trạng thái thanh toán thành công!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        // 5. In hóa đơn (xuất hóa đơn dạng văn bản với thông tin chi tiết)
        public void PrintInvoice(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("Mã hóa đơn phải là số nguyên dương.");

                DataTable invoiceData = _invoiceDAL.GetInvoiceForPrint(invoiceId);
                
                if (invoiceData.Rows.Count == 0)
                {
                    Console.WriteLine("=== CHI TIẾT HÓA ĐƠN ===");
                    Console.WriteLine();
                    Console.WriteLine("Không tìm thấy hóa đơn để in.");
                    return;
                }

                DataRow invoice = invoiceData.Rows[0];
                
                // Display invoice details
                Console.WriteLine(new string('=', 80));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition((Console.WindowWidth - "Hệ Thống Quản Lý Khách Sạn - Chi Tiết Hóa Đơn".Length - 4) / 2, 1);
                Console.Write("═══ Hệ Thống Quản Lý Khách Sạn - Chi Tiết Hóa Đơn ═══");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine();
                
                Console.WriteLine(new string('=', 80));
                Console.WriteLine("                        HÓA ĐƠN THANH TOÁN");
                Console.WriteLine("                         KHÁCH SẠN VTC ACADEMY");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine($"Số hóa đơn: {invoice["InvoiceID"]}                    Ngày: {Convert.ToDateTime(invoice["IssueDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Booking ID: {invoice["BookingID"]}");
                Console.WriteLine();
                Console.WriteLine("THÔNG TIN KHÁCH HÀNG:");
                Console.WriteLine($"Họ tên: {invoice["CustomerName"]}");
                Console.WriteLine($"CMND/CCCD: {invoice["IDCard"]}");
                Console.WriteLine($"Email: {invoice["Email"] ?? "N/A"}");
                Console.WriteLine($"Điện thoại: {invoice["Phone"] ?? "N/A"}");
                Console.WriteLine();
                Console.WriteLine("THÔNG TIN PHÒNG:");
                Console.WriteLine($"Số phòng: {invoice["RoomNumber"]} - Loại: {invoice["RoomType"]}");
                Console.WriteLine($"Check-in: {Convert.ToDateTime(invoice["CheckInDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Check-out: {Convert.ToDateTime(invoice["CheckOutDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Số đêm: {(Convert.ToDateTime(invoice["CheckOutDate"]) - Convert.ToDateTime(invoice["CheckInDate"])).Days}");
                Console.WriteLine($"Giá phòng/đêm: {Convert.ToDecimal(invoice["RoomPrice"]):C0} VND");
                
                // Tính tiền phòng
                int nights = (Convert.ToDateTime(invoice["CheckOutDate"]) - Convert.ToDateTime(invoice["CheckInDate"])).Days;
                decimal roomTotal = nights * Convert.ToDecimal(invoice["RoomPrice"]);
                
                Console.WriteLine();
                Console.WriteLine(new string('-', 80));
                Console.WriteLine("CHI TIẾT THANH TOÁN:");
                Console.WriteLine($"Tiền phòng ({nights} đêm): {roomTotal:C0} VND");
                
                // Hiển thị dịch vụ bổ sung
                DataTable services = _invoiceDAL.GetServiceUsageByInvoice(invoiceId);
                decimal serviceTotal = 0;
                if (services.Rows.Count > 0)
                {
                    Console.WriteLine("Dịch vụ bổ sung:");
                    foreach (DataRow service in services.Rows)
                    {
                        decimal serviceAmount = Convert.ToDecimal(service["TotalPrice"]);
                        serviceTotal += serviceAmount;
                        Console.WriteLine($"  - {service["ServiceName"]} ({service["Type"]}) x{service["Quantity"]}: {serviceAmount:C0} VND");
                    }
                }
                else
                {
                    Console.WriteLine("Dịch vụ bổ sung: Không có");
                }
                
                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Tổng tiền phòng: {roomTotal:C0} VND");
                Console.WriteLine($"Tổng tiền dịch vụ: {serviceTotal:C0} VND");
                Console.WriteLine($"TỔNG CỘNG: {Convert.ToDecimal(invoice["TotalAmount"]):C0} VND");
                Console.WriteLine($"Trạng thái thanh toán: {invoice["PaymentStatus"]}");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine("              Cảm ơn quý khách đã sử dụng dịch vụ!");
                Console.WriteLine("                     Hẹn gặp lại!");
                Console.WriteLine(new string('=', 80));
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi in hóa đơn: {ex.Message}");
            }
        }

        // 6. Xem chi tiết hóa đơn
        public void ViewInvoiceDetails(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("Mã hóa đơn phải là số nguyên dương.");

                DataTable invoice = _invoiceDAL.GetInvoiceById(invoiceId);
                
                if (invoice.Rows.Count == 0)
                {
                    Console.WriteLine("Không tìm thấy hóa đơn với mã này.");
                    return;
                }

                DataRow row = invoice.Rows[0];
                Console.WriteLine("\n=== CHI TIẾT HÓA ĐƠN ===");
                Console.WriteLine($"Mã hóa đơn: {row["InvoiceID"]}");
                Console.WriteLine($"Mã booking: {row["BookingID"]}");
                Console.WriteLine($"Khách hàng: {row["CustomerName"]}");
                Console.WriteLine($"CMND/CCCD: {row["IDCard"]}");
                Console.WriteLine($"Phòng: {row["RoomNumber"]} ({row["RoomType"]})");
                Console.WriteLine($"Giá phòng/đêm: {Convert.ToDecimal(row["RoomPrice"]):C0} VND");
                Console.WriteLine($"Check-in: {Convert.ToDateTime(row["CheckInDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Check-out: {Convert.ToDateTime(row["CheckOutDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Tổng tiền: {Convert.ToDecimal(row["TotalAmount"]):C0} VND");
                Console.WriteLine($"Ngày phát hành: {Convert.ToDateTime(row["IssueDate"]):dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Trạng thái thanh toán: {row["PaymentStatus"]}");

                // Hiển thị dịch vụ sử dụng
                ViewInvoiceServices(invoiceId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xem chi tiết hóa đơn: {ex.Message}");
            }
        }

        // Helper method để hiển thị dịch vụ trong hóa đơn
        private void ViewInvoiceServices(int invoiceId)
        {
            try
            {
                DataTable services = _invoiceDAL.GetServiceUsageByInvoice(invoiceId);
                
                if (services.Rows.Count > 0)
                {
                    Console.WriteLine("\n--- DỊCH VỤ SỬ DỤNG ---");
                    Console.WriteLine("{0,-25} {1,-15} {2,-10} {3,-15} {4,-15} {5,-15}",
                        "Tên dịch vụ", "Loại", "Số lượng", "Đơn giá", "Thành tiền", "Ngày sử dụng");
                    Console.WriteLine(new string('-', 100));

                    decimal totalServiceAmount = 0;
                    foreach (DataRow row in services.Rows)
                    {
                        decimal totalPrice = Convert.ToDecimal(row["TotalPrice"]);
                        totalServiceAmount += totalPrice;
                        string serviceName = row["ServiceName"]?.ToString() ?? "";
                        Console.WriteLine("{0,-25} {1,-15} {2,-10} {3,-15:C0} {4,-15:C0} {5,-15:dd/MM/yyyy}",
                            serviceName.Length > 24 ? serviceName.Substring(0, 21) + "..." : serviceName,
                            row["Type"],
                            row["Quantity"],
                            Convert.ToDecimal(row["UnitPrice"]),
                            totalPrice,
                            Convert.ToDateTime(row["Date"]));
                    }
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine($"Tổng tiền dịch vụ: {totalServiceAmount:C0} VND");
                }
                else
                {
                    Console.WriteLine("\n--- Không có dịch vụ nào được sử dụng ---");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi hiển thị dịch vụ: {ex.Message}");
            }
        }

        // Method để lấy tất cả hóa đơn (dùng cho UI)
        public DataTable GetAllInvoices()
        {
            try
            {
                return _invoiceDAL.GetAllInvoices();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
            }
        }

        // Method to get DataTable for UI display
        public DataTable GetAllInvoicesForDisplay(string? customerFilter = null, DateTime? fromDate = null, DateTime? toDate = null, string? statusFilter = null)
        {
            try
            {
                return _invoiceDAL.GetAllInvoices(customerFilter, fromDate, toDate, statusFilter);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
            }
        }

        // Method to search invoices and return DataTable for UI display
        public DataTable SearchInvoicesForDisplay(string? invoiceId = null, string? customerName = null, 
                                                 string? fromDateInput = null, string? toDateInput = null, string? paymentStatus = null)
        {
            try
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;

                // Parse dates
                if (!string.IsNullOrEmpty(fromDateInput))
                {
                    if (!DateTime.TryParseExact(fromDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                        throw new ArgumentException("Ngày bắt đầu không đúng định dạng (dd/MM/yyyy).");
                    fromDate = parsedFromDate;
                }

                if (!string.IsNullOrEmpty(toDateInput))
                {
                    if (!DateTime.TryParseExact(toDateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                        throw new ArgumentException("Ngày kết thúc không đúng định dạng (dd/MM/yyyy).");
                    toDate = parsedToDate;
                }

                // Validate payment status
                if (!string.IsNullOrEmpty(paymentStatus) && !IsValidPaymentStatus(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không hợp lệ. Chỉ chấp nhận: Paid, Unpaid");

                // Validate date range
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                    throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

                return _invoiceDAL.SearchInvoices(invoiceId, customerName, fromDate, toDate, paymentStatus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}");
            }
        }

        // Helper method để kiểm tra trạng thái thanh toán hợp lệ
        private bool IsValidPaymentStatus(string status)
        {
            string[] validStatuses = { "Paid", "Unpaid" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        // Method to get invoice details for UI display (returns structured data instead of printing)
        public (DataTable InvoiceData, DataTable ServiceData) GetInvoiceDetailsForDisplay(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("Mã hóa đơn phải là số nguyên dương.");

                DataTable invoiceData = _invoiceDAL.GetInvoiceForPrint(invoiceId);
                
                if (invoiceData.Rows.Count == 0)
                    throw new Exception("Không tìm thấy hóa đơn.");

                DataTable serviceData = _invoiceDAL.GetServiceUsageByInvoice(invoiceId);
                
                return (invoiceData, serviceData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}");
            }
        }
    }
}
