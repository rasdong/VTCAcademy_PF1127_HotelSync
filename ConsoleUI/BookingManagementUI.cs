
using System;
using System.Data;
using HotelManagementSystem.InvoiceManagement; // Thêm namespace để gọi InvoiceBL

namespace HotelManagementSystem
{
    public class BookingManagementUI : BaseUI
    {
        private readonly BookingBLL _bookingBLL = new BookingBLL();
        private readonly InvoiceBL _invoiceBL = new InvoiceBL(); // Thêm InvoiceBL
        private int? lastBookingId;

        public BookingManagementUI(string? username, string? role, int? userId) 
            : base(username, role, userId)
        {
        }

        public void ShowBookingManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Đặt Phòng");
                SetupBox(80, 20);

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
                    ShowErrorMessage(CleanErrorMessage(ex.Message)); // Sử dụng hàm CleanErrorMessage
                    Console.ReadKey();
                }
            }
        }

        private string CleanErrorMessage(string message)
        {
            string cleaned = message;
            if (cleaned.Contains("Lỗi trong logic nghiệp vụ khi"))
                cleaned = cleaned.Replace("Lỗi trong logic nghiệp vụ khi ", "");
            if (cleaned.Contains("Lỗi khi "))
                cleaned = cleaned.Replace("Lỗi khi ", "");
            // Giữ nguyên thông báo từ stored procedure nếu nó không khớp với các tiền tố trên
            return string.IsNullOrEmpty(cleaned) ? message : cleaned;
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

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Ngày Check-out (yyyy-MM-dd HH:mm): ");
            string? checkOutDateInput = ReadInputWithEscape(x + 35, y + 10);
            if (checkOutDateInput == null) return;

            try
            {
                int bookingId = _bookingBLL.CreateBooking(IDCardInput, roomIdInput, checkInDateInput, 
                    checkOutDateInput, currentUserId ?? 0, currentUsername ?? "");
                lastBookingId = bookingId;
                ShowSuccessMessage($"Tạo đặt phòng thành công! BookingID: {bookingId}. Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CleanErrorMessage(ex.Message));
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
            Console.Write($"ID đặt phòng { (lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "") }: ");
            string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
            if (bookingIdInput == null) return;

            try
            {
                if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
                    bookingIdInput = lastBookingId.ToString();
                if (!int.TryParse(bookingIdInput, out int bookingId) || bookingId <= 0)
                    throw new ArgumentException("ID đặt phòng phải là số nguyên dương.");
                _bookingBLL.CancelBooking(bookingIdInput, currentUserId ?? 0, currentUsername ?? "");
                if (lastBookingId == bookingId) lastBookingId = null;
                ShowSuccessMessage("Hủy đặt phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CleanErrorMessage(ex.Message));
                Console.ReadKey();
            }
        }

        private void ShowCheckIn()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Check-in");
            SetupBox(60, 12);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write($"ID đặt phòng {(lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "")}");
            string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
            if (bookingIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Số CMND/CCCD: ");
            string? IDCardInput = ReadInputWithEscape(x + 16, y + 6);
            if (IDCardInput == null) return;

            try
            {
                if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
                    bookingIdInput = lastBookingId.ToString();
                _bookingBLL.CheckIn(bookingIdInput, IDCardInput, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Check-in thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CleanErrorMessage(ex.Message));
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
            Console.Write($"ID đặt phòng { (lastBookingId.HasValue ? $"(gần nhất: {lastBookingId})" : "") }: ");
            string? bookingIdInput = ReadInputWithEscape(x + 16 + (lastBookingId.HasValue ? 12 + lastBookingId.Value.ToString().Length : 0), y + 4);
            if (bookingIdInput == null) return;

            try
            {
                if (string.IsNullOrEmpty(bookingIdInput) && lastBookingId.HasValue)
                    bookingIdInput = lastBookingId.ToString();
                _bookingBLL.CheckOut(bookingIdInput ?? "", currentUserId ?? 0, currentUsername ?? "");
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
                ShowErrorMessage(CleanErrorMessage(ex.Message));
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
                    columnWidths[col] = headers[col].Length + 2;
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
        
                for (int i = 0; i < bookingHistory.Rows.Count; i++)
                {
                    Console.SetCursorPosition(x + 2, y + 11 + i);
                    Console.Write((bookingHistory.Rows[i]["BookingID"]?.ToString() ?? "").PadRight(columnWidths[0]));
                    Console.Write((bookingHistory.Rows[i]["CustomerID"]?.ToString() ?? "").PadRight(columnWidths[1]));
                    Console.Write((bookingHistory.Rows[i]["RoomID"]?.ToString() ?? "").PadRight(columnWidths[2]));
                    Console.Write(((DateTime)bookingHistory.Rows[i]["CheckInDate"]).ToString("yyyy-MM-dd HH:mm").PadRight(columnWidths[3]));
                    Console.Write(((DateTime)bookingHistory.Rows[i]["CheckOutDate"]).ToString("yyyy-MM-dd HH:mm").PadRight(columnWidths[4]));
                    Console.Write(bookingHistory.Rows[i]["Status"].ToString());
                }
        
                Console.SetCursorPosition(x + 2, y + 11 + bookingHistory.Rows.Count + 1);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CleanErrorMessage(ex.Message));
                Console.ReadKey();
            }
        }
    }
}
