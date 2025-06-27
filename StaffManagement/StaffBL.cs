using System;
using System.Data;
using System.Linq;

namespace HotelManagementSystem
{
    public class StaffBLL
    {
        private StaffDAL _staffDAL;

        public StaffBLL()
        {
            _staffDAL = new StaffDAL();
        }

        // 1. Thêm nhân viên mới
        public void AddStaff(string name, string role, string phone, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role))
                    throw new ArgumentException("Tên và vai trò không được để trống.");

                if (!IsValidRole(role))
                    throw new ArgumentException("Vai trò không hợp lệ. Chỉ chấp nhận: Receptionist, Housekeeping, Manager");

                if (!string.IsNullOrEmpty(phone) && !IsValidPhoneNumber(phone))
                    throw new ArgumentException("Số điện thoại không hợp lệ.");

                _staffDAL.AddStaff(name, role, phone, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi thêm nhân viên: {ex.Message}");
            }
        }

        // 2. Cập nhật thông tin nhân viên
        public void UpdateStaff(string staffIdInput, string name, string role, string phone, int updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation
                if (!int.TryParse(staffIdInput, out int staffId) || staffId <= 0)
                    throw new ArgumentException("ID nhân viên phải là số nguyên dương.");

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role))
                    throw new ArgumentException("Tên và vai trò không được để trống.");

                if (!IsValidRole(role))
                    throw new ArgumentException("Vai trò không hợp lệ. Chỉ chấp nhận: Receptionist, Housekeeping, Manager");

                if (!string.IsNullOrEmpty(phone) && !IsValidPhoneNumber(phone))
                    throw new ArgumentException("Số điện thoại không hợp lệ.");

                _staffDAL.UpdateStaff(staffId, name, role, phone, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi cập nhật nhân viên: {ex.Message}");
            }
        }

        // 3. Xóa nhân viên
        public void DeleteStaff(string staffIdInput, int updatedBy, string updatedByUsername)
        {
            try
            {
                if (!int.TryParse(staffIdInput, out int staffId) || staffId <= 0)
                    throw new ArgumentException("ID nhân viên phải là số nguyên dương.");

                _staffDAL.DeleteStaff(staffId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi xóa nhân viên: {ex.Message}");
            }
        }

        // 4. Xem danh sách nhân viên
        public DataTable GetAllStaff()
        {
            try
            {
                return _staffDAL.GetAllStaff();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi lấy danh sách nhân viên: {ex.Message}");
            }
        }

        // 5. Tìm kiếm nhân viên theo vai trò
        public DataTable SearchStaffByRole(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                    throw new ArgumentException("Vai trò không được để trống.");

                if (!IsValidRole(role))
                    throw new ArgumentException("Vai trò không hợp lệ. Chỉ chấp nhận: Receptionist, Housekeeping, Manager");

                return _staffDAL.SearchStaffByRole(role);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi tìm kiếm nhân viên: {ex.Message}");
            }
        }

        // 6. Gán nhiệm vụ cho nhân viên
        public void AssignStaffToTask(string staffIdInput, string taskType, string roomIdInput, string bookingIdInput, int assignedBy, string assignedByUsername)
        {
            try
            {
                if (!int.TryParse(staffIdInput, out int staffId) || staffId <= 0)
                    throw new ArgumentException("ID nhân viên phải là số nguyên dương.");

                if (string.IsNullOrEmpty(taskType))
                    throw new ArgumentException("Loại nhiệm vụ không được để trống.");

                if (!IsValidTaskType(taskType))
                    throw new ArgumentException("Loại nhiệm vụ không hợp lệ. Chỉ chấp nhận: cleaning, maintenance, service");

                int? roomId = null;
                int? bookingId = null;

                if (!string.IsNullOrEmpty(roomIdInput))
                {
                    if (!int.TryParse(roomIdInput, out int rId) || rId <= 0)
                        throw new ArgumentException("ID phòng phải là số nguyên dương.");
                    roomId = rId;
                }

                if (!string.IsNullOrEmpty(bookingIdInput))
                {
                    if (!int.TryParse(bookingIdInput, out int bId) || bId <= 0)
                        throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");
                    bookingId = bId;
                }

                _staffDAL.AssignStaffToTask(staffId, taskType, roomId, bookingId, assignedBy, assignedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi trong logic nghiệp vụ khi gán nhiệm vụ: {ex.Message}");
            }
        }

        // Helper methods
        private bool IsValidRole(string role)
        {
            return role == "Receptionist" || role == "Housekeeping" || role == "Manager";
        }

        private bool IsValidTaskType(string taskType)
        {
            return taskType == "cleaning" || taskType == "maintenance" || taskType == "service";
        }

        private bool IsValidPhoneNumber(string phone)
        {
            // Kiểm tra số điện thoại Việt Nam (10-11 số, bắt đầu bằng 0)
            if (string.IsNullOrEmpty(phone)) return true; // Phone có thể null
            return phone.Length >= 10 && phone.Length <= 11 && phone.StartsWith("0") && phone.All(char.IsDigit);
        }
    }
}