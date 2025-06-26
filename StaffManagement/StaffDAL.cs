using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class StaffDAL
    {
        public bool CheckStaffExists(int staffId)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("SELECT COUNT(*) FROM Staff WHERE StaffID = @staffId", connection);
            command.Parameters.AddWithValue("@staffId", staffId);
            return (long)command.ExecuteScalar() > 0;
        }

        public void AddStaff(int staffId, string name, string role, int updatedBy, string updatedByUsername)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("addEmployeeWithTransaction", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_StaffID", staffId);
            command.Parameters.AddWithValue("@p_Name", name);
            command.Parameters.AddWithValue("@p_Role", role);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);
            command.ExecuteNonQuery();
        }

        public int DeleteStaff(int staffId, int updatedBy, string updatedByUsername)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("deleteEmployeeWithTransaction", connection)
            { 

                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_StaffID", staffId);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);

            object result = command.ExecuteScalar();
            return result != null && Convert.ToInt32(result) == 1 ? 1 : 0;
        }


        public int AssignStaffRole(int staffId, string newRole, int updatedBy, string updatedByUsername)
        {
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("assignEmployeeRoleWithTransaction", connection)
            { 
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_StaffID", staffId);
            command.Parameters.AddWithValue("@p_NewRole", newRole);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);

            object result = command.ExecuteScalar();
            return result != null && Convert.ToInt32(result) == 1 ? 1 : 0;
        }


        public List<Dictionary<string, string>> GetStaffByRole(string role, int updatedBy, string updatedByUsername)
        {
            var results = new List<Dictionary<string, string>>();
            using var connection = DataHelper.Instance.GetConnection();
            connection.Open();
            using var command = new MySqlCommand("getEmployeesByRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_Role", string.IsNullOrEmpty(role) || role.Equals("All", StringComparison.OrdinalIgnoreCase) ? DBNull.Value : role);
            command.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);
            command.Parameters.AddWithValue("@p_UpdatedByUsername", updatedByUsername);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var staff = new Dictionary<string, string>
                {
                    { "StaffID", reader["StaffID"].ToString() },
                    { "Name", reader["Name"].ToString() },
                    { "Role", reader["Role"].ToString() }
                };
                results.Add(staff);
            }
            return results;
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
