using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;

namespace HotelManagementSystem.Tests.BLTests
{
    /// <summary>
    /// Test class cho RoomBL
    /// Kiểm tra tất cả các phương thức trong business logic layer của Room Management
    /// </summary>
    [TestFixture]
    public class RoomBLTests : BaseTestClass
    {
        private RoomBLL _roomBLL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _roomBLL = new RoomBLL();
            SeedTestData(); // Thêm dữ liệu test cơ bản
        }

        #region AddRoom Tests

        [Test]
        public void AddRoom_ValidInput_ShouldSucceed()
        {
            // Arrange
            string roomNumber = "102";
            string roomType = "Single";
            string price = "600000";
            string amenities = "[\"TV\", \"WiFi\"]";
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act & Assert
            Assert.DoesNotThrow(() => _roomBLL.AddRoom(roomNumber, roomType, price, amenities, updatedBy, updatedByUsername));
            
            // Verify room was added to database
            var rooms = ExecuteQuery($"SELECT * FROM Rooms WHERE RoomNumber = '{roomNumber}'");
            rooms.Rows.Count.Should().Be(1);
            rooms.Rows[0]["RoomType"].ToString().Should().Be(roomType);
        }

        [Test]
        public void AddRoom_EmptyRoomNumber_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.AddRoom("", "Single", "600000", "[\"TV\"]", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Số phòng không được để trống"));
        }

        [Test]
        public void AddRoom_InvalidRoomType_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.AddRoom("102", "InvalidType", "600000", "[\"TV\"]", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Loại phòng phải là Single, Double hoặc Suite"));
        }

        [Test]
        public void AddRoom_InvalidPrice_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.AddRoom("102", "Single", "invalid", "[\"TV\"]", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Giá phải là số hợp lệ và lớn hơn 0"));
        }

        #endregion

        #region UpdateRoom Tests

        [Test]
        public void UpdateRoom_ValidInput_ShouldSucceed()
        {
            // Arrange
            string roomId = "1";
            string roomNumber = "101-Updated";
            string roomType = "Single";
            string price = "650000";
            string amenities = "[\"TV\", \"WiFi\", \"Updated\"]";
            string status = "Available";
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act & Assert
            Assert.DoesNotThrow(() => _roomBLL.UpdateRoom(roomId, roomNumber, roomType, price, amenities, status, updatedBy, updatedByUsername));
            
            // Verify room was updated in database
            var rooms = ExecuteQuery($"SELECT * FROM Rooms WHERE RoomID = {roomId}");
            rooms.Rows.Count.Should().Be(1);
            rooms.Rows[0]["RoomNumber"].ToString().Should().Be(roomNumber);
        }

        [Test]
        public void UpdateRoom_InvalidRoomId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.UpdateRoom("invalid", "102", "Single", "600000", "[\"TV\"]", "Available", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID phòng không hợp lệ"));
        }

        #endregion

        #region DeleteRoom Tests

        [Test]
        public void DeleteRoom_ValidInput_ShouldSucceed()
        {
            // Arrange
            string roomId = "1";

            // Act & Assert
            Assert.DoesNotThrow(() => _roomBLL.DeleteRoom(roomId, 1, "testadmin"));
            
            // Verify room was deleted from database
            var rooms = ExecuteQuery($"SELECT * FROM Rooms WHERE RoomID = {roomId}");
            rooms.Rows.Count.Should().Be(0);
        }

        [Test]
        public void DeleteRoom_InvalidRoomId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.DeleteRoom("invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID phòng không hợp lệ"));
        }

        #endregion

        #region CleanRoom Tests

        [Test]
        public void CleanRoom_ValidInput_ShouldSucceed()
        {
            // Arrange - Set room status to 'Uncleaned' first
            ExecuteNonQuery("UPDATE Rooms SET Status = 'Uncleaned' WHERE RoomID = 1");
            string roomId = "1";

            // Act & Assert
            Assert.DoesNotThrow(() => _roomBLL.CleanRoom(roomId, 1, "testadmin"));
            
            // Verify room status was updated to 'Available'
            var rooms = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = {roomId}");
            rooms.Rows.Count.Should().Be(1);
            rooms.Rows[0]["Status"].ToString().Should().Be("Available");
        }

        #endregion

        #region GetAllRooms Tests

        [Test]
        public void GetAllRooms_ShouldReturnDataTable()
        {
            // Act
            DataTable result = _roomBLL.GetAllRooms();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            Assert.That(result.Columns.Contains("RoomID"), Is.True);
            Assert.That(result.Columns.Contains("RoomNumber"), Is.True);
            Assert.That(result.Columns.Contains("RoomType"), Is.True);
            Assert.That(result.Columns.Contains("Price"), Is.True);
            Assert.That(result.Columns.Contains("Status"), Is.True);
        }

        #endregion

        #region SearchRooms Tests

        [Test]
        public void SearchRooms_ByStatus_ShouldReturnFilteredResults()
        {
            // Act
            DataTable result = _roomBLL.SearchRooms("Available", "", null, null);

            // Assert
            result.Should().NotBeNull();
            
            // Verify filtering logic
            foreach (DataRow row in result.Rows)
            {
                row["Status"].ToString().Should().Be("Available");
            }
        }

        [Test]
        public void SearchRooms_InvalidStatus_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.SearchRooms("InvalidStatus", "", null, null));
            Assert.That(exception?.Message, Does.Contain("Trạng thái phòng không hợp lệ"));
        }

        #endregion

        #region CheckRoomAvailability Tests

        [Test]
        public void CheckRoomAvailability_ValidDateRange_ShouldReturnAvailableRooms()
        {
            // Arrange
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(3);

            // Act
            DataTable result = _roomBLL.CheckRoomAvailability(startDate, endDate);

            // Assert
            result.Should().NotBeNull();
            Assert.That(result.Columns.Contains("RoomID"), Is.True);
            Assert.That(result.Columns.Contains("RoomNumber"), Is.True);
            Assert.That(result.Columns.Contains("Status"), Is.True);
        }

        [Test]
        public void CheckRoomAvailability_InvalidDateRange_ShouldThrowException()
        {
            // Arrange
            DateTime startDate = DateTime.Now.AddDays(3);
            DateTime endDate = DateTime.Now.AddDays(1); // End date before start date

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _roomBLL.CheckRoomAvailability(startDate, endDate));
            Assert.That(exception?.Message, Does.Contain("Ngày check-in phải trước ngày check-out"));
        }

        #endregion
    }
}
