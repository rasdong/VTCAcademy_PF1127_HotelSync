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

        public void AddEmployee(int staffId, string name, string role)
        {
            if (!_currentRole.In("Admin"))
            {
                Console.WriteLine("Only Admin can add employees.");
                return;
            }
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Staff ID must be a positive integer consisting only of digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsLetter) || !name.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Name must contain letters with diacritics and may include spaces.");
                return;
            }
            if (!new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(role))
            {
                Console.WriteLine("Role must be Receptionist, Housekeeping, or Manager.");
                return;
            }

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("addEmployee", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_StaffID", staffId);
                command.Parameters.AddWithValue("@p_Name", name.Trim());
                command.Parameters.AddWithValue("@p_Role", role);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                command.ExecuteNonQuery();

                LogAction($"Added staff ID {staffId} ({name}) with role {role}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error adding staff: {ex.Message}");
            }
        }

        public void DeleteEmployee(int staffId)
        {
            if (!_currentRole.In("Admin"))
            {
                Console.WriteLine("Only Admin can delete employees.");
                return;
            }
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Staff ID must be a positive integer consisting only of digits.");
                return;
            }

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("deleteEmployee", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_StaffID", staffId);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    Console.WriteLine($"Staff ID {staffId} not found.");
                    return;
                }

                LogAction($"Deleted staff ID {staffId}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error deleting staff: {ex.Message}");
            }
        }

        public void AssignRole(int staffId, string newRole)
        {
            if (!_currentRole.In("Admin"))
            {
                Console.WriteLine("Only Admin can assign roles.");
                return;
            }
            if (staffId <= 0 || !staffId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Staff ID must be a positive integer consisting only of digits.");
                return;
            }
            if (!new[] { "Receptionist", "Housekeeping", "Manager" }.Contains(newRole))
            {
                Console.WriteLine("Role must be Receptionist, Housekeeping, or Manager.");
                return;
            }

            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();

            try
            {
                using var command = new MySqlCommand("assignEmployeeRole", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@p_StaffID", staffId);
                command.Parameters.AddWithValue("@p_NewRole", newRole);
                command.Parameters.AddWithValue("@p_UpdatedBy", _currentUserId);
                command.Parameters.AddWithValue("@p_UpdatedByUsername", _currentUsername);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    Console.WriteLine($"Staff ID {staffId} not found.");
                    return;
                }

                LogAction($"Assigned new role {newRole} to staff ID {staffId}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error assigning role: {ex.Message}");
            }
        }

        public List<Dictionary<string, string>> GetEmployeesByRole(string role)
        {
            if (!_currentRole.In("Admin", "Manager"))
            {
                Console.WriteLine("Only Admin or Manager can view employee list.");
                return new List<Dictionary<string, string>>();
            }
            if (!new[] { "Receptionist", "Housekeeping", "Manager", "All" }.Contains(role))
            {
                Console.WriteLine("Role must be Receptionist, Housekeeping, Manager, or All.");
                return new List<Dictionary<string, string>>();
            }

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
                Console.WriteLine($"Error retrieving staff: {ex.Message}");
            }

            return results;
        }
    }
}
