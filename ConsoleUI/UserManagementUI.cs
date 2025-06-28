using System;
using System.Data;

namespace HotelManagementSystem
{
    public class UserManagementUI : BaseUI
    {
        private readonly UserBLL _userBLL = new UserBLL();

        public UserManagementUI(string? username, string? role, int? userId) 
            : base(username, role, userId)
        {
        }

        public void ShowUserManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Người Dùng");
                SetupBox(80, 20);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Thêm người dùng mới");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Cập nhật thông tin người dùng");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Xóa người dùng");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Xem danh sách người dùng");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Đổi mật khẩu người dùng");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Kích hoạt/Vô hiệu hóa tài khoản");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("7. Quay lại");
                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 18);
                Console.Write("0. Thoát");

                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.Write("Lựa chọn của bạn: ");
                string? choice = ReadInputWithEscape(x + 20, y + height - 2);
                if (choice == null)
                    return;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowAddUser();
                            break;
                        case "2":
                            ShowUpdateUser();
                            break;
                        case "3":
                            ShowDeleteUser();
                            break;
                        case "4":
                            ShowUserList();
                            break;
                        case "5":
                            ShowChangePassword();
                            break;
                        case "6":
                            ShowToggleUserStatus();
                            break;
                        case "7":
                            return;
                        case "0":
                            Console.Clear();
                            Environment.Exit(0);
                            break;
                        default:
                            ShowErrorMessage("Lựa chọn không hợp lệ! Nhấn phím bất kỳ để thử lại...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Lỗi: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        private void ShowAddUser()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Thêm Người Dùng Mới");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên đăng nhập: ");
            string? username = ReadInputWithEscape(x + 16, y + 4);
            if (username == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Mật khẩu: ");
            Console.SetCursorPosition(x + 12, y + 6);
            string password = ReadPassword();

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Vai trò (Admin/Receptionist/Housekeeping): ");
            string? role = ReadInputWithEscape(x + 42, y + 8);
            if (role == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Email: ");
            string? email = ReadInputWithEscape(x + 8, y + 10);
            if (email == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 12);
            Console.Write("Họ tên đầy đủ: ");
            string? fullName = ReadInputWithEscape(x + 16, y + 12);
            if (fullName == null) return;

            try
            {
                _userBLL.AddUser(username, password, role, email, fullName, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Thêm người dùng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowUpdateUser()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Cập Nhật Người Dùng");
            SetupBox(80, 18);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID người dùng: ");
            string? userIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (userIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Tên đăng nhập: ");
            string? username = ReadInputWithEscape(x + 16, y + 6);
            if (username == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Vai trò (Admin/Receptionist/Housekeeping): ");
            string? role = ReadInputWithEscape(x + 42, y + 8);
            if (role == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Email: ");
            string? email = ReadInputWithEscape(x + 8, y + 10);
            if (email == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 12);
            Console.Write("Họ tên đầy đủ: ");
            string? fullName = ReadInputWithEscape(x + 16, y + 12);
            if (fullName == null) return;

            try
            {
                _userBLL.UpdateUser(userIdInput, username, role, email, fullName, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Cập nhật người dùng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowDeleteUser()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Xóa Người Dùng");
            SetupBox(60, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID người dùng: ");
            string? userIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (userIdInput == null) return;

            try
            {
                _userBLL.DeleteUser(userIdInput, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Xóa người dùng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowUserList()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Người Dùng");
            SetupBox(120, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH NGƯỜI DÙNG ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            DataTable users;
            try
            {
                users = _userBLL.GetAllUsers();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Tên đăng nhập", "Vai trò", "Email", "Họ tên", "Trạng thái", "Ngày tạo" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length + 2;
            }

            Console.SetCursorPosition(x + 2, y + 6);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
            {
                Console.Write(headers[col].PadRight(columnWidths[col]));
            }
            Console.WriteLine();

            Console.SetCursorPosition(x + 2, y + 7);
            Console.WriteLine(new string('─', width - 4));
            Console.ResetColor();

            for (int i = 0; i < users.Rows.Count; i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                Console.Write((users.Rows[i]["UserID"]?.ToString() ?? "").PadRight(columnWidths[0]));
                Console.Write((users.Rows[i]["Username"]?.ToString() ?? "").PadRight(columnWidths[1]));
                Console.Write((users.Rows[i]["RoleName"]?.ToString() ?? "").PadRight(columnWidths[2]));
                Console.Write((users.Rows[i]["Email"]?.ToString() ?? "").PadRight(columnWidths[3]));
                Console.Write((users.Rows[i]["FullName"]?.ToString() ?? "").PadRight(columnWidths[4]));
                Console.Write((users.Rows[i]["IsActive"]?.ToString() ?? "").PadRight(columnWidths[5]));
                Console.Write(Convert.ToDateTime(users.Rows[i]["CreatedAt"]).ToString("yyyy-MM-dd"));

                if (i < users.Rows.Count - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (users.Rows.Count - 1) * 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowChangePassword()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Đổi Mật Khẩu");
            SetupBox(80, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID người dùng: ");
            string? userIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (userIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Mật khẩu mới: ");
            Console.SetCursorPosition(x + 16, y + 6);
            string newPassword = ReadPassword();

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Xác nhận mật khẩu: ");
            Console.SetCursorPosition(x + 21, y + 8);
            string confirmPassword = ReadPassword();

            if (newPassword != confirmPassword)
            {
                ShowErrorMessage("Mật khẩu xác nhận không khớp! Nhấn phím bất kỳ để thử lại...");
                Console.ReadKey();
                return;
            }

            try
            {
                _userBLL.ChangePassword(userIdInput, newPassword, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Đổi mật khẩu thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowToggleUserStatus()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kích Hoạt/Vô Hiệu Hóa");
            SetupBox(80, 12);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID người dùng: ");
            string? userIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (userIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Trạng thái (true: Kích hoạt, false: Vô hiệu hóa): ");
            string? statusInput = ReadInputWithEscape(x + 50, y + 6);
            if (statusInput == null) return;

            try
            {
                bool isActive = statusInput.ToLower() == "true" || statusInput == "1";
                _userBLL.ToggleUserStatus(userIdInput, isActive, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage($"Cập nhật trạng thái thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                var (success, userId, role) = _userBLL.Login(username, password);
                if (success)
                {
                    currentUserId = userId;
                    currentUsername = username;
                    currentRole = role;
                }
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đăng nhập: {ex.Message}");
                return false;
            }
        }

        public (int? userId, string? role) GetCurrentUserInfo()
        {
            return (currentUserId, currentRole);
        }

        public bool Register(string username, string password, string role, string email, string fullName)
        {
            try
            {
                // Đối với đăng ký mới, sử dụng user system với ID = 1
                _userBLL.AddUser(username, password, role, email, fullName, 1, "System");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đăng ký: {ex.Message}");
                return false;
            }
        }
    }
}
