using System;
using System.Runtime.InteropServices;
using System.Threading;
using HotelManagementSystem.ConsoleUI;

namespace HotelManagementSystem
{
    public class MainConsoleUI : BaseUI
    {
        private UserManagementUI? _userManagementUI;
        private RoomManagementUI? _roomManagementUI;
        private BookingManagementUI? _bookingManagementUI;
        private ServiceManagementUI? _serviceManagementUI;
        private CustomerManagementUI? _customerManagementUI;
        private InvoiceManagementUI? _invoiceManagementUI;
        private StaffManagementUI? _staffManagementUI;
        private ReportAndAnalyticsUI? _reportAndAnalyticsUI;

        public MainConsoleUI() : base(null, null, null)
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

        public void ShowWelcomeScreen()
        {
            Console.Clear();
            DrawHeader("🏨 Chào Mừng Đến Với Hệ Thống Quản Lý Khách Sạn VTC Academy");
            SetupBox(90, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('═', width - 4));

            // ASCII Art cho logo
            Console.SetCursorPosition(x + 15, y + 5);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.SetCursorPosition(x + 15, y + 6);
            Console.WriteLine("║          🏨 HOTEL MANAGEMENT SYSTEM 🏨              ║");
            Console.SetCursorPosition(x + 15, y + 7);
            Console.WriteLine("║                                                      ║");
            Console.SetCursorPosition(x + 15, y + 8);
            Console.WriteLine("║     Phiên bản: 2.0 - Modular Architecture           ║");
            Console.SetCursorPosition(x + 15, y + 9);
            Console.WriteLine("║     Phát triển bởi: VTC Academy Team                ║");
            Console.SetCursorPosition(x + 15, y + 10);
            Console.WriteLine("║     Hệ thống quản lý khách sạn hiện đại             ║");
            Console.SetCursorPosition(x + 15, y + 11);
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            
            Console.ResetColor();
            Console.SetCursorPosition(x + 25, y + 15);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("🌟 Chào mừng bạn đến với hệ thống! 🌟");
            
            Console.SetCursorPosition(x + 30, y + 17);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Nhấn phím bất kỳ để tiếp tục...");
            Console.ResetColor();
            Console.ReadKey();
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
            SetupBox(80, 18);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên đăng nhập: ");
            string? username = ReadInputWithEscape(x + 17, y + 4);
            if (username == null)
                return (null, null);

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Mật khẩu: ");
            Console.SetCursorPosition(x + 13, y + 6);
            string password = ReadPassword();
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            return (username, password);
        }

        public (string?, string?, string?, string?, string?, string?) ShowRegisterScreen()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
            SetupBox(90, 24);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Tên đăng nhập mới: ");
            string? newUsername = ReadInputWithEscape(x + 21, y + 4);
            if (newUsername == null)
                return (null, null, null, null, null, null);

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Mật khẩu mới: ");
            Console.SetCursorPosition(x + 17, y + 6);
            string newPassword = ReadPassword();
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Xác nhận mật khẩu: ");
            Console.SetCursorPosition(x + 22, y + 8);
            string confirmPassword = ReadPassword();
            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Vai trò (Admin/Receptionist/Housekeeping): ");
            string? role = ReadInputWithEscape(x + 44, y + 10);
            if (role == null)
                return (null, null, null, null, null, null);

            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 12);
            Console.Write("Email: ");
            string? email = ReadInputWithEscape(x + 9, y + 12);
            if (email == null)
                return (null, null, null, null, null, null);

            Console.SetCursorPosition(x + 2, y + 13);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 14);
            Console.Write("Họ tên đầy đủ: ");
            string? fullName = ReadInputWithEscape(x + 16, y + 14);
            if (fullName == null)
                return (null, null, null, null, null, null);

            return (newUsername, newPassword, confirmPassword, role, email ?? "", fullName);
        }

        private void InitializeUserManagementUI()
        {
            if (_userManagementUI == null)
            {
                _userManagementUI = new UserManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeRoomManagementUI()
        {
            if (_roomManagementUI == null)
            {
                _roomManagementUI = new RoomManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeBookingManagementUI()
        {
            if (_bookingManagementUI == null)
            {
                _bookingManagementUI = new BookingManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeServiceManagementUI()
        {
            if (_serviceManagementUI == null)
            {
                _serviceManagementUI = new ServiceManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeCustomerManagementUI()
        {
            if (_customerManagementUI == null)
            {
                _customerManagementUI = new CustomerManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeInvoiceManagementUI()
        {
            if (_invoiceManagementUI == null)
            {
                _invoiceManagementUI = new InvoiceManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeStaffManagementUI()
        {
            if (_staffManagementUI == null)
            {
                _staffManagementUI = new StaffManagementUI(currentUsername, currentRole, currentUserId);
            }
        }

        private void InitializeReportAndAnalyticsUI()
        {
            if (_reportAndAnalyticsUI == null)
            {
                _reportAndAnalyticsUI = new ReportAndAnalyticsUI(currentUsername, currentRole, currentUserId);
            }
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
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                SetupBox(80, 18);
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));
                
                WriteTextInBox("Lựa chọn không hợp lệ! Vui lòng thử lại.", y + 5, ConsoleColor.Red);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
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
                    // Ghi log đăng xuất
                    SimpleLogger.LogAuth(currentUsername ?? "Unknown", "Logout");
                    
                    currentUsername = null;
                    currentRole = null;
                    currentUserId = null;
                    _userManagementUI = null;
                    _roomManagementUI = null;
                    _bookingManagementUI = null;
                    _serviceManagementUI = null;
                    _customerManagementUI = null;
                    _invoiceManagementUI = null;
                    _staffManagementUI = null;
                    _reportAndAnalyticsUI = null;
                    ShowSuccessMessage("Đăng xuất thành công!");
                    Thread.Sleep(1000);
                    Console.Clear();
                    break;
                case "Quản lý người dùng":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access User Management");
                    InitializeUserManagementUI();
                    _userManagementUI?.ShowUserManagement();
                    break;
                case "Quản lý phòng":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Room Management");
                    InitializeRoomManagementUI();
                    _roomManagementUI?.ShowRoomManagement();
                    break;
                case "Quản lý đặt phòng":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Booking Management");
                    InitializeBookingManagementUI();
                    _bookingManagementUI?.ShowBookingManagement();
                    break;
                case "Quản lý dịch vụ":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Service Management");
                    InitializeServiceManagementUI();
                    _serviceManagementUI?.ShowServiceManagement();
                    break;
                case "Quản lý khách hàng":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Customer Management");
                    InitializeCustomerManagementUI();
                    _customerManagementUI?.ShowCustomerManagement();
                    break;
                case "Quản lý hóa đơn":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Invoice Management");
                    InitializeInvoiceManagementUI();
                    _invoiceManagementUI?.ShowInvoiceManagement();
                    break;
                case "Quản lý nhân viên":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Staff Management");
                    InitializeStaffManagementUI();
                    _staffManagementUI?.ShowStaffManagement();
                    break;
                case "Xem báo cáo":
                    SimpleLogger.LogActivity(currentUsername ?? "Unknown", "Access Reports");
                    InitializeReportAndAnalyticsUI();
                    _reportAndAnalyticsUI?.ShowReportAndAnalytics();
                    break;
                default:
                    ShowInfoMessage($"Đã chọn: {option}. (Chưa được triển khai - Nhấn phím bất kỳ để quay lại...)");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
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

        public void Run()
        {
            // Hiển thị màn hình chào mừng trước
            ShowWelcomeScreen();
            
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

                        try
                        {
                            InitializeUserManagementUI();
                            if (_userManagementUI!.Login(username, password))
                            {
                                // Update current user info from successful login
                                currentUsername = username;
                                var (userId, role) = _userManagementUI.GetCurrentUserInfo();
                                currentUserId = userId;
                                currentRole = role;
                                
                                // Ghi log đăng nhập thành công
                                SimpleLogger.LogAuth(username, "Login successful");
                                
                                ShowSuccessMessage("Đăng nhập thành công!");
                                Thread.Sleep(1000);
                                Console.Clear();
                            }
                            else
                            {
                                // Ghi log đăng nhập thất bại
                                SimpleLogger.LogAuth(username, "Login failed - Invalid credentials");
                                
                                Console.Clear();
                                DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                                SetupBox(80, 18);
                                
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.SetCursorPosition(x + 2, y + 2);
                                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                                Console.SetCursorPosition(x + 2, y + 3);
                                Console.Write(new string('─', width - 4));
                                
                                WriteTextInBox("Tên đăng nhập hoặc mật khẩu không đúng!", y + 5, ConsoleColor.Red);
                                
                                Console.SetCursorPosition(x + 2, y + height - 3);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("Nhấn phím bất kỳ để quay lại...");
                                Console.ResetColor();
                                Console.ReadKey();
                                Console.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ghi log lỗi đăng nhập
                            SimpleLogger.LogError(username, "Login error", ex);
                            
                            Console.Clear();
                            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                            SetupBox(80, 18);
                            
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(x + 2, y + 2);
                            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                            Console.SetCursorPosition(x + 2, y + 3);
                            Console.Write(new string('─', width - 4));
                            
                            WriteTextInBox($"Lỗi đăng nhập: {ex.Message}", y + 5, ConsoleColor.Red);
                            
                            Console.SetCursorPosition(x + 2, y + height - 3);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Nhấn phím bất kỳ để quay lại...");
                            Console.ResetColor();
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else if (choice == "2")
                    {
                        var registerResult = ShowRegisterScreen();
                        var newUsername = registerResult.Item1;
                        var newPassword = registerResult.Item2;
                        var confirmPassword = registerResult.Item3;
                        var role = registerResult.Item4;
                        var email = registerResult.Item5;
                        var fullName = registerResult.Item6;
                        
                        if (newUsername == null || newPassword == null || confirmPassword == null || role == null || fullName == null)
                            continue;

                        if (newPassword != confirmPassword)
                        {
                            Console.Clear();
                            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                            SetupBox(80, 18);
                            
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(x + 2, y + 2);
                            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                            Console.SetCursorPosition(x + 2, y + 3);
                            Console.Write(new string('─', width - 4));
                            
                            WriteTextInBox("Mật khẩu xác nhận không khớp!", y + 5, ConsoleColor.Red);
                            
                            Console.SetCursorPosition(x + 2, y + height - 3);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Nhấn phím bất kỳ để quay lại...");
                            Console.ResetColor();
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }

                        try
                        {
                            InitializeUserManagementUI();
                            if (_userManagementUI!.Register(newUsername, newPassword, role, email ?? "", fullName))
                            {
                                ShowSuccessMessage("Đăng ký thành công! Nhấn phím bất kỳ để quay lại...");
                                Console.ReadKey();
                                Console.Clear();
                            }
                            else
                            {
                                Console.Clear();
                                DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                                SetupBox(80, 18);
                                
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.SetCursorPosition(x + 2, y + 2);
                                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                                Console.SetCursorPosition(x + 2, y + 3);
                                Console.Write(new string('─', width - 4));
                                
                                WriteTextInBox("Đăng ký thất bại!", y + 5, ConsoleColor.Red);
                                
                                Console.SetCursorPosition(x + 2, y + height - 3);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("Nhấn phím bất kỳ để quay lại...");
                                Console.ResetColor();
                                Console.ReadKey();
                                Console.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Clear();
                            DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                            SetupBox(80, 18);
                            
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(x + 2, y + 2);
                            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                            Console.SetCursorPosition(x + 2, y + 3);
                            Console.Write(new string('─', width - 4));
                            
                            WriteTextInBox($"Lỗi đăng ký: {ex.Message}", y + 5, ConsoleColor.Red);
                            
                            Console.SetCursorPosition(x + 2, y + height - 3);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Nhấn phím bất kỳ để quay lại...");
                            Console.ResetColor();
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        DrawHeader("Hệ Thống Quản Lý Khách Sạn");
                        SetupBox(80, 18);
                        
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(x + 2, y + 2);
                        Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
                        Console.SetCursorPosition(x + 2, y + 3);
                        Console.Write(new string('─', width - 4));
                        
                        WriteTextInBox("Lựa chọn không hợp lệ!", y + 5, ConsoleColor.Red);
                        
                        Console.SetCursorPosition(x + 2, y + height - 3);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Nhấn phím bất kỳ để quay lại...");
                        Console.ResetColor();
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
