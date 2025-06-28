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

        // 1. Tạo hóa đơn mới
        public void CreateInvoice(string bookingIdInput, string customerIdInput, string totalAmountInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID booking phải là số nguyên dương.");

                if (!int.TryParse(customerIdInput, out int customerId) || customerId <= 0)
                    throw new ArgumentException("ID khách hàng phải là số nguyên dương.");

                if (!decimal.TryParse(totalAmountInput, out decimal totalAmount) || totalAmount < 0)
                    throw new ArgumentException("Tổng tiền phải là số không âm.");

                if (totalAmount > 100000000) // Giới hạn 100 triệu
                    throw new ArgumentException("Tổng tiền không được vượt quá 100,000,000 VND.");

                bool result = _invoiceDAL.AddInvoice(bookingId, customerId, totalAmount, updatedBy, updatedByUsername);
                if (!result)
                    throw new Exception("Không thể tạo hóa đơn. Vui lòng kiểm tra lại thông tin.");

                Console.WriteLine("Tạo hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo hóa đơn: {ex.Message}");
            }
        }

        // 2. Tạo hóa đơn với tính toán tự động
        public void CreateInvoiceWithAutoCalculation(string bookingIdInput, string customerIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID booking phải là số nguyên dương.");

                if (!int.TryParse(customerIdInput, out int customerId) || customerId <= 0)
                    throw new ArgumentException("ID khách hàng phải là số nguyên dương.");

                int invoiceId = _invoiceDAL.CreateInvoiceWithCalculation(bookingId, customerId, updatedBy, updatedByUsername);
                if (invoiceId <= 0)
                    throw new Exception("Không thể tạo hóa đơn với tính toán tự động.");

                Console.WriteLine($"Tạo hóa đơn thành công! ID hóa đơn: {invoiceId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo hóa đơn tự động: {ex.Message}");
            }
        }

        // 3. Cập nhật trạng thái thanh toán
        public void UpdatePaymentStatus(string invoiceIdInput, string paymentStatus, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("ID hóa đơn phải là số nguyên dương.");

                if (!IsValidPaymentStatus(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không hợp lệ. Chỉ chấp nhận: Pending, Paid, Cancelled");

                bool result = _invoiceDAL.UpdateInvoicePaymentStatus(invoiceId, paymentStatus, updatedBy, updatedByUsername);
                if (!result)
                    throw new Exception("Không thể cập nhật trạng thái thanh toán.");

                Console.WriteLine("Cập nhật trạng thái thanh toán thành công!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi cập nhật trạng thái: {ex.Message}");
            }
        }

        // 4. Tính toán số tiền hóa đơn
        public void CalculateInvoiceAmount(string bookingIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID booking phải là số nguyên dương.");

                decimal totalAmount = _invoiceDAL.CalculateInvoiceAmount(bookingId);
                
                Console.WriteLine($"Tổng tiền cho booking ID {bookingId}: {totalAmount:C0} VND");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tính toán: {ex.Message}");
            }
        }

        // 5. Xem danh sách tất cả hóa đơn
        public void ViewAllInvoices()
        {
            try
            {
                DataTable invoices = _invoiceDAL.GetAllInvoices();
                
                if (invoices.Rows.Count == 0)
                {
                    Console.WriteLine("Không có hóa đơn nào trong hệ thống.");
                    return;
                }

                Console.WriteLine("\n=== DANH SÁCH HÓA ĐƠN ===");
                Console.WriteLine("{0,-10} {1,-15} {2,-25} {3,-20} {4,-15} {5,-20} {6,-15}",
                    "ID", "BookingID", "Khách hàng", "Tổng tiền", "Ngày tạo", "Trạng thái", "Phòng");
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi xem danh sách: {ex.Message}");
            }
        }

        // 6. Xem chi tiết hóa đơn
        public void ViewInvoiceDetails(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("ID hóa đơn phải là số nguyên dương.");

                DataTable invoice = _invoiceDAL.GetInvoiceById(invoiceId);
                
                if (invoice.Rows.Count == 0)
                {
                    Console.WriteLine("Không tìm thấy hóa đơn với ID này.");
                    return;
                }

                DataRow row = invoice.Rows[0];
                Console.WriteLine("\n=== CHI TIẾT HÓA ĐƠN ===");
                Console.WriteLine($"ID Hóa đơn: {row["InvoiceID"]}");
                Console.WriteLine($"ID Booking: {row["BookingID"]}");
                Console.WriteLine($"Khách hàng: {row["CustomerName"]}");
                Console.WriteLine($"CMND/CCCD: {row["IDCard"]}");
                Console.WriteLine($"Phòng: {row["RoomNumber"]} ({row["RoomType"]})");
                Console.WriteLine($"Giá phòng/đêm: {Convert.ToDecimal(row["RoomPrice"]):C0} VND");
                Console.WriteLine($"Check-in: {Convert.ToDateTime(row["CheckInDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Check-out: {Convert.ToDateTime(row["CheckOutDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Tổng tiền: {Convert.ToDecimal(row["TotalAmount"]):C0} VND");
                Console.WriteLine($"Ngày tạo: {Convert.ToDateTime(row["IssueDate"]):dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Trạng thái: {row["PaymentStatus"]}");

                // Hiển thị dịch vụ sử dụng
                ViewInvoiceServices(invoiceId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi xem chi tiết: {ex.Message}");
            }
        }

        // 7. Xem dịch vụ trong hóa đơn
        private void ViewInvoiceServices(int invoiceId)
        {
            try
            {
                DataTable services = _invoiceDAL.GetServiceUsageByInvoice(invoiceId);
                
                if (services.Rows.Count > 0)
                {
                    Console.WriteLine("\n--- DỊCH VỤ SỬ DỤNG ---");
                    Console.WriteLine("{0,-25} {1,-10} {2,-15} {3,-15} {4,-15}",
                        "Tên dịch vụ", "Số lượng", "Đơn giá", "Thành tiền", "Ngày sử dụng");
                    Console.WriteLine(new string('-', 85));

                    decimal totalServiceAmount = 0;
                    foreach (DataRow row in services.Rows)
                    {
                        decimal totalPrice = Convert.ToDecimal(row["TotalPrice"]);
                        totalServiceAmount += totalPrice;
                        string serviceName = row["ServiceName"]?.ToString() ?? "";
                        Console.WriteLine("{0,-25} {1,-10} {2,-15:C0} {3,-15:C0} {4,-15:dd/MM/yyyy}",
                            serviceName.Length > 24 ? serviceName.Substring(0, 21) + "..." : serviceName,
                            row["Quantity"],
                            Convert.ToDecimal(row["UnitPrice"]),
                            totalPrice,
                            Convert.ToDateTime(row["Date"]));
                    }
                    Console.WriteLine(new string('-', 85));
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

        // 8. Tìm kiếm hóa đơn
        public void SearchInvoices(string? customerName = null, string? fromDateInput = null, string? toDateInput = null, 
                                  string? paymentStatus = null, string? minAmountInput = null, string? maxAmountInput = null)
        {
            try
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;
                decimal? minAmount = null;
                decimal? maxAmount = null;

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

                // Parse amounts
                if (!string.IsNullOrEmpty(minAmountInput))
                {
                    if (!decimal.TryParse(minAmountInput, out decimal parsedMinAmount) || parsedMinAmount < 0)
                        throw new ArgumentException("Số tiền tối thiểu phải là số không âm.");
                    minAmount = parsedMinAmount;
                }

                if (!string.IsNullOrEmpty(maxAmountInput))
                {
                    if (!decimal.TryParse(maxAmountInput, out decimal parsedMaxAmount) || parsedMaxAmount < 0)
                        throw new ArgumentException("Số tiền tối đa phải là số không âm.");
                    maxAmount = parsedMaxAmount;
                }

                // Validate payment status
                if (!string.IsNullOrEmpty(paymentStatus) && !IsValidPaymentStatus(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không hợp lệ. Chỉ chấp nhận: Pending, Paid, Cancelled");

                // Validate date range
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                    throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

                // Validate amount range
                if (minAmount.HasValue && maxAmount.HasValue && minAmount > maxAmount)
                    throw new ArgumentException("Số tiền tối thiểu phải nhỏ hơn hoặc bằng số tiền tối đa.");

                DataTable results = _invoiceDAL.SearchInvoices(customerName, fromDate, toDate, paymentStatus, minAmount, maxAmount);
                
                if (results.Rows.Count == 0)
                {
                    Console.WriteLine("Không tìm thấy hóa đơn nào phù hợp với điều kiện tìm kiếm.");
                    return;
                }

                Console.WriteLine($"\n=== TÌM THẤY {results.Rows.Count} HÓA ĐƠN ===");
                Console.WriteLine("{0,-10} {1,-15} {2,-25} {3,-20} {4,-15} {5,-20}",
                    "ID", "BookingID", "Khách hàng", "Tổng tiền", "Ngày tạo", "Trạng thái");
                Console.WriteLine(new string('-', 105));

                foreach (DataRow row in results.Rows)
                {
                    string custName = row["CustomerName"]?.ToString() ?? "";
                    Console.WriteLine("{0,-10} {1,-15} {2,-25} {3,-20:C0} {4,-15:dd/MM/yyyy} {5,-20}",
                        row["InvoiceID"],
                        row["BookingID"],
                        custName.Length > 24 ? custName.Substring(0, 21) + "..." : custName,
                        Convert.ToDecimal(row["TotalAmount"]),
                        Convert.ToDateTime(row["IssueDate"]),
                        row["PaymentStatus"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tìm kiếm: {ex.Message}");
            }
        }

        // 9. In hóa đơn
        public void PrintInvoice(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("ID hóa đơn phải là số nguyên dương.");

                DataTable invoiceData = _invoiceDAL.GetInvoiceForPrint(invoiceId);
                
                if (invoiceData.Rows.Count == 0)
                {
                    Console.WriteLine("Không tìm thấy hóa đơn để in.");
                    return;
                }

                DataRow invoice = invoiceData.Rows[0];
                
                Console.WriteLine("\n" + new string('=', 80));
                Console.WriteLine("                        HÓA ĐƠN THANH TOÁN");
                Console.WriteLine("                         KHÁCH SẠN ABC");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine($"Số hóa đơn: {invoice["InvoiceID"]}                    Ngày: {Convert.ToDateTime(invoice["IssueDate"]):dd/MM/yyyy}");
                Console.WriteLine($"Booking ID: {invoice["BookingID"]}");
                Console.WriteLine();
                Console.WriteLine("THÔNG TIN KHÁCH HÀNG:");
                Console.WriteLine($"Họ tên: {invoice["CustomerName"]}");
                Console.WriteLine($"CMND/CCCD: {invoice["IDCard"]}");
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
                
                // Hiển thị dịch vụ
                DataTable services = _invoiceDAL.GetServiceUsageByInvoice(invoiceId);
                decimal serviceTotal = 0;
                if (services.Rows.Count > 0)
                {
                    Console.WriteLine("Dịch vụ:");
                    foreach (DataRow service in services.Rows)
                    {
                        decimal serviceAmount = Convert.ToDecimal(service["TotalPrice"]);
                        serviceTotal += serviceAmount;
                        Console.WriteLine($"  - {service["ServiceName"]} x{service["Quantity"]}: {serviceAmount:C0} VND");
                    }
                }
                
                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Tổng tiền phòng: {roomTotal:C0} VND");
                Console.WriteLine($"Tổng tiền dịch vụ: {serviceTotal:C0} VND");
                Console.WriteLine($"TỔNG CỘNG: {Convert.ToDecimal(invoice["TotalAmount"]):C0} VND");
                Console.WriteLine($"Trạng thái: {invoice["PaymentStatus"]}");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine("              Cảm ơn quý khách đã sử dụng dịch vụ!");
                Console.WriteLine(new string('=', 80));
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi in hóa đơn: {ex.Message}");
            }
        }

        // 10. Xóa hóa đơn (Admin only)
        public void DeleteInvoice(string invoiceIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("ID hóa đơn phải là số nguyên dương.");

                Console.Write($"Bạn có chắc chắn muốn xóa hóa đơn ID {invoiceId}? (y/n): ");
                string? confirm = Console.ReadLine();
                if (confirm?.ToLower() != "y")
                {
                    Console.WriteLine("Hủy thao tác xóa.");
                    return;
                }

                bool result = _invoiceDAL.DeleteInvoice(invoiceId, updatedBy, updatedByUsername);
                if (!result)
                    throw new Exception("Không thể xóa hóa đơn.");

                Console.WriteLine("Xóa hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi xóa hóa đơn: {ex.Message}");
            }
        }

        // Helper methods
        private bool IsValidPaymentStatus(string status)
        {
            string[] validStatuses = { "Pending", "Paid", "Cancelled" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
