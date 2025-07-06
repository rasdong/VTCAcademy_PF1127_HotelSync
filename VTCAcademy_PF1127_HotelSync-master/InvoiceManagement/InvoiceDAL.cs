using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace HotelManagementSystem.InvoiceManagement
{
    public class InvoiceDAL
    {
        #region Add Invoice
        public bool AddInvoice(int bookingId, int customerId, decimal totalAmount, int updatedBy, string updatedByUsername)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("createInvoice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_BookingID", bookingId);
                        command.Parameters.AddWithValue("@p_CustomerID", customerId);
                        command.Parameters.AddWithValue("@p_TotalAmount", totalAmount);
                        command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
                        command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
                        
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm hóa đơn: {ex.Message}");
                return false;
            }
        }

        public int CreateInvoiceWithCalculation(int bookingId, int updatedBy, string updatedByUsername)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("createInvoiceWithCalculation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_BookingID", bookingId);
                        command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
                        command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
                        
                        var outputParam = new MySqlParameter("@p_InvoiceID", MySqlDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        
                        command.ExecuteNonQuery();
                        return Convert.ToInt32(outputParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo hóa đơn với tính toán: {ex.Message}");
                return -1;
            }
        }
        #endregion

        #region Update Invoice
        public bool UpdateInvoicePaymentStatus(int invoiceId, string paymentStatus, int updatedBy, string updatedByUsername)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("updateInvoicePaymentStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_InvoiceID", invoiceId);
                        command.Parameters.AddWithValue("@p_PaymentStatus", paymentStatus);
                        command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
                        command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
                        
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật trạng thái thanh toán: {ex.Message}");
                return false;
            }
        }

        public decimal CalculateInvoiceAmount(int bookingId)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("calculateInvoiceAmount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_BookingID", bookingId);
                        
                        var outputParam = new MySqlParameter("@p_TotalAmount", MySqlDbType.Decimal)
                        {
                            Direction = ParameterDirection.Output,
                            Precision = 10,
                            Scale = 2
                        };
                        command.Parameters.Add(outputParam);
                        
                        command.ExecuteNonQuery();
                        return Convert.ToDecimal(outputParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tính toán số tiền hóa đơn: {ex.Message}");
                return 0;
            }
        }
        #endregion

        #region Get Invoice Data
        public DataTable GetAllInvoices(string? customerFilter = null, DateTime? fromDate = null, DateTime? toDate = null, string? statusFilter = null)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InvoiceID, i.BookingID, c.Name as CustomerName, 
                               i.TotalAmount, i.IssueDate, i.PaymentStatus, r.RoomNumber
                        FROM Invoices i
                        JOIN Customers c ON i.CustomerID = c.CustomerID
                        JOIN Bookings b ON i.BookingID = b.BookingID
                        JOIN Rooms r ON b.RoomID = r.RoomID
                        WHERE 1=1";

                    // Thêm điều kiện lọc
                    if (!string.IsNullOrEmpty(customerFilter))
                        query += " AND c.Name LIKE @customerFilter";
                    if (fromDate.HasValue)
                        query += " AND DATE(i.IssueDate) >= @fromDate";
                    if (toDate.HasValue)
                        query += " AND DATE(i.IssueDate) <= @toDate";
                    if (!string.IsNullOrEmpty(statusFilter))
                        query += " AND i.PaymentStatus = @statusFilter";

                    query += " ORDER BY i.IssueDate DESC";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(customerFilter))
                            command.Parameters.AddWithValue("@customerFilter", "%" + customerFilter + "%");
                        if (fromDate.HasValue)
                            command.Parameters.AddWithValue("@fromDate", fromDate.Value.Date);
                        if (toDate.HasValue)
                            command.Parameters.AddWithValue("@toDate", toDate.Value.Date);
                        if (!string.IsNullOrEmpty(statusFilter))
                            command.Parameters.AddWithValue("@statusFilter", statusFilter);

                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }

        // Overload method để tương thích với code cũ
        public DataTable GetAllInvoices()
        {
            return GetAllInvoices(null, null, null, null);
        }

        public DataTable GetInvoiceById(int invoiceId)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InvoiceID, i.BookingID, c.Name as CustomerName, c.IDCard, c.Email, c.Phone,
                               r.RoomNumber, r.RoomType, r.Price as RoomPrice, i.TotalAmount, i.IssueDate, i.PaymentStatus,
                               b.CheckInDate, b.CheckOutDate
                        FROM Invoices i
                        JOIN Customers c ON i.CustomerID = c.CustomerID
                        JOIN Bookings b ON i.BookingID = b.BookingID
                        JOIN Rooms r ON b.RoomID = r.RoomID
                        WHERE i.InvoiceID = @invoiceId";
                    
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@invoiceId", invoiceId);
                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable GetServiceUsageByInvoice(int invoiceId)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT s.ServiceName, s.Type, su.Quantity, s.Price as UnitPrice, su.TotalPrice, su.Date
                        FROM ServiceUsage su
                        JOIN Services s ON su.ServiceID = s.ServiceID
                        JOIN Invoices i ON su.BookingID = i.BookingID
                        WHERE i.InvoiceID = @invoiceId
                        ORDER BY su.Date DESC";
                    
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@invoiceId", invoiceId);
                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin dịch vụ của hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable SearchInvoices(string? invoiceId = null, string? customerName = null, DateTime? fromDate = null, DateTime? toDate = null, string? paymentStatus = null)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InvoiceID, i.BookingID, c.Name as CustomerName, 
                               i.TotalAmount, i.IssueDate, i.PaymentStatus
                        FROM Invoices i
                        JOIN Customers c ON i.CustomerID = c.CustomerID
                        JOIN Bookings b ON i.BookingID = b.BookingID
                        WHERE 1=1";

                    // Thêm điều kiện tìm kiếm
                    if (!string.IsNullOrEmpty(invoiceId))
                        query += " AND i.InvoiceID = @invoiceId";
                    if (!string.IsNullOrEmpty(customerName))
                        query += " AND c.Name LIKE @customerName";
                    if (fromDate.HasValue)
                        query += " AND DATE(i.IssueDate) >= @fromDate";
                    if (toDate.HasValue)
                        query += " AND DATE(i.IssueDate) <= @toDate";
                    if (!string.IsNullOrEmpty(paymentStatus))
                        query += " AND i.PaymentStatus = @paymentStatus";

                    query += " ORDER BY i.IssueDate DESC";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(invoiceId))
                        {
                            if (int.TryParse(invoiceId, out int id))
                                command.Parameters.AddWithValue("@invoiceId", id);
                        }
                        if (!string.IsNullOrEmpty(customerName))
                            command.Parameters.AddWithValue("@customerName", "%" + customerName + "%");
                        if (fromDate.HasValue)
                            command.Parameters.AddWithValue("@fromDate", fromDate.Value.Date);
                        if (toDate.HasValue)
                            command.Parameters.AddWithValue("@toDate", toDate.Value.Date);
                        if (!string.IsNullOrEmpty(paymentStatus))
                            command.Parameters.AddWithValue("@paymentStatus", paymentStatus);

                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }

        // Legacy search method để tương thích với code cũ
        public DataTable SearchInvoices(string? customerName = null, DateTime? fromDate = null, DateTime? toDate = null, 
                                       string? paymentStatus = null, decimal? minAmount = null, decimal? maxAmount = null)
        {
            return SearchInvoices(null, customerName, fromDate, toDate, paymentStatus);
        }
        #endregion

        #region Print Invoice
        public DataTable GetInvoiceForPrint(int invoiceId)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InvoiceID, i.BookingID, c.Name as CustomerName, c.IDCard, c.Email, c.Phone,
                               r.RoomNumber, r.RoomType, r.Price as RoomPrice, i.TotalAmount, i.IssueDate, i.PaymentStatus,
                               b.CheckInDate, b.CheckOutDate
                        FROM Invoices i
                        JOIN Customers c ON i.CustomerID = c.CustomerID
                        JOIN Bookings b ON i.BookingID = b.BookingID
                        JOIN Rooms r ON b.RoomID = r.RoomID
                        WHERE i.InvoiceID = @invoiceId";
                    
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@invoiceId", invoiceId);
                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin in hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }
        #endregion

        #region Delete Invoice (Admin only)
        public bool DeleteInvoice(int invoiceId, int updatedBy, string updatedByUsername)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Kiểm tra xem hóa đơn có tồn tại không
                            string checkQuery = "SELECT COUNT(*) FROM Invoices WHERE InvoiceID = @invoiceId";
                            using (var checkCommand = new MySqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@invoiceId", invoiceId);
                                long count = (long)checkCommand.ExecuteScalar();
                                if (count == 0)
                                {
                                    transaction.Rollback();
                                    Console.WriteLine("Hóa đơn không tồn tại!");
                                    return false;
                                }
                            }

                            // Xóa hóa đơn
                            string deleteQuery = "DELETE FROM Invoices WHERE InvoiceID = @invoiceId";
                            using (var deleteCommand = new MySqlCommand(deleteQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@invoiceId", invoiceId);
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Ghi log
                            string logQuery = "INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@userId, @action, @username)";
                            using (var logCommand = new MySqlCommand(logQuery, connection, transaction))
                            {
                                logCommand.Parameters.AddWithValue("@userId", updatedBy);
                                logCommand.Parameters.AddWithValue("@action", $"Xóa hóa đơn ID {invoiceId}");
                                logCommand.Parameters.AddWithValue("@username", updatedByUsername);
                                logCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa hóa đơn: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
