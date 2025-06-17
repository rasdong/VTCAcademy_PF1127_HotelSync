using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class StaffManagement
    {
        private readonly string _currentRole;
        private readonly int _currentUserId;
        private readonly string _currentUsername;

        public StaffManagement(string role, int userId, string username)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || userId <= 0)
                throw new ArgumentException("Invalid role, user ID, or username provided.");
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

        public void AddEmployee(int staffId, string name, string role)
        {
            if (!_currentRole.In("Admin"))
                throw new UnauthorizedAccessException("Only Admin can add employees.");
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
                throw new ArgumentException("Staff ID must be a positive integer consisting only of digits.");
            if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsLetter) || !name.Any(c => c == ' ' || char.IsLetter(c)))
                throw new ArgumentException("Name must contain letters with diacritics and may include spaces.");
            if (!new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(role))
                throw new ArgumentException("Role must be Receptionist, Housekeeping, or Manager.");

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var command = new MySqlCommand("addEmployee", connection, transaction)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_StaffID", staffId);
                command.Parameters.AddWithValue("@p_Name", name.Trim());
                command.Parameters.AddWithValue("@p_Role", role);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogAction($"Added staff ID {staffId} ({name}) with role {role}");
            }
            catch (MySqlException ex)
            {
                transaction.Rollback();
                throw new Exception($"Error adding staff: {ex.Message}");
            }
        }

        public void DeleteEmployee(int staffId)
        {
            if (!_currentRole.In("Admin"))
                throw new UnauthorizedAccessException("Only Admin can delete employees.");
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
                throw new ArgumentException("Staff ID must be a positive integer consisting only of digits.");

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var command = new MySqlCommand("deleteEmployee", connection, transaction)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_StaffID", staffId);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new Exception($"Staff ID {staffId} not found.");

                transaction.Commit();
                LogAction($"Deleted staff ID {staffId}");
            }
            catch (MySqlException ex)
            {
                transaction.Rollback();
                throw new Exception($"Error deleting staff: {ex.Message}");
            }
        }

        public void AssignRole(int staffId, string newRole)
        {
            if (!_currentRole.In("Admin"))
                throw new UnauthorizedAccessException("Only Admin can assign roles.");
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
                throw new ArgumentException("Staff ID must be a positive integer consisting only of digits.");
            if (!new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(newRole))
                throw new ArgumentException("Role must be Receptionist, Housekeeping, or Manager.");

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var command = new MySqlCommand("assignEmployeeRole", connection, transaction)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_StaffID", staffId);
                command.Parameters.AddWithValue("@p_NewRole", newRole);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new Exception($"Staff ID {staffId} not found.");

                transaction.Commit();
                LogAction($"Assigned new role {newRole} to staff ID {staffId}");
            }
            catch (MySqlException ex)
            {
                transaction.Rollback();
                throw new Exception($"Error assigning role: {ex.Message}");
            }
        }

        public List<Dictionary<string, string>> GetEmployeesByRole(string role)
        {
            if (!_currentRole.In("Admin", "Manager"))
                throw new UnauthorizedAccessException("Only Admin or Manager can view employee list.");
            if (!new[] { "Receptionist", "Housekeeping", "Manager", "All" }.Contains(role))
                throw new ArgumentException("Role must be Receptionist, Housekeeping, Manager, or All.");

            var results = new List<Dictionary<string, string>>();
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("getEmployeesByRole", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_Role", role == "All" ? DBNull.Value : role);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var employee = new Dictionary<string, string>
                    {
                        { "StaffID", reader["StaffID"].ToString() },
                        { "Name", reader["Name"].ToString() },
                        { "Role", reader["Role"].ToString() }
                    };
                    results.Add(employee);
                }

                LogAction($"Viewed staff with role {role}");
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Error retrieving staff: {ex.Message}");
            }

            return results;
        }
    }
}
