using System;
using System.Data;
using System.Text.RegularExpressions;

namespace HotelManagementSystem
{
    public class UserBLL
    {
        private readonly UserDAL _userDAL = new UserDAL();

        private bool CheckUserPermission(int updatedBy, string requiredPermission)
        {
            // TODO: Kiểm tra quyền từ bảng Roles và Permissions
            return true; // Giả lập
        }

        private void ValidateUserInput(
            string? userIdInput = null,
            string? username = null,
            string? password = null,
            string? role = null,
            string? email = null,
            string? fullName = null,
            int updatedBy = 0,
            string? updatedByUsername = null,
            bool isCreate = false,
            bool isUpdate = false)
        {
            if (isCreate || isUpdate)
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Tên đăng nhập không được để trống.");

                if (username.Length < 3 || username.Length > 50)
                    throw new ArgumentException("Tên đăng nhập phải từ 3-50 ký tự.");

                if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                    throw new ArgumentException("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới.");

                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Vai trò không được để trống.");

                if (!IsValidRole(role))
                    throw new ArgumentException("Vai trò không hợp lệ. Chỉ chấp nhận: Admin, Receptionist, Housekeeping.");

                // Email và FullName không bắt buộc vì database không có các cột này
            }

            if (isCreate && string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mật khẩu không được để trống.");

            if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");

            if (isUpdate && !string.IsNullOrWhiteSpace(userIdInput))
            {
                if (!int.TryParse(userIdInput, out int userId) || userId <= 0)
                    throw new ArgumentException("ID người dùng phải là số nguyên dương.");
            }

            if (updatedBy <= 0)
                throw new ArgumentException("Thông tin người cập nhật không hợp lệ.");

            if (string.IsNullOrWhiteSpace(updatedByUsername))
                throw new ArgumentException("Tên người cập nhật không được để trống.");
        }

        private bool IsValidRole(string role)
        {
            string[] validRoles = { "Admin", "Receptionist", "Housekeeping" };
            return Array.Exists(validRoles, r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public DataTable GetAllUsers()
        {
            try
            {
                return _userDAL.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
        }

        public int AddUser(string username, string password, string role, string email, string fullName, int updatedBy, string updatedByUsername)
        {
            ValidateUserInput(
                username: username,
                password: password,
                role: role,
                email: email,
                fullName: fullName,
                updatedBy: updatedBy,
                updatedByUsername: updatedByUsername,
                isCreate: true
            );

            if (!CheckUserPermission(updatedBy, "CREATE_USER"))
                throw new UnauthorizedAccessException("Bạn không có quyền thêm người dùng.");

            try
            {
                if (_userDAL.CheckUserExists(username))
                    throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");

                int roleId = _userDAL.GetRoleId(role);
                
                // TODO: Hash password
                string hashedPassword = password; // Tạm thời chưa hash

                return _userDAL.AddUser(username, hashedPassword, roleId, email, fullName, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm người dùng: {ex.Message}");
            }
        }

        public void UpdateUser(string userIdInput, string username, string role, string email, string fullName, int updatedBy, string updatedByUsername)
        {
            ValidateUserInput(
                userIdInput: userIdInput,
                username: username,
                role: role,
                email: email,
                fullName: fullName,
                updatedBy: updatedBy,
                updatedByUsername: updatedByUsername,
                isUpdate: true
            );

            if (!CheckUserPermission(updatedBy, "UPDATE_USER"))
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật người dùng.");

            try
            {
                int userId = int.Parse(userIdInput);
                int roleId = _userDAL.GetRoleId(role);
                
                _userDAL.UpdateUser(userId, username, roleId, email, fullName, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật người dùng: {ex.Message}");
            }
        }

        public void DeleteUser(string userIdInput, int updatedBy, string updatedByUsername)
        {
            if (string.IsNullOrWhiteSpace(userIdInput) || !int.TryParse(userIdInput, out int userId) || userId <= 0)
                throw new ArgumentException("ID người dùng phải là số nguyên dương.");

            if (updatedBy <= 0)
                throw new ArgumentException("Thông tin người xóa không hợp lệ.");

            if (!CheckUserPermission(updatedBy, "DELETE_USER"))
                throw new UnauthorizedAccessException("Bạn không có quyền xóa người dùng.");

            if (userId == updatedBy)
                throw new InvalidOperationException("Không thể xóa chính tài khoản của mình.");

            try
            {
                _userDAL.DeleteUser(userId, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa người dùng: {ex.Message}");
            }
        }

        public void ChangePassword(string userIdInput, string newPassword, int updatedBy, string updatedByUsername)
        {
            if (string.IsNullOrWhiteSpace(userIdInput) || !int.TryParse(userIdInput, out int userId) || userId <= 0)
                throw new ArgumentException("ID người dùng phải là số nguyên dương.");

            ValidateUserInput(
                password: newPassword,
                updatedBy: updatedBy,
                updatedByUsername: updatedByUsername
            );

            if (!CheckUserPermission(updatedBy, "CHANGE_PASSWORD"))
                throw new UnauthorizedAccessException("Bạn không có quyền đổi mật khẩu.");

            try
            {
                // TODO: Hash password
                string hashedPassword = newPassword; // Tạm thời chưa hash
                
                _userDAL.ChangePassword(userId, hashedPassword, updatedBy, updatedByUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đổi mật khẩu: {ex.Message}");
            }
        }

        public void ToggleUserStatus(string userIdInput, bool isActive, int updatedBy, string updatedByUsername)
        {
            // Ghi chú: Chức năng này chưa được hỗ trợ do bảng Users không có cột IsActive
            throw new NotImplementedException("Chức năng bật/tắt trạng thái user chưa được hỗ trợ do bảng Users không có cột IsActive");
        }

        public (bool success, int userId, string role) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Tên đăng nhập không được để trống.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mật khẩu không được để trống.");

            try
            {
                // TODO: Hash password và so sánh
                return _userDAL.Login(username, password);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng nhập: {ex.Message}");
            }
        }
    }
}