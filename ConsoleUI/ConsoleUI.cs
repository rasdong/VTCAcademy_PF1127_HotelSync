using System;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Data;

namespace HotelManagementSystem
{
    public class ConsoleUI
    {
        private int width;
        private int height;
        private int x;
        private int y;
        private string? currentUsername;
        private string? currentRole;
        private int? currentUserId;
        private int? lastBookingId;
        private readonly RoomBLL _roomBLL = new RoomBLL();
        private readonly BookingBLL _bookingBLL = new BookingBLL();

        public ConsoleUI()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                int desiredBufferWidth = 120;
                int desiredBufferHeight = 1000;
                try
                {
                    Console.SetWindowSize(Math.Min(120, Console.LargestWindowWidth), Math.Min(40, Console.LargestWindowHeight));
                    Console.BufferWidth = Math.Max(desiredBufferWidth, Console.WindowWidth);
                    Console.BufferHeight = Math.Max(desiredBufferHeight, Console.WindowHeight);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi: Không thể đặt kích thước cửa sổ hoặc vùng đệm console.");
                    Console.WriteLine($"Chi tiết lỗi: {ex.Message}");
                    Console.WriteLine("Nhấn phím bất kỳ để thoát...");
                    Console.ReadKey();
                    Console.Clear();
                    Environment.Exit(0);
                }
            }
        }

        public void DrawHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition((Console.WindowWidth - title.Length - 4) / 2, 1);
            Console.Write("═══ " + title + " ═══");

            string[] hotelSyncArt = new[]
            {
                "  ██╗  ██╗ ██████╗ ████████╗███████╗██╗     ███████╗██╗   ██╗███╗   ██╗ ██████╗ ",
                "  ██║  ██║██╔═══██╗╚══██╔══╝██╔════╝██║     ██╔════╝╚██╗ ██╔╝████╗  ██║██╔════╝ ",
                "  ███████║██║   ██║   ██║   █████╗  ██║     ███████╗ ╚████╔╝ ██╔██╗ ██║██║     ",
                "  ██╔══██║██║   ██║   ██║   ██╔══╝  ██║     ╚════██║  ╚██╔╝  ██║╚██╗██║██║     ",
                "  ██║  ██║╚██████╔╝   ██║   ███████╗███████╗███████║   ██║   ██║ ╚████║╚██████╗",
                "  ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚══════╝╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═══╝ ╚═════╝ "
            };

            int artWidth = hotelSyncArt[0].Length;
            int startX = (Console.WindowWidth - artWidth) / 2;
            int startY = 3;

            for (int i = 0; i < hotelSyncArt.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.Write(hotelSyncArt[i]);
            }

            Console.ResetColor();
        }

        public void SetupBox(int boxWidth, int boxHeight)
        {
            width = Math.Min(boxWidth, Console.WindowWidth - 2);
            height = boxHeight;
            x = Math.Max(0, (Console.WindowWidth - width) / 2);
            y = Math.Max(0, (Console.WindowHeight - height) / 2);

            DrawBox();
        }

        private void DrawBox()
        {
            if (Console.WindowWidth < width || Console.WindowHeight < height)
            {
                Console.Clear();
                Console.WriteLine("Lỗi: Cửa sổ console quá nhỏ! Vui lòng mở rộng cửa sổ.");
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(x, y);
            Console.Write("┌" + new string('─', width) + "┐");
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│" + new string(' ', width) + "│");
            }
            Console.SetCursorPosition(x, y + height - 1);
            Console.Write("└" + new string('─', width) + "┘");
            Console.ResetColor();
        }

        private string? ReadInputWithEscape(int inputX, int inputY)
        {
            Console.SetCursorPosition(inputX, inputY);
            string input = string.Empty;
            int maxLength = 20; // Giới hạn độ dài nhập để tránh đè lên giao diện
            Console.Write(new string(' ', maxLength)); // Xóa vùng nhập liệu cũ
            Console.SetCursorPosition(inputX, inputY);
        
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                        return null;
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return input;
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input[..^1];
                        Console.SetCursorPosition(inputX + input.Length, inputY);
                        Console.Write(' ');
                        Console.SetCursorPosition(inputX + input.Length, inputY);
                        Console.Write(input);
                    }
                    else if (keyInfo.KeyChar != '\0' && input.Length < maxLength - 1)
                    {
                        input += keyInfo.KeyChar;
                        Console.Write(keyInfo.KeyChar);
                    }
                }
            }
        }
        
        private string ReadPassword()
        {
            string password = string.Empty;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return string.Empty;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }

        private bool Login(string username, string password)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = "SELECT u.UserID, u.Username, r.RoleName FROM Users u JOIN Roles r ON u.RoleID = r.RoleID WHERE u.Username = @username AND u.Password = @password";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password); // Nên mã hóa mật khẩu trong thực tế
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentUserId = reader.GetInt32("UserID");
                            currentUsername = reader.GetString("Username");
                            currentRole = reader.GetString("RoleName");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool Register(string username, string password, string role)
        {
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    long count = (long)checkCommand.ExecuteScalar();
                    if (count > 0)
                        return false;
                }

                string roleQuery = "SELECT RoleID FROM Roles WHERE RoleName = @role";
                int roleId;
                using (var roleCommand = new MySqlCommand(roleQuery, connection))
                {
                    roleCommand.Parameters.AddWithValue("@role", role);
                    var result = roleCommand.ExecuteScalar();
                    if (result == null)
                        return false;
                    roleId = (int)result;
                }

                string insertQuery = "INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername) VALUES (@username, @password, @roleId, @updatedBy, @updatedByUsername)";
                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@username", username);
                    insertCommand.Parameters.AddWithValue("@password", password); // Nên mã hóa mật khẩu
                    insertCommand.Parameters.AddWithValue("@roleId", roleId);
                    insertCommand.Parameters.AddWithValue("@updatedBy", currentUserId ?? (object)DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@updatedByUsername", currentUsername ?? (object)DBNull.Value);
                    insertCommand.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public string? ShowInitialScreen()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(60, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("1. Đăng nhập");
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("2. Đăng ký");
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("0. Thoát");
            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write("Lựa chọn của bạn: ");
            return ReadInputWithEscape(x + 20, y + 11);
        }

        public (string?, string?) ShowLoginScreen()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(60, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên đăng nhập: ");
            string? username = ReadInputWithEscape(x + 16, y + 4);
            if (username == null)
                return (null, null);

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Mật khẩu: ");
            Console.SetCursorPosition(x + 12, y + 6);
            string password = ReadPassword();
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            return (username, password);
        }

        public (string?, string?, string?, string?) ShowRegisterScreen()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(60, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên đăng nhập mới: ");
            string? newUsername = ReadInputWithEscape(x + 20, y + 4);
            if (newUsername == null)
                return (null, null, null, null);

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
            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Vai trò (Admin/Receptionist/Housekeeping): ");
            string? role = ReadInputWithEscape(x + 38, y + 10);
            if (role == null)
                return (null, null, null, null);

            return (newUsername, newPassword, confirmPassword, role);
        }

        public void ShowMainMenu()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            int optionCount = currentRole switch
            {
                "Admin" => 10,
                "Receptionist" => 7,
                "Housekeeping" => 5,
                _ => 2
            };
            SetupBox(70, 8 + optionCount * 2 + 4);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write($"Chào mừng, {currentUsername}! (Vai trò: {currentRole ?? "Chưa đăng nhập"})");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            string[] options = currentRole switch
            {
                "Admin" => new[] { "Quản lý phòng", "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Quản lý nhân viên", "Quản lý người dùng", "Xem báo cáo", "Đăng xuất", "Thoát" },
                "Receptionist" => new[] { "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Xem báo cáo", "Đăng xuất", "Thoát" },
                "Housekeeping" => new[] { "Quản lý phòng", "Quản lý dịch vụ", "Xem báo cáo", "Đăng xuất", "Thoát" },
                _ => new[] { "Đăng xuất", "Thoát" }
            };

            for (int i = 0; i < options.Length; i++)
            {
                int optionY = y + 6 + i * 2;
                Console.SetCursorPosition(x + 2, optionY);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{(i == options.Length - 1 ? 0 : i + 1)}. ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(options[i]);
                if (i < options.Length - 1)
                {
                    Console.SetCursorPosition(x + 2, optionY + 1);
                    Console.Write(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + height - 4);
            Console.Write("Lựa chọn của bạn: ");
            string? choiceInput = ReadInputWithEscape(x + 20, y + height - 4);
            if (choiceInput == "0" || choiceInput == options.Length.ToString())
            {
                HandleOption("Thoát");
            }
            else if (int.TryParse(choiceInput, out int choice) && choice > 0 && choice <= options.Length - 1)
            {
                HandleOption(options[choice - 1]);
            }
            else if (!string.IsNullOrEmpty(choiceInput))
            {
                ShowErrorMessage("Lựa chọn không hợp lệ! Nhấn phím bất kỳ để thử lại...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void HandleOption(string option)
        {
            switch (option)
            {
                case "Thoát":
                    Console.Clear();
                    Environment.Exit(0);
                    break;
                case "Đăng xuất":
                    currentUsername = null;
                    currentRole = null;
                    currentUserId = null;
                    ShowSuccessMessage("Đăng xuất thành công!");
                    Thread.Sleep(1000);
                    Console.Clear();
                    break;
                case "Quản lý phòng":
                    ShowRoomManagement();
                    break;
                case "Quản lý đặt phòng":
                    ShowBookingManagement();
                    break;
                case "Quản lý hóa đơn":
                    ShowInvoiceManagement();
                    break;
                case "Xem báo cáo":
                    ShowReportScreen();
                    break;
                case "Quản lý khách hàng":
                case "Quản lý dịch vụ":
                case "Quản lý nhân viên":
                case "Quản lý người dùng":
                    ShowPlaceholder(option);
                    break;
                default:
                    ShowInfoMessage($"Đã chọn: {option}. (Chưa được triển khai - Nhấn phím bất kỳ để quay lại...)");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
        }

        private void ShowRoomManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Phòng");
                SetupBox(80, 18); // Tăng chiều cao để chứa thêm tùy chọn "0. Thoát"
        
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Thêm phòng mới");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Cập nhật thông tin phòng");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Xóa phòng");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Xem danh sách phòng");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Dọn phòng");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Quay lại");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("0. Thoát");
                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));
        
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
                            ShowAddRoom();
                            break;
                        case "2":
                            ShowUpdateRoom();
                            break;
                        case "3":
                            ShowDeleteRoom();
                            break;
                        case "4":
                            ShowRoomList();
                            break;
                        case "5":
                            ShowCleanRoom();
                            break;
                        case "6":
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

        private void ShowAddRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Thêm Phòng Mới");
            SetupBox(80, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Số phòng: ");
            string? roomNumber = ReadInputWithEscape(x + 12, y + 4);
            if (roomNumber == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Loại phòng (Single/Double/Suite): ");
            string? roomType = ReadInputWithEscape(x + 34, y + 6);
            if (roomType == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Giá (VND): ");
            string? priceInput = ReadInputWithEscape(x + 13, y + 8);
            if (priceInput == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Tiện nghi (JSON, ví dụ: [\"TV\", \"WiFi\"]): ");
            string? amenities = ReadInputWithEscape(x + 42, y + 10);
            if (amenities == null) return;

            try
            {
                _roomBLL.AddRoom(roomNumber, roomType, priceInput, amenities, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Thêm phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowUpdateRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Cập Nhật Phòng");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID phòng: ");
            string? roomIdInput = ReadInputWithEscape(x + 12, y + 4);
            if (roomIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Số phòng: ");
            string? roomNumber = ReadInputWithEscape(x + 12, y + 6);
            if (roomNumber == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Loại phòng (Single/Double/Suite): ");
            string? roomType = ReadInputWithEscape(x + 34, y + 8);
            if (roomType == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Giá (VND): ");
            string? priceInput = ReadInputWithEscape(x + 13, y + 10);
            if (priceInput == null) return;

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 12);
            Console.Write("Tiện nghi (JSON, ví dụ: [\"TV\", \"WiFi\"]): ");
            string? amenities = ReadInputWithEscape(x + 42, y + 12);
            if (amenities == null) return;

            Console.SetCursorPosition(x + 2, y + 13);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 14);
            Console.Write("Trạng thái (Available/Occupied/Under Maintenance): ");
            string? status = ReadInputWithEscape(x + 50, y + 14);
            if (status == null) return;

            try
            {
                _roomBLL.UpdateRoom(roomIdInput, roomNumber, roomType, priceInput, amenities, status, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Cập nhật phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowDeleteRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Xóa Phòng");
            SetupBox(60, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID phòng: ");
            string? roomIdInput = ReadInputWithEscape(x + 12, y + 4);
            if (roomIdInput == null) return;

            try
            {
                _roomBLL.DeleteRoom(roomIdInput, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Xóa phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowRoomList()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Phòng");
            SetupBox(100, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH PHÒNG ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            DataTable rooms;
            try
            {
                rooms = _roomBLL.GetAllRooms();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Số phòng", "Loại phòng", "Giá (VND)", "Trạng thái", "Tiện nghi" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length;
            }

            string[,] roomData = new string[rooms.Rows.Count, headers.Length];
            for (int i = 0; i < rooms.Rows.Count; i++)
            {
                roomData[i, 0] = rooms.Rows[i]["RoomID"].ToString();
                roomData[i, 1] = rooms.Rows[i]["RoomNumber"].ToString();
                roomData[i, 2] = rooms.Rows[i]["RoomType"].ToString();
                roomData[i, 3] = Convert.ToDecimal(rooms.Rows[i]["Price"]).ToString("N0");
                roomData[i, 4] = rooms.Rows[i]["Status"].ToString();
                roomData[i, 5] = rooms.Rows[i]["Amenities"].ToString();

                for (int col = 0; col < headers.Length; col++)
                {
                    int length = roomData[i, col].Length;
                    if (length > columnWidths[col])
                        columnWidths[col] = length;
                    columnWidths[col] += 2;
                }
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

            for (int i = 0; i < roomData.GetLength(0); i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                for (int col = 0; col < roomData.GetLength(1); col++)
                {
                    Console.Write(roomData[i, col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
                if (i < roomData.GetLength(0) - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (roomData.GetLength(0) - 1) * 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowCleanRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Dọn Phòng");
            SetupBox(60, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID phòng: ");
            string? roomIdInput = ReadInputWithEscape(x + 12, y + 4);
            if (roomIdInput == null) return;

            try
            {
                _roomBLL.CleanRoom(roomIdInput, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Dọn phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowBookingManagement()
{
    while (true)
    {
        Console.Clear();
        DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Đặt Phòng");
        SetupBox(80, 20); // Tăng chiều cao để chứa đủ 8 tùy chọn

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(x + 2, y + 2);
        Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
        Console.SetCursorPosition(x + 2, y + 3);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 4);
        Console.Write("1. Tạo đặt phòng");
        Console.SetCursorPosition(x + 2, y + 5);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 6);
        Console.Write("2. Hủy đặt phòng");
        Console.SetCursorPosition(x + 2, y + 7);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 8);
        Console.Write("3. Check-in");
        Console.SetCursorPosition(x + 2, y + 9);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 10);
        Console.Write("4. Check-out");
        Console.SetCursorPosition(x + 2, y + 11);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 12);
        Console.Write("5. Gia hạn đặt phòng");
        Console.SetCursorPosition(x + 2, y + 13);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 14);
        Console.Write("6. Xem lịch sử đặt phòng");
        Console.SetCursorPosition(x + 2, y + 15);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 16);
        Console.Write("7. Quay lại");
        Console.SetCursorPosition(x + 2, y + 17);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 18);
        Console.Write("0. Thoát");
        Console.SetCursorPosition(x + 2, y + 19);
        Console.Write(new string('─', width - 4));

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
                    ShowCreateBooking();
                    break;
                case "2":
                    ShowCancelBooking();
                    break;
                case "3":
                    ShowCheckIn();
                    break;
                case "4":
                    ShowCheckOut();
                    break;
                case "5":
                    ShowExtendBooking();
                    break;
                case "6":
                    ShowGetBookingHistory();
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
        private void ShowCreateBooking()
{
    Console.Clear();
    DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tạo Đặt Phòng");
    SetupBox(80, 14);

    Console.ForegroundColor = ConsoleColor.White;
    Console.SetCursorPosition(x + 2, y + 2);
    Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
    Console.SetCursorPosition(x + 2, y + 3);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 4);
    Console.Write("Số CMND/CCCD: ");
    string? IDCardInput = ReadInputWithEscape(x + 16, y + 4);
    if (IDCardInput == null) return;

    Console.SetCursorPosition(x + 2, y + 5);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 6);
    Console.Write("ID Phòng: ");
    string? roomIdInput = ReadInputWithEscape(x + 12, y + 6);
    if (roomIdInput == null) return;

    Console.SetCursorPosition(x + 2, y + 7);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 8);
    Console.Write("Ngày Check-in (yyyy-MM-dd HH:mm): ");
    string? checkInDateInput = ReadInputWithEscape(x + 34, y + 8);
    if (checkInDateInput == null) return;

    Console.SetCursorPosition(x + 2, y + 9);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y +2 );
    Console.Write("Ngày Check-out (yyyy-MM-dd HH:mm): ");
    string? checkOutDateInput = ReadInputWithEscape(x + 35, y + 10);
    if (checkOutDateInput == null) return;

    try
    {
        int bookingId = _bookingBLL.CreateBooking(IDCardInput, roomIdInput, checkInDateInput, 
            checkOutDateInput, currentUserId ?? 0, currentUsername ?? "");
        lastBookingId = bookingId; // Lưu BookingID để sử dụng cho các chức năng khác
        ShowSuccessMessage($"Tạo đặt phòng thành công! BookingID: {bookingId}. Nhấn phím bất kỳ để quay lại...");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        ShowErrorMessage($"Lỗi: {ex.Message}");
        Console.ReadKey();
    }
}

        private void ShowCancelBooking()
{
    Console.Clear();
    DrawHeader("Hệ Thống Quản Lý Khách Sạn - Hủy Đặt Phòng");
    SetupBox(60, 12);

    Console.ForegroundColor = ConsoleColor.White;
    Console.SetCursorPosition(x + 2, y + 2);
    Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
    Console.SetCursorPosition(x + 2, y + 3);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 4);
    Console.Write($"ID đặt phòng { (lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "") } ");
    string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
    if (bookingIdInput == null) return;

    try
    {
        if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
            bookingIdInput = lastBookingId.ToString();
        if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
            throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");
        _bookingBLL.CancelBooking(bookingIdInput, currentUserId ?? 0, currentUsername ?? "");
        if (lastBookingId == bookingId) lastBookingId = null; // Xóa BookingID nếu hủy
        ShowSuccessMessage("Hủy đặt phòng thành công! Nhấn phím bất kỳ để quay lại...");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        ShowErrorMessage($"Lỗi: {ex.Message}");
        Console.ReadKey();
    }
}

        private void ShowCheckIn()
{
    Console.Clear();
    DrawHeader("Hệ Thống Quản Lý Khách Sạn - Check-in");
    SetupBox(60, 10);

    Console.ForegroundColor = ConsoleColor.White;
    Console.SetCursorPosition(x + 2, y + 2);
    Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
    Console.SetCursorPosition(x + 2, y + 3);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 4);
    Console.Write($"ID đặt phòng { (lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "") } ");
    string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
    if (bookingIdInput == null) return;

    try
    {
        if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
            bookingIdInput = lastBookingId.ToString();
        _bookingBLL.CheckIn(bookingIdInput, currentUserId ?? 0, currentUsername ?? "");
        ShowSuccessMessage("Check-in thành công! Nhấn phím bất kỳ để quay lại...");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        ShowErrorMessage($"Lỗi: {ex.Message}");
        Console.ReadKey();
    }
}

        private void ShowCheckOut()
{
    Console.Clear();
    DrawHeader("Hệ Thống Quản Lý Khách Sạn - Check-out");
    SetupBox(60, 10);

    Console.ForegroundColor = ConsoleColor.White;
    Console.SetCursorPosition(x + 2, y + 2);
    Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
    Console.SetCursorPosition(x + 2, y + 3);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 4);
    Console.Write($"ID đặt phòng { (lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "") } ");
    string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
    if (bookingIdInput == null) return;

    try
    {
        if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
            bookingIdInput = lastBookingId.ToString();
        _bookingBLL.CheckOut(bookingIdInput, currentUserId ?? 0, currentUsername ?? "");
        ShowSuccessMessage("Check-out thành công! Nhấn phím bất kỳ để quay lại...");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        ShowErrorMessage($"Lỗi: {ex.Message}");
        Console.ReadKey();
    }
}

        private void ShowExtendBooking()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Gia Hạn Đặt Phòng");
            SetupBox(60, 12);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write($"ID đặt phòng { (lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "") } ");
            string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
            if (bookingIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Ngày gia hạn (yyyy-MM-dd): ");
            string? newEndDateInput = ReadInputWithEscape(x + 28, y + 5);
            if (newEndDateInput == null) return;

            try
            {
                if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
                    bookingIdInput = lastBookingId.ToString();
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");
                if (!DateTime.TryParse(newEndDateInput, out DateTime newEndDate))
                    throw new ArgumentException("Ngày gia hạn không hợp lệ.");
                _bookingBLL.ExtendBooking(bookingIdInput, newEndDate, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Gia hạn đặt phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }
        private void ShowGetBookingHistory()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Xem Lịch Sử Đặt Phòng");
            SetupBox(100, 16);
        
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));
        
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID khách hàng (CustomerID): ");
            string? customerIdInput = ReadInputWithEscape(x + 30, y + 4);
            if (customerIdInput == null) return;
        
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));
        
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("ID phòng (RoomID, để trống nếu không lọc): ");
            string? roomIdInput = ReadInputWithEscape(x + 38, y + 6);
            if (roomIdInput == null) return;
        
            try
            {
                int customerId = int.Parse(customerIdInput);
                int? roomId = string.IsNullOrEmpty(roomIdInput) ? (int?)null : int.Parse(roomIdInput);
                DataTable bookingHistory = _bookingBLL.GetBookingHistory(customerId, roomId);
                
                Console.SetCursorPosition(x + 2, y + 7);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--- LỊCH SỬ ĐẶT PHÒNG ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write(new string('─', width - 4));
        
                string[] headers = new[] { "ID Đặt Phòng", "ID Khách Hàng", "ID Phòng", "Ngày Check-in", "Ngày Check-out", "Trạng Thái" };
                int[] columnWidths = new int[headers.Length];
                for (int col = 0; col < headers.Length; col++)
                {
                    columnWidths[col] = headers[col].Length;
                }
        
                string[,] bookingData = new string[bookingHistory.Rows.Count, headers.Length];
                for (int i = 0; i < bookingHistory.Rows.Count; i++)
                {
                    bookingData[i, 0] = bookingHistory.Rows[i]["BookingID"].ToString();
                    bookingData[i, 1] = bookingHistory.Rows[i]["CustomerID"].ToString();
                    bookingData[i, 2] = bookingHistory.Rows[i]["RoomID"].ToString();
                    bookingData[i, 3] = ((DateTime)bookingHistory.Rows[i]["CheckInDate"]).ToString("yyyy-MM-dd HH:mm");
                    bookingData[i, 4] = ((DateTime)bookingHistory.Rows[i]["CheckOutDate"]).ToString("yyyy-MM-dd HH:mm");
                    bookingData[i, 5] = bookingHistory.Rows[i]["Status"].ToString();
        
                    for (int col = 0; col < headers.Length; col++)
                    {
                        int length = bookingData[i, col].Length;
                        if (length > columnWidths[col])
                            columnWidths[col] = length;
                        columnWidths[col] += 2;
                    }
                }
        
                Console.SetCursorPosition(x + 2, y + 9);
                Console.ForegroundColor = ConsoleColor.Magenta;
                for (int col = 0; col < headers.Length; col++)
                {
                    Console.Write(headers[col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
        
                Console.SetCursorPosition(x + 2, y + 10);
                Console.WriteLine(new string('─', width - 4));
                Console.ResetColor();
        
                for (int i = 0; i < bookingData.GetLength(0); i++)
                {
                    Console.SetCursorPosition(x + 2, y + 11 + i * 2);
                    for (int col = 0; col < bookingData.GetLength(1); col++)
                    {
                        Console.Write(bookingData[i, col].PadRight(columnWidths[col]));
                    }
                    Console.WriteLine();
                    if (i < bookingData.GetLength(0) - 1)
                    {
                        Console.SetCursorPosition(x + 2, y + 12 + i * 2);
                        Console.WriteLine(new string('─', width - 4));
                    }
                }
        
                Console.SetCursorPosition(x + 2, y + 11 + (bookingData.GetLength(0) - 1) * 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowInvoiceManagement()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(100, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH HÓA ĐƠN ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            string[,] invoices;
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT i.InvoiceID, c.Name, c.Phone, i.IssueDate, i.TotalAmount, i.PaymentStatus
                    FROM Invoices i
                    JOIN Customers c ON i.CustomerID = c.CustomerID
                    ORDER BY i.InvoiceID";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var tempInvoices = new System.Collections.Generic.List<string[]>();
                        while (reader.Read())
                        {
                            tempInvoices.Add(new[]
                            {
                                reader.GetInt32("InvoiceID").ToString(),
                                reader.GetString("Name"),
                                reader.GetString("Phone"),
                                reader.GetDateTime("IssueDate").ToString("yyyy-MM-dd HH:mm"),
                                reader.GetDecimal("TotalAmount").ToString("N0"),
                                reader.GetString("PaymentStatus") == "Paid" ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN"
                            });
                        }
                        invoices = new string[tempInvoices.Count, 6];
                        for (int i = 0; i < tempInvoices.Count; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                invoices[i, j] = tempInvoices[i][j];
                            }
                        }
                    }
                }
            }

            string[] headers = new[] { "Mã HĐ", "Tên Khách Hàng", "Điện Thoại", "Ngày Phát Hành", "Tổng (VND)", "Trạng Thái" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length;
                for (int row = 0; row < invoices.GetLength(0); row++)
                {
                    int length = invoices[row, col].Length;
                    if (length > columnWidths[col])
                        columnWidths[col] = length;
                }
                columnWidths[col] += 2;
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

            for (int i = 0; i < invoices.GetLength(0); i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                for (int col = 0; col < invoices.GetLength(1); col++)
                {
                    Console.Write(invoices[i, col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
                if (i < invoices.GetLength(0) - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (invoices.GetLength(0) - 1) * 2);
            Console.WriteLine("+ Nhấn ESC để quay lại");
            Console.SetCursorPosition(x + 2, y + 13 + (invoices.GetLength(0) - 1) * 2);
            Console.Write("+ Nhập mã hóa đơn để thanh toán: ");

            string? invoiceIdInput = ReadInputWithEscape(x + 34, y + 13 + (invoices.GetLength(0) - 1) * 2);
            if (invoiceIdInput == null)
            {
                Console.Clear();
                return;
            }

            if (!int.TryParse(invoiceIdInput, out int id))
            {
                ShowErrorMessage($"Mã hóa đơn {invoiceIdInput} không hợp lệ!");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            bool isValidInvoice = false;
            bool isUnpaid = false;
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string checkQuery = "SELECT PaymentStatus FROM Invoices WHERE InvoiceID = @invoiceId";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@invoiceId", id);
                    var result = checkCommand.ExecuteScalar();
                    if (result != null)
                    {
                        isValidInvoice = true;
                        isUnpaid = result.ToString() == "Unpaid";
                    }
                }

                if (isValidInvoice && isUnpaid)
                {
                    string updateQuery = "UPDATE Invoices SET PaymentStatus = 'Paid', UpdatedBy = @updatedBy, UpdatedByUsername = @updatedByUsername WHERE InvoiceID = @invoiceId";
                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@invoiceId", id);
                        updateCommand.Parameters.AddWithValue("@updatedBy", currentUserId ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@updatedByUsername", currentUsername ?? (object)DBNull.Value);
                        updateCommand.ExecuteNonQuery();
                    }
                    ShowSuccessMessage($"Thanh toán hóa đơn {invoiceIdInput} thành công!");
                    Thread.Sleep(1000);
                    Console.Clear();
                }
                else
                {
                    ShowErrorMessage($"Mã hóa đơn {invoiceIdInput} không hợp lệ hoặc đã thanh toán!");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private void ShowReportScreen()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(80, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- BÁO CÁO TỔNG QUAN ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            string[,] reportData;
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Bookings) AS TotalBookings,
                        (SELECT COUNT(*) FROM Invoices WHERE PaymentStatus = 'Paid') AS PaidInvoices,
                        (SELECT COUNT(*) FROM Invoices WHERE PaymentStatus = 'Unpaid') AS UnpaidInvoices,
                        (SELECT COALESCE(SUM(TotalAmount), 0) FROM Invoices WHERE PaymentStatus = 'Paid') AS Revenue,
                        (SELECT COUNT(*) FROM Rooms WHERE Status = 'Available') AS AvailableRooms";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reportData = new string[,]
                            {
                                { "Tổng đặt phòng", reader.GetInt32("TotalBookings").ToString() },
                                { "Đã thanh toán", reader.GetInt32("PaidInvoices").ToString() },
                                { "Chưa thanh toán", reader.GetInt32("UnpaidInvoices").ToString() },
                                { "Doanh thu (VND)", reader.GetDecimal("Revenue").ToString("N0") },
                                { "Phòng trống", reader.GetInt32("AvailableRooms").ToString() }
                            };
                        }
                        else
                        {
                            reportData = new string[,]
                            {
                                { "Tổng đặt phòng", "0" },
                                { "Đã thanh toán", "0" },
                                { "Chưa thanh toán", "0" },
                                { "Doanh thu (VND)", "0" },
                                { "Phòng trống", "0" }
                            };
                        }
                    }
                }

                string logQuery = "CALL generateReport(@reportType, @updatedBy, @updatedByUsername)";
                using (var logCommand = new MySqlCommand(logQuery, connection))
                {
                    logCommand.Parameters.AddWithValue("@reportType", "Tổng quan");
                    logCommand.Parameters.AddWithValue("@updatedBy", currentUserId ?? (object)DBNull.Value);
                    logCommand.Parameters.AddWithValue("@updatedByUsername", currentUsername ?? (object)DBNull.Value);
                    logCommand.ExecuteNonQuery();
                }
            }

            string[] headers = new[] { "Danh mục", "Giá trị" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length;
                for (int row = 0; row < reportData.GetLength(0); row++)
                {
                    int length = reportData[row, col].Length;
                    if (length > columnWidths[col])
                        columnWidths[col] = length;
                }
                columnWidths[col] += 2;
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

            for (int i = 0; i < reportData.GetLength(0); i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                for (int col = 0; col < reportData.GetLength(1); col++)
                {
                    Console.Write(reportData[i, col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
                if (i < reportData.GetLength(0) - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (reportData.GetLength(0) - 1) * 2);
            Console.WriteLine("+ Nhấn ESC để quay lại");
            Console.ReadKey();
            Console.Clear();
        }

        private void ShowPlaceholder(string option)
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(80, 10);
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write($"{option} - Chưa được triển khai bởi thành viên nhóm.");
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
            Console.Clear();
        }

        public void ShowSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message);
            Console.ResetColor();
        }

        public void ShowErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message);
            Console.ResetColor();
        }

        public void ShowInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message);
            Console.ResetColor();
        }

        public void Run()
        {
            while (true)
            {
                if (currentUsername == null)
                {
                    string? choice = ShowInitialScreen();
                    if (choice == null)
                    {
                        Console.Clear();
                        Environment.Exit(0);
                    }

                    if (choice == "0")
                    {
                        Console.Clear();
                        Environment.Exit(0);
                    }
                    else if (choice == "1")
                    {
                        var (username, password) = ShowLoginScreen();
                        if (username == null || password == null)
                            continue;

                        if (Login(username, password))
                        {
                            ShowSuccessMessage("Đăng nhập thành công!");
                            Thread.Sleep(1000);
                            Console.Clear();
                        }
                        else
                        {
                            ShowErrorMessage("Tên đăng nhập hoặc mật khẩu không đúng!");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else if (choice == "2")
                    {
                        var (newUsername, newPassword, confirmPassword, role) = ShowRegisterScreen();
                        if (newUsername == null || newPassword == null || confirmPassword == null || role == null)
                            continue;

                        if (newPassword != confirmPassword)
                        {
                            ShowErrorMessage("Mật khẩu xác nhận không khớp!");
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }

                        if (Register(newUsername, newPassword, role))
                        {
                            ShowSuccessMessage("Đăng ký thành công! Nhấn phím bất kỳ để quay lại...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            ShowErrorMessage("Tên đăng nhập đã tồn tại hoặc vai trò không hợp lệ!");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else
                {
                    ShowMainMenu();
                }
            }
        }
    }
}