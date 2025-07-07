using System;
using System.Data;
using System.Globalization;

namespace HotelManagementSystem
{
    public class BookingBLL
    {
        private readonly BookingDAL _bookingDAL = new BookingDAL();

        private void ValidateBookingInput(
    string idCard = null,
    string bookingIdInput = null,
    string roomIdInput = null,
    string checkInDateInput = null,
    string checkOutDateInput = null,
    DateTime? newCheckOutDate = null,
    int updatedBy = 0,
    string updatedByUsername = null,
    bool isCreate = false,
    bool isExtend = false)
        {
            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(idCard))
                    throw new ArgumentException("Số chứng minh nhân dân/căn cước công dân không hợp lệ.");

                if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out int roomId) || roomId <= 0)
                    throw new ArgumentException("ID phòng không hợp lệ.");

                DateTime checkInDate;
                if (string.IsNullOrWhiteSpace(checkInDateInput))
                {
                    checkInDate = DateTime.Now;
                }
                else if (!DateTime.TryParseExact(checkInDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkInDate))
                {
                    throw new ArgumentException("Ngày check-in không hợp lệ.");
                }
                else if (checkInDate < DateTime.Now)
                {
                    throw new ArgumentException("Ngày check-in không thể trước thời gian hiện tại.");
                }

                DateTime checkOutDate;
                if (string.IsNullOrWhiteSpace(checkOutDateInput) ||
                    !DateTime.TryParseExact(checkOutDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkOutDate))
                    throw new ArgumentException("Ngày check-out không hợp lệ.");

                if (checkInDate >= checkOutDate)
                    throw new ArgumentException("Ngày check-in phải trước ngày check-out.");
            }
            else if (bookingIdInput != null)
            {
                if (string.IsNullOrWhiteSpace(bookingIdInput) || !int.TryParse(bookingIdInput, out int bookingId))
                    throw new ArgumentException("ID đặt phòng không hợp lệ.");
            }

            if (isExtend && newCheckOutDate.HasValue && newCheckOutDate <= DateTime.Now)
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
        
                if (!_bookingDAL.CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền tạo đặt phòng.");
        
                int roomId = int.Parse(roomIdInput);
        
                DateTime checkInDate;
                if (string.IsNullOrWhiteSpace(checkInDateInput))
                    checkInDate = DateTime.Now;
                else
                    checkInDate = DateTime.ParseExact(checkInDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        
                DateTime checkOutDate = DateTime.ParseExact(checkOutDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        
                return _bookingDAL.CreateBooking(IDCard, roomId, checkInDate, checkOutDate, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void CancelBooking(string bookingIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateBookingInput(bookingIdInput: bookingIdInput, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
                if (!_bookingDAL.CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền hủy đặt phòng.");

                int bookingId = int.Parse(bookingIdInput);
                _bookingDAL.CancelBooking(bookingId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CheckIn(string bookingIdInput, string IDCard, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateBookingInput(
                    bookingIdInput: bookingIdInput,
                    idCard: IDCard,
                    updatedBy: updatedBy,
                    updatedByUsername: updatedByUsername
                );

                if (!_bookingDAL.CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền thực hiện check-in.");

                int bookingId = int.Parse(bookingIdInput);
                DataTable dt = _bookingDAL.CheckBookingExists(bookingId);
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                if (dt.Rows[0]["Status"].ToString() != "Active")
                    throw new ArgumentException("Đặt phòng không ở trạng thái Active để check-in.");

                // Gọi DAL chạy SP đã xử lý CCCD & Room.Status
                _bookingDAL.CheckIn(bookingId, IDCard, updatedBy, updatedByUsername);
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



        public void CheckOut(string bookingIdInput, string IDCard, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateBookingInput(bookingIdInput: bookingIdInput, idCard: IDCard, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
        
                if (!_bookingDAL.CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền thực hiện check-out.");
    
                int bookingId = int.Parse(bookingIdInput);
                DataTable dt = _bookingDAL.CheckBookingExists(bookingId);
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                if (dt.Rows[0]["Status"].ToString() != "CheckedIn")
                    throw new ArgumentException("Đặt phòng phải ở trạng thái CheckedIn để check-out.");
        
                string bookingIDCard = dt.Rows[0]["IDCard"].ToString();
                if (bookingIDCard != IDCard)
                    throw new ArgumentException("Căn cước công dân không khớp với thông tin đặt phòng.");

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


        public void ExtendBooking(string bookingIdInput, DateTime newCheckOutDate, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateBookingInput(bookingIdInput: bookingIdInput, newCheckOutDate: newCheckOutDate, updatedBy: updatedBy, updatedByUsername: updatedByUsername, isExtend: true);
                if (!_bookingDAL.CheckUserPermission(updatedBy, "manage_bookings"))
                    throw new ArgumentException("Người dùng không có quyền gia hạn đặt phòng.");

                int bookingId = int.Parse(bookingIdInput);
                DataTable dt = _bookingDAL.CheckBookingExists(bookingId);
                if (dt.Rows.Count == 0)
                    throw new ArgumentException("Đặt phòng không tồn tại.");
                if (dt.Rows[0]["Status"].ToString() != "CheckedIn")
                    throw new ArgumentException("Đặt phòng phải ở trạng thái CheckedIn để gia hạn.");

                _bookingDAL.ExtendBooking(bookingId, newCheckOutDate, updatedBy, updatedByUsername);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi gia hạn đặt phòng: {ex.Message}");
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
                if (ex.Message.Contains("Khách hàng không tồn tại"))
                    throw new ArgumentException("Khách hàng không tồn tại.");
                if (ex.Message.Contains("Phòng không tồn tại"))
                    throw new ArgumentException("Phòng không tồn tại.");
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy lịch sử đặt phòng: {ex.Message}");
            }
        }

        public DataTable GetAllBookings()
        {
            try
            {
                return _bookingDAL.GetAllBookings();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách đặt phòng: {ex.Message}");
            }
        }
    }
}