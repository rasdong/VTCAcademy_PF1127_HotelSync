using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem.InvoiceManagement
{
    public class InvoiceManager
    {
        private readonly string connectionString;

        public InvoiceManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Lấy danh sách hóa đơn
        public List<Invoice> GetAllInvoices()
        {
            var invoices = new List<Invoice>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Invoices ORDER BY IssueDate DESC";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        invoices.Add(new Invoice
                        {
                            InvoiceID = reader.GetInt32("InvoiceID"),
                            BookingID = reader.GetInt32("BookingID"),
                            CustomerID = reader.GetInt32("CustomerID"),
                            TotalAmount = reader.GetDecimal("TotalAmount"),
                            IssueDate = reader.GetDateTime("IssueDate"),
                            PaymentStatus = reader.GetString("PaymentStatus"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy"),
                            UpdatedByUsername = reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? null : reader.GetString("UpdatedByUsername")
                        });
                    }
                }
            }
            return invoices;
        }

        // Tạo hóa đơn mới (dùng stored procedure createDetailedInvoice)
        public bool CreateInvoice(int bookingId, int updatedBy, string updatedByUsername)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("createDetailedInvoice", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_BookingID", bookingId);
                    cmd.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
                    cmd.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // Tìm hóa đơn theo ID
        public Invoice? GetInvoiceById(int invoiceId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Invoices WHERE InvoiceID = @InvoiceID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Invoice
                            {
                                InvoiceID = reader.GetInt32("InvoiceID"),
                                BookingID = reader.GetInt32("BookingID"),
                                CustomerID = reader.GetInt32("CustomerID"),
                                TotalAmount = reader.GetDecimal("TotalAmount"),
                                IssueDate = reader.GetDateTime("IssueDate"),
                                PaymentStatus = reader.GetString("PaymentStatus"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.GetDateTime("UpdatedAt"),
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy"),
                                UpdatedByUsername = reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? null : reader.GetString("UpdatedByUsername")
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Cập nhật trạng thái thanh toán hóa đơn (dùng stored procedure updateInvoicePaymentStatus)
        public bool UpdatePaymentStatus(int invoiceId, string paymentStatus, int updatedBy, string updatedByUsername)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("updateInvoicePaymentStatus", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_InvoiceID", invoiceId);
                    cmd.Parameters.AddWithValue("@p_PaymentStatus", paymentStatus);
                    cmd.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
                    cmd.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // Tìm kiếm/lọc hóa đơn theo trạng thái/thông tin khách hàng
        public List<Invoice> SearchInvoices(string? paymentStatus = null, int? customerId = null)
        {
            var invoices = new List<Invoice>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Invoices WHERE 1=1";
                if (!string.IsNullOrEmpty(paymentStatus))
                    query += " AND PaymentStatus = @PaymentStatus";
                if (customerId.HasValue)
                    query += " AND CustomerID = @CustomerID";
                query += " ORDER BY IssueDate DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(paymentStatus))
                        cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                    if (customerId.HasValue)
                        cmd.Parameters.AddWithValue("@CustomerID", customerId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            invoices.Add(new Invoice
                            {
                                InvoiceID = reader.GetInt32("InvoiceID"),
                                BookingID = reader.GetInt32("BookingID"),
                                CustomerID = reader.GetInt32("CustomerID"),
                                TotalAmount = reader.GetDecimal("TotalAmount"),
                                IssueDate = reader.GetDateTime("IssueDate"),
                                PaymentStatus = reader.GetString("PaymentStatus"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.GetDateTime("UpdatedAt"),
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy"),
                                UpdatedByUsername = reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? null : reader.GetString("UpdatedByUsername")
                            });
                        }
                    }
                }
            }
            return invoices;
        }

        // Ghi nhận sử dụng dịch vụ cho booking (thêm vào ServiceUsage)
        public bool AddServiceUsage(int bookingId, int serviceId, int customerId, int quantity, DateTime date, int updatedBy, string updatedByUsername)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO ServiceUsage (BookingID, ServiceID, CustomerID, Quantity, Date, TotalPrice, PaymentStatus, UpdatedBy, UpdatedByUsername)
                                 VALUES (@BookingID, @ServiceID, @CustomerID, @Quantity, @Date, 0, 'Unpaid', @UpdatedBy, @UpdatedByUsername)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                    cmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Lấy danh sách dịch vụ đã sử dụng cho booking
        public List<(string ServiceName, string Type, int Quantity, decimal TotalPrice, string PaymentStatus)> GetServiceUsagesByBooking(int bookingId)
        {
            var result = new List<(string, string, int, decimal, string)>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT s.ServiceName, s.Type, su.Quantity, su.TotalPrice, su.PaymentStatus
                                 FROM ServiceUsage su
                                 JOIN Services s ON su.ServiceID = s.ServiceID
                                 WHERE su.BookingID = @BookingID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add((
                                reader.GetString("ServiceName"),
                                reader.GetString("Type"),
                                reader.GetInt32("Quantity"),
                                reader.GetDecimal("TotalPrice"),
                                reader.GetString("PaymentStatus")
                            ));
                        }
                    }
                }
            }
            return result;
        }

        // Xóa dịch vụ đã sử dụng khỏi booking
        public bool DeleteServiceUsage(int usageId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM ServiceUsage WHERE UsageID = @UsageID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsageID", usageId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
