using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class CustomerManagement
    {
        private readonly string _currentRole;
        private readonly int _currentUserId;
        private readonly string _currentUsername;

        public CustomerManagement(string role, int userId, string username)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || userId <= 0)
            {
                Console.WriteLine("Invalid role, user ID, or username provided.");
                return;
            }

            _currentRole = role;
            _currentUserId = userId;
            _currentUsername = username;
        }

        private void LogAction(string action)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand(
                "INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES (@userId, @action, @username)",
                connection);
            command.Parameters.AddWithValue("@userId", _currentUserId);
            command.Parameters.AddWithValue("@action", action);
            command.Parameters.AddWithValue("@username", _currentUsername);
            command.ExecuteNonQuery();
        }

        public void AddCustomer(int customerId, string name, string idCard, string phone, string email, string nationality)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                Console.WriteLine("Only Admin or Receptionist can add customers.");
                return;
            }
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Customer ID must be a positive integer consisting only of digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsLetter) || !name.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Name must contain letters with diacritics and may include spaces.");
                return;
            }
            if (string.IsNullOrWhiteSpace(idCard) || idCard.Length != 12 || !idCard.All(char.IsDigit))
            {
                Console.WriteLine("IDCard must be exactly 12 digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !phone.All(char.IsDigit))
            {
                Console.WriteLine("Phone number must be exactly 10 digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(nationality) || !nationality.Any(char.IsLetter) || !nationality.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Nationality must contain letters with diacritics and may include spaces.");
                return;
            }

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("addCustomer", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_CustomerID", customerId);
                command.Parameters.AddWithValue("@p_Name", name.Trim());
                command.Parameters.AddWithValue("@p_IDCard", idCard.Trim());
                command.Parameters.AddWithValue("@p_Phone", phone.Trim());
                command.Parameters.AddWithValue("@p_Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email.Trim());
                command.Parameters.AddWithValue("@p_Nationality", nationality.Trim());
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                command.ExecuteNonQuery();

                LogAction($"Added customer ID {customerId} ({name})");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
            }
        }

        public void UpdateCustomer(int customerId, string name, string idCard, string phone, string email, string nationality)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                Console.WriteLine("Only Admin or Receptionist can update customers.");
                return;
            }
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Customer ID must be a positive integer consisting only of digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsLetter) || !name.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Name must contain letters with diacritics and may include spaces.");
                return;
            }
            if (string.IsNullOrWhiteSpace(idCard) || idCard.Length != 12 || !idCard.All(char.IsDigit))
            {
                Console.WriteLine("IDCard must be exactly 12 digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !phone.All(char.IsDigit))
            {
                Console.WriteLine("Phone number must be exactly 10 digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(nationality) || !nationality.Any(char.IsLetter) || !nationality.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Nationality must contain letters with diacritics and may include spaces.");
                return;
            }

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("updateCustomer", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_CustomerID", customerId);
                command.Parameters.AddWithValue("@p_Name", name.Trim());
                command.Parameters.AddWithValue("@p_IDCard", idCard.Trim());
                command.Parameters.AddWithValue("@p_Phone", phone.Trim());
                command.Parameters.AddWithValue("@p_Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email.Trim());
                command.Parameters.AddWithValue("@p_Nationality", nationality.Trim());
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    Console.WriteLine($"Customer ID {customerId} not found.");
                    return;
                }

                LogAction($"Updated customer ID {customerId} ({name})");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
            }
        }

        public void DeleteCustomer(int customerId)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                Console.WriteLine("Only Admin or Receptionist can delete customers.");
                return;
            }
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Customer ID must be a positive integer consisting only of digits.");
                return;
            }

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("deleteCustomer", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_CustomerID", customerId);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    Console.WriteLine($"Customer ID {customerId} not found.");
                    return;
                }

                LogAction($"Deleted customer ID {customerId}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
            }
        }

        public List<Dictionary<string, string>> SearchCustomer(int customerId)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                Console.WriteLine("Only Admin or Receptionist can search customers.");
                return new List<Dictionary<string, string>>();
            }
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Customer ID must be a positive integer consisting only of digits.");
                return new List<Dictionary<string, string>>();
            }

            var results = new List<Dictionary<string, string>>();
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("searchCustomer", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_CustomerID", customerId);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);

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

                LogAction($"Searched for customer ID {customerId}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error searching customer: {ex.Message}");
            }

            return results;
        }

        public List<Dictionary<string, string>> GetBookingHistory(int customerId)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                Console.WriteLine("Only Admin or Receptionist can view booking history.");
                return new List<Dictionary<string, string>>();
            }
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Customer ID must be a positive integer consisting only of digits.");
                return new List<Dictionary<string, string>>();
            }

            var history = new List<Dictionary<string, string>>();
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("getCustomerBookingHistory", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_CustomerID", customerId);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);

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

                LogAction($"Viewed booking history for customer ID {customerId}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error retrieving booking history: {ex.Message}");
            }

            return history;
        }
    }

    public static class StringExtensions
    {
        public static bool In(this string str, params string[] values)
        {
            return values.Contains(str);
        }
    }
}
