using System.Data;
using System.Text.RegularExpressions;

namespace VTCAcademy_PF1127_HotelSync.CustomerManagement
{
    /// <summary>
    /// Business Logic Layer cho module quản lý khách hàng
    /// Xử lý logic nghiệp vụ và kiểm tra dữ liệu đầu vào
    /// </summary>
    public class CustomerBL
    {
        private readonly CustomerDAL customerDAL;

        public CustomerBL()
        {
            customerDAL = new CustomerDAL();
        }

        /// <summary>
        /// Thêm khách hàng mới với validation đầy đủ
        /// </summary>
        /// <param name="name">Tên khách hàng</param>
        /// <param name="idCard">CMND/Passport</param>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="email">Email</param>
        /// <param name="nationality">Quốc tịch</param>
        /// <param name="updatedBy">ID người tạo</param>
        /// <param name="updatedByUsername">Username người tạo</param>
        /// <returns>Tuple với success status và message</returns>
        public (bool Success, string Message, int CustomerId) AddCustomer(string name, string idCard, string phone, string email, string nationality, int? updatedBy, string updatedByUsername)
        {
            try
            {
                // Validation dữ liệu đầu vào
                var validation = ValidateCustomerData(name, idCard, phone, email, nationality);
                if (!validation.IsValid)
                {
                    return (false, validation.ErrorMessage, -1);
                }

                // Kiểm tra CMND/Passport đã tồn tại chưa
                if (customerDAL.IsIdCardExists(idCard))
                {
                    return (false, "CMND/Passport đã tồn tại trong hệ thống!", -1);
                }

                // Thêm khách hàng
                int customerId = customerDAL.AddCustomer(name, idCard, phone, email, nationality, updatedBy, updatedByUsername);
                
                if (customerId > 0)
                {
                    return (true, $"Thêm khách hàng thành công! ID: {customerId}", customerId);
                }
                else
                {
                    return (false, "Lỗi khi thêm khách hàng vào database!", -1);
                }
            }
            catch (Exception ex)
            {
                // Log error instead of Console.WriteLine to avoid UI output pollution
                HotelManagementSystem.SimpleLogger.LogError("CustomerBL", "AddCustomer", ex);
                return (false, "Đã xảy ra lỗi hệ thống khi thêm khách hàng!", -1);
            }
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <param name="name">Tên mới</param>
        /// <param name="phone">Số điện thoại mới</param>
        /// <param name="email">Email mới</param>
        /// <param name="nationality">Quốc tịch mới</param>
        /// <param name="updatedBy">ID người cập nhật</param>
        /// <param name="updatedByUsername">Username người cập nhật</param>
        /// <returns>Tuple với success status và message</returns>
        public (bool Success, string Message) UpdateCustomer(int customerId, string name, string phone, string email, string nationality, int? updatedBy, string updatedByUsername)
        {
            try
            {
                // Kiểm tra khách hàng có tồn tại không
                var customer = customerDAL.GetCustomerById(customerId);
                if (customer == null)
                {
                    return (false, "Không tìm thấy khách hàng cần cập nhật!");
                }

                // Validation dữ liệu (không kiểm tra IDCard vì không cho phép thay đổi)
                var validation = ValidateCustomerUpdateData(name, phone, email, nationality);
                if (!validation.IsValid)
                {
                    return (false, validation.ErrorMessage);
                }

                // Cập nhật khách hàng
                bool success = customerDAL.UpdateCustomer(customerId, name, phone, email, nationality, updatedBy, updatedByUsername);
                
                if (success)
                {
                    return (true, "Cập nhật thông tin khách hàng thành công!");
                }
                else
                {
                    return (false, "Lỗi khi cập nhật thông tin khách hàng!");
                }
            }
            catch (Exception ex)
            {
                // Log error to file instead of console
                HotelManagementSystem.SimpleLogger.LogError("CustomerBL", "UpdateCustomer", ex);
                return (false, "Đã xảy ra lỗi hệ thống khi cập nhật khách hàng!");
            }
        }

        /// <summary>
        /// Xóa khách hàng (chỉ khi không có booking/hóa đơn liên quan)
        /// </summary>
        /// <param name="customerId">ID khách hàng cần xóa</param>
        /// <returns>Tuple với success status và message</returns>
        public (bool Success, string Message) DeleteCustomer(int customerId)
        {
            try
            {
                // Kiểm tra khách hàng có tồn tại không
                var customer = customerDAL.GetCustomerById(customerId);
                if (customer == null)
                {
                    return (false, "Không tìm thấy khách hàng cần xóa!");
                }

                // Xóa khách hàng
                bool success = customerDAL.DeleteCustomer(customerId);
                
                if (success)
                {
                    return (true, "Xóa khách hàng thành công!");
                }
                else
                {
                    return (false, "Không thể xóa khách hàng vì có booking hoặc hóa đơn liên quan!");
                }
            }
            catch (Exception ex)
            {
                // Log error to file instead of console
                HotelManagementSystem.SimpleLogger.LogError("CustomerBL", "DeleteCustomer", ex);
                return (false, "Đã xảy ra lỗi hệ thống khi xóa khách hàng!");
            }
        }

        /// <summary>
        /// Tìm khách hàng theo ID
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <returns>Thông tin khách hàng hoặc null</returns>
        public DataRow? GetCustomerById(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    Console.WriteLine("ID khách hàng không hợp lệ!");
                    return null;
                }

                return customerDAL.GetCustomerById(customerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.GetCustomerById: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tìm khách hàng theo CMND/Passport
        /// </summary>
        /// <param name="idCard">CMND/Passport</param>
        /// <returns>Thông tin khách hàng hoặc null</returns>
        public DataRow? GetCustomerByIdCard(string idCard)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idCard))
                {
                    Console.WriteLine("CMND/Passport không được để trống!");
                    return null;
                }

                return customerDAL.GetCustomerByIdCard(idCard);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.GetCustomerByIdCard: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo tên
        /// </summary>
        /// <param name="name">Tên khách hàng (có thể là một phần)</param>
        /// <returns>Danh sách khách hàng phù hợp</returns>
        public DataTable SearchCustomersByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Tên tìm kiếm không được để trống!");
                    return new DataTable();
                }

                if (name.Trim().Length < 2)
                {
                    Console.WriteLine("Tên tìm kiếm phải có ít nhất 2 ký tự!");
                    return new DataTable();
                }

                return customerDAL.SearchCustomersByName(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.SearchCustomersByName: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo số điện thoại
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <returns>Danh sách khách hàng phù hợp</returns>
        public DataTable SearchCustomersByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    Console.WriteLine("Số điện thoại tìm kiếm không được để trống!");
                    return new DataTable();
                }

                return customerDAL.SearchCustomersByPhone(phone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.SearchCustomersByPhone: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        /// <returns>Danh sách tất cả khách hàng</returns>
        public DataTable GetAllCustomers()
        {
            try
            {
                return customerDAL.GetAllCustomers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.GetAllCustomers: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy lịch sử của khách hàng (bao gồm booking và hóa đơn)
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <returns>Tuple chứa lịch sử booking và hóa đơn</returns>
        public (DataTable BookingHistory, DataTable InvoiceHistory) GetCustomerHistory(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    Console.WriteLine("ID khách hàng không hợp lệ!");
                    return (new DataTable(), new DataTable());
                }

                // Kiểm tra khách hàng có tồn tại không
                var customer = customerDAL.GetCustomerById(customerId);
                if (customer == null)
                {
                    Console.WriteLine("Không tìm thấy khách hàng!");
                    return (new DataTable(), new DataTable());
                }

                var bookingHistory = customerDAL.GetCustomerBookingHistory(customerId);
                var invoiceHistory = customerDAL.GetCustomerInvoiceHistory(customerId);

                return (bookingHistory, invoiceHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.GetCustomerHistory: {ex.Message}");
                return (new DataTable(), new DataTable());
            }
        }

        /// <summary>
        /// Validation dữ liệu khách hàng khi thêm mới
        /// </summary>
        /// <param name="name">Tên khách hàng</param>
        /// <param name="idCard">CMND/Passport</param>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="email">Email</param>
        /// <param name="nationality">Quốc tịch</param>
        /// <returns>Kết quả validation</returns>
        private (bool IsValid, string ErrorMessage) ValidateCustomerData(string name, string idCard, string phone, string email, string nationality)
        {
            // Kiểm tra tên
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Tên khách hàng không được để trống!");
            }

            if (name.Trim().Length < 2 || name.Trim().Length > 100)
            {
                return (false, "Tên khách hàng phải từ 2-100 ký tự!");
            }

            // Kiểm tra CMND/Passport
            if (string.IsNullOrWhiteSpace(idCard))
            {
                return (false, "CMND/Passport không được để trống!");
            }

            if (idCard.Trim().Length < 8 || idCard.Trim().Length > 20)
            {
                return (false, "CMND/Passport phải từ 8-20 ký tự!");
            }

            // Kiểm tra số điện thoại (nếu có)
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!IsValidPhoneNumber(phone))
                {
                    return (false, "Số điện thoại không hợp lệ! (Định dạng: 0xxxxxxxxx hoặc +84xxxxxxxxx)");
                }
            }

            // Kiểm tra email (nếu có)
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!IsValidEmail(email))
                {
                    return (false, "Email không hợp lệ!");
                }

                if (email.Trim().Length > 100)
                {
                    return (false, "Email không được vượt quá 100 ký tự!");
                }
            }

            // Kiểm tra quốc tịch
            if (!string.IsNullOrWhiteSpace(nationality) && nationality.Trim().Length > 50)
            {
                return (false, "Quốc tịch không được vượt quá 50 ký tự!");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Validation dữ liệu khách hàng khi cập nhật (không kiểm tra IDCard)
        /// </summary>
        /// <param name="name">Tên khách hàng</param>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="email">Email</param>
        /// <param name="nationality">Quốc tịch</param>
        /// <returns>Kết quả validation</returns>
        private (bool IsValid, string ErrorMessage) ValidateCustomerUpdateData(string name, string phone, string email, string nationality)
        {
            // Kiểm tra tên
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Tên khách hàng không được để trống!");
            }

            if (name.Trim().Length < 2 || name.Trim().Length > 100)
            {
                return (false, "Tên khách hàng phải từ 2-100 ký tự!");
            }

            // Kiểm tra số điện thoại (nếu có)
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!IsValidPhoneNumber(phone))
                {
                    return (false, "Số điện thoại không hợp lệ! (Định dạng: 0xxxxxxxxx hoặc +84xxxxxxxxx)");
                }
            }

            // Kiểm tra email (nếu có)
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!IsValidEmail(email))
                {
                    return (false, "Email không hợp lệ!");
                }

                if (email.Trim().Length > 100)
                {
                    return (false, "Email không được vượt quá 100 ký tự!");
                }
            }

            // Kiểm tra quốc tịch
            if (!string.IsNullOrWhiteSpace(nationality) && nationality.Trim().Length > 50)
            {
                return (false, "Quốc tịch không được vượt quá 50 ký tự!");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Kiểm tra định dạng số điện thoại Việt Nam
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <returns>True nếu hợp lệ, False nếu không</returns>
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Xóa khoảng trắng và ký tự đặc biệt
            string cleanPhone = Regex.Replace(phone.Trim(), @"[\s\-\(\)]", "");

            // Kiểm tra định dạng số điện thoại Việt Nam
            // Chấp nhận: 0xxxxxxxxx (10 số) hoặc +84xxxxxxxxx
            string pattern = @"^(\+84|84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-9]|9[0-9])[0-9]{7}$";
            
            return Regex.IsMatch(cleanPhone, pattern);
        }

        /// <summary>
        /// Kiểm tra định dạng email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>True nếu hợp lệ, False nếu không</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email.Trim(), pattern);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Format thông tin khách hàng để hiển thị
        /// </summary>
        /// <param name="customerRow">DataRow chứa thông tin khách hàng</param>
        /// <returns>Chuỗi thông tin được format</returns>
        public string FormatCustomerInfo(DataRow customerRow)
        {
            if (customerRow == null)
                return "Không có thông tin khách hàng.";

            try
            {
                string info = $"ID: {customerRow["CustomerID"]}\n";
                info += $"Tên: {customerRow["Name"]}\n";
                info += $"CMND/Passport: {customerRow["IDCard"]}\n";
                info += $"Điện thoại: {(customerRow["Phone"] == DBNull.Value ? "Chưa cập nhật" : customerRow["Phone"])}\n";
                info += $"Email: {(customerRow["Email"] == DBNull.Value ? "Chưa cập nhật" : customerRow["Email"])}\n";
                info += $"Quốc tịch: {customerRow["Nationality"]}\n";
                info += $"Ngày tạo: {Convert.ToDateTime(customerRow["CreatedAt"]):dd/MM/yyyy HH:mm:ss}\n";
                info += $"Cập nhật lần cuối: {Convert.ToDateTime(customerRow["UpdatedAt"]):dd/MM/yyyy HH:mm:ss}\n";
                
                if (customerRow["UpdatedByUsername"] != DBNull.Value)
                {
                    info += $"Người cập nhật: {customerRow["UpdatedByUsername"]}\n";
                }

                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi format thông tin khách hàng: {ex.Message}");
                return "Lỗi hiển thị thông tin khách hàng.";
            }
        }

        /// <summary>
        /// Lấy thống kê về khách hàng
        /// </summary>
        /// <returns>Thông tin thống kê</returns>
        public (int TotalCustomers, int CustomersWithBookings, int CustomersWithInvoices) GetCustomerStatistics()
        {
            try
            {
                var allCustomers = customerDAL.GetAllCustomers();
                int totalCustomers = allCustomers.Rows.Count;

                int customersWithBookings = 0;
                int customersWithInvoices = 0;

                foreach (DataRow customer in allCustomers.Rows)
                {
                    int customerId = Convert.ToInt32(customer["CustomerID"]);
                    
                    var bookingHistory = customerDAL.GetCustomerBookingHistory(customerId);
                    if (bookingHistory.Rows.Count > 0)
                    {
                        customersWithBookings++;
                    }

                    var invoiceHistory = customerDAL.GetCustomerInvoiceHistory(customerId);
                    if (invoiceHistory.Rows.Count > 0)
                    {
                        customersWithInvoices++;
                    }
                }

                return (totalCustomers, customersWithBookings, customersWithInvoices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thống kê khách hàng: {ex.Message}");
                return (0, 0, 0);
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng tổng hợp (theo tên, CMND, hoặc SĐT)
        /// </summary>
        /// <param name="name">Tên khách hàng (có thể là một phần), null để bỏ qua</param>
        /// <param name="idCard">CMND/Passport chính xác, null để bỏ qua</param>
        /// <param name="phone">Số điện thoại chính xác, null để bỏ qua</param>
        /// <returns>Danh sách khách hàng phù hợp</returns>
        public DataTable SearchCustomers(string? name = null, string? idCard = null, string? phone = null)
        {
            try
            {
                // Kiểm tra ít nhất một tiêu chí tìm kiếm được cung cấp
                if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(idCard) && string.IsNullOrWhiteSpace(phone))
                {
                    Console.WriteLine("Cần cung cấp ít nhất một tiêu chí tìm kiếm!");
                    return new DataTable();
                }

                // Validation các tiêu chí tìm kiếm
                if (!string.IsNullOrWhiteSpace(name) && name.Trim().Length < 2)
                {
                    Console.WriteLine("Tên tìm kiếm phải có ít nhất 2 ký tự!");
                    return new DataTable();
                }

                return customerDAL.SearchCustomers(name, idCard, phone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CustomerBL.SearchCustomers: {ex.Message}");
                return new DataTable();
            }
        }
    }
}
