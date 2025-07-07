using HotelManagementSystem;
using System;

namespace HotelManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Khởi tạo logging system
                SimpleLogger.Initialize();
                SimpleLogger.LogSystem("Application started");
                
                MainConsoleUI ui = new MainConsoleUI();
                ui.Run();
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("System", "Application crashed", ex);
                Console.WriteLine($"Lỗi nghiêm trọng: {ex.Message}");
                Console.WriteLine("Nhấn phím bất kỳ để thoát...");
                Console.ReadKey();
            }
            finally
            {
                SimpleLogger.LogSystem("Application ended");
            }
        }
    }
}