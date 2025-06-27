using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;

namespace HotelManagementSystem.Tests.BLTests
{
    /// <summary>
    /// Test class cho ServiceBLL
    /// Kiểm tra tất cả các phương thức trong business logic layer của Service Management
    /// </summary>
    [TestFixture]
    public class ServiceBLTests : BaseTestClass
    {
        private ServiceBLL _serviceBL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _serviceBL = new ServiceBLL();
            SeedTestData(); // Thêm dữ liệu test cơ bản
            SeedServiceTestData(); // Thêm dữ liệu test cho service
        }

        /// <summary>
        /// Thêm dữ liệu test cần thiết cho service
        /// </summary>
        private void SeedServiceTestData()
        {
            // Thêm services table
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Services (
                    ServiceID INT AUTO_INCREMENT PRIMARY KEY,
                    ServiceName VARCHAR(50) NOT NULL,
                    Type ENUM('Food', 'Laundry', 'Spa', 'Other') NOT NULL,
                    Price DECIMAL(10, 2) NOT NULL,
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL 
                );");

            // Thêm service test data
            ExecuteNonQuery(@"
                INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername) 
                VALUES ('Test Service', 'Food', 100000, 1, 'testadmin');");
        }

        #region AddService Tests

        [Test]
        public void AddService_ValidInput_ShouldSucceed()
        {
            // Arrange
            string serviceName = "New Test Service";
            string type = "Laundry";
            string price = "150000";
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act & Assert
            Assert.DoesNotThrow(() => _serviceBL.AddService(serviceName, type, price, updatedBy, updatedByUsername));
            
            // Verify service was added
            var services = ExecuteQuery($"SELECT * FROM Services WHERE ServiceName = '{serviceName}'");
            services.Rows.Count.Should().Be(1);
            services.Rows[0]["Type"].ToString().Should().Be(type);
            Convert.ToDecimal(services.Rows[0]["Price"]).Should().Be(150000);
        }

        [Test]
        public void AddService_EmptyServiceName_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.AddService("", "Food", "100000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Tên dịch vụ không được để trống"));
        }

        [Test]
        public void AddService_InvalidType_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.AddService("Test Service", "InvalidType", "100000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Loại dịch vụ phải là Food, Laundry, Spa hoặc Other"));
        }

        [Test]
        public void AddService_InvalidPrice_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.AddService("Test Service", "Food", "invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Giá phải là số hợp lệ và lớn hơn 0"));
        }

        [Test]
        public void AddService_NegativePrice_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.AddService("Test Service", "Food", "-100", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Giá phải là số hợp lệ và lớn hơn 0"));
        }

        [Test]
        public void AddService_ZeroPrice_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.AddService("Test Service", "Food", "0", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Giá phải là số hợp lệ và lớn hơn 0"));
        }

        #endregion

        #region UpdateService Tests

        [Test]
        public void UpdateService_ValidInput_ShouldSucceed()
        {
            // Arrange
            string serviceId = "1"; // Service exists from SeedServiceTestData
            string newServiceName = "Updated Service";
            string newType = "Spa";
            string newPrice = "200000";

            // Act & Assert
            Assert.DoesNotThrow(() => _serviceBL.UpdateService(serviceId, newServiceName, newType, newPrice, 1, "testadmin"));
            
            // Verify service was updated
            var services = ExecuteQuery($"SELECT * FROM Services WHERE ServiceID = {serviceId}");
            services.Rows.Count.Should().Be(1);
            services.Rows[0]["ServiceName"].ToString().Should().Be(newServiceName);
            services.Rows[0]["Type"].ToString().Should().Be(newType);
            Convert.ToDecimal(services.Rows[0]["Price"]).Should().Be(200000);
        }

        [Test]
        public void UpdateService_InvalidServiceId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.UpdateService("invalid", "Test Service", "Food", "100000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID dịch vụ không hợp lệ"));
        }

        [Test]
        public void UpdateService_EmptyServiceName_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.UpdateService("1", "", "Food", "100000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Tên dịch vụ không được để trống"));
        }

        #endregion

        #region DeleteService Tests

        [Test]
        public void DeleteService_ValidId_ShouldSucceed()
        {
            // Arrange
            string serviceId = "1";

            // Act & Assert
            Assert.DoesNotThrow(() => _serviceBL.DeleteService(serviceId, 1, "testadmin"));
            
            // Verify service was deleted
            var services = ExecuteQuery($"SELECT * FROM Services WHERE ServiceID = {serviceId}");
            services.Rows.Count.Should().Be(0);
        }

        [Test]
        public void DeleteService_InvalidId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _serviceBL.DeleteService("invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID dịch vụ không hợp lệ"));
        }

        #endregion

        #region GetAllServices Tests

        [Test]
        public void GetAllServices_ShouldReturnDataTable()
        {
            // Act
            DataTable result = _serviceBL.GetAllServices();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            Assert.That(result.Columns.Contains("ServiceID"), Is.True);
            Assert.That(result.Columns.Contains("ServiceName"), Is.True);
            Assert.That(result.Columns.Contains("Type"), Is.True);
            Assert.That(result.Columns.Contains("Price"), Is.True);
        }

        #endregion

        #region SearchServices Tests

        [Test]
        public void SearchServicesByType_WithValidType_ShouldReturnFilteredResults()
        {
            // Act
            DataTable result = _serviceBL.SearchServicesByType("Food");

            // Assert
            result.Should().NotBeNull();
            foreach (DataRow row in result.Rows)
            {
                row["Type"].ToString().Should().Be("Food");
            }
        }

        [Test]
        public void SearchServicesByType_WithInvalidType_ShouldThrowException()
        {
            // Act & Assert
            Action action = () => _serviceBL.SearchServicesByType("InvalidType");
            action.Should().Throw<Exception>()
                .WithMessage("*Loại dịch vụ không hợp lệ*");
        }

        #endregion
    }
}
