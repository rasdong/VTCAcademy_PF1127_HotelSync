using System;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Collections.Generic;
using HotelManagementSystem.InvoiceManagement;
using HotelManagementSystem;

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
        private int currentUserId;
        private CustomerBL? customerManager;
        private StaffBL? staffManager;
        private RoomManagement? roomManager;
        private BookingManagement? bookingManager;
        private HotelManagementSystem.UserManagement.UserManagement? userManager;
        private InvoiceManager invoiceManager;
        private ServiceManagement? serviceManager;
        private ReportingAndAnalytics? reportingManager;

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
            roomManager = new RoomManagement();
            bookingManager = new BookingManagement();
            userManager = new HotelManagementSystem.UserManagement.UserManagement();
            invoiceManager = new InvoiceManager("Server=localhost;Database=hotel_management;Uid=root;Pwd=26122003;");
            serviceManager = new ServiceManagement("Server=localhost;Database=hotel_management;Uid=root;Pwd=26122003;");
            reportingManager = new ReportingAndAnalytics("Server=localhost;Database=hotel_management;Uid=root;Pwd=26122003;");
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

        private string? ReadInputWithEscape(int inputX, int inputY, string prompt, Func<string, bool> validation, string errorMessage)
        {
            string? input;
            do
            {
                Console.SetCursorPosition(inputX, inputY);
                Console.Write(new string(' ', width - 4));
                Console.SetCursorPosition(inputX, inputY);
                Console.Write(prompt);
                input = ReadInputWithEscape(inputX + prompt.Length, inputY);
                if (input == null) return null;
                if (!validation(input))
                {
                    ShowErrorMessage(errorMessage);
                    Thread.Sleep(1000);
                    Console.SetCursorPosition(inputX, inputY);
                    Console.Write(new string(' ', width - 4));
                }
            } while (input != null && !validation(input));
            return input;
        }

        private string? ReadInputWithEscape(int inputX, int inputY)
        {
            Console.SetCursorPosition(inputX, inputY);
            string input = string.Empty;
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
                        return input.Trim();
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input[..^1];
                        Console.SetCursorPosition(inputX, inputY);
                        Console.Write(new string(' ', input.Length + 1));
                        Console.SetCursorPosition(inputX, inputY);
                        Console.Write(input);
                    }
                    else if (keyInfo.KeyChar != '\0')
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
                    command.Parameters.AddWithValue("@password", password);
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

                string insertQuery = "INSERT INTO Users (Username, Password, RoleID) VALUES (@username, @password, @roleId)";
                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@username", username);
                    insertCommand.Parameters.AddWithValue("@password", password);
                    insertCommand.Parameters.AddWithValue("@roleId", roleId);
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

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Lựa chọn của bạn: ");
            return ReadInputWithEscape(x + 20, y + 9);
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
            string newPassword = ReadPassword();
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Xác nhận mật khẩu: ");
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
                "Admin" => 9,
                "Receptionist" => 6,
                "Housekeeping" => 4,
                "Manager" => 6,
                _ => 1
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
                "Admin" => new[] { "Quản lý phòng", "Quản lý khách hàng", "Quản lý nhân viên", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Quản lý người dùng", "Xem báo cáo", "Đăng xuất" },
                "Receptionist" => new[] { "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Xem báo cáo", "Đăng xuất" },
                "Housekeeping" => new[] { "Quản lý phòng", "Quản lý dịch vụ", "Xem báo cáo", "Đăng xuất" },
                "Manager" => new[] { "Quản lý khách hàng", "Quản lý nhân viên", "Quản lý đặt phòng", "Quản lý hóa đơn", "Xem báo cáo", "Đăng xuất" },
                _ => new[] { "Đăng xuất" }
            };

            for (int i = 0; i < options.Length; i++)
            {
                int optionY = y + 6 + i * 2;
                Console.SetCursorPosition(x + 2, optionY);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{i + 1}. ");
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
            if (int.TryParse(choiceInput, out int choice) && choice > 0 && choice <= options.Length)
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
                case "Đăng xuất":
                    currentUsername = null;
                    currentRole = null;
                    currentUserId = 0;
                    customerManager = null;
                    staffManager = null;
                    ShowSuccessMessage("Đăng xuất thành công!");
                    Thread.Sleep(1000);
                    Console.Clear();
                    break;
                case "Quản lý khách hàng":
                    if (customerManager == null && currentRole != null && currentUsername != null && currentUserId > 0)
                        customerManager = new CustomerBL(currentRole, currentUserId, currentUsername);
                    ShowCustomerManagement();
                    break;
                case "Quản lý nhân viên":
                    ShowEmployeeManagement();
                    break;
                case "Quản lý hóa đơn":
                    ShowInvoiceManagement();
                    break;
                case "Xem báo cáo":
                    ShowReportScreen();
                    break;
                case "Quản lý phòng":
                    ShowRoomManagement();
                    break;
                case "Quản lý đặt phòng":
                    ShowBookingManagement();
                    break;
                case "Quản lý dịch vụ":
                    ShowServiceManagement();
                    break;
                case "Quản lý người dùng":
                    ShowUserManagement();
                    break;
                default:
                    ShowInfoMessage($"Đã chọn: {option}. (Chưa được triển khai - Nhấn phím bất kỳ để quay lại...)");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
        }

        private void ShowCustomerManagement()
        {
            if (customerManager == null)
            {
                ShowErrorMessage("Chưa khởi tạo quản lý khách hàng! Vui lòng đăng nhập lại.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            while (true)
            {
                Console.Clear();
                DrawHeader("Quản Lý Khách Hàng");
                int optionCount = 6;
                SetupBox(80, 8 + optionCount * 2 + 4);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write($"Chào mừng, {currentUsername}! (Vai trò: {currentRole ?? "Chưa đăng nhập"})");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                string[] options = new[] {
                    "Thêm khách hàng",
                    "Sửa thông tin khách hàng",
                    "Xóa khách hàng",
                    "Tìm kiếm khách hàng",
                    "Xem lịch sử đặt phòng",
                    "Quay lại"
                };
                for (int i = 0; i < options.Length; i++)
                {
                    int optionY = y + 6 + i * 2;
                    Console.SetCursorPosition(x + 2, optionY);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{i + 1}. ");
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
                string? choice = ReadInputWithEscape(x + 20, y + height - 4);

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        DrawHeader("Thêm Khách Hàng");
                        SetupBox(80, 18);
                        string? idInput, name, idCard, phone, email, nationality;

                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 20, y + 2, "Mã khách hàng: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã khách hàng phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int customerId = int.Parse(idInput);

                        Console.SetCursorPosition(x + 2, y + 3);
                        name = ReadInputWithEscape(x + 10, y + 3, "Tên: ", s => !string.IsNullOrWhiteSpace(s) && s.Any(char.IsLetter) && s.Any(c => c == ' ' || char.IsLetter(c)), "Tên phải chứa chữ có dấu và có thể có khoảng cách! Vui lòng nhập lại.");
                        if (name == null) break;

                        Console.SetCursorPosition(x + 2, y + 4);
                        idCard = ReadInputWithEscape(x + 20, y + 4, "IDCard: ", s => !string.IsNullOrWhiteSpace(s) && s.Length == 12 && s.All(char.IsDigit), "IDCard phải có đúng 12 chữ số! Vui lòng nhập lại.");
                        if (idCard == null) break;

                        Console.SetCursorPosition(x + 2, y + 5);
                        phone = ReadInputWithEscape(x + 10, y + 5, "SĐT: ", s => !string.IsNullOrWhiteSpace(s) && s.Length == 10 && s.All(char.IsDigit), "SĐT phải có đúng 10 chữ số! Vui lòng nhập lại.");
                        if (phone == null) break;

                        Console.SetCursorPosition(x + 2, y + 6);
                        email = ReadInputWithEscape(x + 12, y + 6, "Email: ", s => string.IsNullOrWhiteSpace(s) || s.Contains("@") && s.Contains("."), "Email không hợp lệ! Vui lòng nhập lại (có thể để trống).");
                        if (email == null) break;

                        Console.SetCursorPosition(x + 2, y + 7);
                        nationality = ReadInputWithEscape(x + 15, y + 7, "Quốc tịch: ", s => !string.IsNullOrWhiteSpace(s) && s.Any(char.IsLetter) && s.Any(c => c == ' ' || char.IsLetter(c)), "Quốc tịch phải chứa chữ có dấu và có thể có khoảng cách! Vui lòng nhập lại.");
                        if (nationality == null) break;

                        try
                        {
                            customerManager.AddCustomer(customerId, name, idCard, phone, email ?? "", nationality);
                            ShowSuccessMessage("Đã thêm khách hàng thành công!");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        DrawHeader("Sửa Thông Tin Khách Hàng");
                        SetupBox(80, 16);
                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 25, y + 2, "Mã khách hàng cần sửa: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã khách hàng phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int idEdit = int.Parse(idInput);

                        Console.SetCursorPosition(x + 2, y + 3);
                        name = ReadInputWithEscape(x + 15, y + 3, "Tên mới: ", s => !string.IsNullOrWhiteSpace(s) && s.Any(char.IsLetter) && s.Any(c => c == ' ' || char.IsLetter(c)), "Tên phải chứa chữ có dấu và có thể có khoảng cách! Vui lòng nhập lại.");
                        if (name == null) break;

                        Console.SetCursorPosition(x + 2, y + 4);
                        idCard = ReadInputWithEscape(x + 20, y + 4, "IDCard: ", s => !string.IsNullOrWhiteSpace(s) && s.Length == 12 && s.All(char.IsDigit), "IDCard phải có đúng 12 chữ số! Vui lòng nhập lại.");
                        if (idCard == null) break;

                        Console.SetCursorPosition(x + 2, y + 5);
                        phone = ReadInputWithEscape(x + 10, y + 5, "SĐT: ", s => !string.IsNullOrWhiteSpace(s) && s.Length == 10 && s.All(char.IsDigit), "SĐT phải có đúng 10 chữ số! Vui lòng nhập lại.");
                        if (phone == null) break;

                        Console.SetCursorPosition(x + 2, y + 6);
                        email = ReadInputWithEscape(x + 12, y + 6, "Email: ", s => string.IsNullOrWhiteSpace(s) || s.Contains("@") && s.Contains("."), "Email không hợp lệ! Vui lòng nhập lại (có thể để trống).");
                        if (email == null) break;

                        Console.SetCursorPosition(x + 2, y + 7);
                        nationality = ReadInputWithEscape(x + 15, y + 7, "Quốc tịch: ", s => !string.IsNullOrWhiteSpace(s) && s.Any(char.IsLetter) && s.Any(c => c == ' ' || char.IsLetter(c)), "Quốc tịch phải chứa chữ có dấu và có thể có khoảng cách! Vui lòng nhập lại.");
                        if (nationality == null) break;

                        try
                        {
                            customerManager.UpdateCustomer(idEdit, name, idCard, phone, email ?? "", nationality);
                            ShowSuccessMessage("Đã cập nhật thông tin khách hàng!");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        DrawHeader("Xóa Khách Hàng");
                        SetupBox(80, 10);
                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 25, y + 2, "Mã khách hàng cần xóa: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã khách hàng phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int idDel = int.Parse(idInput);

                        Console.SetCursorPosition(x + 2, y + 3);
                        var confirm = ReadInputWithEscape(x + 20, y + 3, "Xác nhận xóa (Y/N): ", s => s.ToUpper() == "Y" || s.ToUpper() == "N", "Vui lòng nhập Y hoặc N!");
                        if (confirm == null || confirm.ToUpper() != "Y") break;

                        try
                        {
                            customerManager.DeleteCustomer(idDel);
                            ShowSuccessMessage("Đã xóa khách hàng thành công!");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        DrawHeader("Tìm Kiếm Khách Hàng");
                        SetupBox(80, 20);
                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 20, y + 2, "Mã khách hàng: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã khách hàng phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int customerIdSearch = int.Parse(idInput);
                        try
                        {
                            var results = customerManager.SearchCustomer(customerIdSearch);
                            Console.SetCursorPosition(x + 2, y + 4);
                            Console.WriteLine("Kết quả tìm kiếm:");
                            Console.SetCursorPosition(x + 2, y + 5);
                            Console.WriteLine(new string('─', width - 4));

                            if (results.Count == 0)
                            {
                                Console.SetCursorPosition(x + 2, y + 6);
                                Console.WriteLine($"Không tìm thấy khách hàng với ID {customerIdSearch}.");
                                Console.SetCursorPosition(x + 2, y + 8);
                                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                                Console.ReadKey();
                                break;
                            }

                            string[] headers = { "CustomerID", "Name", "IDCard", "Phone", "Email", "Nationality" };
                            int[] columnWidths = new int[headers.Length];
                            for (int col = 0; col < headers.Length; col++)
                            {
                                columnWidths[col] = headers[col].Length;
                                foreach (var cust in results)
                                {
                                    int length = cust[headers[col]].Length;
                                    if (length > columnWidths[col])
                                        columnWidths[col] = Math.Min(length, width / headers.Length - 2);
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

                            int rowIndex = 0;
                            foreach (var cust in results)
                            {
                                Console.SetCursorPosition(x + 2, y + 8 + rowIndex * 2);
                                Console.Write(cust["CustomerID"].PadRight(columnWidths[0]));
                                Console.Write(cust["Name"].PadRight(columnWidths[1]));
                                Console.Write(cust["IDCard"].PadRight(columnWidths[2]));
                                Console.Write(cust["Phone"].PadRight(columnWidths[3]));
                                Console.Write(cust["Email"].PadRight(columnWidths[4]));
                                Console.Write(cust["Nationality"].PadRight(columnWidths[5]));
                                Console.WriteLine();
                                if (rowIndex < results.Count - 1)
                                {
                                    Console.SetCursorPosition(x + 2, y + 9 + rowIndex * 2);
                                    Console.WriteLine(new string('─', width - 4));
                                }
                                rowIndex++;
                            }

                            Console.SetCursorPosition(x + 2, y + 10 + rowIndex * 2);
                            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                            Console.ReadKey();
                        }
                        break;

                    case "5":
                        Console.Clear();
                        DrawHeader("Lịch Sử Đặt Phòng");
                        SetupBox(100, 20);
                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 20, y + 2, "Mã khách hàng: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã khách hàng phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int idHistory = int.Parse(idInput);
                        try
                        {
                            var history = customerManager.GetBookingHistory(idHistory);
                            Console.SetCursorPosition(x + 2, y + 4);
                            Console.WriteLine("Lịch sử đặt phòng:");
                            Console.SetCursorPosition(x + 2, y + 5);
                            Console.WriteLine(new string('─', width - 4));

                            if (history.Count == 0)
                            {
                                Console.SetCursorPosition(x + 2, y + 6);
                                Console.WriteLine($"Không tìm thấy lịch sử đặt phòng cho khách hàng ID {idHistory}.");
                                Console.SetCursorPosition(x + 2, y + 8);
                                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                                Console.ReadKey();
                                break;
                            }

                            string[] headers = { "BookingID", "CheckInDate", "CheckOutDate", "Status", "RoomNumber", "InvoiceID", "TotalAmount", "PaymentStatus" };
                            int[] columnWidths = new int[headers.Length];
                            for (int col = 0; col < headers.Length; col++)
                            {
                                columnWidths[col] = headers[col].Length;
                                foreach (var row in history)
                                {
                                    int length = row[headers[col]].Length;
                                    if (length > columnWidths[col])
                                        columnWidths[col] = Math.Min(length, width / headers.Length - 2);
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

                            int rowIndex = 0;
                            foreach (var row in history)
                            {
                                Console.SetCursorPosition(x + 2, y + 8 + rowIndex * 2);
                                Console.Write(row["BookingID"].PadRight(columnWidths[0]));
                                Console.Write(row["CheckInDate"].PadRight(columnWidths[1]));
                                Console.Write(row["CheckOutDate"].PadRight(columnWidths[2]));
                                Console.Write(row["Status"].PadRight(columnWidths[3]));
                                Console.Write(row["RoomNumber"].PadRight(columnWidths[4]));
                                Console.Write(row["InvoiceID"].PadRight(columnWidths[5]));
                                Console.Write(row["TotalAmount"].PadRight(columnWidths[6]));
                                Console.Write(row["PaymentStatus"].PadRight(columnWidths[7]));
                                Console.WriteLine();
                                if (rowIndex < history.Count - 1)
                                {
                                    Console.SetCursorPosition(x + 2, y + 9 + rowIndex * 2);
                                    Console.WriteLine(new string('─', width - 4));
                                }
                                rowIndex++;
                            }

                            Console.SetCursorPosition(x + 2, y + 10 + rowIndex * 2);
                            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                            Console.ReadKey();
                        }
                        break;

                    case "6":
                        Console.Clear();
                        return;

                    default:
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowEmployeeManagement()
        {
            if (staffManager == null)
            {
                staffManager = new StaffBL(currentRole ?? string.Empty, currentUserId, currentUsername ?? string.Empty);
            }

            while (true)
            {
                Console.Clear();
                DrawHeader("Quản Lý Nhân Viên");
                int optionCount = 5;
                SetupBox(80, 8 + optionCount * 2 + 4);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write($"Chào mừng, {currentUsername}! (Vai trò: {currentRole ?? "Chưa đăng nhập"})");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                string[] options = new[] {
                    "Thêm nhân viên",
                    "Xóa nhân viên",
                    "Gán vai trò mới",
                    "Xem danh sách nhân viên",
                    "Quay lại"
                };
                for (int i = 0; i < options.Length; i++)
                {
                    int optionY = y + 6 + i * 2;
                    Console.SetCursorPosition(x + 2, optionY);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{i + 1}. ");
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
                string? choice = ReadInputWithEscape(x + 20, y + height - 4);

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        DrawHeader("Thêm Nhân Viên");
                        SetupBox(80, 14);
                        string? idInput, name, role;

                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 20, y + 2, "Mã nhân viên: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã nhân viên phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int employeeId = int.Parse(idInput);

                        Console.SetCursorPosition(x + 2, y + 3);
                        name = ReadInputWithEscape(x + 10, y + 3, "Tên: ", s => !string.IsNullOrWhiteSpace(s) && s.Any(char.IsLetter) && s.Any(c => char.IsLetter(c) || c == ' '), "Tên phải chứa chữ cái có dấu và có thể có khoảng cách! Vui lòng nhập lại.");
                        if (name == null) break;

                        Console.SetCursorPosition(x + 2, y + 4);
                        role = ReadInputWithEscape(x + 10, y + 4, "Vai trò (Receptionist/Housekeeping/Manager): ", s => !string.IsNullOrWhiteSpace(s) && new[] { "receptionist", "housekeeping", "manager" }.Any(r => r.Equals(s.ToLower())), "Vai trò phải là Receptionist, Housekeeping, hoặc Manager! Vui lòng nhập lại.");
                        if (role == null) break;
                        role = char.ToUpper(role[0]) + role.Substring(1).ToLower();

                        try
                        {
                            staffManager.AddStaff(employeeId, name, role);
                            ShowSuccessMessage("Đã thêm nhân viên thành công!");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        DrawHeader("Xóa Nhân Viên");
                        SetupBox(80, 10);
                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 20, y + 2, "Mã nhân viên cần xóa: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã nhân viên phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int idDel = int.Parse(idInput);
                        try
                        {
                            staffManager.DeleteStaff(idDel);
                            ShowSuccessMessage("Đã xóa nhân viên thành công!");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        DrawHeader("Gán Vai Trò Mới");
                        SetupBox(80, 12);
                        Console.SetCursorPosition(x + 2, y + 2);
                        idInput = ReadInputWithEscape(x + 20, y + 2, "Mã nhân viên: ", s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int id) && id > 0 && s.All(char.IsDigit), "Mã nhân viên phải là số nguyên dương gồm các chữ số! Vui lòng nhập lại.");
                        if (idInput == null) break;
                        int idAssign = int.Parse(idInput);

                        Console.SetCursorPosition(x + 2, y + 3);
                        role = ReadInputWithEscape(x + 20, y + 3, "Vai trò mới (Receptionist/Housekeeping/Manager): ", s => !string.IsNullOrWhiteSpace(s) && new[] { "receptionist", "housekeeping", "manager" }.Any(r => r.Equals(s.ToLower())), "Vai trò phải là Receptionist, Housekeeping, hoặc Manager! Vui lòng nhập lại.");
                        if (role == null) break;
                        role = char.ToUpper(role[0]) + role.Substring(1).ToLower();

                        try
                        {
                            staffManager.AssignRole(idAssign, role);
                            ShowSuccessMessage("Đã gán vai trò mới thành công!");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        DrawHeader("Danh Sách Nhân Viên");
                        SetupBox(80, 20);
                        Console.SetCursorPosition(x + 2, y + 2);
                        string? roleInput = ReadInputWithEscape(x + 20, y + 2, "Vai trò (Receptionist/Housekeeping/Manager/All): ", s => !string.IsNullOrWhiteSpace(s) && new[] { "receptionist", "housekeeping", "manager", "all" }.Any(r => r.Equals(s.ToLower())), "Vai trò phải là Receptionist, Housekeeping, Manager, hoặc All! Vui lòng nhập lại.");
                        if (roleInput == null) break;
                        roleInput = char.ToUpper(roleInput[0]) + roleInput.Substring(1).ToLower();

                        try
                        {
                            var results = staffManager.GetStaffByRole(roleInput);
                            Console.SetCursorPosition(x + 2, y + 4);
                            Console.WriteLine("Danh sách nhân viên:");
                            Console.SetCursorPosition(x + 2, y + 5);
                            Console.WriteLine(new string('─', width - 4));

                            if (results.Count == 0)
                            {
                                Console.SetCursorPosition(x + 2, y + 6);
                                Console.WriteLine($"Không tìm thấy nhân viên với vai trò {roleInput}.");
                                Console.SetCursorPosition(x + 2, y + 8);
                                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                                Console.ReadKey();
                                break;
                            }

                            string[] headers = { "StaffID", "Name", "Role" };
                            int[] columnWidths = new int[headers.Length];
                            for (int col = 0; col < headers.Length; col++)
                            {
                                columnWidths[col] = headers[col].Length;
                                foreach (var emp in results)
                                {
                                    int length = emp[headers[col]].Length;
                                    if (length > columnWidths[col])
                                        columnWidths[col] = Math.Min(length, width / headers.Length - 2);
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

                            int rowIndex = 0;
                            foreach (var emp in results)
                            {
                                Console.SetCursorPosition(x + 2, y + 8 + rowIndex * 2);
                                Console.Write(emp["StaffID"].PadRight(columnWidths[0]));
                                Console.Write(emp["Name"].PadRight(columnWidths[1]));
                                Console.Write(emp["Role"].PadRight(columnWidths[2]));
                                Console.WriteLine();
                                if (rowIndex < results.Count - 1)
                                {
                                    Console.SetCursorPosition(x + 2, y + 9 + rowIndex * 2);
                                    Console.WriteLine(new string('─', width - 4));
                                }
                                rowIndex++;
                            }

                            Console.SetCursorPosition(x + 2, y + 10 + rowIndex * 2);
                            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage(ex.Message);
                        }
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.Clear();
                        return;

                    default:
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowUserManagement()
        {
            if (currentRole != null && currentRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                while (true)
                {
                    Console.Clear();
                    DrawHeader("Quản Lý Người Dùng");
                    int optionCount = 4;
                    SetupBox(80, 8 + optionCount * 2 + 4);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(x + 2, y + 2);
                    Console.Write($"Chào mừng, {currentUsername}! (Vai trò: {currentRole})");
                    Console.SetCursorPosition(x + 2, y + 3);
                    Console.Write(new string('─', width - 4));

                    Console.SetCursorPosition(x + 2, y + 4);
                    Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                    Console.SetCursorPosition(x + 2, y + 5);
                    Console.Write(new string('─', width - 4));

                    string[] options = new[] {
                        "Xem danh sách user",
                        "Phân quyền user",
                        "Xóa user",
                        "Quay lại"
                    };
                    for (int i = 0; i < options.Length; i++)
                    {
                        int optionY = y + 6 + i * 2;
                        Console.SetCursorPosition(x + 2, optionY);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($"{i + 1}. ");
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
                    string? choice = ReadInputWithEscape(x + 20, y + height - 4);

                    switch (choice)
                    {
                        case "1":
                            Console.Clear();
                            DrawHeader("Danh Sách User");
                            SetupBox(100, 30);
                            var users = userManager?.GetAllUsers();
                            if (users != null && users.Count > 0)
                            {
                                string[] headers = { "UserID", "Username", "Role", "CreatedAt", "UpdatedAt" };
                                int[] columnWidths = { 8, 20, 15, 20, 20 };
                                Console.SetCursorPosition(x + 2, y + 2);
                                for (int i = 0; i < headers.Length; i++)
                                    Console.Write(headers[i].PadRight(columnWidths[i]));
                                Console.WriteLine();
                                Console.SetCursorPosition(x + 2, y + 3);
                                Console.WriteLine(new string('─', width - 4));
                                int rowIndex = 0;
                                foreach (var u in users)
                                {
                                    Console.SetCursorPosition(x + 2, y + 4 + rowIndex);
                                    Console.Write(u.UserID.ToString().PadRight(columnWidths[0]));
                                    Console.Write(u.Username.PadRight(columnWidths[1]));
                                    Console.Write(u.Role.PadRight(columnWidths[2]));
                                    Console.Write(u.CreatedAt.ToString("yyyy-MM-dd HH:mm").PadRight(columnWidths[3]));
                                    Console.Write(u.UpdatedAt.ToString("yyyy-MM-dd HH:mm").PadRight(columnWidths[4]));
                                    Console.WriteLine();
                                    rowIndex++;
                                }
                            }
                            else
                            {
                                Console.SetCursorPosition(x + 2, y + 4);
                                Console.WriteLine("Không có user nào!");
                            }
                            Console.SetCursorPosition(x + 2, y + height - 2);
                            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                            Console.ReadKey();
                            break;
                        case "2":
                            Console.Clear();
                            DrawHeader("Phân Quyền User");
                            string? userIdStr = ReadInputWithEscape(x + 20, y + 2, "UserID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
                            if (userIdStr == null) break;
                            int userId = int.Parse(userIdStr);
                            Console.SetCursorPosition(x + 2, y + 3);
                            string? newRole = ReadInputWithEscape(x + 20, y + 3, "Vai trò mới (Admin/Receptionist/Housekeeping/Manager): ", s => new[]{"Admin","Receptionist","Housekeeping","Manager"}.Contains(s), "Sai vai trò!");
                            if (newRole == null) break;
                            var user = userManager?.GetUserById(userId);
                            if (user == null)
                            {
                                ShowErrorMessage($"Không tìm thấy user với ID {userId}!");
                                Console.ReadKey();
                                break;
                            }
                            user.Role = newRole;
                            user.UpdatedAt = DateTime.Now;
                            userManager?.UpdateUser(user);
                            ShowSuccessMessage($"Đã phân quyền user {user.Username} thành {newRole}!");
                            Console.ReadKey();
                            break;
                        case "3":
                            Console.Clear();
                            DrawHeader("Xóa User");
                            SetupBox(60, 10);
                            Console.SetCursorPosition(x + 2, y + 2);
                            string? delUserIdStr = ReadInputWithEscape(x + 20, y + 2, "UserID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
                            if (delUserIdStr == null) break;
                            int delUserId = int.Parse(delUserIdStr);
                            if (userManager?.RemoveUser(delUserId) == true)
                                ShowSuccessMessage("Xóa user thành công!");
                            else
                                ShowErrorMessage("Không tìm thấy user!");
                            Console.ReadKey();
                            break;
                        case "4":
                            Console.Clear();
                            return;
                        default:
                            ShowErrorMessage("Lựa chọn không hợp lệ!");
                            Console.ReadKey();
                            break;
                    }
                }
            }
            else
            {
                ShowErrorMessage("Chỉ Admin mới có quyền quản lý người dùng!");
                Console.ReadKey();
            }
        }

        private void ShowInvoiceManagement()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(120, 28);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH HÓA ĐƠN TỔNG ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            var invoices = invoiceManager?.GetAllInvoices() ?? new List<Invoice>();
            string[] headers = { "Mã HĐ", "Mã Đặt Phòng", "Mã KH", "Tổng tiền (VND)", "Ngày lập", "Trạng thái", "Người cập nhật" };
            int[] columnWidths = { 8, 12, 8, 18, 20, 14, 18 };

            Console.SetCursorPosition(x + 2, y + 6);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
                Console.Write(headers[col].PadRight(columnWidths[col]));
            Console.WriteLine();
            Console.SetCursorPosition(x + 2, y + 7);
            Console.WriteLine(new string('─', width - 4));
            Console.ResetColor();

            int rowBase = 8;
            for (int i = 0; i < invoices.Count; i++)
            {
                var inv = invoices[i];
                Console.SetCursorPosition(x + 2, y + rowBase + i);
                Console.Write(inv.InvoiceID.ToString().PadRight(columnWidths[0]));
                Console.Write(inv.BookingID.ToString().PadRight(columnWidths[1]));
                Console.Write(inv.CustomerID.ToString().PadRight(columnWidths[2]));
                Console.Write(inv.TotalAmount.ToString("N0").PadRight(columnWidths[3]));
                Console.Write(inv.IssueDate.ToString("yyyy-MM-dd HH:mm").PadRight(columnWidths[4]));
                Console.Write((inv.PaymentStatus == "Paid" ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN").PadRight(columnWidths[5]));
                Console.Write((inv.UpdatedByUsername ?? "").PadRight(columnWidths[6]));
                Console.WriteLine();
            }

            Console.SetCursorPosition(x + 2, y + rowBase + invoices.Count + 1);
            Console.WriteLine("+ Nhấn ESC để quay lại");
            Console.SetCursorPosition(x + 2, y + rowBase + invoices.Count + 2);
            Console.Write("+ Nhập mã hóa đơn để thanh toán: ");

            string invoiceIdStr = ReadInputWithEscape(x + 34, y + rowBase + invoices.Count + 2) ?? string.Empty;
            if (string.IsNullOrEmpty(invoiceIdStr))
            {
                Console.Clear();
                return;
            }
            if (!int.TryParse(invoiceIdStr, out int invoiceId))
            {
                ShowErrorMessage($"Mã hóa đơn {invoiceIdStr} không hợp lệ!");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            var invoice = invoiceManager?.GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                ShowErrorMessage($"Không tìm thấy hóa đơn với mã {invoiceId}!");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            if (invoice.PaymentStatus == "Paid")
            {
                ShowErrorMessage($"Hóa đơn {invoiceId} đã thanh toán!");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            // Thực hiện thanh toán hóa đơn tổng
            bool result = invoiceManager != null && invoiceManager.UpdatePaymentStatus(invoiceId, "Paid", currentUserId, currentUsername ?? "");
            if (result)
            {
                ShowSuccessMessage($"Thanh toán hóa đơn {invoiceId} thành công!");
                Thread.Sleep(1000);
                Console.Clear();
            }
            else
            {
                ShowErrorMessage($"Thanh toán hóa đơn {invoiceId} thất bại!");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Ví dụ sử dụng trong ShowReportScreen (bạn có thể mở rộng thêm):
        private void ShowReportScreen()
        {
            Console.Clear();
            DrawHeader("Báo Cáo & Thống Kê");
            SetupBox(100, 20);
            DateTime today = DateTime.Today;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            decimal totalRevenue = reportingManager?.GetTotalRevenue(startOfMonth, endOfMonth) ?? 0;
            double occupancy = reportingManager?.GetRoomOccupancyRate(startOfMonth, endOfMonth) ?? 0;
            var topService = reportingManager?.GetTopService(startOfMonth, endOfMonth);
            Console.SetCursorPosition(x + 2, y + 2);
            Console.WriteLine($"Doanh thu tháng này: {totalRevenue:N0} VND");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.WriteLine($"Tỷ lệ lấp đầy phòng: {occupancy:F2}%");
            if (topService != null && !string.IsNullOrEmpty(topService.Value.ServiceName))
            {
                Console.SetCursorPosition(x + 2, y + 4);
                Console.WriteLine($"Dịch vụ doanh thu cao nhất: {topService.Value.ServiceName} ({topService.Value.TotalRevenue:N0} VND, {topService.Value.TimesUsed} lượt)");
            }
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
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

        private void ShowRoomManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Quản Lý Phòng");
                int optionCount = 7;
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

                string[] options = new[] {
                    "Xem danh sách phòng",
                    "Thêm phòng mới",
                    "Sửa thông tin phòng",
                    "Xóa phòng",
                    "Tìm kiếm phòng",
                    "Kiểm tra phòng trống theo thời gian",
                    "Quay lại"
                };
                for (int i = 0; i < options.Length; i++)
                {
                    int optionY = y + 6 + i * 2;
                    Console.SetCursorPosition(x + 2, optionY);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{i + 1}. ");
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
                int choice = 0;
                int.TryParse(choiceInput, out choice);
                switch (choice)
                {
                    case 1:
                        // Xem danh sách phòng
                        ShowRoomList();
                        break;
                    case 2:
                        // Thêm phòng mới
                        ShowAddRoom();
                        break;
                    case 3:
                        // Sửa thông tin phòng
                        ShowEditRoom();
                        break;
                    case 4:
                        // Xóa phòng
                        ShowDeleteRoom();
                        break;
                    case 5:
                        // Tìm kiếm phòng
                        ShowSearchRoom();
                        break;
                    case 6:
                        // Kiểm tra phòng trống theo thời gian
                        ShowCheckRoomAvailability();
                        break;
                    case 7:
                        Console.Clear();
                        return;
                    default:
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowRoomList()
        {
            var rooms = roomManager?.GetAllRooms();
            Console.Clear();
            DrawHeader("Danh Sách Phòng");
            SetupBox(120, 30);
            if (rooms != null && rooms.Count > 0)
            {
                string[] headers = { "RoomID", "RoomNumber", "RoomType", "Price", "Status", "Amenities" };
                int[] columnWidths = { 8, 12, 10, 10, 18, 40 };
                Console.SetCursorPosition(x + 2, y + 2);
                for (int i = 0; i < headers.Length; i++)
                    Console.Write(headers[i].PadRight(columnWidths[i]));
                Console.WriteLine();
                Console.SetCursorPosition(x + 2, y + 3);
                Console.WriteLine(new string('─', width - 4));
                int rowIndex = 0;
                foreach (var room in rooms)
                {
                    Console.SetCursorPosition(x + 2, y + 4 + rowIndex);
                    Console.Write(room.RoomID.ToString().PadRight(columnWidths[0]));
                    Console.Write(room.RoomNumber.PadRight(columnWidths[1]));
                    Console.Write(room.RoomType.PadRight(columnWidths[2]));
                    Console.Write(room.Price.ToString("N0").PadRight(columnWidths[3]));
                    Console.Write(room.Status.PadRight(columnWidths[4]));
                    Console.Write(room.Amenities.PadRight(columnWidths[5]));
                    Console.WriteLine();
                    rowIndex++;
                }
            }
            else
            {
                Console.SetCursorPosition(x + 2, y + 4);
                Console.WriteLine("Không có phòng nào!");
            }
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowAddRoom()
        {
            Console.Clear();
            DrawHeader("Thêm Phòng Mới");
            SetupBox(80, 16);
            Console.SetCursorPosition(x + 2, y + 2);
            string? roomNumber = ReadInputWithEscape(x + 20, y + 2, "Số phòng: ", s => !string.IsNullOrWhiteSpace(s), "Không được để trống!");
            if (roomNumber == null) return;
            Console.SetCursorPosition(x + 2, y + 3);
            string? roomType = ReadInputWithEscape(x + 20, y + 3, "Loại phòng (Single/Double/Suite): ", s => new[]{"Single","Double","Suite"}.Contains(s), "Chỉ nhận Single/Double/Suite!");
            if (roomType == null) return;
            Console.SetCursorPosition(x + 2, y + 4);
            string? priceStr = ReadInputWithEscape(x + 20, y + 4, "Giá: ", s => decimal.TryParse(s, out var v) && v > 0, "Giá phải là số dương!");
            if (priceStr == null) return;
            decimal price = decimal.Parse(priceStr);
            Console.SetCursorPosition(x + 2, y + 5);
            string? amenities = ReadInputWithEscape(x + 20, y + 5, "Tiện nghi: ", s => !string.IsNullOrWhiteSpace(s), "Không được để trống!");
            if (amenities == null) return;
            try
            {
                roomManager?.AddRoom(roomNumber, roomType, price, amenities, currentUserId, currentUsername ?? "");
                ShowSuccessMessage("Thêm phòng thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            Console.ReadKey();
        }

        private void ShowEditRoom()
        {
            Console.Clear();
            DrawHeader("Sửa Thông Tin Phòng");
            SetupBox(80, 18);
            Console.SetCursorPosition(x + 2, y + 2);
            string? roomIdStr = ReadInputWithEscape(x + 20, y + 2, "RoomID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (roomIdStr == null) return;
            int roomId = int.Parse(roomIdStr);
            Console.SetCursorPosition(x + 2, y + 3);
            string? roomNumber = ReadInputWithEscape(x + 20, y + 3, "Số phòng: ", s => !string.IsNullOrWhiteSpace(s), "Không được để trống!");
            if (roomNumber == null) return;
            Console.SetCursorPosition(x + 2, y + 4);
            string? roomType = ReadInputWithEscape(x + 20, y + 4, "Loại phòng (Single/Double/Suite): ", s => new[]{"Single","Double","Suite"}.Contains(s), "Chỉ nhận Single/Double/Suite!");
            if (roomType == null) return;
            Console.SetCursorPosition(x + 2, y + 5);
            string? priceStr = ReadInputWithEscape(x + 20, y + 5, "Giá: ", s => decimal.TryParse(s, out var v) && v > 0, "Giá phải là số dương!");
            if (priceStr == null) return;
            decimal price = decimal.Parse(priceStr);
            Console.SetCursorPosition(x + 2, y + 6);
            string? status = ReadInputWithEscape(x + 20, y + 6, "Trạng thái (Available/Occupied/Under Maintenance): ", s => new[]{"Available","Occupied","Under Maintenance"}.Contains(s), "Sai trạng thái!");
            if (status == null) return;
            Console.SetCursorPosition(x + 2, y + 7);
            string? amenities = ReadInputWithEscape(x + 20, y + 7, "Tiện nghi: ", s => !string.IsNullOrWhiteSpace(s), "Không được để trống!");
            if (amenities == null) return;
            try
            {
                roomManager?.UpdateRoom(roomId, roomNumber, roomType, price, status, amenities, currentUserId, currentUsername ?? "");
                ShowSuccessMessage("Cập nhật phòng thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            Console.ReadKey();
        }

        private void ShowDeleteRoom()
        {
            Console.Clear();
            DrawHeader("Xóa Phòng");
            SetupBox(60, 10);
            Console.SetCursorPosition(x + 2, y + 2);
            string? roomIdStr = ReadInputWithEscape(x + 20, y + 2, "RoomID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (roomIdStr == null) return;
            int roomId = int.Parse(roomIdStr);
            try
            {
                roomManager?.DeleteRoom(roomId, currentUserId, currentUsername ?? "");
                ShowSuccessMessage("Xóa phòng thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            Console.ReadKey();
        }

        private void ShowSearchRoom()
        {
            Console.Clear();
            DrawHeader("Tìm Kiếm Phòng");
            SetupBox(80, 14);
            Console.SetCursorPosition(x + 2, y + 2);
            string? status = ReadInputWithEscape(x + 20, y + 2, "Trạng thái (Available/Occupied/Under Maintenance hoặc để trống): ", s => string.IsNullOrWhiteSpace(s) || new[]{"Available","Occupied","Under Maintenance"}.Contains(s), "Sai trạng thái!");
            if (status == null) return;
            Console.SetCursorPosition(x + 2, y + 3);
            string? roomType = ReadInputWithEscape(x + 20, y + 3, "Loại phòng (Single/Double/Suite hoặc để trống): ", s => string.IsNullOrWhiteSpace(s) || new[]{"Single","Double","Suite"}.Contains(s), "Sai loại!");
            if (roomType == null) return;
            Console.SetCursorPosition(x + 2, y + 4);
            string? priceStr = ReadInputWithEscape(x + 20, y + 4, "Giá tối thiểu (hoặc để trống): ", s => string.IsNullOrWhiteSpace(s) || decimal.TryParse(s, out var v), "Sai giá!");
            decimal? minPrice = string.IsNullOrWhiteSpace(priceStr) ? null : decimal.Parse(priceStr);
            Console.SetCursorPosition(x + 2, y + 5);
            priceStr = ReadInputWithEscape(x + 20, y + 5, "Giá tối đa (hoặc để trống): ", s => string.IsNullOrWhiteSpace(s) || decimal.TryParse(s, out var v), "Sai giá!");
            decimal? maxPrice = string.IsNullOrWhiteSpace(priceStr) ? null : decimal.Parse(priceStr);
            try
            {
                var searchRooms = roomManager?.SearchRooms(status, roomType, minPrice, maxPrice);
                Console.Clear();
                DrawHeader("Kết Quả Tìm Kiếm Phòng");
                SetupBox(120, 30);
                if (searchRooms != null && searchRooms.Count > 0)
                {
                    string[] headers = { "RoomID", "RoomNumber", "RoomType", "Price", "Status", "Amenities" };
                    int[] columnWidths = { 8, 12, 10, 10, 18, 40 };
                    Console.SetCursorPosition(x + 2, y + 2);
                    for (int i = 0; i < headers.Length; i++)
                        Console.Write(headers[i].PadRight(columnWidths[i]));
                    Console.WriteLine();
                    Console.SetCursorPosition(x + 2, y + 3);
                    Console.WriteLine(new string('─', width - 4));
                    int rowIndex = 0;
                    foreach (var room in searchRooms)
                    {
                        Console.SetCursorPosition(x + 2, y + 4 + rowIndex);
                        Console.Write(room.RoomID.ToString().PadRight(columnWidths[0]));
                        Console.Write(room.RoomNumber.PadRight(columnWidths[1]));
                        Console.Write(room.RoomType.PadRight(columnWidths[2]));
                        Console.Write(room.Price.ToString("N0").PadRight(columnWidths[3]));
                        Console.Write(room.Status.PadRight(columnWidths[4]));
                        Console.Write(room.Amenities.PadRight(columnWidths[5]));
                        Console.WriteLine();
                        rowIndex++;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x + 2, y + 4);
                    Console.WriteLine("Không có phòng nào phù hợp!");
                }
                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }

        private void ShowCheckRoomAvailability()
        {
            Console.Clear();
            DrawHeader("Kiểm Tra Phòng Trống");
            SetupBox(80, 12);
            Console.SetCursorPosition(x + 2, y + 2);
            string? checkInStr = ReadInputWithEscape(x + 20, y + 2, "Ngày nhận (yyyy-MM-dd): ", s => DateTime.TryParse(s, out _), "Sai định dạng!");
            if (checkInStr == null) return;
            DateTime checkIn = DateTime.Parse(checkInStr);
            Console.SetCursorPosition(x + 2, y + 3);
            string? checkOutStr = ReadInputWithEscape(x + 20, y + 3, "Ngày trả (yyyy-MM-dd): ", s => DateTime.TryParse(s, out _), "Sai định dạng!");
            if (checkOutStr == null) return;
            DateTime checkOut = DateTime.Parse(checkOutStr);
            try
            {
                var availableRooms = roomManager?.CheckRoomAvailability(checkIn, checkOut);
                Console.Clear();
                DrawHeader("Phòng Trống Theo Thời Gian");
                SetupBox(120, 30);
                if (availableRooms != null && availableRooms.Count > 0)
                {
                    string[] headers = { "RoomID", "RoomNumber", "RoomType", "Price", "Status" };
                    int[] columnWidths = { 8, 12, 10, 10, 18 };
                    Console.SetCursorPosition(x + 2, y + 2);
                    for (int i = 0; i < headers.Length; i++)
                        Console.Write(headers[i].PadRight(columnWidths[i]));
                    Console.WriteLine();
                    Console.SetCursorPosition(x + 2, y + 3);
                    Console.WriteLine(new string('─', width - 4));
                    int rowIndex = 0;
                    foreach (var room in availableRooms)
                    {
                        Console.SetCursorPosition(x + 2, y + 4 + rowIndex);
                        Console.Write(room.RoomID.ToString().PadRight(columnWidths[0]));
                        Console.Write(room.RoomNumber.PadRight(columnWidths[1]));
                        Console.Write(room.RoomType.PadRight(columnWidths[2]));
                        Console.Write(room.Price.ToString("N0").PadRight(columnWidths[3]));
                        Console.Write(room.Status.PadRight(columnWidths[4]));
                        Console.WriteLine();
                        rowIndex++;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x + 2, y + 4);
                    Console.WriteLine("Không có phòng trống!");
                }
                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }

        private void ShowBookingManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Quản Lý Đặt Phòng");
                int optionCount = 6;
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

                string[] options = new[] {
                    "Xem danh sách đặt phòng",

                    "Thêm đặt phòng mới",
                    "Sửa thông tin đặt phòng",
                    "Hủy đặt phòng",
                    "Tìm kiếm đặt phòng theo trạng thái",
                    "Quay lại"
                };
                for (int i = 0; i < options.Length; i++)
                {
                    int optionY = y + 6 + i * 2;
                    Console.SetCursorPosition(x + 2, optionY);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{i + 1}. ");
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
                int choice = 0;
                int.TryParse(choiceInput, out choice);
                switch (choice)
                {
                    case 1:
                        ShowBookingList();
                        break;
                    case 2:
                        ShowAddBooking();
                        break;
                    case 3:
                        ShowEditBooking();
                        break;
                    case 4:
                        ShowCancelBooking();
                        break;
                    case 5:
                        ShowSearchBooking();
                        break;
                    case 6:
                        Console.Clear();
                        return;
                    default:
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowBookingList()
        {
            var bookings = bookingManager?.GetAllBookings();
            Console.Clear();
            DrawHeader("Danh Sách Đặt Phòng");
            SetupBox(100, 30);
            if (bookings != null && bookings.Count > 0)
            {
                string[] headers = { "BookingID", "CustomerID", "RoomID", "CheckInDate", "CheckOutDate", "Status" };
                int[] columnWidths = { 10, 12, 10, 15, 15, 12 };
                Console.SetCursorPosition(x + 2, y + 2);
                for (int i = 0; i < headers.Length; i++)
                    Console.Write(headers[i].PadRight(columnWidths[i]));
                Console.WriteLine();
                Console.SetCursorPosition(x + 2, y + 3);
                Console.WriteLine(new string('─', width - 4));
                int rowIndex = 0;
                foreach (var booking in bookings)
                {
                    Console.SetCursorPosition(x + 2, y + 4 + rowIndex);
                    Console.Write(booking.BookingID.ToString().PadRight(columnWidths[0]));
                    Console.Write(booking.CustomerID.ToString().PadRight(columnWidths[1]));
                    Console.Write(booking.RoomID.ToString().PadRight(columnWidths[2]));
                    Console.Write(booking.CheckInDate.ToString("yyyy-MM-dd").PadRight(columnWidths[3]));
                    Console.Write(booking.CheckOutDate.ToString("yyyy-MM-dd").PadRight(columnWidths[4]));
                    Console.Write(booking.Status.PadRight(columnWidths[5]));
                    Console.WriteLine();
                    rowIndex++;
                }
            }
            else
            {
                Console.SetCursorPosition(x + 2, y + 4);
                Console.WriteLine("Không có đặt phòng nào!");
            }
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void ShowAddBooking()
        {
            Console.Clear();
            DrawHeader("Thêm Đặt Phòng Mới");
            SetupBox(80, 14);
            Console.SetCursorPosition(x + 2, y + 2);
            string? customerIdStr = ReadInputWithEscape(x + 20, y + 2, "Mã khách hàng: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (customerIdStr == null) return;
            int customerId = int.Parse(customerIdStr);
            Console.SetCursorPosition(x + 2, y + 3);
            string? roomIdStr = ReadInputWithEscape(x + 20, y + 3, "RoomID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (roomIdStr == null) return;
            int roomId = int.Parse(roomIdStr);
            Console.SetCursorPosition(x + 2, y + 4);
            string? checkInStr = ReadInputWithEscape(x + 20, y + 4, "Ngày nhận (yyyy-MM-dd): ", s => DateTime.TryParse(s, out _), "Sai định dạng!");
            if (checkInStr == null) return;
            DateTime checkIn = DateTime.Parse(checkInStr);
            Console.SetCursorPosition(x + 2, y + 5);
            string? checkOutStr = ReadInputWithEscape(x + 20, y + 5, "Ngày trả (yyyy-MM-dd): ", s => DateTime.TryParse(s, out _), "Sai định dạng!");
            if (checkOutStr == null) return;
            DateTime checkOut = DateTime.Parse(checkOutStr);
            try
            {
                bookingManager?.AddBooking(customerId, roomId, checkIn, checkOut, currentUserId, currentUsername ?? string.Empty);
                ShowSuccessMessage("Thêm đặt phòng thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            Console.ReadKey();
        }

        private void ShowEditBooking()
        {
            Console.Clear();
            DrawHeader("Sửa Thông Tin Đặt Phòng");
            SetupBox(80, 14);
            Console.SetCursorPosition(x + 2, y + 2);
            string? bookingIdStr = ReadInputWithEscape(x + 20, y + 2, "BookingID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (bookingIdStr == null) return;
            int bookingId = int.Parse(bookingIdStr);
            Console.SetCursorPosition(x + 2, y + 3);
            string? roomIdStr = ReadInputWithEscape(x + 20, y + 3, "RoomID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (roomIdStr == null) return;
            int roomId = int.Parse(roomIdStr);
            Console.SetCursorPosition(x + 2, y + 4);
            string? checkInStr = ReadInputWithEscape(x + 20, y + 4, "Ngày nhận (yyyy-MM-dd): ", s => DateTime.TryParse(s, out _), "Sai định dạng!");
            if (checkInStr == null) return;
            DateTime checkIn = DateTime.Parse(checkInStr);
            Console.SetCursorPosition(x + 2, y + 5);
            string? checkOutStr = ReadInputWithEscape(x + 20, y + 5, "Ngày trả (yyyy-MM-dd): ", s => DateTime.TryParse(s, out _), "Sai định dạng!");
            if (checkOutStr == null) return;
            DateTime checkOut = DateTime.Parse(checkOutStr);
            Console.SetCursorPosition(x + 2, y + 6);
            string? status = ReadInputWithEscape(x + 20, y + 6, "Trạng thái (Confirmed/Canceled): ", s => new[]{"Confirmed","Canceled"}.Contains(s), "Sai trạng thái!");
            if (status == null) return;
            try
            {
                bookingManager?.UpdateBooking(bookingId, roomId, checkIn, checkOut, status, currentUserId, currentUsername ?? string.Empty);
                ShowSuccessMessage("Cập nhật thông tin đặt phòng thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            Console.ReadKey();
        }

        private void ShowCancelBooking()
        {
            Console.Clear();
            DrawHeader("Hủy Đặt Phòng");
            SetupBox(60, 10);
            Console.SetCursorPosition(x + 2, y + 2);
            string? bookingIdStr = ReadInputWithEscape(x + 20, y + 2, "BookingID: ", s => int.TryParse(s, out var v) && v > 0, "ID phải là số dương!");
            if (bookingIdStr == null) return;
            int bookingId = int.Parse(bookingIdStr);
            Console.SetCursorPosition(x + 2, y + 3);
            string? reason = ReadInputWithEscape(x + 20, y + 3, "Lý do hủy: ", s => !string.IsNullOrWhiteSpace(s), "Vui lòng nhập lý do!");
            if (reason == null) return;
            try
            {
                bookingManager?.CancelBooking(bookingId, reason, currentUserId, currentUsername ?? string.Empty);
                ShowSuccessMessage("Hủy đặt phòng thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            Console.ReadKey();
        }

        private void ShowSearchBooking()
        {
            Console.Clear();
            DrawHeader("Tìm Kiếm Đặt Phòng");
            SetupBox(80, 10);
            Console.SetCursorPosition(x + 2, y + 2);
            string? status = ReadInputWithEscape(x + 20, y + 2, "Trạng thái (Confirmed/Canceled hoặc để trống): ", s => string.IsNullOrWhiteSpace(s) || new[]{"Confirmed","Canceled"}.Contains(s), "Sai trạng thái!");
            if (status == null) return;
            try
            {
                var searchBookings = bookingManager?.SearchBookings(string.IsNullOrWhiteSpace(status) ? null : status);
                Console.Clear();
                DrawHeader("Kết Quả Tìm Kiếm Đặt Phòng");
                SetupBox(100, 30);
                if (searchBookings != null && searchBookings.Count > 0)
                {
                    string[] headers = { "BookingID", "CustomerID", "RoomID", "CheckInDate", "CheckOutDate", "Status" };
                    int[] columnWidths = { 10, 12, 10, 15, 15, 12 };
                    Console.SetCursorPosition(x + 2, y + 2);
                    for (int i = 0; i < headers.Length; i++)
                        Console.Write(headers[i].PadRight(columnWidths[i]));
                    Console.WriteLine();
                    Console.SetCursorPosition(x + 2, y + 3);
                    Console.WriteLine(new string('─', width - 4));
                    int rowIndex = 0;
                    foreach (var booking in searchBookings)
                    {
                        Console.SetCursorPosition(x + 2, y + 4 + rowIndex);
                        Console.Write(booking.BookingID.ToString().PadRight(columnWidths[0]));
                        Console.Write(booking.CustomerID.ToString().PadRight(columnWidths[1]));
                        Console.Write(booking.RoomID.ToString().PadRight(columnWidths[2]));
                        Console.Write(booking.CheckInDate.ToString("yyyy-MM-dd").PadRight(columnWidths[3]));
                        Console.Write(booking.CheckOutDate.ToString("yyyy-MM-dd").PadRight(columnWidths[4]));
                        Console.Write(booking.Status.PadRight(columnWidths[5]));
                        Console.WriteLine();
                        rowIndex++;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x + 2, y + 4);
                    Console.WriteLine("Không có đặt phòng nào phù hợp!");
                }
                Console.SetCursorPosition(x + 2, y + height - 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }

        private void ShowServiceManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Quản Lý Dịch Vụ");
                int optionCount = 7;
                SetupBox(80, 8 + optionCount * 2 + 4);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write($"Chào mừng, {currentUsername}! (Vai trò: {currentRole ?? "Chưa đăng nhập"})");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                string[] options = new[]
                {
                    "Xem danh sách dịch vụ",
                    "Thêm dịch vụ mới",
                    "Cập nhật dịch vụ",
                    "Xóa dịch vụ",
                    "Ghi nhận dịch vụ cho booking",
                    "Xem dịch vụ đã sử dụng theo booking",
                    "Quay lại menu chính"
                };
                for (int i = 0; i < options.Length; i++)
                {
                    int optionY = y + 6 + i * 2;
                    Console.SetCursorPosition(x + 2, optionY);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{i + 1}. ");
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
                string? choice = ReadInputWithEscape(x + 20, y + height - 4);
                if (!int.TryParse(choice, out int opt) || opt < 1 || opt > options.Length)
                {
                    ShowErrorMessage("Lựa chọn không hợp lệ!");
                    Console.ReadKey();
                    continue;
                }
                if (opt == options.Length) break;
                switch (opt)
                {
                    case 1:
                        ShowAllServices();
                        break;
                    case 2:
                        AddService();
                        break;
                    case 3:
                        UpdateService();
                        break;
                    case 4:
                        DeleteService();
                        break;
                    case 5:
                        AddServiceUsageToBooking();
                        break;
                    case 6:
                        ShowServiceUsageByBooking();
                        break;
                }
            }
        }

        private void ShowAllServices()
        {
            Console.Clear();
            DrawHeader("Danh Sách Dịch Vụ");
            var services = serviceManager?.GetAllServices() ?? new List<Service>();
            Console.SetCursorPosition(x + 2, y + 3);
            Console.WriteLine($"{"ID",4} {"Tên dịch vụ",-25} {"Loại",-10} {"Giá",10}");
            foreach (var s in services)
            {
                Console.SetCursorPosition(x + 2, Console.CursorTop);
                Console.WriteLine($"{s.ServiceID,4} {s.ServiceName,-25} {s.Type,-10} {s.Price,10:N0}");
            }
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void AddService()
        {
            Console.Clear();
            DrawHeader("Thêm Dịch Vụ Mới");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write("Tên dịch vụ: ");
            string? name = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Loại (Food/Laundry/Spa/Other): ");
            string? type = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Giá: ");
            string? priceStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) || !decimal.TryParse(priceStr, out decimal price))
            {
                ShowErrorMessage("Thông tin không hợp lệ!");
                Console.ReadKey();
                return;
            }
            var service = new Service { ServiceName = name, Type = type, Price = price, UpdatedBy = currentUserId, UpdatedByUsername = currentUsername };
            if (serviceManager?.AddService(service) == true)
                ShowSuccessMessage("Thêm dịch vụ thành công!");
            else
                ShowErrorMessage("Thêm dịch vụ thất bại!");
            Console.ReadKey();
        }

        private void UpdateService()
        {
            Console.Clear();
            DrawHeader("Cập Nhật Dịch Vụ");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write("Nhập ID dịch vụ cần sửa: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                ShowErrorMessage("ID không hợp lệ!");
                Console.ReadKey();
                return;
            }
            var service = serviceManager?.GetServiceById(id);
            if (service == null)
            {
                ShowErrorMessage("Không tìm thấy dịch vụ!");
                Console.ReadKey();
                return;
            }
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write($"Tên dịch vụ ({service.ServiceName}): ");
            string? name = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write($"Loại ({service.Type}): ");
            string? type = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write($"Giá ({service.Price}): ");
            string? priceStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) service.ServiceName = name;
            if (!string.IsNullOrWhiteSpace(type)) service.Type = type;
            if (decimal.TryParse(priceStr, out decimal price)) service.Price = price;
            service.UpdatedBy = currentUserId;
            service.UpdatedByUsername = currentUsername;
            if (serviceManager?.UpdateService(service) == true)
                ShowSuccessMessage("Cập nhật thành công!");
            else
                ShowErrorMessage("Cập nhật thất bại!");
            Console.ReadKey();
        }

        private void DeleteService()
        {
            Console.Clear();
            DrawHeader("Xóa Dịch Vụ");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write("Nhập ID dịch vụ cần xóa: ");
            string? idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                ShowErrorMessage("ID không hợp lệ!");
                Console.ReadKey();
                return;
            }
            if (serviceManager?.DeleteService(id) == true)
                ShowSuccessMessage("Xóa thành công!");
            else
                ShowErrorMessage("Xóa thất bại!");
            Console.ReadKey();
        }

        private void AddServiceUsageToBooking()
        {
            Console.Clear();
            DrawHeader("Ghi Nhận Dịch Vụ Cho Booking");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write("BookingID: ");
            string? bookingIdStr = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ServiceID: ");
            string? serviceIdStr = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("CustomerID: ");
            string? customerIdStr = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Số lượng: ");
            string? quantityStr = Console.ReadLine();
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Ngày sử dụng (yyyy-MM-dd): ");
            string? dateStr = Console.ReadLine();
            if (!int.TryParse(bookingIdStr, out int bookingId) || !int.TryParse(serviceIdStr, out int serviceId) || !int.TryParse(customerIdStr, out int customerId) || !int.TryParse(quantityStr, out int quantity) || !DateTime.TryParse(dateStr, out DateTime date))
            {
                ShowErrorMessage("Thông tin không hợp lệ!");
                Console.ReadKey();
                return;
            }
            if (invoiceManager.AddServiceUsage(bookingId, serviceId, customerId, quantity, date, currentUserId, currentUsername ?? "") == true)
                ShowSuccessMessage("Ghi nhận dịch vụ thành công!");
            else
                ShowErrorMessage("Ghi nhận thất bại!");
            Console.ReadKey();
        }

        private void ShowServiceUsageByBooking()
        {
            Console.Clear();
            DrawHeader("Dịch Vụ Đã Sử Dụng Theo Booking");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write("Nhập BookingID: ");
            string? bookingIdStr = Console.ReadLine();
            if (!int.TryParse(bookingIdStr, out int bookingId))
            {
                ShowErrorMessage("ID không hợp lệ!");
                Console.ReadKey();
                return;
            }
            var usages = invoiceManager.GetServiceUsagesByBooking(bookingId);
            Console.SetCursorPosition(x + 2, y + 5);
            Console.WriteLine($"{"Tên dịch vụ",-25} {"Loại",-10} {"Số lượng",8} {"Tổng tiền",12} {"Trạng thái",10}");
            foreach (var u in usages)
            {
                Console.SetCursorPosition(x + 2, Console.CursorTop);
                Console.WriteLine($"{u.ServiceName,-25} {u.Type,-10} {u.Quantity,8} {u.TotalPrice,12:N0} {u.PaymentStatus,10}");
            }
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        public void ShowSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message.PadRight(width - 4));
            Console.ResetColor();
        }

        public void ShowErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message.PadRight(width - 4));
            Console.ResetColor();
        }

        public void ShowInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message.PadRight(width - 4));
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

                    if (choice == "1")
                    {
                        var (username, password) = ShowLoginScreen();
                        if (username == null || password == null)
                            continue;

                        if (Login(username, password))
                        {
                            ShowSuccessMessage("Đăng nhập thành công!");
                            Thread.Sleep(1000);
                            Console.Clear();
                            customerManager = new CustomerBL(currentRole ?? string.Empty, currentUserId, currentUsername ?? string.Empty);
                            staffManager = new StaffBL(currentRole ?? string.Empty, currentUserId, currentUsername ?? string.Empty);
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
