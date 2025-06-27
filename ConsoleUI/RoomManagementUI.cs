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
                SetupBox(80, 18);
        
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
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
                Console.Write("6. Quay lại");
                Console.SetCursorPosition(x + 2, y + 15);
                Console.Write(new string('─', width - 4));
        
                Console.SetCursorPosition(x + 2, y + 16);
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
                    ShowErrorMessage($"Lỗi: {ex.Message}");
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
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
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
                _roomBLL.AddRoom(roomNumber, roomType, priceInput, amenities, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Thêm phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
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
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
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
            Console.Write("Trạng thái (Available/Occupied/Under Maintenance): ");
            string? status = ReadInputWithEscape(x + 50, y + 14);
            if (status == null) return;

            try
            {
                _roomBLL.UpdateRoom(roomIdInput, roomNumber, roomType, priceInput, amenities, status, currentUserId ?? 0, currentUsername ?? "");
                ShowSuccessMessage("Cập nhật phòng thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
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
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
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
                ShowErrorMessage($"Lỗi: {ex.Message}");
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
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
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
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Số phòng", "Loại phòng", "Giá (VND)", "Trạng thái", "Tiện nghi" };
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
                roomData[i, 5] = rooms.Rows[i]["Amenities"].ToString() ?? "";

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

        private void ShowCleanRoom()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Dọn Phòng");
            SetupBox(60, 10);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (GMT+7)");
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
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}
