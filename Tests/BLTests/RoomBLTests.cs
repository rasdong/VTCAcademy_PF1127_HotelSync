using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;

namespace HotelManagementSystem.Tests.BLTests
{
    [TestFixture]
    public class RoomBLTests : BaseTestClass
    {
        private RoomBLL _roomBLL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _roomBLL = new RoomBLL();
            SeedTestData();
        }

        #region AddRoom Tests

        [Test]
        public void AddRoom_ValidInput_ShouldSucceed()
        {
            string roomNumber = "102";
            string roomType = "Single";
            string price = "600000";
            string amenities = "[\"TV\", \"WiFi\"]";

            Assert.DoesNotThrow(() =>
                _roomBLL.AddRoom(roomNumber, roomType, price, amenities, 1, "testadmin"));

            var rooms = ExecuteQuery($"SELECT * FROM Rooms WHERE RoomNumber = '{roomNumber}'");
            rooms.Rows.Count.Should().Be(1);
            rooms.Rows[0]["RoomType"].ToString().Should().Be(roomType);
        }

        [Test]
        public void AddRoom_DuplicateRoomNumber_ShouldThrowException()
        {
            string roomNumber = "102";
            _roomBLL.AddRoom(roomNumber, "Single", "500000", "[\"TV\"]", 1, "testadmin");

            var exception = Assert.Throws<Exception>(() =>
                _roomBLL.AddRoom(roomNumber, "Single", "500000", "[\"TV\"]", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Số phòng đã tồn tại"));
        }

        [Test]
        public void AddRoom_InvalidPrice_ShouldThrowException()
        {
            var exception = Assert.Throws<Exception>(() =>
                _roomBLL.AddRoom("105", "Single", "invalid", "[\"TV\"]", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Giá phải là số hợp lệ và lớn hơn 0"));
        }

        #endregion

        #region UpdateRoom Tests

        [Test]
        public void UpdateRoom_ValidInput_ShouldSucceed()
        {
            string roomId = "1";
            string roomNumber = "101-Updated";

            Assert.DoesNotThrow(() =>
                _roomBLL.UpdateRoom(roomId, roomNumber, "Single", "650000", "[\"TV\"]", "Available", 1, "testadmin"));

            var rooms = ExecuteQuery($"SELECT * FROM Rooms WHERE RoomID = {roomId}");
            rooms.Rows[0]["RoomNumber"].ToString().Should().Be(roomNumber);
        }

        [Test]
        public void UpdateRoom_InvalidPrice_ShouldThrowException()
        {
            var exception = Assert.Throws<Exception>(() =>
                _roomBLL.UpdateRoom("1", "101", "Single", "-100", "[\"TV\"]", "Available", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Giá phải là số hợp lệ và lớn hơn 0"));
        }

        [Test]
        public void UpdateRoom_InvalidRoomId_ShouldThrowException()
        {
            var exception = Assert.Throws<Exception>(() =>
                _roomBLL.UpdateRoom("999", "101", "Single", "500000", "[\"TV\"]", "Available", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID phòng không hợp lệ"));
        }

        #endregion

        #region DeleteRoom Tests

        [Test]
        public void DeleteRoom_ValidInput_ShouldSucceed()
        {
            string roomId = "1";
            ExecuteNonQuery("UPDATE Rooms SET Status = 'Available' WHERE RoomID = 1");

            Assert.DoesNotThrow(() => _roomBLL.DeleteRoom(roomId, 1, "testadmin"));

            var rooms = ExecuteQuery($"SELECT * FROM Rooms WHERE RoomID = {roomId}");
            rooms.Rows.Count.Should().Be(0);
        }

        [Test]
        public void DeleteRoom_OccupiedRoom_ShouldThrowException()
        {
            ExecuteNonQuery("UPDATE Rooms SET Status = 'Occupied' WHERE RoomID = 1");

            var exception = Assert.Throws<Exception>(() => _roomBLL.DeleteRoom("1", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Không thể xóa phòng đang được sử dụng"));
        }

        #endregion

        #region CleanRoom Tests

        [Test]
        public void CleanRoom_ValidInput_ShouldSucceed()
        {
            ExecuteNonQuery("UPDATE Rooms SET Status = 'Uncleaned' WHERE RoomID = 1");

            Assert.DoesNotThrow(() => _roomBLL.CleanRoom("1", 1, "testadmin"));

            var rooms = ExecuteQuery("SELECT Status FROM Rooms WHERE RoomID = 1");
            rooms.Rows[0]["Status"].ToString().Should().Be("Available");
        }

        [Test]
        public void CleanRoom_RoomAlreadyClean_ShouldThrowException()
        {
            ExecuteNonQuery("UPDATE Rooms SET Status = 'Available' WHERE RoomID = 1");

            var exception = Assert.Throws<Exception>(() => _roomBLL.CleanRoom("1", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Phòng đã sạch, không cần dọn lại"));
        }

        [Test]
        public void CleanRoom_NonExistentRoom_ShouldThrowException()
        {
            var exception = Assert.Throws<Exception>(() => _roomBLL.CleanRoom("999", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Phòng không tồn tại"));
        }

        #endregion

        #region GetAllRooms Tests

        [Test]
        public void GetAllRooms_ShouldReturnDataTable()
        {
            DataTable result = _roomBLL.GetAllRooms();

            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            result.Columns.Contains("RoomID").Should().BeTrue();
            result.Columns.Contains("RoomNumber").Should().BeTrue();
            result.Columns.Contains("RoomType").Should().BeTrue();
            result.Columns.Contains("Price").Should().BeTrue();
            result.Columns.Contains("Status").Should().BeTrue();
        }

        #endregion

        #region SearchRooms Tests

        [Test]
        public void SearchRooms_ByStatus_ShouldReturnFilteredResults()
        {
            DataTable result = _roomBLL.SearchRooms("Available", "", null, null);

            result.Should().NotBeNull();
            foreach (DataRow row in result.Rows)
            {
                row["Status"].ToString().Should().Be("Available");
            }
        }

        [Test]
        public void SearchRooms_NoMatch_ShouldReturnEmpty()
        {
            DataTable result = _roomBLL.SearchRooms("VIP", "", null, null);
            result.Rows.Count.Should().Be(0);
        }

        #endregion

        #region CheckRoomAvailability Tests

        [Test]
        public void CheckRoomAvailability_ValidDateRange_ShouldReturnAvailableRooms()
        {
            var result = _roomBLL.CheckRoomAvailability(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3));

            result.Should().NotBeNull();
            result.Columns.Contains("RoomID").Should().BeTrue();
        }

        [Test]
        public void CheckRoomAvailability_InvalidDateRange_ShouldThrowException()
        {
            var exception = Assert.Throws<Exception>(() =>
                _roomBLL.CheckRoomAvailability(DateTime.Now.AddDays(3), DateTime.Now.AddDays(1)));
            Assert.That(exception?.Message, Does.Contain("Ngày check-in phải trước ngày check-out"));
        }

        #endregion
    }
}
