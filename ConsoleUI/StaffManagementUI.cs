using System;
using System.Data;
using System.Linq;
using HotelManagementSystem;

namespace HotelManagementSystem.ConsoleUI
{
    public class StaffManagementUI : BaseUI
    {
        private readonly StaffBLL _staffBL;

        public StaffManagementUI(string? username = null, string? role = null, int? userId = null) : base(username, role, userId)
        {
            _staffBL = new StaffBLL();
        }

        public void ShowStaffManagement()
        {
            while (true)
            {
                Console.Clear();
                DrawHeader("Hệ Thống Quản Lý Khách Sạn - Quản Lý Nhân Viên");
                SetupBox(80, 22);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x + 2, y + 2);
                Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
                if (!string.IsNullOrEmpty(currentUsername))
                {
                    Console.Write($" | Người dùng: {currentUsername}");
                    if (!string.IsNullOrEmpty(currentRole))
                    {
                        Console.Write($" ({currentRole})");
                    }
                }
                Console.SetCursorPosition(x + 2, y + 3);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write("1. Thêm nhân viên mới");
                Console.SetCursorPosition(x + 2, y + 5);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 6);
                Console.Write("2. Xem tất cả nhân viên");
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 8);
                Console.Write("3. Tìm kiếm nhân viên");
                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 10);
                Console.Write("4. Cập nhật thông tin nhân viên");
                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("5. Xóa nhân viên");
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write(new string('─', width - 4));

                Console.SetCursorPosition(x + 2, y + 14);
                Console.Write("6. Xem nhân viên theo vai trò");
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
                            AddNewStaff();
                            break;
                        case "2":
                            ViewAllStaff();
                            break;
                        case "3":
                            SearchStaff();
                            break;
                        case "4":
                            UpdateStaff();
                            break;
                        case "5":
                            DeleteStaff();
                            break;
                        case "6":
                            ViewStaffByDepartment();
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
                    ShowErrorMessage($"Lỗi: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        private void AddNewStaff()
        {
            Console.Clear();
            DrawHeader("Thêm Nhân Viên Mới");
            SetupBox(80, 18);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Họ và tên: ");
            string? fullName = ReadInputWithEscape(x + 13, y + 5);
            if (fullName == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write("Vai trò (Receptionist/Housekeeping/Manager): ");
            string? role = ReadInputWithEscape(x + 44, y + 7);
            if (role == null) return;

            // Validate role
            if (!new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(role))
            {
                ShowErrorMessage("Vai trò không hợp lệ! Chỉ chấp nhận: Receptionist, Housekeeping, Manager");
                Console.ReadKey();
                return;
            }

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Số điện thoại: ");
            string? phone = ReadInputWithEscape(x + 17, y + 9);
            if (phone == null) return;

            try
            {
                _staffBL.AddStaff(fullName, role, phone, currentUserId ?? 1, currentUsername ?? "System");
                ShowSuccessMessage("Thêm nhân viên thành công! Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ViewAllStaff()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Danh Sách Nhân Viên");
            SetupBox(120, 25);

            ShowDateTimeInfo();
            
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- DANH SÁCH NHÂN VIÊN ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write(new string('─', width - 4));

            DataTable staffList;
            try
            {
                staffList = _staffBL.GetAllStaff();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
                return;
            }

            if (staffList.Rows.Count == 0)
            {
                Console.SetCursorPosition(x + 2, y + 8);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Không có nhân viên nào trong hệ thống.");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 10);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Họ tên", "Vai trò", "SĐT", "Ngày tạo" };
            int[] columnWidths = new[] { 5, 25, 15, 15, 12 };

            Console.SetCursorPosition(x + 2, y + 8);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
            {
                Console.Write(headers[col].PadRight(columnWidths[col]));
            }
            Console.WriteLine();

            Console.SetCursorPosition(x + 2, y + 9);
            Console.WriteLine(new string('─', width - 4));
            Console.ResetColor();

            int maxRowsToShow = Math.Min(staffList.Rows.Count, 10); // Giới hạn hiển thị 10 dòng
            for (int i = 0; i < maxRowsToShow; i++)
            {
                Console.SetCursorPosition(x + 2, y + 10 + i * 2);
                
                string id = (staffList.Rows[i]["StaffID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                string name = (staffList.Rows[i]["Name"]?.ToString() ?? "").PadRight(columnWidths[1]);
                string role = (staffList.Rows[i]["Role"]?.ToString() ?? "").PadRight(columnWidths[2]);
                string phone = (staffList.Rows[i]["Phone"]?.ToString() ?? "").PadRight(columnWidths[3]);
                string createdAt = staffList.Rows[i]["CreatedAt"] != DBNull.Value ? 
                    Convert.ToDateTime(staffList.Rows[i]["CreatedAt"]).ToString("yyyy-MM-dd") : "";

                // Cắt ngắn nếu quá dài
                if (name.Length > columnWidths[1]) name = name.Substring(0, columnWidths[1] - 3) + "...";

                Console.Write(id);
                Console.Write(name);
                Console.Write(role);
                Console.Write(phone);
                Console.Write(createdAt);

                if (i < maxRowsToShow - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 11 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            if (staffList.Rows.Count > 10)
            {
                Console.SetCursorPosition(x + 2, y + 10 + maxRowsToShow * 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Hiển thị {maxRowsToShow}/{staffList.Rows.Count} nhân viên. Sử dụng tìm kiếm để xem chi tiết.");
                Console.ResetColor();
            }

            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void SearchStaff()
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - Tìm Kiếm Nhân Viên");
            SetupBox(80, 20);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write("Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " (GMT+7)");
            if (!string.IsNullOrEmpty(currentUsername))
            {
                Console.Write($" | Người dùng: {currentUsername}");
                if (!string.IsNullOrEmpty(currentRole))
                {
                    Console.Write($" ({currentRole})");
                }
            }
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 3);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write("1. Tìm theo tên");
            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write("2. Tìm theo vai trò");
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write("3. Tìm kiếm tổng hợp");
            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write("0. Quay lại");

            Console.SetCursorPosition(x + 2, y + 12);
            Console.Write("Lựa chọn: ");
            string? choice = ReadInputWithEscape(x + 12, y + 12);
            if (choice == null) return;

            try
            {
                DataTable results = new DataTable();
                string searchTerm = "";

                switch (choice)
                {
                    case "1":
                        Console.SetCursorPosition(x + 2, y + 14);
                        Console.Write("Nhập tên nhân viên: ");
                        searchTerm = ReadInputWithEscape(x + 21, y + 14) ?? "";
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            var allStaff = _staffBL.GetAllStaff();
                            var filteredResults = allStaff.Clone();
                            foreach (DataRow row in allStaff.Rows)
                            {
                                if (row["Name"].ToString()!.ToLower().Contains(searchTerm.ToLower()))
                                {
                                    filteredResults.ImportRow(row);
                                }
                            }
                            results = filteredResults;
                        }
                        break;
                    case "2":
                        Console.SetCursorPosition(x + 2, y + 14);
                        Console.Write("Nhập vai trò (Receptionist/Housekeeping/Manager): ");
                        searchTerm = ReadInputWithEscape(x + 49, y + 14) ?? "";
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            results = _staffBL.SearchStaffByRole(searchTerm);
                        }
                        break;
                    case "3":
                        Console.SetCursorPosition(x + 2, y + 14);
                        Console.Write("Tên nhân viên (để trống nếu không lọc): ");
                        string? name = ReadInputWithEscape(x + 40, y + 14);
                        if (name == null) return;

                        Console.SetCursorPosition(x + 2, y + 16);
                        Console.Write("Vai trò (để trống nếu không lọc): ");
                        string? role = ReadInputWithEscape(x + 35, y + 16);
                        if (role == null) return;

                        if (!string.IsNullOrEmpty(role))
                        {
                            results = _staffBL.SearchStaffByRole(role);
                        }
                        else
                        {
                            results = _staffBL.GetAllStaff();
                        }

                        // Lọc thêm theo tên nếu có
                        if (!string.IsNullOrEmpty(name))
                        {
                            var filteredResults = results.Clone();
                            foreach (DataRow row in results.Rows)
                            {
                                if (row["Name"].ToString()!.ToLower().Contains(name.ToLower()))
                                {
                                    filteredResults.ImportRow(row);
                                }
                            }
                            results = filteredResults;
                        }
                        searchTerm = $"Tên: {name}, Vai trò: {role}";
                        break;
                    case "0":
                        return;
                    default:
                        ShowErrorMessage("Lựa chọn không hợp lệ!");
                        Console.ReadKey();
                        return;
                }

                // Hiển thị kết quả
                if (string.IsNullOrEmpty(searchTerm) && choice != "3")
                {
                    ShowErrorMessage("Vui lòng nhập từ khóa tìm kiếm!");
                    Console.ReadKey();
                    return;
                }
                
                ShowSearchResults(results, choice == "3" ? "Tìm kiếm tổng hợp" : $"Tìm kiếm: {searchTerm}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowSearchResults(DataTable staffList, string searchTitle)
        {
            Console.Clear();
            DrawHeader("Hệ Thống Quản Lý Khách Sạn - " + searchTitle);
            SetupBox(120, 25);

            ShowDateTimeInfo();
            
            Console.SetCursorPosition(x + 2, y + 4);
            Console.Write(new string('─', width - 4));

            Console.SetCursorPosition(x + 2, y + 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--- " + searchTitle.ToUpper() + " ---");
            Console.ResetColor();
            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write(new string('─', width - 4));

            if (staffList.Rows.Count == 0)
            {
                Console.SetCursorPosition(x + 2, y + 8);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Không tìm thấy nhân viên nào phù hợp với tiêu chí tìm kiếm.");
                Console.ResetColor();
                Console.SetCursorPosition(x + 2, y + 10);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey();
                return;
            }

            string[] headers = new[] { "ID", "Họ tên", "Vai trò", "SĐT", "Ngày tạo" };
            int[] columnWidths = new[] { 5, 25, 15, 15, 12 };

            Console.SetCursorPosition(x + 2, y + 8);
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 0; col < headers.Length; col++)
            {
                Console.Write(headers[col].PadRight(columnWidths[col]));
            }
            Console.WriteLine();

            Console.SetCursorPosition(x + 2, y + 9);
            Console.WriteLine(new string('─', width - 4));
            Console.ResetColor();

            int maxRowsToShow = Math.Min(staffList.Rows.Count, 10); // Giới hạn hiển thị 10 dòng
            for (int i = 0; i < maxRowsToShow; i++)
            {
                Console.SetCursorPosition(x + 2, y + 10 + i * 2);
                
                string id = (staffList.Rows[i]["StaffID"]?.ToString() ?? "").PadRight(columnWidths[0]);
                string name = (staffList.Rows[i]["Name"]?.ToString() ?? "").PadRight(columnWidths[1]);
                string role = (staffList.Rows[i]["Role"]?.ToString() ?? "").PadRight(columnWidths[2]);
                string phone = (staffList.Rows[i]["Phone"]?.ToString() ?? "").PadRight(columnWidths[3]);
                string createdAt = staffList.Rows[i]["CreatedAt"] != DBNull.Value ? 
                    Convert.ToDateTime(staffList.Rows[i]["CreatedAt"]).ToString("yyyy-MM-dd") : "";

                // Cắt ngắn nếu quá dài
                if (name.Length > columnWidths[1]) name = name.Substring(0, columnWidths[1] - 3) + "...";

                Console.Write(id);
                Console.Write(name);
                Console.Write(role);
                Console.Write(phone);
                Console.Write(createdAt);

                if (i < maxRowsToShow - 1)
                {
                    Console.SetCursorPosition(x + 2, y + 11 + i * 2);
                    Console.WriteLine(new string('─', width - 4));
                }
            }

            if (staffList.Rows.Count > 10)
            {
                Console.SetCursorPosition(x + 2, y + 10 + maxRowsToShow * 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Hiển thị {maxRowsToShow}/{staffList.Rows.Count} nhân viên. Tìm thấy tổng cộng {staffList.Rows.Count} kết quả.");
                Console.ResetColor();
            }

            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        private void UpdateStaff()
        {
            Console.Clear();
            DrawHeader("Cập Nhật Thông Tin Nhân Viên");
            SetupBox(80, 26);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Nhân viên: ");
            string? staffId = ReadInputWithEscape(x + 16, y + 5);
            if (staffId == null) return;

            if (!int.TryParse(staffId, out int staffIdInt))
            {
                ShowErrorMessage("ID nhân viên không hợp lệ!");
                Console.ReadKey();
                return;
            }

            // Get current staff info first
            try
            {
                // Lấy tất cả nhân viên và tìm theo ID
                var allStaff = _staffBL.GetAllStaff();
                var matchingRows = allStaff.AsEnumerable().Where(row => row.Field<int>("StaffId") == staffIdInt);
                
                if (!matchingRows.Any())
                {
                    ShowErrorMessage("Không tìm thấy nhân viên!");
                    Console.ReadKey();
                    return;
                }

                var currentStaff = matchingRows.First();
                
                Console.SetCursorPosition(x + 2, y + 7);
                Console.Write($"Họ và tên [{currentStaff["Name"]}]: ");
                string? fullName = ReadInputWithEscape(x + 30 + currentStaff["Name"].ToString()!.Length, y + 7);
                if (fullName == null) return;
                if (string.IsNullOrEmpty(fullName)) fullName = currentStaff["Name"].ToString();

                Console.SetCursorPosition(x + 2, y + 9);
                Console.Write($"Vai trò [{currentStaff["Role"]}]: ");
                string? role = ReadInputWithEscape(x + 12 + currentStaff["Role"].ToString()!.Length, y + 9);
                if (role == null) return;
                if (string.IsNullOrEmpty(role)) role = currentStaff["Role"].ToString();

                Console.SetCursorPosition(x + 2, y + 11);
                Console.Write($"Số điện thoại [{currentStaff["Phone"]}]: ");
                string? phone = ReadInputWithEscape(x + 22 + currentStaff["Phone"].ToString()!.Length, y + 11);
                if (phone == null) return;
                if (string.IsNullOrEmpty(phone)) phone = currentStaff["Phone"].ToString();

                // Gọi UpdateStaff với signature đúng: (staffIdInput, name, role, phone, updatedBy, updatedByUsername)
                _staffBL.UpdateStaff(staffId, fullName!, role!, phone!, 1, "Admin");
                ShowSuccessMessage("Cập nhật thông tin nhân viên thành công!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
            }

            Console.ReadKey();
        }

        private void DeleteStaff()
        {
            Console.Clear();
            DrawHeader("Xóa Nhân Viên");
            SetupBox(70, 14);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("ID Nhân viên: ");
            string? staffId = ReadInputWithEscape(x + 16, y + 5);
            if (staffId == null) return;

            Console.SetCursorPosition(x + 2, y + 7);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("CẢNH BÁO: Thao tác này không thể hoàn tác!");
            Console.ResetColor();

            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write("Xác nhận xóa (y/n): ");
            string? confirm = ReadInputWithEscape(x + 22, y + 9);
            if (confirm == null || confirm.ToLower() != "y") return;

            try
            {
                if (int.TryParse(staffId, out int staffIdInt))
                {
                    // Gọi DeleteStaff với signature đúng: (staffIdInput, updatedBy, updatedByUsername)
                    _staffBL.DeleteStaff(staffId, 1, "Admin");
                    ShowSuccessMessage("Xóa nhân viên thành công! Nhấn phím bất kỳ để quay lại...");
                }
                else
                {
                    ShowErrorMessage("ID nhân viên không hợp lệ!");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ViewStaffByDepartment()
        {
            Console.Clear();
            DrawHeader("Xem Nhân Viên Theo Vai Trò");
            SetupBox(70, 12);

            ShowDateTimeInfo();

            Console.SetCursorPosition(x + 2, y + 5);
            Console.Write("Vai trò (Receptionist/Housekeeping/Manager): ");
            string? role = ReadInputWithEscape(x + 45, y + 5);
            if (role == null) return;

            try
            {
                // Sử dụng SearchStaffByRole thay vì GetStaffByDepartment
                var staffList = _staffBL.SearchStaffByRole(role);
                
                ShowSearchResults(staffList, $"Nhân Viên Vai Trò: {role.ToUpper()}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Lỗi: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void ShowDateTimeInfo()
        {
            Console.SetCursorPosition(x + 2, y + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            if (!string.IsNullOrEmpty(currentUsername))
            {
                Console.Write($" | Người dùng: {currentUsername}");
                if (!string.IsNullOrEmpty(currentRole))
                {
                    Console.Write($" ({currentRole})");
                }
            }
            Console.ResetColor();
        }
    }
}
