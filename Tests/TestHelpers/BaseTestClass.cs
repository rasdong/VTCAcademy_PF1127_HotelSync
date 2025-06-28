using NUnit.Framework;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace HotelManagementSystem.Tests.TestHelpers
{
    /// <summary>
    /// Base class cho tất cả test classes
    /// Cung cấp các phương thức và setup chung
    /// </summary>
    [TestFixture]
    public abstract class BaseTestClass
    {
        protected string ConnectionString { get; private set; } = "";
        protected const string TestDatabaseName = "Hotel_management_test";
        
        /// <summary>
        /// Setup chung cho tất cả tests
        /// </summary>
        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            // Khởi tạo connection string cho test database với password đúng
            ConnectionString = "Server=localhost;Database=" + TestDatabaseName + ";Uid=root;Pwd=123321;";
            
            // Tạo test database nếu chưa tồn tại
            CreateTestDatabase();
        }

        /// <summary>
        /// Setup trước mỗi test
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            // Cleanup dữ liệu test trước mỗi test
            CleanupTestData();
        }

        /// <summary>
        /// Cleanup sau mỗi test
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            // Cleanup dữ liệu test sau mỗi test
            CleanupTestData();
        }

        /// <summary>
        /// Cleanup cuối cùng
        /// </summary>
        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            // Có thể xóa test database nếu cần
            // DropTestDatabase();
        }

        /// <summary>
        /// Tạo test database
        /// </summary>
        protected void CreateTestDatabase()
        {
            string masterConnectionString = "Server=localhost;Uid=root;Pwd=123321;";
            
            try
            {
                using (var connection = new MySqlConnection(masterConnectionString))
                {
                    connection.Open();
                    
                    // Tạo database
                    using (var command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {TestDatabaseName}", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                
                // Tạo tables và stored procedures từ Hotel_Final.sql
                ExecuteSqlScript();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Không thể tạo test database: {ex.Message}");
            }
        }

        /// <summary>
        /// Thực thi SQL script để tạo cấu trúc database
        /// </summary>
        protected void ExecuteSqlScript()
        {
            // Thực thi một phần script cần thiết cho test
            // Ở đây chỉ tạo các bảng cơ bản, không cần toàn bộ script
            string[] createTableScripts = {
                @"USE " + TestDatabaseName + @";
                
                CREATE TABLE IF NOT EXISTS Roles (
                    RoleID INT AUTO_INCREMENT PRIMARY KEY,
                    RoleName VARCHAR(50) NOT NULL UNIQUE,
                    Permissions JSON NOT NULL,
                    CreatedAt VARCHAR(255),
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL 
                );",
                
                @"CREATE TABLE IF NOT EXISTS Users (
                    UserID INT AUTO_INCREMENT PRIMARY KEY,
                    Username VARCHAR(50) NOT NULL UNIQUE,
                    Password VARCHAR(255) NOT NULL,
                    RoleID INT NOT NULL,
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL 
                );",
                
                @"CREATE TABLE IF NOT EXISTS Rooms (
                    RoomID INT AUTO_INCREMENT PRIMARY KEY,
                    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
                    RoomType ENUM('Single', 'Double', 'Suite') NOT NULL,
                    Price DECIMAL(10, 2) NOT NULL,
                    Status ENUM('Available', 'Occupied', 'Under Maintenance','Uncleaned') NOT NULL DEFAULT 'Available',
                    Amenities JSON NOT NULL,
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL 
                );",
                
                @"CREATE TABLE IF NOT EXISTS Customers (
                    CustomerID INT AUTO_INCREMENT PRIMARY KEY,
                    Name VARCHAR(100) NOT NULL,
                    IDCard VARCHAR(20) NOT NULL UNIQUE,
                    Phone VARCHAR(15),
                    Email VARCHAR(100),
                    Nationality VARCHAR(50) DEFAULT 'Vietnam',
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL 
                );"
            };

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                foreach (string script in createTableScripts)
                {
                    using (var command = new MySqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Cleanup dữ liệu test
        /// </summary>
        protected void CleanupTestData()
        {
            string[] cleanupScripts = {
                "DELETE FROM Users WHERE UserID > 0;",
                "DELETE FROM Rooms WHERE RoomID > 0;",
                "DELETE FROM Customers WHERE CustomerID > 0;",
                "ALTER TABLE Users AUTO_INCREMENT = 1;",
                "ALTER TABLE Rooms AUTO_INCREMENT = 1;",
                "ALTER TABLE Customers AUTO_INCREMENT = 1;"
            };

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    foreach (string script in cleanupScripts)
                    {
                        using (var command = new MySqlCommand(script, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không fail test
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm dữ liệu test cơ bản
        /// </summary>
        protected void SeedTestData()
        {
            string[] seedScripts = {
                "INSERT INTO Roles (RoleName, Permissions, UpdatedBy, UpdatedByUsername, CreatedAt) VALUES" +
                "  ('Admin', '{\"permissions\": [\"manage_rooms\", \"manage_customers\", \"manage_bookings\"]}', NULL, NULL, '2024-01-01 10:00:00');",
                
                "INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername) VALUES" +
                "  ('testadmin', 'admin123', 1, NULL, NULL);",
                
                "INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) VALUES" +
                "  ('Test Customer', '123456789', '0901234567', 'test@email.com', 'Vietnam', 1, 'testadmin');",
                
                "INSERT INTO Rooms (RoomNumber, RoomType, Price, Status, Amenities, UpdatedBy, UpdatedByUsername) VALUES" +
                "  ('101', 'Single', 500000.00, 'Available', '{\"amenities\": [\"TV\", \"WiFi\"]}', 1, 'testadmin');"
            };

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                foreach (string script in seedScripts)
                {
                    using (var command = new MySqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Tạo MySqlConnection cho test
        /// </summary>
        /// <returns>MySqlConnection</returns>
        protected MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Thực thi query và trả về DataTable
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <returns>DataTable</returns>
        protected DataTable ExecuteQuery(string query)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// Thực thi non-query command
        /// </summary>
        /// <param name="query">SQL command</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        protected int ExecuteNonQuery(string query)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
