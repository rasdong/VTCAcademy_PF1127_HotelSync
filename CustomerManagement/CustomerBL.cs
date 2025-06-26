using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagementSystem
{
    public class CustomerBL
    {
        private readonly string _currentRole;
        private readonly int _currentUserId;
        private readonly string _currentUsername;
        private readonly CustomerDAL _customerDAL;

        public CustomerBL(string role, int userId, string username)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || userId <= 0)
            {
                throw new ArgumentException("Invalid role, user ID, or username provided.");
            }

            _currentRole = role;
            _currentUserId = userId;
            _currentUsername = username;
            _customerDAL = new CustomerDAL();
        }

        private bool ValidateCustomerData(int customerId, string name, string idCard, string phone, string email, string nationality, out string errorMessage)
        {
            errorMessage = "";
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
                errorMessage = "Customer ID must be a positive integer consisting only of digits.";
            else if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsLetter) || !name.Any(c => c == ' ' || char.IsLetter(c)))
                errorMessage = "Name must contain letters with diacritics and may include spaces.";
            else if (string.IsNullOrWhiteSpace(idCard) || idCard.Length != 12 || !idCard.All(char.IsDigit))
                errorMessage = "IDCard must be exactly 12 digits.";
            else if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !phone.All(char.IsDigit))
                errorMessage = "Phone number must be exactly 10 digits.";
            else if (!string.IsNullOrWhiteSpace(email) && (!email.Contains("@") || !email.Contains(".")))
                errorMessage = "Email must be valid or empty.";
            else if (string.IsNullOrWhiteSpace(nationality) || !nationality.Any(char.IsLetter) || !nationality.Any(c => c == ' ' || char.IsLetter(c)))
                errorMessage = "Nationality must contain letters with diacritics and may include spaces.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void LogAction(string action)
        {
            try
            {
                _customerDAL.LogAction(_currentUserId, action, _currentUsername);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging action: {ex.Message}");
            }
        }

        public void AddCustomer(int customerId, string name, string idCard, string phone, string email, string nationality)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                throw new UnauthorizedAccessException("Only Admin or Receptionist can add customers.");
            }

            if (!ValidateCustomerData(customerId, name, idCard, phone, email, nationality, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            try
            {
                bool exists = _customerDAL.CheckCustomerExists(customerId, idCard);
                if (exists)
                {
                    throw new InvalidOperationException("Customer ID or IDCard already exists.");
                }

                _customerDAL.AddCustomer(customerId, name.Trim(), idCard.Trim(), phone.Trim(), email?.Trim(), nationality.Trim(), _currentUserId, _currentUsername);
                LogAction($"Added customer ID {customerId} ({name})");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding customer: {ex.Message}", ex);
            }
        }

        public void UpdateCustomer(int customerId, string name, string idCard, string phone, string email, string nationality)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                throw new UnauthorizedAccessException("Only Admin or Receptionist can update customers.");
            }

            if (!ValidateCustomerData(customerId, name, idCard, phone, email, nationality, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            try
            {
                int rowsAffected = _customerDAL.UpdateCustomer(customerId, name.Trim(), idCard.Trim(), phone.Trim(), email?.Trim(), nationality.Trim(), _currentUserId, _currentUsername);
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Customer ID {customerId} not found.");
                }

                LogAction($"Updated customer ID {customerId} ({name})");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating customer: {ex.Message}", ex);
            }
        }

        public void DeleteCustomer(int customerId)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                throw new UnauthorizedAccessException("Only Admin or Receptionist can delete customers.");
            }

            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                throw new ArgumentException("Customer ID must be a positive integer consisting only of digits.");
            }

            try
            {
                int rowsAffected = _customerDAL.DeleteCustomer(customerId, _currentUserId, _currentUsername);
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Customer ID {customerId} not found.");
                }

                LogAction($"Deleted customer ID {customerId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting customer: {ex.Message}", ex);
            }
        }

        public List<Dictionary<string, string>> SearchCustomer(int customerId)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                throw new UnauthorizedAccessException("Only Admin or Receptionist can search customers.");
            }

            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                throw new ArgumentException("Customer ID must be a positive integer consisting only of digits.");
            }

            try
            {
                var results = _customerDAL.SearchCustomer(customerId, _currentUserId, _currentUsername);
                LogAction($"Searched for customer ID {customerId}");
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching customer: {ex.Message}", ex);
            }
        }

        public List<Dictionary<string, string>> GetBookingHistory(int customerId)
        {
            if (!_currentRole.In("Admin", "Receptionist"))
            {
                throw new UnauthorizedAccessException("Only Admin or Receptionist can view booking history.");
            }

            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                throw new ArgumentException("Customer ID must be a positive integer consisting only of digits.");
            }

            try
            {
                var history = _customerDAL.GetBookingHistory(customerId, _currentUserId, _currentUsername);
                LogAction($"Viewed booking history for customer ID {customerId}");
                return history;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving booking history: {ex.Message}", ex);
            }
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
