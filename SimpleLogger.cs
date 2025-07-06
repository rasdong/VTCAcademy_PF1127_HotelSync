using System;
using System.IO;
using System.Text;

namespace HotelManagementSystem
{
    public static class SimpleLogger
    {
        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Ghi log hoạt động của user
        /// </summary>
        public static void LogActivity(string username, string action, string details = "")
        {
            WriteLog("ACTIVITY", username, action, details);
        }

        /// <summary>
        /// Ghi log lỗi
        /// </summary>
        public static void LogError(string username, string error, Exception ex = null)
        {
            string details = ex != null ? $"Exception: {ex.Message}" : "";
            WriteLog("ERROR", username, error, details);
        }

        /// <summary>
        /// Ghi log đăng nhập/đăng xuất
        /// </summary>
        public static void LogAuth(string username, string action)
        {
            WriteLog("AUTH", username, action, "");
        }

        /// <summary>
        /// Ghi log thông tin hệ thống
        /// </summary>
        public static void LogSystem(string message)
        {
            WriteLog("SYSTEM", "System", message, "");
        }

        /// <summary>
        /// Ghi log vào file
        /// </summary>
        private static void WriteLog(string type, string username, string action, string details)
        {
            try
            {
                lock (_lockObject)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    StringBuilder logEntry = new StringBuilder();
                    
                    logEntry.Append($"[{timestamp}] [{type}] ");
                    logEntry.Append($"User: {username ?? "Unknown"} | ");
                    logEntry.Append($"Action: {action}");
                    
                    if (!string.IsNullOrEmpty(details))
                    {
                        logEntry.Append($" | Details: {details}");
                    }
                    
                    logEntry.AppendLine();

                    File.AppendAllText(_logFilePath, logEntry.ToString(), Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                // Nếu không thể ghi log, in ra console
                Console.WriteLine($"Logger Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Khởi tạo file log
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    string header = $"=== HOTEL MANAGEMENT SYSTEM LOG ===\n" +
                                   $"Log started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                   $"{'='}" +
                                   $"\n\n";
                    File.WriteAllText(_logFilePath, header, Encoding.UTF8);
                }
                else
                {
                    // Thêm dấu phân cách cho session mới
                    string sessionStart = $"\n--- NEW SESSION STARTED: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\n";
                    File.AppendAllText(_logFilePath, sessionStart, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot initialize log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy đường dẫn file log
        /// </summary>
        public static string GetLogFilePath()
        {
            return _logFilePath;
        }
    }
}
