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
        public int CreateInvoice(string bookingIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID booking phải là số nguyên dương.");

                int invoiceId = _invoiceDAL.CreateInvoiceWithCalculation(bookingId, updatedBy, updatedByUsername);
                if (invoiceId <= 0)
                    throw new Exception("Không thể tạo hóa đơn tự động.");

                // Trả về invoiceId thay vì void
                return invoiceId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }

        // 2. Xem danh sách hóa đơn (có thể lọc theo khách hàng, ngày, trạng thái)
        // 2. Xem danh sách hóa đơn - chỉ dùng để validation/logic, UI sử dụng GetAllInvoicesForDisplay
        public void ViewAllInvoices(string? customerFilter = null, DateTime? fromDate = null, DateTime? toDate = null, string? statusFilter = null)
        {
            try
            {
                DataTable invoices = _invoiceDAL.GetAllInvoices(customerFilter, fromDate, toDate, statusFilter);
                // Business logic layer không nên có Console output, UI sẽ xử lý hiển thị
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
            }
        }

        // 3. Tìm kiếm hóa đơn - chỉ dùng để validation/logic, UI sử dụng SearchInvoicesForDisplay
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
                // Business logic layer không nên có Console output, UI sẽ xử lý hiển thị
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

                // Không in ra console, trả về thành công cho UI xử lý
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        // 5. In hóa đơn - chỉ dùng để validation/logic, UI sử dụng GetInvoiceDetailsForDisplay
        public void PrintInvoice(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("Mã hóa đơn phải là số nguyên dương.");

                DataTable invoiceData = _invoiceDAL.GetInvoiceForPrint(invoiceId);
                
                if (invoiceData.Rows.Count == 0)
                    throw new Exception("Không tìm thấy hóa đơn để in.");

                // Business logic layer không nên có Console output, UI sẽ xử lý hiển thị
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi in hóa đơn: {ex.Message}");
            }
        }

        // 6. Xem chi tiết hóa đơn - chỉ dùng để validation/logic, UI sử dụng GetInvoiceDetailsForDisplay
        public void ViewInvoiceDetails(string invoiceIdInput)
        {
            try
            {
                // Validation
                if (!int.TryParse(invoiceIdInput, out int invoiceId) || invoiceId <= 0)
                    throw new ArgumentException("Mã hóa đơn phải là số nguyên dương.");

                DataTable invoice = _invoiceDAL.GetInvoiceById(invoiceId);
                
                if (invoice.Rows.Count == 0)
                    throw new Exception("Không tìm thấy hóa đơn với mã này.");

                // Business logic layer không nên có Console output, UI sẽ xử lý hiển thị
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xem chi tiết hóa đơn: {ex.Message}");
            }
        }

        // Helper method để lấy dịch vụ trong hóa đơn - không có Console output
        private void ViewInvoiceServices(int invoiceId)
        {
            try
            {
                DataTable services = _invoiceDAL.GetServiceUsageByInvoice(invoiceId);
                // Business logic layer không nên có Console output, UI sẽ xử lý hiển thị
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dịch vụ: {ex.Message}");
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
