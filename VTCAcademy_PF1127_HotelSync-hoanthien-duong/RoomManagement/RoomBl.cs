using System;
using System.Data;
using System.Linq;

namespace HotelManagementSystem
{
    public class RoomBLL
    {
        private readonly RoomDAL _roomDAL = new RoomDAL();

        private void ValidateRoomInput(
            string roomIdInput = null,
            string roomNumber = null,
            string roomType = null,
            string priceInput = null,
            string amenities = null,
            string status = null,
            int updatedBy = 0,
            string updatedByUsername = null,
            bool isCreate = false,
            bool isUpdate = false)
        {
            if (isCreate || isUpdate)
            {
                if (string.IsNullOrWhiteSpace(roomNumber))
                    throw new ArgumentException("Số phòng không được để trống.");
                if (string.IsNullOrWhiteSpace(roomType))
                    throw new ArgumentException("Loại phòng không được để trống.");
                if (!new[] { "Single", "Double", "Suite" }.Contains(roomType))
                    throw new ArgumentException("Loại phòng phải là Single, Double hoặc Suite.");
                if (string.IsNullOrWhiteSpace(priceInput) || !decimal.TryParse(priceInput, out decimal price) || price <= 0)
                    throw new ArgumentException("Giá phải là số hợp lệ và lớn hơn 0.");
                if (string.IsNullOrWhiteSpace(amenities)) // Kiểm tra Amenities không rỗng
                    throw new ArgumentException("Tiện nghi không được để trống.");
            }
        
            if (isUpdate)
            {
                if (string.IsNullOrWhiteSpace(status))
                    throw new ArgumentException("Trạng thái phòng không được để trống.");
                if (!new[] { "Available", "Occupied", "Under Maintenance", "Uncleaned" }.Contains(status))
                    throw new ArgumentException("Trạng thái phòng phải là Available, Occupied, Under Maintenance hoặc Uncleaned.");
            }
        
            if (roomIdInput != null)
            {
                if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out int roomId) || roomId <= 0)
                    throw new ArgumentException("ID phòng không hợp lệ.");
            }
        
            if (updatedBy <= 0)
                throw new ArgumentException("ID người dùng không hợp lệ.");
            if (string.IsNullOrWhiteSpace(updatedByUsername))
                throw new ArgumentException("Tên người dùng không được để trống.");
        }

        public int AddRoom(string roomNumber, string roomType, string priceInput, string amenities, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateRoomInput(roomNumber: roomNumber, roomType: roomType, priceInput: priceInput, amenities: amenities, updatedBy: updatedBy, updatedByUsername: updatedByUsername, isCreate: true);
                if (!_roomDAL.CheckUserPermission(updatedBy, "manage_rooms"))
                    throw new ArgumentException("Người dùng không có quyền thêm phòng.");
        
                decimal price = decimal.Parse(priceInput);
                return _roomDAL.AddRoom(roomNumber, roomType, price, amenities, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void UpdateRoom(string roomIdInput, string roomNumber, string roomType, string priceInput, string amenities, string status, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateRoomInput(roomIdInput: roomIdInput, roomNumber: roomNumber, roomType: roomType, priceInput: priceInput, amenities: amenities, status: status, updatedBy: updatedBy, updatedByUsername: updatedByUsername, isUpdate: true);
                if (!_roomDAL.CheckUserPermission(updatedBy, "manage_rooms"))
                    throw new ArgumentException("Người dùng không có quyền cập nhật phòng.");

                int roomId = int.Parse(roomIdInput);
                decimal price = decimal.Parse(priceInput);
                _roomDAL.UpdateRoom(roomId, roomNumber, roomType, price, amenities, status, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void DeleteRoom(string roomIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateRoomInput(roomIdInput: roomIdInput, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
                if (!_roomDAL.CheckUserPermission(updatedBy, "manage_rooms"))
                    throw new ArgumentException("Người dùng không có quyền xóa phòng.");

                int roomId = int.Parse(roomIdInput);
                _roomDAL.DeleteRoom(roomId, updatedBy, updatedByUsername);
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Truyền trực tiếp thông báo lỗi từ SQL
                throw new Exception(ex.Message, ex);
            }
        }

        public void CleanRoom(string roomIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                ValidateRoomInput(roomIdInput: roomIdInput, updatedBy: updatedBy, updatedByUsername: updatedByUsername);
                if (!_roomDAL.CheckUserPermission(updatedBy, "manage_services"))
                    throw new ArgumentException("Người dùng không có quyền dọn phòng.");

                int roomId = int.Parse(roomIdInput);
                _roomDAL.CleanRoom(roomId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public DataTable GetAllRooms()
        {
            try
            {
                return _roomDAL.GetAllRooms();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public DataTable SearchRooms(string status, string roomType, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                    throw new ArgumentException("Giá tối thiểu không thể lớn hơn giá tối đa.");
                if (!string.IsNullOrWhiteSpace(status) && !new[] { "Available", "Occupied", "Under Maintenance", "Uncleaned" }.Contains(status))
                    throw new ArgumentException("Trạng thái phòng không hợp lệ.");
                if (!string.IsNullOrWhiteSpace(roomType) && !new[] { "Single", "Double", "Suite" }.Contains(roomType))
                    throw new ArgumentException("Loại phòng không hợp lệ.");

                return _roomDAL.SearchRooms(status, roomType, minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public DataTable CheckRoomAvailability(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate >= endDate)
                    throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
                return _roomDAL.CheckRoomAvailability(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}