// File: RoomBLL.cs
using System;
using System.Data;

namespace HotelManagementSystem
{
    public class RoomBLL
    {
        private readonly RoomDAL _roomDAL = new RoomDAL();

        private bool CheckUserPermission(int updatedBy, string requiredPermission)
        {
            // TODO: Kiểm tra quyền từ bảng Roles và Permissions
            return true; // Giả lập
        }

        public void AddRoom(string roomNumber, string roomType, string priceInput, string amenities, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomNumber))
                    throw new ArgumentException("Số phòng không được để trống.");
                if (string.IsNullOrWhiteSpace(roomType))
                    throw new ArgumentException("Loại phòng không được để trống.");
                if (!new[] { "Single", "Double", "Suite" }.Contains(roomType))
                    throw new ArgumentException("Loại phòng phải là Single, Double hoặc Suite.");
                if (string.IsNullOrWhiteSpace(priceInput) || !decimal.TryParse(priceInput, out decimal price) || price <= 0)
                    throw new ArgumentException("Giá phải là số hợp lệ và lớn hơn 0.");
                if (string.IsNullOrWhiteSpace(amenities))
                    throw new ArgumentException("Tiện nghi không được để trống.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_rooms"))
                    throw new ArgumentException("Người dùng không có quyền thêm phòng.");

                _roomDAL.AddRoom(roomNumber, roomType, price, amenities, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi thêm phòng: " + ex.Message);
            }
        }

        public void UpdateRoom(string roomIdInput, string roomNumber, string roomType, string priceInput, string amenities, string status, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out int roomId) || roomId <= 0)
                    throw new ArgumentException("ID phòng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(roomNumber))
                    throw new ArgumentException("Số phòng không được để trống.");
                if (string.IsNullOrWhiteSpace(roomType))
                    throw new ArgumentException("Loại phòng không được để trống.");
                if (!new[] { "Single", "Double", "Suite" }.Contains(roomType))
                    throw new ArgumentException("Loại phòng phải là Single, Double hoặc Suite.");
                if (string.IsNullOrWhiteSpace(priceInput) || !decimal.TryParse(priceInput, out decimal price) || price <= 0)
                    throw new ArgumentException("Giá phải là số hợp lệ và lớn hơn 0.");
                if (string.IsNullOrWhiteSpace(amenities))
                    throw new ArgumentException("Tiện nghi không được để trống.");
                if (string.IsNullOrWhiteSpace(status))
                    throw new ArgumentException("Trạng thái phòng không được để trống.");
                if (!new[] { "Available", "Occupied", "Under Maintenance" }.Contains(status))
                    throw new ArgumentException("Trạng thái phòng phải là Available, Occupied hoặc Under Maintenance.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_rooms"))
                    throw new ArgumentException("Người dùng không có quyền cập nhật phòng.");

                _roomDAL.UpdateRoom(roomId, roomNumber, roomType, price, amenities, status, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi cập nhật phòng: " + ex.Message);
            }
        }

        public void DeleteRoom(string roomIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out int roomId) || roomId <= 0)
                    throw new ArgumentException("ID phòng không hợp lệ.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_rooms"))
                    throw new ArgumentException("Người dùng không có quyền xóa phòng.");

                _roomDAL.DeleteRoom(roomId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi xóa phòng: " + ex.Message);
            }
        }

        public void CleanRoom(string roomIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out int roomId) || roomId <= 0)
                    throw new ArgumentException("ID phòng không hợp lệ.");
                if (updatedBy <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ.");
                if (string.IsNullOrWhiteSpace(updatedByUsername))
                    throw new ArgumentException("Tên người dùng không được để trống.");
                if (!CheckUserPermission(updatedBy, "manage_services"))
                    throw new ArgumentException("Người dùng không có quyền dọn phòng.");

                _roomDAL.CleanRoom(roomId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi dọn phòng: " + ex.Message);
            }
        }

        // Các phương thức GetAllRooms, SearchRooms, CheckRoomAvailability giữ nguyên
        public DataTable GetAllRooms()
        {
            try
            {
                return _roomDAL.GetAllRooms();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi lấy danh sách phòng: " + ex.Message);
            }
        }

        public DataTable SearchRooms(string status, string roomType, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                    throw new ArgumentException("Giá tối thiểu không thể lớn hơn giá tối đa.");
                if (!string.IsNullOrWhiteSpace(status) && !new[] { "Available", "Occupied", "Under Maintenance" }.Contains(status))
                    throw new ArgumentException("Trạng thái phòng không hợp lệ.");
                if (!string.IsNullOrWhiteSpace(roomType) && !new[] { "Single", "Double", "Suite" }.Contains(roomType))
                    throw new ArgumentException("Loại phòng không hợp lệ.");

                return _roomDAL.SearchRooms(status, roomType, minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi tìm kiếm phòng: " + ex.Message);
            }
        }

        public DataTable CheckRoomAvailability(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate >= endDate)
                    throw new ArgumentException("Ngày check-in phải trước ngày check-out.");
                return _roomDAL.CheckRoomAvailability(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi trong logic nghiệp vụ khi kiểm tra tình trạng phòng: " + ex.Message);
            }
        }
    }
}