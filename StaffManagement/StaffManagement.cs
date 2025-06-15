using MySql.Data.MySqlClient;
using System;

namespace HotelManagementSystem
{
    public class StaffManagement
    {
        public void AddStaff(string name, string role, string phone)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"INSERT INTO Staff (Name, Role, Phone)
                             VALUES (@name, @role, @phone)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Nhân viên đã được thêm.");
        }

        public void UpdateStaff(int staffId, string name, string role, string phone)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"UPDATE Staff SET Name=@name, Role=@role, Phone=@phone WHERE StaffID=@id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", staffId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.Parameters.AddWithValue("@phone", phone);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "✔ Đã cập nhật nhân viên." : "✘ Không tìm thấy nhân viên.");
        }

        public void DeleteStaff(int staffId)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = "DELETE FROM Staff WHERE StaffID = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", staffId);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "✔ Đã xóa nhân viên." : "✘ Không tìm thấy nhân viên.");
        }

        public void AssignWork(int staffId, string taskDescription)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"INSERT INTO Logs (UserID, Action, UpdatedByUsername)
                             VALUES (@staffId, @action, 'System')";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@staffId", staffId);
            cmd.Parameters.AddWithValue("@action", "Gán công việc: " + taskDescription);
            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Đã gán công việc cho nhân viên.");
        }

        public void ListStaffByRole(string role)
        {
            using var conn = DataHelper.Instance.GetConnection();
            conn.Open();
            string query = @"SELECT * FROM Staff WHERE Role = @role";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@role", role);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"[{reader["StaffID"]}] {reader["Name"]} | {reader["Role"]} | {reader["Phone"]}");
            }
        }
    }
}
