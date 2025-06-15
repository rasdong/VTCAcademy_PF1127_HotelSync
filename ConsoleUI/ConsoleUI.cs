using System;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Collections.Generic;

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
                string query = "SELECT u.Username, r.RoleName FROM Users u JOIN Roles r ON u.RoleID = r.RoleID WHERE u.Username = @username AND u.Password = @password";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password); // Nên mã hóa mật khẩu trong thực tế
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
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
                    insertCommand.Parameters.AddWithValue("@password", password); // Nên mã hóa mật khẩu
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
                "Admin" => new[] { "Quản lý phòng", "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Quản lý nhân viên", "Quản lý người dùng", "Xem báo cáo", "Đăng xuất" },
                "Receptionist" => new[] { "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Xem báo cáo", "Đăng xuất" },
                "Housekeeping" => new[] { "Quản lý phòng", "Quản lý dịch vụ", "Xem báo cáo", "Đăng xuất" },
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
            string? choiceInput = Console.ReadLine();
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
                    ShowSuccessMessage("Đăng xuất thành công!");
                    Thread.Sleep(1000);
                    Console.Clear();
                    break;
                case "Quản lý hóa đơn":
                    ShowInvoiceManagement();
                    break;
                case "Xem báo cáo":
                    ShowReportScreen();
                    break;
                case "Quản lý phòng":
                case "Quản lý khách hàng":
                case "Quản lý đặt phòng":
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

            string[,] orders;
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT su.UsageID, c.Name, c.Phone, su.Date, su.TotalPrice, su.PaymentStatus
                    FROM ServiceUsage su
                    JOIN Customers c ON su.CustomerID = c.CustomerID
                    ORDER BY su.UsageID";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var tempOrders = new List<string[]>();
                        while (reader.Read())
                        {
                            tempOrders.Add(new[]
                            {
                                reader.GetInt32("UsageID").ToString(),
                                reader.GetString("Name"),
                                reader.GetString("Phone"),
                                reader.GetDateTime("Date").ToString("yyyy-MM-dd HH:mm"),
                                reader.GetDecimal("TotalPrice").ToString("N0"),
                                reader.GetString("PaymentStatus") == "Paid" ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN"
                            });
                        }
                        orders = new string[tempOrders.Count, 6];
                        for (int i = 0; i < tempOrders.Count; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                orders[i, j] = tempOrders[i][j];
                            }
                        }
                    }
                }
            }

            string[] headers = new[] { "Mã HĐ", "Tên Khách Hàng", "Điện Thoại", "Ngày", "Tổng (VND)", "Trạng Thái" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length;
                for (int row = 0; row < orders.GetLength(0); row++)
                {
                    int length = orders[row, col].Length;
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

            for (int i = 0; i < orders.GetLength(0); i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                for (int col = 0; col < orders.GetLength(1); col++)
                {
                    Console.Write(orders[i, col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
                if (i < orders.GetLength(0) - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (orders.GetLength(0) - 1) * 2);
            Console.WriteLine("+ Nhấn ESC để quay lại");
            Console.SetCursorPosition(x + 2, y + 13 + (orders.GetLength(0) - 1) * 2);
            Console.Write("+ Nhập mã hóa đơn để thanh toán: ");

            string orderId = ReadInputWithEscape(x + 34, y + 13 + (orders.GetLength(0) - 1) * 2) ?? string.Empty;
            if (string.IsNullOrEmpty(orderId))
            {
                Console.Clear();
                return;
            }

            if (!int.TryParse(orderId, out int id))
            {
                ShowErrorMessage($"Mã hóa đơn {orderId} không hợp lệ!");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            bool isValidOrder = false;
            bool isUnpaid = false;
            using (var connection = DataHelper.Instance.GetConnection())
            {
                connection.Open();
                string checkQuery = "SELECT PaymentStatus FROM ServiceUsage WHERE UsageID = @usageId";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@usageId", id);
                    var result = checkCommand.ExecuteScalar();
                    if (result != null)
                    {
                        isValidOrder = true;
                        isUnpaid = result.ToString() == "Unpaid";
                    }
                }

                if (isValidOrder && isUnpaid)
                {
                    string updateQuery = "UPDATE ServiceUsage SET PaymentStatus = 'Paid' WHERE UsageID = @usageId";
                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@usageId", id);
                        updateCommand.ExecuteNonQuery();
                    }
                    ShowSuccessMessage($"Thanh toán hóa đơn {orderId} thành công!");
                    Thread.Sleep(1000);
                    Console.Clear();
                }
                else
                {
                    ShowErrorMessage($"Mã hóa đơn {orderId} không hợp lệ hoặc đã thanh toán!");
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
                        (SELECT COUNT(*) FROM ServiceUsage WHERE PaymentStatus = 'Paid') AS PaidInvoices,
                        (SELECT COUNT(*) FROM ServiceUsage WHERE PaymentStatus = 'Unpaid') AS UnpaidInvoices,
                        (SELECT COALESCE(SUM(TotalPrice), 0) FROM ServiceUsage WHERE PaymentStatus = 'Paid') AS Revenue,
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