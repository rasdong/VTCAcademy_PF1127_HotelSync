using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace HotelManagementSystem
{
    public class ServiceDAL
    {
        // 1. Thêm dịch vụ mới
        public void AddService(string serviceName, string type, decimal price, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Gọi stored procedure addService
                MySqlCommand cmd = new MySqlCommand("addService", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_ServiceName", serviceName);
                cmd.Parameters.AddWithValue("p_Type", type);
                cmd.Parameters.AddWithValue("p_Price", price);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Thêm dịch vụ mới: {serviceName}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi thêm dịch vụ: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 2. Cập nhật thông tin dịch vụ
        public void UpdateService(int serviceId, string serviceName, string type, decimal price, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra dịch vụ tồn tại
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Services WHERE ServiceID = @ServiceID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@ServiceID", serviceId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Dịch vụ không tồn tại.");

                // Gọi stored procedure updateService
                MySqlCommand cmd = new MySqlCommand("updateService", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_ServiceID", serviceId);
                cmd.Parameters.AddWithValue("p_ServiceName", serviceName);
                cmd.Parameters.AddWithValue("p_Type", type);
                cmd.Parameters.AddWithValue("p_Price", price);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Cập nhật dịch vụ ID {serviceId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi cập nhật dịch vụ: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 3. Xóa dịch vụ
        public void DeleteService(int serviceId, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Kiểm tra dịch vụ tồn tại
                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Services WHERE ServiceID = @ServiceID", conn)
                {
                    Transaction = transaction
                };
                checkCmd.Parameters.AddWithValue("@ServiceID", serviceId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                    throw new Exception("Dịch vụ không tồn tại.");

                // Kiểm tra dịch vụ có đang được sử dụng không
                MySqlCommand usageCheckCmd = new MySqlCommand("SELECT COUNT(*) FROM ServiceUsage WHERE ServiceID = @ServiceID", conn)
                {
                    Transaction = transaction
                };
                usageCheckCmd.Parameters.AddWithValue("@ServiceID", serviceId);
                int usageCount = Convert.ToInt32(usageCheckCmd.ExecuteScalar());
                if (usageCount > 0)
                    throw new Exception("Không thể xóa dịch vụ vì đã có khách hàng sử dụng.");

                // Gọi stored procedure deleteService
                MySqlCommand cmd = new MySqlCommand("deleteService", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_ServiceID", serviceId);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Xóa dịch vụ ID {serviceId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi xóa dịch vụ: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 4. Xem danh sách tất cả dịch vụ
        public DataTable GetAllServices()
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("getAllServices", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dịch vụ: {ex.Message}");
            }
        }

        // 5. Tìm kiếm dịch vụ theo loại
        public DataTable SearchServicesByType(string type)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT ServiceID, ServiceName, Type, Price, CreatedAt, UpdatedAt FROM Services WHERE Type = @Type", conn);
                    cmd.Parameters.AddWithValue("@Type", type);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm dịch vụ: {ex.Message}");
            }
        }

        // 6. Ghi nhận yêu cầu dịch vụ (thêm dịch vụ vào đặt phòng)
        public void AddServiceUsage(int bookingId, int serviceId, int customerId, int quantity, DateTime date, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // Lấy giá dịch vụ
                MySqlCommand priceCmd = new MySqlCommand("SELECT Price FROM Services WHERE ServiceID = @ServiceID", conn)
                {
                    Transaction = transaction
                };
                priceCmd.Parameters.AddWithValue("@ServiceID", serviceId);
                object priceResult = priceCmd.ExecuteScalar();
                if (priceResult == null)
                    throw new Exception("Dịch vụ không tồn tại.");

                decimal unitPrice = Convert.ToDecimal(priceResult);
                decimal totalPrice = unitPrice * quantity;

                // Gọi stored procedure addServiceUsage
                MySqlCommand cmd = new MySqlCommand("addServiceUsage", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("p_BookingID", bookingId);
                cmd.Parameters.AddWithValue("p_ServiceID", serviceId);
                cmd.Parameters.AddWithValue("p_CustomerID", customerId);
                cmd.Parameters.AddWithValue("p_Quantity", quantity);
                cmd.Parameters.AddWithValue("p_Date", date);
                cmd.Parameters.AddWithValue("p_TotalPrice", totalPrice);
                cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("p_UpdatedByUsername", updatedByUsername);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Thêm dịch vụ ID {serviceId} vào booking ID {bookingId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi ghi nhận yêu cầu dịch vụ: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // 7. Xem danh sách dịch vụ theo booking
        public DataTable GetServiceUsageByBooking(int bookingId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT su.UsageID, s.ServiceName, s.Type, su.Quantity, s.Price as UnitPrice, 
                               su.TotalPrice, su.Date, su.PaymentStatus
                        FROM ServiceUsage su
                        JOIN Services s ON su.ServiceID = s.ServiceID
                        WHERE su.BookingID = @BookingID", conn);
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách dịch vụ theo booking: {ex.Message}");
            }
        }

        // 8. Tính tổng phí dịch vụ cho một booking
        public decimal GetTotalServiceCostByBooking(int bookingId)
        {
            try
            {
                using (MySqlConnection conn = DataHelper.Instance.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT COALESCE(SUM(TotalPrice), 0) FROM ServiceUsage WHERE BookingID = @BookingID", conn);
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi tính phí dịch vụ: {ex.Message}");
            }
        }

        // 9. Cập nhật trạng thái thanh toán dịch vụ
        public void UpdateServicePaymentStatus(int usageId, string paymentStatus, int updatedBy, string updatedByUsername)
        {
            MySqlConnection conn = DataHelper.Instance.GetConnection();
            MySqlTransaction? transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                MySqlCommand cmd = new MySqlCommand("UPDATE ServiceUsage SET PaymentStatus = @PaymentStatus, UpdatedAt = CURRENT_TIMESTAMP WHERE UsageID = @UsageID", conn)
                {
                    Transaction = transaction
                };
                cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                cmd.Parameters.AddWithValue("@UsageID", usageId);
                cmd.ExecuteNonQuery();

                // Ghi log
                MySqlCommand logCmd = new MySqlCommand("INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@UserID, @Action, @UpdatedByUsername)", conn)
                {
                    Transaction = transaction
                };
                logCmd.Parameters.AddWithValue("@UserID", updatedBy);
                logCmd.Parameters.AddWithValue("@Action", $"Cập nhật trạng thái thanh toán dịch vụ ID {usageId}");
                logCmd.Parameters.AddWithValue("@UpdatedByUsername", updatedByUsername);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (MySqlException ex)
            {
                transaction?.Rollback();
                throw new Exception($"Lỗi khi cập nhật trạng thái thanh toán: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
