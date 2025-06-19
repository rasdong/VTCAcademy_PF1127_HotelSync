using System;
using System.Data;

namespace HotelManagementSystem
{
    public class BookingBLL
    {
        private readonly BookingDAL _bookingDAL = new BookingDAL();

        private bool CheckUserPermission(int updatedBy, string requiredPermission)
        {
            // TODO: Kiểm tra quyền từ bảng Roles và Permissions
            return true; // Thay bằng logic thực tế
        }

        public int CreateBooking(string IDCard, string roomIdInput, string checkInDateInput, string checkOutDateInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Kiểm tra các tham số đầu vào
                if (string.IsNullOrWhiteSpace(IDCard))
                    throw new ArgumentException("Số chứng minh nhân dân/căn cước công dân không hợp lệ.");
                if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out int roomId) || roomId <= 0)
                    throw new ArgumentException("ID phòng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(checkInDateInput) || !DateTime.TryParse(checkInDateInput, out DateTime checkInDate))
                    throw new ArgumentException("Ngày check-in không hợp lệ.");
                if (string.IsNullOrWhiteSpace(checkOutDateInput) || !DateTime.TryParse(checkOutDateInput, out DateTime checkOutDate))
                    throw new ArgumentException("Ngày check-out không hợp lệ.");
                if (checkInDate >= checkOutDate)
                    throw new ArgumentException("Ngày check-in phải trước ngày check-out.");
                if (checkInDate < DateTime.Now)
                    throw new ArgumentException("Ngày check-in không thể trước thời gian hiện tại.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền tạo đặt phòng.");

                // Gọi tầng DAL để tạo booking và nhận BookingID
                int bookingId = _bookingDAL.CreateBooking(IDCard, roomId, checkInDate, checkOutDate, updatedBy, updatedByUsername);

                // Trả về BookingID để người dùng sử dụng cho các chức năng tiếp theo
                return bookingId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tạo đặt phòng: {ex.Message}");
            }
        }

        public void CancelBooking(string bookingIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bookingIdInput) || !int.TryParse(bookingIdInput, out int bookingId))
                    throw new ArgumentException("ID đặt phòng không hợp lệ.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền hủy đặt phòng.");

                _bookingDAL.CancelBooking(bookingId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException ex)
            {
                throw; // Truyền lại ArgumentException để giao diện hiển thị thông báo chính xác
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể hủy đặt phòng: {ex.Message}");
            }
        }

        public void CheckIn(string bookingIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Kiểm tra các tham số đầu vào
                if (string.IsNullOrWhiteSpace(bookingIdInput) || !int.TryParse(bookingIdInput, out int bookingId))
                    throw new ArgumentException("ID đặt phòng không hợp lệ.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền thực hiện check-in.");
        
                // Kiểm tra CheckInDate từ bảng Bookings
                DataTable dt = _bookingDAL.GetBookingHistory(bookingId, null); // Lấy thông tin đặt phòng
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                DateTime checkInDate = Convert.ToDateTime(dt.Rows[0]["CheckInDate"]);
                if (checkInDate > DateTime.Now)
                    throw new ArgumentException($"Không thể check-in trước ngày {checkInDate:dd/MM/yyyy HH:mm}.");
        
                // Gọi tầng DAL để thực hiện check-in
                _bookingDAL.CheckIn(bookingId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException ex)
            {
                throw; // Truyền lại ArgumentException để giao diện hiển thị thông báo chính xác
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi check-in: {ex.Message}");
            }
        }

        public void CheckOut(string bookingIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bookingIdInput) || !int.TryParse(bookingIdInput, out int bookingId))
                    throw new ArgumentException("ID đặt phòng không hợp lệ.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền thực hiện check-out.");
        
                DataTable dt = _bookingDAL.GetBookingHistory(bookingId, null);
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                string status = dt.Rows[0]["Status"].ToString();
                if (status != "Active")
                    throw new ArgumentException("Đặt phòng không ở trạng thái Active để check-out.");
        
                _bookingDAL.CheckOut(bookingId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi check-out: {ex.Message}");
            }
        }

        public void ExtendBooking(string bookingIdInput, DateTime newEndDate, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bookingIdInput) || !int.TryParse(bookingIdInput, out int bookingId))
                    throw new ArgumentException("ID đặt phòng không hợp lệ.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (newEndDate <= DateTime.Now)
                    throw new ArgumentException("Ngày gia hạn phải lớn hơn ngày hiện tại.");
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền gia hạn đặt phòng.");

                _bookingDAL.ExtendBooking(bookingId, newEndDate, updatedBy, updatedByUsername);
            }
            catch (ArgumentException ex)
            {
                throw; // Truyền lại ArgumentException để giao diện hiển thị thông báo chính xác
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể gia hạn đặt phòng: {ex.Message}");
            }
        }

        public DataTable GetBookingHistory(int customerId, int? roomId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("ID khách hàng không hợp lệ.");
                return _bookingDAL.GetBookingHistory(customerId, roomId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy lịch sử đặt phòng: {ex.Message}");
            }
        }
    }
}