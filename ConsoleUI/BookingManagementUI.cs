using System;
using System.Data;
using System.Globalization;

namespace HotelManagementSystem
{
    public class BookingManagementUI : BaseUI
    {
        private readonly BookingBLL _bookingBLL = new BookingBLL();

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
                SetupBox(80, 24);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Tạo đặt phòng mới");
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
                Console.Write("7. Xem danh sách đặt phòng");
                Console.SetCursorPosition(x + 2, y + 17);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 18);
                Console.Write("8. Quay lại");
                Console.SetCursorPosition(x + 2, y + 19);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 20);
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
                            ShowBookingHistory();
                            break;
                        case "7":
                            ShowBookingList();
                            break;
                        case "8":
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
                    ShowErrorMessage(ex.Message);
                    Console.ReadKey();
                }
            }
        }

        private void ShowCreateBooking()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tạo Đặt Phòng Mới");
            SetupBox(90, 18);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Số CMND/CCCD: ");
            string? idCard = ReadInputWithEscape(x + 16, y + 4);
            if (idCard == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("ID phòng: ");
            string? roomIdInput = ReadInputWithEscape(x + 12, y + 6);
            if (roomIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("Ngày check-in (dd/MM/yyyy HH:mm): ");
            string? checkInDateInput = ReadInputWithEscape(x + 34, y + 8);
            if (checkInDateInput == null) return;

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("Ngày check-out (dd/MM/yyyy HH:mm): ");
            string? checkOutDateInput = ReadInputWithEscape(x + 35, y + 10);
            if (checkOutDateInput == null) return;

            try
            {
                int bookingId = _bookingBLL.CreateBooking(idCard, roomIdInput, checkInDateInput, checkOutDateInput, currentUserId ?? 0, currentUsername ?? "");
                
                WriteTextInBox($"Tạo đặt phòng thành công! BookingID: {bookingId}", y + 12, ConsoleColor.Green);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                WriteTextInBox($"Lỗi: {ex.Message}", y + 12, ConsoleColor.Red);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private void ShowCancelBooking()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Hủy Đặt Phòng");
            SetupBox(80, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (bookingIdInput == null) return;

            try
            {
                _bookingBLL.CancelBooking(bookingIdInput, currentUserId ?? 0, currentUsername ?? "");
                
                WriteTextInBox("Hủy đặt phòng thành công!", y + 6, ConsoleColor.Green);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                WriteTextInBox($"Lỗi: {ex.Message}", y + 6, ConsoleColor.Red);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private void ShowCheckIn()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Check-in");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (bookingIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Số CMND/CCCD: ");
            string? idCard = ReadInputWithEscape(x + 16, y + 6);
            if (idCard == null) return;

            try
            {
                _bookingBLL.CheckIn(bookingIdInput, idCard, currentUserId ?? 0, currentUsername ?? "");
                
                WriteTextInBox("Check-in thành công!", y + 8, ConsoleColor.Green);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                WriteTextInBox($"Lỗi: {ex.Message}", y + 8, ConsoleColor.Red);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private void ShowCheckOut()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Check-out");
            SetupBox(80, 16);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            // Nhập BookingID
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (bookingIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            // Nhập IDCard để xác minh
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Số CMND/CCCD: ");
            string? idCard = ReadInputWithEscape(x + 16, y + 6);
            if (idCard == null) return;

            try
            {
                _bookingBLL.CheckOut(bookingIdInput, idCard, currentUserId ?? 0, currentUsername ?? "");
                
                WriteTextInBox("Check-out thành công!", y + 8, ConsoleColor.Green);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                WriteTextInBox($"Lỗi: {ex.Message}", y + 8, ConsoleColor.Red);
                
                Console.SetCursorPosition(x + 2, y + height - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
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
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID đặt phòng: ");
            string? bookingIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (bookingIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("Ngày check-out mới (dd/MM/yyyy HH:mm): ");
            string? newCheckOutDateInput = ReadInputWithEscape(x + 39, y + 6);
            if (newCheckOutDateInput == null) return;

            try
            {
                DateTime newCheckOutDate = DateTime.ParseExact(
                    newCheckOutDateInput,
                    "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture
                );

                _bookingBLL.ExtendBooking(bookingIdInput, newCheckOutDate, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Gia hạn đặt phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }



        private void ShowBookingHistory()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Lịch Sử Đặt Phòng");
            SetupBox(100, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("ID khách hàng: ");
            string? customerIdInput = ReadInputWithEscape(x + 16, y + 4);
            if (customerIdInput == null) return;

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("ID phòng (để trống nếu không lọc): ");
            string? roomIdInput = ReadInputWithEscape(x + 36, y + 6);
            if (roomIdInput == null) return;

            try
            {
                int customerId = int.Parse(customerIdInput);
                int? roomId = string.IsNullOrWhiteSpace(roomIdInput) ? null : int.Parse(roomIdInput);
                DataTable bookings = _bookingBLL.GetBookingHistory(customerId, roomId);

                Console.SetCursorPosition(x + 2, y + 7);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--- LỊCH SỬ ĐẶT PHÒNG ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write(new string('─', width - 4));

                string[] headers = new[] { "ID", "Khách hàng", "IDPhòng", "Check-in", "Check-out", "Trạng thái" };
                int[] columnWidths = new int[headers.Length];
                for (int col = 0; col < headers.Length; col++)
                {
                    columnWidths[col] = headers[col].Length;
                }

                string[,] bookingData = new string[bookings.Rows.Count, headers.Length];
                for (int i = 0; i < bookings.Rows.Count; i++)
                {
                    bookingData[i, 0] = bookings.Rows[i]["BookingID"].ToString() ?? "";
                    bookingData[i, 1] = bookings.Rows[i]["CustomerID"].ToString() ?? "";
                    bookingData[i, 2] = bookings.Rows[i]["RoomID"].ToString() ?? "";
                    bookingData[i, 3] = Convert.ToDateTime(bookings.Rows[i]["CheckInDate"]).ToString("dd/MM/yyyy HH:mm");
                    bookingData[i, 4] = Convert.ToDateTime(bookings.Rows[i]["CheckOutDate"]).ToString("dd/MM/yyyy HH:mm");
                    bookingData[i, 5] = bookings.Rows[i]["Status"].ToString() ?? "";

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

                Console.SetCursorPosition(x + 2, y + 15 + (bookingData.GetLength(0) - 1) * 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }

        private void ShowBookingList()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Đặt Phòng");
            SetupBox(100, 22);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH ĐẶT PHÒNG ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            DataTable bookings;
            try
            {
                bookings = _bookingBLL.GetAllBookings();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Khách hàng", "IDPhòng", "Check-in", "Check-out", "Trạng thái" };
            int[] columnWidths = new int[headers.Length];
            for (int col = 0; col < headers.Length; col++)
            {
                columnWidths[col] = headers[col].Length;
            }

            string[,] bookingData = new string[bookings.Rows.Count, headers.Length];
            for (int i = 0; i < bookings.Rows.Count; i++)
            {
                bookingData[i, 0] = bookings.Rows[i]["BookingID"].ToString() ?? "";
                bookingData[i, 1] = bookings.Rows[i]["CustomerID"].ToString() ?? "";
                bookingData[i, 2] = bookings.Rows[i]["RoomID"].ToString() ?? "";
                bookingData[i, 3] = Convert.ToDateTime(bookings.Rows[i]["CheckInDate"]).ToString("dd/MM/yyyy HH:mm");
                bookingData[i, 4] = Convert.ToDateTime(bookings.Rows[i]["CheckOutDate"]).ToString("dd/MM/yyyy HH:mm");
                bookingData[i, 5] = bookings.Rows[i]["Status"].ToString() ?? "";

                for (int col = 0; col < headers.Length; col++)
                {
                    int length = bookingData[i, col].Length;
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

            for (int i = 0; i < bookingData.GetLength(0); i++)
            {
                Console.SetCursorPosition(x + 2, y + 8 + i * 2);
                for (int col = 0; col < bookingData.GetLength(1); col++)
                {
                    Console.Write(bookingData[i, col].PadRight(columnWidths[col]));
                }
                Console.WriteLine();
                if (i < bookingData.GetLength(0) - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 9 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (bookingData.GetLength(0) - 1) * 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }
    }
}