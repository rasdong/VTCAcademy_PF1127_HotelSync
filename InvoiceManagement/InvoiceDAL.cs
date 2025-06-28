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

        public int CreateInvoiceWithCalculation(int bookingId, int customerId, int updatedBy, string updatedByUsername)
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
                        command.Parameters.AddWithValue("@p_CustomerID", customerId);
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
        public DataTable GetAllInvoices()
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("getAllInvoices", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
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

        public DataTable GetInvoiceById(int invoiceId)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InvoiceID, i.BookingID, c.Name as CustomerName, c.IDCard, 
                               r.RoomNumber, r.RoomType, i.TotalAmount, i.IssueDate, i.PaymentStatus,
                               b.CheckInDate, b.CheckOutDate, r.Price as RoomPrice
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
                        SELECT s.ServiceName, su.Quantity, s.Price as UnitPrice, su.TotalPrice, su.Date
                        FROM ServiceUsage su
                        JOIN Services s ON su.ServiceID = s.ServiceID
                        JOIN Invoices i ON su.BookingID = i.BookingID
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
                Console.WriteLine($"Lỗi khi lấy thông tin dịch vụ của hóa đơn: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable SearchInvoices(string? customerName = null, DateTime? fromDate = null, DateTime? toDate = null, 
                                       string? paymentStatus = null, decimal? minAmount = null, decimal? maxAmount = null)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("searchInvoices", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_CustomerName", customerName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_FromDate", fromDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_ToDate", toDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_PaymentStatus", paymentStatus ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_MinAmount", minAmount ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_MaxAmount", maxAmount ?? (object)DBNull.Value);
                        
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
        #endregion

        #region Print Invoice
        public DataTable GetInvoiceForPrint(int invoiceId)
        {
            try
            {
                using (var connection = DataHelper.Instance.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand("printInvoice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_InvoiceID", invoiceId);
                        
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
