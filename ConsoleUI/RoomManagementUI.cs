using System;
using System.Data;

namespace HotelManagementSystem
{
    public class RoomManagementUI : BaseUI
    {
        private readonly RoomBLL _roomBLL = new RoomBLL();

        public RoomManagementUI(string? username, string? role, int? userId)
            : base(username, role, userId)
        {
        }

        public void ShowRoomManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Phòng");
                SetupBox(80, 22);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
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
                Console.Write("6. Tìm kiếm phòng");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 16);
                Console.Write("7. Kiểm tra tình trạng phòng");
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
                            ShowSearchRooms();
                            break;
                        case "7":
                            ShowCheckRoomAvailability();
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

        private void ShowAddRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Thêm Phòng Mới");
            SetupBox(80, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
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
                int roomId = _roomBLL.AddRoom(roomNumber, roomType, priceInput, amenities, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage($"Thêm phòng thành công! RoomID: {roomId}. Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
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
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
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
            Console.Write("Trạng thái (Available/Occupied/Under Maintenance/Uncleaned): ");
            string? status = ReadInputWithEscape(x + 60, y + 14);
            if (status == null) return;

            try
            {
                _roomBLL.UpdateRoom(roomIdInput, roomNumber, roomType, priceInput, amenities, status, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Cập nhật phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
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
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
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
                ShowErrorMessage(ex.Message);
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
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
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
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Số phòng", "Loại phòng", "Giá (VND)", "Trạng thái", "Tiện nghi" };
            int[] columnWidths = new int[headers.Length];

            // Đặt độ rộng tối thiểu cho từng cột
            columnWidths[0] = 5;  // ID
            columnWidths[1] = 10; // Số phòng
            columnWidths[2] = 12; // Loại phòng
            columnWidths[3] = 15; // Giá (VND)
            columnWidths[4] = 15; // Trạng thái
            columnWidths[5] = 20; // Tiện nghi

            string[,] roomData = new string[rooms.Rows.Count, headers.Length];
            for (int i = 0; i < rooms.Rows.Count; i++)
            {
                roomData[i, 0] = rooms.Rows[i]["RoomID"].ToString() ?? "";
                roomData[i, 1] = rooms.Rows[i]["RoomNumber"].ToString() ?? "";
                roomData[i, 2] = rooms.Rows[i]["RoomType"].ToString() ?? "";
                roomData[i, 3] = Convert.ToDecimal(rooms.Rows[i]["Price"]).ToString("N0");
                roomData[i, 4] = rooms.Rows[i]["Status"].ToString() ?? "";
                roomData[i, 5] = TruncateText(rooms.Rows[i]["Amenities"].ToString() ?? "", columnWidths[5] - 3); // Cắt ngắn tiện nghi

                // Cập nhật độ rộng cột nếu cần
                for (int col = 0; col < headers.Length; col++)
                {
                    int length = roomData[i, col].Length;
                    if (length > columnWidths[col])
                        columnWidths[col] = Math.Min(length, columnWidths[col] + 5); // Tăng độ rộng tối đa 5 ký tự
                }
            }

            // Hiển thị tiêu đề cột
            Console.SetCursorPosition(x + 2, y + 6);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
            {
                Console.Write(headers[col].PadRight(columnWidths[col]));
            }
            Console.WriteLine();

            // Hiển thị đường phân cách
            Console.SetCursorPosition(x + 2, y + 7);
            Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
            Console.ResetColor();

            // Hiển thị dữ liệu
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
                    Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
                }
            }

            Console.SetCursorPosition(x + 2, y + 12 + (roomData.GetLength(0) - 1) * 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        // Phương thức hỗ trợ cắt ngắn văn bản
        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            return text.Substring(0, maxLength) + "...";
        }

        private void ShowCleanRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Dọn Phòng");
            SetupBox(60, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
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
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }

        private void ShowSearchRooms()
{
    Console.Clear();
    DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tìm Kiếm Phòng");
    SetupBox(100, 22);

    Console.ForegroundColor = ConsoleColor.White;
    Console.SetCursorPosition(x + 2, y + 2);
    Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
    Console.SetCursorPosition(x + 2, y + 3);
    Console.Write(new string('─', width - 4));

  Console.SetCursorPosition(x + 2, y + 4);
Console.Write("Trạng thái (Available/Occupied/Under Maintenance/Uncleaned)");
Console.SetCursorPosition(x + 2, y + 5);
Console.Write("Để trống nếu không lọc");

    string? status = ReadInputWithEscape(x + 76, y + 4);
    if (status == null) return;

    Console.SetCursorPosition(x + 2, y + 5);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 6);
    Console.Write("Loại phòng (Single/Double/Suite, để trống nếu không lọc): ");
    string? roomType = ReadInputWithEscape(x + 58, y + 6);
    if (roomType == null) return;

    Console.SetCursorPosition(x + 2, y + 7);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 8);
    Console.Write("Giá tối thiểu (VND, để trống nếu không lọc): ");
    string? minPriceInput = ReadInputWithEscape(x + 44, y + 8);
    if (minPriceInput == null) return;

    Console.SetCursorPosition(x + 2, y + 9);
    Console.Write(new string('─', width - 4));

    Console.SetCursorPosition(x + 2, y + 10);
    Console.Write("Giá tối đa (VND, để trống nếu không lọc): ");
    string? maxPriceInput = ReadInputWithEscape(x + 42, y + 10);
    if (maxPriceInput == null) return;

    try
    {
        decimal? minPrice = string.IsNullOrWhiteSpace(minPriceInput) ? null : decimal.Parse(minPriceInput);
        decimal? maxPrice = string.IsNullOrWhiteSpace(maxPriceInput) ? null : decimal.Parse(maxPriceInput);
        DataTable rooms = _roomBLL.SearchRooms(status, roomType, minPrice, maxPrice);

        Console.Clear();
        DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kết Quả Tìm Kiếm Phòng");
        SetupBox(100, 22);

        Console.SetCursorPosition(x + 2, y + 2);
        Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
        Console.SetCursorPosition(x + 2, y + 3);
        Console.Write(new string('─', width - 4));

        Console.SetCursorPosition(x + 2, y + 4);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("--- KẾT QUẢ TÌM KIẾM PHÒNG ---");
        Console.ResetColor();
        Console.SetCursorPosition(x + 2, y + 5);
        Console.Write(new string('─', width - 4));

        string[] headers = new[] { "ID", "Số phòng", "Loại phòng", "Giá (VND)", "Trạng thái", "Tiện nghi" };
        int[] columnWidths = new int[headers.Length];
        columnWidths[0] = 5;
        columnWidths[1] = 10;
        columnWidths[2] = 12;
        columnWidths[3] = 15;
        columnWidths[4] = 15;
        columnWidths[5] = 20;

        string[,] roomData = new string[rooms.Rows.Count, headers.Length];
        for (int i = 0; i < rooms.Rows.Count; i++)
        {
            roomData[i, 0] = rooms.Rows[i]["RoomID"].ToString() ?? "";
            roomData[i, 1] = rooms.Rows[i]["RoomNumber"].ToString() ?? "";
            roomData[i, 2] = rooms.Rows[i]["RoomType"].ToString() ?? "";
            roomData[i, 3] = Convert.ToDecimal(rooms.Rows[i]["Price"]).ToString("N0");
            roomData[i, 4] = rooms.Rows[i]["Status"].ToString() ?? "";
            roomData[i, 5] = TruncateText(rooms.Rows[i]["Amenities"].ToString() ?? "", columnWidths[5] - 3);

            for (int col = 0; col < headers.Length; col++)
            {
                int length = roomData[i, col].Length;
                if (length > columnWidths[col])
                    columnWidths[col] = Math.Min(length, columnWidths[col] + 5);
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
        Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
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
                Console.WriteLine(new string('─', columnWidths.Sum() + headers.Length - 1));
            }
        }

        Console.SetCursorPosition(x + 2, y + 12 + (roomData.GetLength(0) - 1) * 2);
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
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kiểm Tra Tình Trạng Phòng");
            SetupBox(80, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("Ngày kết thúc (dd/MM/yyyy): ");
            string? endDateInput = ReadInputWithEscape(x + 30, y + 4);
            if (endDateInput == null) return;

            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.ParseExact(endDateInput, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DataTable rooms = _roomBLL.CheckRoomAvailability(startDate, endDate);

                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Kết Quả Kiểm Tra Tình Trạng Phòng");
                SetupBox(100, 22);

                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " (GMT+7)");
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"--- PHÒNG TRỐNG TỪ {startDate:dd/MM/yyyy HH:mm} ĐẾN {endDate:dd/MM/yyyy} ---");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                string[] headers = new[] { "ID", "Số phòng", "Loại phòng", "Giá (VND)", "Trạng thái" };
                int[] columnWidths = new int[headers.Length];
                for (int col = 0; col < headers.Length; col++)
                {
                    columnWidths[col] = headers[col].Length;
                }

                string[,] roomData = new string[rooms.Rows.Count, headers.Length];
                for (int i = 0; i < rooms.Rows.Count; i++)
                {
                    roomData[i, 0] = rooms.Rows[i]["RoomID"].ToString() ?? "";
                    roomData[i, 1] = rooms.Rows[i]["RoomNumber"].ToString() ?? "";
                    roomData[i, 2] = rooms.Rows[i]["RoomType"].ToString() ?? "";
                    roomData[i, 3] = Convert.ToDecimal(rooms.Rows[i]["Price"]).ToString("N0");
                    roomData[i, 4] = rooms.Rows[i]["Status"].ToString() ?? "";

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
            catch (FormatException)
            {
                ShowErrorMessage("Ngày kết thúc không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                Console.ReadKey();
            }
        }

    }
}