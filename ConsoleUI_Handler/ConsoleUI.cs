using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace HotelManagementSystem
{
    public class ConsoleUI : IUserInterface
    {
        private int width;
        private int height;
        private int x;
        private int y;
        private readonly IReportDataProvider reportDataProvider;
        private readonly DatabaseHelper dbHelper;

        public ConsoleUI(IReportDataProvider? reportDataProvider = null)
        {
            this.reportDataProvider = reportDataProvider ?? new StaticReportDataProvider();
            dbHelper = new DatabaseHelper();
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
            string? role = ReadInputWithEscape(x + 40, y + 10);
            if (role == null)
                return (null, null, null, null);

            return (newUsername, newPassword, confirmPassword, role);
        }

        public void ShowMainMenu(User currentUser, Action<string> onOptionSelected)
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(70, 8 + (currentUser.Role == "Admin" ? 9 : currentUser.Role == "Receptionist" ? 6 : 4) * 2 + 4);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write($"Chào mừng, {currentUser.Username}!");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            string[] options = currentUser.Role switch
            {
                "Admin" => new[] { "Quản lý phòng", "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Quản lý nhân viên", "Quản lý người dùng", "Xem báo cáo", "Đăng xuất" },
                "Receptionist" => new[] { "Quản lý khách hàng", "Quản lý đặt phòng", "Quản lý hóa đơn", "Quản lý dịch vụ", "Đăng xuất" },
                "Housekeeping" => new[] { "Quản lý phòng", "Quản lý dịch vụ", "Đăng xuất" },
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
                onOptionSelected(options[choice - 1]);
            }
            else if (!string.IsNullOrEmpty(choiceInput))
            {
                ShowErrorMessage("Lựa chọn không hợp lệ! Nhấn phím bất kỳ để thử lại...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        public void ShowInvoiceManagement()
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
            Console.Write("--- DANH SÁCH ĐƠN HÀNG ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            string[,] orders;
            using (MySqlConnection conn = dbHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT su.UsageID, c.Name, c.Phone, su.Date, su.TotalPrice, su.PaymentStatus 
                                   FROM ServiceUsage su 
                                   JOIN Customers c ON su.CustomerID = c.CustomerID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            var orderList = new List<string[]>();
                            while (reader.Read())
                            {
                                orderList.Add(new[]
                                {
                                    reader.GetInt32("UsageID").ToString(),
                                    reader.GetString("Name"),
                                    reader.GetString("Phone"),
                                    reader.GetDateTime("Date").ToString("yyyy-MM-dd HH:mm"),
                                    string.Format("{0:N0}", reader.GetDecimal("TotalPrice")),
                                    reader.GetString("PaymentStatus") == "Paid" ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN"
                                });
                            }
                            orders = new string[orderList.Count, 6];
                            for (int i = 0; i < orderList.Count; i++)
                            {
                                for (int j = 0; j < 6; j++)
                                {
                                    orders[i, j] = orderList[i][j];
                                }
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    ShowErrorMessage("Lỗi cơ sở dữ liệu: " + ex.Message);
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
            }

            string[] headers = new[] { "Mã ĐH", "Tên Khách Hàng", "Điện Thoại", "Ngày Đặt", "Tổng (VND)", "Trạng Thái" };
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
            Console.Write("+ Nhập mã đơn hàng để thanh toán: ");

            string orderId = ReadInputWithEscape(x + 34, y + 13 + (orders.GetLength(0) - 1) * 2) ?? string.Empty;
            if (string.IsNullOrEmpty(orderId))
            {
                Console.Clear();
                return;
            }

            if (!int.TryParse(orderId, out int id))
            {
                ShowErrorMessage($"Mã đơn hàng {orderId} không hợp lệ!");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            bool isValidOrder = false;
            bool isUnpaid = false;
            using (MySqlConnection conn = dbHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT PaymentStatus FROM ServiceUsage WHERE UsageID = @UsageID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsageID", id);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            isValidOrder = true;
                            isUnpaid = result.ToString() == "Unpaid";
                        }
                    }

                    if (isValidOrder && isUnpaid)
                    {
                        string updateQuery = "UPDATE ServiceUsage SET PaymentStatus = 'Paid', UpdatedAt = CURRENT_TIMESTAMP WHERE UsageID = @UsageID";
                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@UsageID", id);
                            cmd.ExecuteNonQuery();
                            ShowSuccessMessage($"Thanh toán đơn hàng {orderId} thành công!");
                            Thread.Sleep(1000);
                            Console.Clear();
                        }
                    }
                    else
                    {
                        ShowErrorMessage($"Mã đơn hàng {orderId} không hợp lệ hoặc đã thanh toán!");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                catch (MySqlException ex)
                {
                    ShowErrorMessage("Lỗi cơ sở dữ liệu: " + ex.Message);
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        public void ShowReportScreen()
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

            string[,] reportData = reportDataProvider.GetReportData();

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
    }
}