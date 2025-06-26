using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HotelManagementSystem
{
    public class StaffBL
    {
        private readonly string _currentRole;
        private readonly int _currentUserId;
        private readonly string _currentUsername;
        private readonly StaffDAL _staffDAL;

        public StaffBL(string role, int userId, string username)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || userId <= 0)
            {
                throw new ArgumentException("Invalid role, user ID, or username provided.");
            }

            _currentRole = role;
            _currentUserId = userId;
            _currentUsername = username;
            _staffDAL = new StaffDAL();
        }

        private bool ValidateStaffData(int staffId, string name, string role, out string errorMessage)
        {
            errorMessage = "";
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
                errorMessage = "Staff ID must be a positive integer consisting only of digits.";
            else if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, @"^[a-zA-Z\sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵ]+$"))
                errorMessage = "Name must contain letters with diacritics and may include spaces.";
            else if (string.IsNullOrWhiteSpace(role) || !new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(role, StringComparer.OrdinalIgnoreCase))
                errorMessage = "Role must be Receptionist, Housekeeping, or Manager.";
            return string.IsNullOrEmpty(errorMessage);
        }

        private void LogAction(string action)
        {
            try
            {
                _staffDAL.LogAction(_currentUserId, action, _currentUsername);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error logging action: {ex.Message}", ex);
            }
        }

        public void AddStaff(int staffId, string name, string role)
        {
            if (!_currentRole.In("Admin"))
            {
                throw new UnauthorizedAccessException("Only Admin can add staff.");
            }

            if (!ValidateStaffData(staffId, name, role, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            try
            {
                bool exists = _staffDAL.CheckStaffExists(staffId);
                if (exists)
                {
                    throw new InvalidOperationException("Staff ID already exists.");
                }

                _staffDAL.AddStaff(staffId, name.Trim(), role.Trim(), _currentUserId, _currentUsername);
                LogAction($"Added staff ID {staffId} ({name})");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding staff: {ex.Message}", ex);
            }
        }

        public void DeleteStaff(int staffId)
        {
            if (!_currentRole.In("Admin"))
            {
                throw new UnauthorizedAccessException("Only Admin can delete staff.");
            }

            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
            {
                throw new ArgumentException("Staff ID must be a positive integer consisting only of digits.");
            }

            try
            {
                int rowsAffected = _staffDAL.DeleteStaff(staffId, _currentUserId, _currentUsername);
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Staff ID {staffId} not found.");
                }

                LogAction($"Deleted staff ID {staffId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting staff: {ex.Message}", ex);
            }
        }

        public void AssignRole(int staffId, string newRole)
        {
            if (!_currentRole.In("Admin"))
            {
                throw new UnauthorizedAccessException("Only Admin can assign roles.");
            }

            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
            {
                throw new ArgumentException("Staff ID must be a positive integer consisting only of digits.");
            }

            if (string.IsNullOrWhiteSpace(newRole) || !new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(newRole, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Role must be Receptionist, Housekeeping, or Manager.");
            }

            try
            {
                int rowsAffected = _staffDAL.AssignStaffRole(staffId, newRole.Trim(), _currentUserId, _currentUsername);
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Staff ID {staffId} not found.");
                }

                LogAction($"Assigned role {newRole} to staff ID {staffId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning role: {ex.Message}", ex);
            }
        }

        public List<Dictionary<string, string>> GetStaffByRole(string role)
        {
            if (!_currentRole.In("Admin", "Manager"))
            {
                throw new UnauthorizedAccessException("Only Admin or Manager can view staff lists.");
            }

            if (!string.IsNullOrEmpty(role) && !role.Equals("All", StringComparison.OrdinalIgnoreCase) &&
                !new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Role must be All, Receptionist, Housekeeping, or Manager.");
            }

            try
            {
                var results = _staffDAL.GetStaffByRole(role, _currentUserId, _currentUsername);
                LogAction($"Viewed staff list for role {role ?? "All"}");
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving staff list: {ex.Message}", ex);
            }
        }
    }
}
