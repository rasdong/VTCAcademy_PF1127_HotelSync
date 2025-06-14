using System;

namespace HotelManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserManager userManager = new UserManager();
            IReportDataProvider reportDataProvider = new StaticReportDataProvider(); 
            IUserInterface ui = new ConsoleUI(reportDataProvider);
            User? currentUser = null;

            while (true)
            {
                if (currentUser == null)
                {
                    string? choice = ui.ShowInitialScreen();
                    if (choice == null)
                    {
                        Console.Clear();
                        Environment.Exit(0);
                    }

                    if (choice == "1")
                    {
                        var (username, password) = ui.ShowLoginScreen();
                        if (username == null || password == null)
                            continue;

                        currentUser = userManager.Login(username, password);
                        if (currentUser != null)
                        {
                            ui.ShowSuccessMessage("Đăng nhập thành công!");
                            System.Threading.Thread.Sleep(1000);
                            Console.Clear();
                        }
                        else
                        {
                            ui.ShowErrorMessage("Tên đăng nhập hoặc mật khẩu không đúng!");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else if (choice == "2")
                    {
                        var (newUsername, newPassword, confirmPassword, role) = ui.ShowRegisterScreen();
                        if (newUsername == null || newPassword == null || confirmPassword == null || role == null)
                            continue;

                        if (newPassword != confirmPassword)
                        {
                            ui.ShowErrorMessage("Mật khẩu xác nhận không khớp!");
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }

                        bool success = userManager.Register(newUsername, newPassword, role);
                        if (success)
                        {
                            ui.ShowSuccessMessage("Đăng ký thành công! Nhấn phím bất kỳ để quay lại...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            ui.ShowErrorMessage("Tên đăng nhập đã tồn tại hoặc vai trò không hợp lệ!");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else
                    {
                        ui.ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else
                {
                    ui.ShowMainMenu(currentUser, option =>
                    {
                        if (option == "Đăng xuất")
                        {
                            currentUser = null;
                            ui.ShowSuccessMessage("Đăng xuất thành công!");
                            System.Threading.Thread.Sleep(1000);
                            Console.Clear();
                        }
                        else if (option == "Quản lý hóa đơn")
                        {
                            ui.ShowInvoiceManagement();
                            Console.Clear();
                        }
                        else if (option == "Xem báo cáo")
                        {
                            ui.ShowReportScreen();
                        }
                        else
                        {
                            ui.ShowInfoMessage($"Đã chọn: {option}. (Chỉ là placeholder - Nhấn phím bất kỳ để quay lại...)");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    });
                }
            }
        }
    }
}