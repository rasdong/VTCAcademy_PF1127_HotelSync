using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class ServiceManagement
    {
        private readonly string connectionString;
        public ServiceManagement(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Service> GetAllServices()
        {
            var services = new List<Service>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Services";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            ServiceID = reader.GetInt32("ServiceID"),
                            ServiceName = reader.GetString("ServiceName"),
                            Type = reader.GetString("Type"),
                            Price = reader.GetDecimal("Price"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy"),
                            UpdatedByUsername = reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? null : reader.GetString("UpdatedByUsername")
                        });
                    }
                }
            }
            return services;
        }

        public Service? GetServiceById(int serviceId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Services WHERE ServiceID = @ServiceID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceID", serviceId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Service
                            {
                                ServiceID = reader.GetInt32("ServiceID"),
                                ServiceName = reader.GetString("ServiceName"),
                                Type = reader.GetString("Type"),
                                Price = reader.GetDecimal("Price"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.GetDateTime("UpdatedAt"),
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy"),
                                UpdatedByUsername = reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? null : reader.GetString("UpdatedByUsername")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool AddService(Service service)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername) VALUES (@ServiceName, @Type, @Price, @UpdatedBy, @UpdatedByUsername)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                    command.Parameters.AddWithValue("@Type", service.Type);
                    command.Parameters.AddWithValue("@Price", service.Price);
                    command.Parameters.AddWithValue("@UpdatedBy", service.UpdatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedByUsername", service.UpdatedByUsername ?? (object)DBNull.Value);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateService(Service service)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Services SET ServiceName = @ServiceName, Type = @Type, Price = @Price, UpdatedAt = CURRENT_TIMESTAMP, UpdatedBy = @UpdatedBy, UpdatedByUsername = @UpdatedByUsername WHERE ServiceID = @ServiceID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                    command.Parameters.AddWithValue("@Type", service.Type);
                    command.Parameters.AddWithValue("@Price", service.Price);
                    command.Parameters.AddWithValue("@UpdatedBy", service.UpdatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedByUsername", service.UpdatedByUsername ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ServiceID", service.ServiceID);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteService(int serviceId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Services WHERE ServiceID = @ServiceID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceID", serviceId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Service> SearchServices(string keyword)
        {
            var services = new List<Service>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Services WHERE ServiceName LIKE @Keyword OR Type LIKE @Keyword";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            services.Add(new Service
                            {
                                ServiceID = reader.GetInt32("ServiceID"),
                                ServiceName = reader.GetString("ServiceName"),
                                Type = reader.GetString("Type"),
                                Price = reader.GetDecimal("Price"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.GetDateTime("UpdatedAt"),
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy"),
                                UpdatedByUsername = reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? null : reader.GetString("UpdatedByUsername")
                            });
                        }
                    }
                }
            }
            return services;
        }
    }
}
