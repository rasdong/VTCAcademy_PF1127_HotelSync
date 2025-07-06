using System;
using System.Data;
using System.Linq;

namespace HotelManagementSystem
{
    public class ServiceBLL
    {
        private ServiceDAL _serviceDAL;

        public ServiceBLL()
        {
            _serviceDAL = new ServiceDAL();
        }

        // 1. Thêm dịch vụ mới
        public void AddService(string serviceName, string type, string priceInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(type))
                    throw new ArgumentException("Tên dịch vụ và loại dịch vụ không được để trống.");

                if (!IsValidServiceType(type))
                    throw new ArgumentException("Loại dịch vụ không hợp lệ. Chỉ chấp nhận: Food, Laundry, Spa, Other");

                if (!decimal.TryParse(priceInput, out decimal price) || price <= 0)
                    throw new ArgumentException("Giá dịch vụ phải là số dương.");

                if (price > 10000000) // Giới hạn giá tối đa 10 triệu
                    throw new ArgumentException("Giá dịch vụ không được vượt quá 10,000,000 VND.");

                _serviceDAL.AddService(serviceName, type, price, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi thêm dịch vụ: {ex.Message}");
            }
        }

        // 2. Cập nhật thông tin dịch vụ
        public void UpdateService(string serviceIdInput, string serviceName, string type, string priceInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(serviceIdInput, out int serviceId) || serviceId <= 0)
                    throw new ArgumentException("ID dịch vụ phải là số nguyên dương.");

                if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(type))
                    throw new ArgumentException("Tên dịch vụ và loại dịch vụ không được để trống.");

                if (!IsValidServiceType(type))
                    throw new ArgumentException("Loại dịch vụ không hợp lệ. Chỉ chấp nhận: Food, Laundry, Spa, Other");

                if (!decimal.TryParse(priceInput, out decimal price) || price <= 0)
                    throw new ArgumentException("Giá dịch vụ phải là số dương.");

                if (price > 10000000)
                    throw new ArgumentException("Giá dịch vụ không được vượt quá 10,000,000 VND.");

                _serviceDAL.UpdateService(serviceId, serviceName, type, price, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi cập nhật dịch vụ: {ex.Message}");
            }
        }

        // 3. Xóa dịch vụ
        public void DeleteService(string serviceIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (!int.TryParse(serviceIdInput, out int serviceId) || serviceId <= 0)
                    throw new ArgumentException("ID dịch vụ phải là số nguyên dương.");

                _serviceDAL.DeleteService(serviceId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi xóa dịch vụ: {ex.Message}");
            }
        }

        // 4. Xem danh sách tất cả dịch vụ
        public DataTable GetAllServices()
        {
            try
            {
                return _serviceDAL.GetAllServices();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy danh sách dịch vụ: {ex.Message}");
            }
        }

        // 5. Tìm kiếm dịch vụ theo loại
        public DataTable SearchServicesByType(string type)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                    throw new ArgumentException("Loại dịch vụ không được để trống.");

                if (!IsValidServiceType(type))
                    throw new ArgumentException("Loại dịch vụ không hợp lệ. Chỉ chấp nhận: Food, Laundry, Spa, Other");

                return _serviceDAL.SearchServicesByType(type);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tìm kiếm dịch vụ: {ex.Message}");
            }
        }

        // 6. Ghi nhận yêu cầu dịch vụ
        public void AddServiceUsage(string bookingIdInput, string serviceIdInput, string customerIdInput, string quantityInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");

                if (!int.TryParse(serviceIdInput, out int serviceId) || serviceId <= 0)
                    throw new ArgumentException("ID dịch vụ phải là số nguyên dương.");

                if (!int.TryParse(customerIdInput, out int customerId) || customerId <= 0)
                    throw new ArgumentException("ID khách hàng phải là số nguyên dương.");

                if (!int.TryParse(quantityInput, out int quantity) || quantity <= 0)
                    throw new ArgumentException("Số lượng phải là số nguyên dương.");

                if (quantity > 100) // Giới hạn số lượng tối đa
                    throw new ArgumentException("Số lượng không được vượt quá 100.");

                DateTime serviceDate = DateTime.Now;

                _serviceDAL.AddServiceUsage(bookingId, serviceId, customerId, quantity, serviceDate, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi ghi nhận yêu cầu dịch vụ: {ex.Message}");
            }
        }

        // 7. Xem danh sách dịch vụ theo booking
        public DataTable GetServiceUsageByBooking(string bookingIdInput)
        {
            try
            {
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");

                return _serviceDAL.GetServiceUsageByBooking(bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy danh sách dịch vụ theo booking: {ex.Message}");
            }
        }

        // 8. Tính tổng phí dịch vụ cho một booking
        public decimal GetTotalServiceCostByBooking(string bookingIdInput)
        {
            try
            {
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");

                return _serviceDAL.GetTotalServiceCostByBooking(bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tính phí dịch vụ: {ex.Message}");
            }
        }

        // 9. Cập nhật trạng thái thanh toán dịch vụ
        public void UpdateServicePaymentStatus(string usageIdInput, string paymentStatus, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (!int.TryParse(usageIdInput, out int usageId) || usageId <= 0)
                    throw new ArgumentException("ID sử dụng dịch vụ phải là số nguyên dương.");

                if (string.IsNullOrEmpty(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không được để trống.");

                if (!IsValidPaymentStatus(paymentStatus))
                    throw new ArgumentException("Trạng thái thanh toán không hợp lệ. Chỉ chấp nhận: Paid, Unpaid");

                _serviceDAL.UpdateServicePaymentStatus(usageId, paymentStatus, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi cập nhật trạng thái thanh toán: {ex.Message}");
            }
        }

        // 10. Lấy báo cáo dịch vụ theo khoảng thời gian
        public string GetServiceReportSummary(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

                if ((endDate - startDate).TotalDays > 365)
                    throw new ArgumentException("Khoảng thời gian báo cáo không được vượt quá 1 năm.");

                // Tạo báo cáo tổng quan (có thể mở rộng thêm logic phức tạp)
                return $"Báo cáo dịch vụ từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo báo cáo: {ex.Message}");
            }
        }

        // Helper methods
        private bool IsValidServiceType(string type)
        {
            return type == "Food" || type == "Laundry" || type == "Spa" || type == "Other";
        }

        private bool IsValidPaymentStatus(string status)
        {
            return status == "Paid" || status == "Unpaid";
        }

        // 11. Kiểm tra dịch vụ có thể xóa được không
        public bool CanDeleteService(string serviceIdInput)
        {
            try
            {
                if (!int.TryParse(serviceIdInput, out int serviceId) || serviceId <= 0)
                    return false;

                // Kiểm tra xem dịch vụ có đang được sử dụng không
                using (var conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT COUNT(*) FROM ServiceUsage WHERE ServiceID = @ServiceID", conn);
                    cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // 12. Lấy thông tin chi tiết một dịch vụ
        public DataTable GetServiceDetails(string serviceIdInput)
        {
            try
            {
                if (!int.TryParse(serviceIdInput, out int serviceId) || serviceId <= 0)
                    throw new ArgumentException("ID dịch vụ phải là số nguyên dương.");

                DataTable dt = new DataTable();
                using (var conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM Services WHERE ServiceID = @ServiceID", conn);
                    cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                    var reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy thông tin dịch vụ: {ex.Message}");
            }
        }
    }
}
