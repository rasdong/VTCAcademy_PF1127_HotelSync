using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class CustomerDAL
    {
        public bool CheckCustomerExists(int customerId, string idCard)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("SELECT COUNT(*) FROM Customers WHERE CustomerID = @id OR IDCard = @idCard", connection);
            command.Parameters.AddWithValue("@id", customerId);
            command.Parameters.AddWithValue("@idCard", idCard);
            return (long)command.ExecuteScalar() > 0;
        }

        public void AddCustomer(int customerId, string name, string idCard, string phone, string email, string nationality, int updatedBy, string updatedByUsername)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("addCustomerWithTransaction", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_CustomerID", customerId);
            command.Parameters.AddWithValue("@p_Name", name);
            command.Parameters.AddWithValue("@p_IDCard", idCard);
            command.Parameters.AddWithValue("@p_Phone", phone);
            command.Parameters.AddWithValue("@p_Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email);
            command.Parameters.AddWithValue("@p_Nationality", nationality);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
            command.ExecuteNonQuery();
        }

        public int UpdateCustomer(int customerId, string name, string idCard, string phone, string email, string nationality, int updatedBy, string updatedByUsername)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("updateCustomerWithTransaction", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_CustomerID", customerId);
            command.Parameters.AddWithValue("@p_Name", name);
            command.Parameters.AddWithValue("@p_IDCard", idCard);
            command.Parameters.AddWithValue("@p_Phone", phone);
            command.Parameters.AddWithValue("@p_Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email);
            command.Parameters.AddWithValue("@p_Nationality", nationality);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
            return command.ExecuteNonQuery();
        }

        public int DeleteCustomer(int customerId, int updatedBy, string updatedByUsername)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("deleteCustomerWithTransaction", connection)
            {
                CommandType = CommandType.StoredProcedure
            }; 
            command.Parameters.AddWithValue("@p_CustomerID", customerId);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);

            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }


        public List<Dictionary<string, string>> SearchCustomer(int customerId, int updatedBy, string updatedByUsername)
        {
            var results = new List<Dictionary<string, string>>();
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("searchCustomer", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_CustomerID", customerId);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var customer = new Dictionary<string, string>
                {
                    { "CustomerID", reader["CustomerID"].ToString() },
                    { "Name", reader["Name"].ToString() },
                    { "IDCard", reader["IDCard"].ToString() },
                    { "Phone", reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader["Phone"].ToString() },
                    { "Email", reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader["Email"].ToString() },
                    { "Nationality", reader["Nationality"].ToString() }
                };
                results.Add(customer);
            }
            return results;
        }

        public List<Dictionary<string, string>> GetBookingHistory(int customerId, int updatedBy, string updatedByUsername)
        {
            var history = new List<Dictionary<string, string>>();
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("getCustomerBookingHistory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_CustomerID", customerId);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var booking = new Dictionary<string, string>
                {
                    { "BookingID", reader["BookingID"].ToString() },
                    { "CheckInDate", reader["CheckInDate"].ToString() },
                    { "CheckOutDate", reader["CheckOutDate"].ToString() },
                    { "Status", reader["Status"].ToString() },
                    { "RoomNumber", reader.IsDBNull(reader.GetOrdinal("RoomNumber")) ? "" : reader["RoomNumber"].ToString() },
                    { "InvoiceID", reader.IsDBNull(reader.GetOrdinal("InvoiceID")) ? "" : reader["InvoiceID"].ToString() },
                    { "TotalAmount", reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? "" : reader["TotalAmount"].ToString() },
                    { "PaymentStatus", reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? "" : reader["PaymentStatus"].ToString() }
                };
                history.Add(booking);
            }
            return history;
        }

        public void LogAction(int userId, string action, string username)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand(
                "INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@userId, @action, @username)",
                connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@action", action);
            command.Parameters.AddWithValue("@username", username);
            command.ExecuteNonQuery();
        }
    }
}
