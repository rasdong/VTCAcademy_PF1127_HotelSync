using MySql.Data.MySqlClient;
using System.Data;

namespace VTCAcademy_PF1127_HotelSync.CustomerManagement
{
    /// <summary>
    /// Data Access Layer cho module quản lý khách hàng
    /// Thực hiện các thao tác CRUD với database MySQL
    /// </summary>
    public class CustomerDAL
    {
        private readonly string connectionString;

        public CustomerDAL()
        {
            // Connection string cho MySQL
            connectionString = "server=localhost;uid=root;pwd=123321;database=Hotel_management;";
        }

        /// <summary>
        /// Thêm khách hàng mới vào database
        /// </summary>
        /// <param name="name">Tên khách hàng</param>
        /// <param name="idCard">CMND/Passport</param>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="email">Email</param>
        /// <param name="nationality">Quốc tịch</param>
        /// <param name="updatedBy">ID người tạo</param>
        /// <param name="updatedByUsername">Username người tạo</param>
        /// <returns>ID của khách hàng mới được tạo, -1 nếu lỗi</returns>
        public int AddCustomer(string name, string idCard, string phone, string email, string nationality, int? updatedBy, string updatedByUsername)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) 
                                   VALUES (@Name, @IDCard, @Phone, @Email, @Nationality, @UpdatedBy, @UpdatedByUsername);
                                   SELECT LAST_INSERT_ID();";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name?.Trim());
                        command.Parameters.AddWithValue("@IDCard", idCard?.Trim());
                        command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(phone) ? DBNull.Value : phone.Trim());
                        command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email.Trim());
                        command.Parameters.AddWithValue("@Nationality", string.IsNullOrEmpty(nationality) ? "Vietnam" : nationality.Trim());
                        command.Parameters.AddWithValue("@UpdatedBy", updatedBy ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername ?? (object)DBNull.Value);
                        
                        object result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Lỗi MySQL khi thêm khách hàng: {ex.Message}");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm khách hàng: {ex.Message}");
                return -1;
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
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        public bool UpdateCustomer(int customerId, string name, string phone, string email, string nationality, int? updatedBy, string updatedByUsername)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"UPDATE Customers 
                                   SET Name = @Name, Phone = @Phone, Email = @Email, 
                                       Nationality = @Nationality, UpdatedBy = @UpdatedBy, 
                                       UpdatedByUsername = @UpdatedByUsername, UpdatedAt = CURRENT_TIMESTAMP
                                   WHERE CustomerID = @CustomerID";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        command.Parameters.AddWithValue("@Name", name?.Trim());
                        command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(phone) ? DBNull.Value : phone.Trim());
                        command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email.Trim());
                        command.Parameters.AddWithValue("@Nationality", string.IsNullOrEmpty(nationality) ? "Vietnam" : nationality.Trim());
                        command.Parameters.AddWithValue("@UpdatedBy", updatedBy ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername ?? (object)DBNull.Value);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Lỗi MySQL khi cập nhật khách hàng: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật khách hàng: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa khách hàng (chỉ khi không có booking/hóa đơn liên quan)
        /// </summary>
        /// <param name="customerId">ID khách hàng cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        public bool DeleteCustomer(int customerId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Kiểm tra xem khách hàng có booking hay hóa đơn nào không
                    string checkQuery = @"SELECT COUNT(*) FROM 
                                        (SELECT CustomerID FROM Bookings WHERE CustomerID = @CustomerID
                                         UNION ALL
                                         SELECT CustomerID FROM Invoices WHERE CustomerID = @CustomerID) AS related";
                    
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        int relatedCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                        
                        if (relatedCount > 0)
                        {
                            Console.WriteLine("Không thể xóa khách hàng vì có booking hoặc hóa đơn liên quan.");
                            return false;
                        }
                    }
                    
                    // Xóa khách hàng nếu không có ràng buộc
                    string deleteQuery = "DELETE FROM Customers WHERE CustomerID = @CustomerID";
                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Lỗi MySQL khi xóa khách hàng: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa khách hàng: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tìm khách hàng theo ID
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <returns>DataRow chứa thông tin khách hàng, null nếu không tìm thấy</returns>
        public DataRow? GetCustomerById(int customerId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, 
                                           CreatedAt, UpdatedAt, UpdatedBy, UpdatedByUsername
                                   FROM Customers 
                                   WHERE CustomerID = @CustomerID";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            
                            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm khách hàng theo ID: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tìm khách hàng theo CMND/Passport
        /// </summary>
        /// <param name="idCard">CMND/Passport</param>
        /// <returns>DataRow chứa thông tin khách hàng, null nếu không tìm thấy</returns>
        public DataRow? GetCustomerByIdCard(string idCard)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, 
                                           CreatedAt, UpdatedAt, UpdatedBy, UpdatedByUsername
                                   FROM Customers 
                                   WHERE IDCard = @IDCard";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDCard", idCard?.Trim());
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            
                            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm khách hàng theo CMND: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo tên (tìm kiếm mờ)
        /// </summary>
        /// <param name="name">Tên khách hàng (có thể là một phần)</param>
        /// <returns>DataTable chứa danh sách khách hàng phù hợp</returns>
        public DataTable SearchCustomersByName(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, 
                                           CreatedAt, UpdatedAt, UpdatedBy, UpdatedByUsername
                                   FROM Customers 
                                   WHERE Name LIKE @Name
                                   ORDER BY Name";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", $"%{name?.Trim()}%");
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm khách hàng theo tên: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo số điện thoại
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <returns>DataTable chứa danh sách khách hàng phù hợp</returns>
        public DataTable SearchCustomersByPhone(string phone)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, 
                                           CreatedAt, UpdatedAt, UpdatedBy, UpdatedByUsername
                                   FROM Customers 
                                   WHERE Phone LIKE @Phone
                                   ORDER BY Name";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Phone", $"%{phone?.Trim()}%");
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm khách hàng theo SĐT: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        /// <returns>DataTable chứa danh sách tất cả khách hàng</returns>
        public DataTable GetAllCustomers()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, 
                                           CreatedAt, UpdatedAt, UpdatedBy, UpdatedByUsername
                                   FROM Customers 
                                   ORDER BY Name";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách khách hàng: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy lịch sử booking của khách hàng
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <returns>DataTable chứa lịch sử booking</returns>
        public DataTable GetCustomerBookingHistory(int customerId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT b.BookingID, b.CheckInDate, b.CheckOutDate, b.Status,
                                           r.RoomNumber, r.RoomType, r.Price,
                                           b.TotalAmount, b.CreatedAt
                                   FROM Bookings b
                                   INNER JOIN Rooms r ON b.RoomID = r.RoomID
                                   WHERE b.CustomerID = @CustomerID
                                   ORDER BY b.CreatedAt DESC";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy lịch sử booking: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Lấy lịch sử hóa đơn của khách hàng
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <returns>DataTable chứa lịch sử hóa đơn</returns>
        public DataTable GetCustomerInvoiceHistory(int customerId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = @"SELECT i.InvoiceID, i.BookingID, i.TotalAmount, i.IsPaid, 
                                           i.PaymentDate, i.CreatedAt,
                                           b.CheckInDate, b.CheckOutDate,
                                           r.RoomNumber, r.RoomType
                                   FROM Invoices i
                                   INNER JOIN Bookings b ON i.BookingID = b.BookingID
                                   INNER JOIN Rooms r ON b.RoomID = r.RoomID
                                   WHERE i.CustomerID = @CustomerID
                                   ORDER BY i.CreatedAt DESC";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy lịch sử hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Kiểm tra xem CMND/Passport đã tồn tại chưa
        /// </summary>
        /// <param name="idCard">CMND/Passport</param>
        /// <param name="excludeCustomerId">ID khách hàng cần loại trừ (dùng khi cập nhật)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        public bool IsIdCardExists(string idCard, int? excludeCustomerId = null)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string query = "SELECT COUNT(*) FROM Customers WHERE IDCard = @IDCard";
                    if (excludeCustomerId.HasValue)
                    {
                        query += " AND CustomerID != @ExcludeCustomerID";
                    }
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDCard", idCard?.Trim());
                        if (excludeCustomerId.HasValue)
                        {
                            command.Parameters.AddWithValue("@ExcludeCustomerID", excludeCustomerId.Value);
                        }
                        
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra CMND tồn tại: {ex.Message}");
                return false;
            }
        }
    }
}
