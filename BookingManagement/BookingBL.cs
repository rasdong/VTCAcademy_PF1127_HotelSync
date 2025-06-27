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

        private void ValidateBookingInput(
            string? idCard = null,
            string? bookingIdInput = null,
            string? roomIdInput = null,
            string? checkInDateInput = null,
            string? checkOutDateInput = null,
            DateTime? newEndDate = null,
            int updatedBy = 0,
            string? updatedByUsername = null,
            bool isCreate = false,
            bool isExtend = false)
        {
            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(idCard))
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
            }
            else if (bookingIdInput != null)
            {  
                if (string.IsNullOrWhiteSpace(bookingIdInput) || !int.TryParse(bookingIdInput, out int bookingId))
                    throw new ArgumentException("ID đặt phòng không hợp lệ.");
            }

            if (isExtend && newEndDate.HasValue && newEndDate <= DateTime.Now)
                throw new ArgumentException("Ngày gia hạn phải lớn hơn ngày hiện tại.");

            if (updatedBy <= 0)
                throw new ArgumentException("ID người dùng không hợp lệ.");
            if (string.IsNullOrWhiteSpace(updatedByUsername))
                throw new ArgumentException("Tên người dùng không được để trống.");
        }

        public int CreateBooking(string IDCard, string roomIdInput, string checkInDateInput, string checkOutDateInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateBookingInput(IDCard, null, roomIdInput, checkInDateInput, checkOutDateInput, null, updatedBy, updatedByUsername, isCreate: true);
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền tạo đặt phòng.");

                int roomId = int.Parse(roomIdInput);
                DateTime checkInDate = DateTime.Parse(checkInDateInput);
                DateTime checkOutDate = DateTime.Parse(checkOutDateInput);
                int bookingId = _bookingDAL.CreateBooking(IDCard, roomId, checkInDate, checkOutDate, updatedBy, updatedByUsername);

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
                ValidateBookingInput(bookingIdInput: bookingIdInput, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền hủy đặt phòng.");

                int bookingId = int.Parse(bookingIdInput);
                _bookingDAL.CancelBooking(bookingId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException)
            {
                throw;
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
                ValidateBookingInput(bookingIdInput: bookingIdInput, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền thực hiện check-in.");

                int bookingId = int.Parse(bookingIdInput);
                DataTable dt = _bookingDAL.GetBookingHistory(bookingId, null);
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                DateTime checkInDate = Convert.ToDateTime(dt.Rows[0]["CheckInDate"]);
                if (checkInDate > DateTime.Now)
                    throw new ArgumentException($"Không thể check-in trước ngày {checkInDate:dd/MM/yyyy HH:mm}.");

                _bookingDAL.CheckIn(bookingId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException)
            {
                throw;
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
                ValidateBookingInput(bookingIdInput: bookingIdInput, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền thực hiện check-out.");

                int bookingId = int.Parse(bookingIdInput);
                DataTable dt = _bookingDAL.GetBookingHistory(bookingId, null);
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                string status = dt.Rows[0]["Status"]?.ToString() ?? "";
                if (status != "Active")
                    throw new ArgumentException("Đặt phòng không ở trạng thái Active để check-out.");

                _bookingDAL.CheckOut(bookingId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException)
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
                ValidateBookingInput(bookingIdInput: bookingIdInput, newEndDate: newEndDate, updatedBy: updatedBy, updatedByUsername: updatedByUsername, isExtend: true);
                if (!CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền gia hạn đặt phòng.");

                int bookingId = int.Parse(bookingIdInput);
                _bookingDAL.ExtendBooking(bookingId, newEndDate, updatedBy, updatedByUsername);
            }
            catch (ArgumentException)
            {
                throw;
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