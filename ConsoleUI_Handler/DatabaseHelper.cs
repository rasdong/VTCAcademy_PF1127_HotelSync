using MySql.Data.MySqlClient;
using System.Configuration;

namespace HotelManagementSystem
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["HotelManagementConnection"]?.ConnectionString 
                ?? throw new InvalidOperationException("Chuỗi kết nối cơ sở dữ liệu không được tìm thấy.");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public int ExecuteNonQuery(string query, MySqlParameter[]? parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu: " + ex.Message);
                }
            }
        }

        public object? ExecuteScalar(string query, MySqlParameter[]? parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteScalar();
                    }
                }
                catch (MySqlException ex)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu: " + ex.Message);
                }
            }
        }
    }
}