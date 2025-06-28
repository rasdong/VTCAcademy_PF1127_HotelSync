
using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;

namespace HotelManagementSystem.Tests.BLTests
{
    [TestFixture]
    public class BookingBLTests : BaseTestClass
    {
        private BookingBLL _bookingBLL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _bookingBLL = new BookingBLL();
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Create Users table
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID INT AUTO_INCREMENT PRIMARY KEY,
                    Username VARCHAR(50) NOT NULL UNIQUE,
                    Password VARCHAR(10) NOT NULL,
                    RoleID INT NOT NULL,
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL
                );");

            // Create Roles table
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Roles (
                    RoleID INT AUTO_INCREMENT PRIMARY KEY,
                    RoleName VARCHAR(50) NOT NULL UNIQUE,
                    Permissions JSON NOT NULL,
                    CreatedAt VARCHAR(255),
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL
                );");

            // Create Rooms table
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Rooms (
                    RoomID INT AUTO_INCREMENT PRIMARY KEY,
                    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
                    RoomType ENUM('Single', 'Double', 'Suite') NOT NULL,
                    Price DECIMAL(10, 2) NOT NULL,
                    Status ENUM('Available', 'Occupied', 'Under Maintenance', 'Uncleaned') NOT NULL DEFAULT 'Available',
                    Amenities JSON NOT NULL,
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL
                );");

            // Create Customers table
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Customers (
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
                );");

            // Create Bookings table
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Bookings (
                    BookingID INT AUTO_INCREMENT PRIMARY KEY,
                    CustomerID INT NOT NULL,
                    IDCard VARCHAR(20) NOT NULL,
                    RoomID INT NOT NULL,
                    CheckInDate DATETIME NOT NULL,
                    CheckOutDate DATETIME NOT NULL,
                    Status ENUM('Active', 'Completed', 'Cancelled') NOT NULL DEFAULT 'Active',
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL,
                    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
                    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
                    FOREIGN KEY (IDCard) REFERENCES Customers(IDCard),
                    FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID)
                );");

            
        }

        #region CreateBooking Tests

        [Test]
        public void CreateBooking_ValidInput_ShouldSucceed()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, updatedBy, updatedByUsername);
                bookingId.Should().BeGreaterThan(0);
            });
        }

        [Test]
        public void CreateBooking_EmptyIDCard_ShouldThrowException()
        {
            // Arrange
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking("", roomId, checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Số chứng minh nhân dân/căn cước công dân không hợp lệ"));
        }

        [Test]
        public void CreateBooking_InvalidRoomId_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking(idCard, "999", checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Phòng không tồn tại"));
        }

        [Test]
        public void CreateBooking_InvalidDateRange_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Ngày check-in phải trước ngày check-out"));
        }

        [Test]
        public void CreateBooking_PastCheckInDate_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Ngày check-in không thể trước thời gian hiện tại"));
        }

        [Test]
        public void CreateBooking_NonExistentCustomer_ShouldThrowException()
        {
            // Arrange
            string idCard = "999999999";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Khách hàng không tồn tại"));
        }

        [Test]
        public void CreateBooking_WithoutPermission_ShouldThrowException()
        {
            // Arrange
            ExecuteNonQuery(@"
                INSERT INTO Roles (RoleID, RoleName, Permissions, CreatedAt) VALUES
                (2, 'NoPermission', '[]', '2024-10-15 10:00:00');
                INSERT INTO Users (UserID, Username, Password, RoleID) VALUES
                (2, 'noperm', 'noperm123', 2);
            ");
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 2, "noperm"));
            Assert.That(exception?.Message, Does.Contain("Người dùng không có quyền tạo đặt phòng"));
        }

        #endregion

        #region CancelBooking Tests

        [Test]
        public void CancelBooking_ValidBookingId_ShouldSucceed()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.CancelBooking(bookingId.ToString(), 1, "testadmin"));
            var booking = ExecuteQuery($"SELECT Status FROM Bookings WHERE BookingID = {bookingId}");
            booking.Rows[0]["Status"].ToString().Should().Be("Cancelled");
        }

        [Test]
        public void CancelBooking_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CancelBooking("999", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại hoặc không hợp lệ để hủy"));
        }

        [Test]
        public void CancelBooking_EmptyBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CancelBooking("", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID đặt phòng không hợp lệ"));
        }

        [Test]
        public void CancelBooking_WithoutPermission_ShouldThrowException()
        {
            // Arrange
            ExecuteNonQuery(@"
                INSERT INTO Roles (RoleID, RoleName, Permissions, CreatedAt) VALUES
                (2, 'NoPermission', '[]', '2024-10-15 10:00:00');
                INSERT INTO Users (UserID, Username, Password, RoleID) VALUES
                (2, 'noperm', 'noperm123', 2);
            ");
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CancelBooking(bookingId.ToString(), 2, "noperm"));
            Assert.That(exception?.Message, Does.Contain("Người dùng không có quyền hủy đặt phòng"));
        }

        #endregion

        #region CheckIn Tests

        [Test]
        public void CheckIn_ValidBookingId_ShouldSucceed()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.CheckIn(bookingId.ToString(), idCard, 1, "testadmin"));
            var room = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = 1");
            room.Rows[0]["Status"].ToString().Should().Be("Occupied");
        }

        [Test]
        public void CheckIn_InvalidBookingId_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn("999", idCard, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại"));
        }

        [Test]
        public void CheckIn_FutureCheckInDate_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn(bookingId.ToString(), idCard, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Không thể check-in trước ngày"));
        }

        [Test]
        public void CheckIn_WrongIDCard_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string wrongIdCard = "987654321";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn(bookingId.ToString(), wrongIdCard, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Căn cước công dân không khớp với thông tin đặt phòng"));
        }

        [Test]
        public void CheckIn_WithoutPermission_ShouldThrowException()
        {
            // Arrange
            ExecuteNonQuery(@"
                INSERT INTO Roles (RoleID, RoleName, Permissions, CreatedAt) VALUES
                (2, 'NoPermission', '[]', '2024-10-15 10:00:00');
                INSERT INTO Users (UserID, Username, Password, RoleID) VALUES
                (2, 'noperm', 'noperm123', 2);
            ");
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn(bookingId.ToString(), idCard, 2, "noperm"));
            Assert.That(exception?.Message, Does.Contain("Người dùng không có quyền thực hiện check-in"));
        }

        #endregion

        #region CheckOut Tests
        

        [Test]
        public void CheckOut_ValidActiveBooking_ShouldSucceed()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");
            

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.CheckOut(bookingId.ToString(), 1, "testadmin"));
            var booking = ExecuteQuery($"SELECT Status FROM Bookings WHERE BookingID = {bookingId}");
            booking.Rows[0]["Status"].ToString().Should().Be("Completed");
            var room = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = 1");
            room.Rows[0]["Status"].ToString().Should().Be("Uncleaned");
        }

        [Test]
        public void CheckOut_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckOut("999", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại"));
        }

        [Test]
        public void CheckOut_WithoutPermission_ShouldThrowException()
        {
            // Arrange
            ExecuteNonQuery(@"
                INSERT INTO Roles (RoleID, RoleName, Permissions, CreatedAt) VALUES
                (2, 'NoPermission', '[]', '2024-10-15 10:00:00');
                INSERT INTO Users (UserID, Username, Password, RoleID) VALUES
                (2, 'noperm', 'noperm123', 2);
            ");
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckOut(bookingId.ToString(), 2, "noperm"));
            Assert.That(exception?.Message, Does.Contain("Người dùng không có quyền thực hiện check-out"));
        }

        #endregion

        #region ExtendBooking Tests

        [Test]
        public void ExtendBooking_ValidInput_ShouldSucceed()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");
            DateTime newEndDate = DateTime.Now.AddDays(5);

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.ExtendBooking(bookingId.ToString(), newEndDate, 1, "testadmin"));
            var booking = ExecuteQuery($"SELECT CheckOutDate FROM Bookings WHERE BookingID = {bookingId}");
            DateTime actualEndDate = Convert.ToDateTime(booking.Rows[0]["CheckOutDate"]);
            actualEndDate.Date.Should().Be(newEndDate.Date);
        }

        [Test]
        public void ExtendBooking_InvalidBookingId_ShouldThrowException()
        {
            // Arrange
            DateTime newEndDate = DateTime.Now.AddDays(5);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.ExtendBooking("999", newEndDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại"));
        }

        [Test]
        public void ExtendBooking_PastExtendDate_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");
            DateTime pastEndDate = DateTime.Now.AddDays(-1);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.ExtendBooking(bookingId.ToString(), pastEndDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Ngày gia hạn phải lớn hơn ngày hiện tại"));
        }

        [Test]
        public void ExtendBooking_WithoutPermission_ShouldThrowException()
        {
            // Arrange
            ExecuteNonQuery(@"
                INSERT INTO Roles (RoleID, RoleName, Permissions, CreatedAt) VALUES
                (2, 'NoPermission', '[]', '2024-10-15 10:00:00');
                INSERT INTO Users (UserID, Username, Password, RoleID) VALUES
                (2, 'noperm', 'noperm123', 2);
            ");
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");
            DateTime newEndDate = DateTime.Now.AddDays(5);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.ExtendBooking(bookingId.ToString(), newEndDate, 2, "noperm"));
            Assert.That(exception?.Message, Does.Contain("Người dùng không có quyền gia hạn đặt phòng"));
        }

        #endregion

        #region GetBookingHistory Tests

        [Test]
        public void GetBookingHistory_ValidCustomerId_ShouldReturnData()
        {
            // Arrange
            int customerId = 1;
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act
            DataTable result = _bookingBLL.GetBookingHistory(customerId, null);

            // Assert
            result.Should().NotBeNull();
            Assert.That(result.Columns.Contains("BookingID"), Is.True);
            Assert.That(result.Columns.Contains("CustomerID"), Is.True);
            Assert.That(result.Columns.Contains("RoomID"), Is.True);
            Assert.That(result.Columns.Contains("CheckInDate"), Is.True);
            Assert.That(result.Columns.Contains("CheckOutDate"), Is.True);
            Assert.That(result.Columns.Contains("Status"), Is.True);
        }

        [Test]
        public void GetBookingHistory_InvalidCustomerId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.GetBookingHistory(0, null));
            Assert.That(exception?.Message, Does.Contain("ID khách hàng không hợp lệ"));
        }

        [Test]
        public void GetBookingHistory_NegativeCustomerId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.GetBookingHistory(-1, null));
            Assert.That(exception?.Message, Does.Contain("ID khách hàng không hợp lệ"));
        }

        [Test]
        public void GetBookingHistory_WithRoomIdFilter_ShouldReturnFilteredData()
        {
            // Arrange
            int customerId = 1;
            int roomId = 1;
            string idCard = "123456789";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            _bookingBLL.CreateBooking(idCard, roomId.ToString(), checkInDate, checkOutDate, 1, "testadmin");

            // Act
            DataTable result = _bookingBLL.GetBookingHistory(customerId, roomId);

            // Assert
            result.Should().NotBeNull();
            foreach (DataRow row in result.Rows)
            {
                Convert.ToInt32(row["RoomID"]).Should().Be(roomId);
            }
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Test]
        public void CreateBooking_MultipleBookingsForSameRoom_ShouldHandleConflicts()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate1 = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate1 = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            string checkInDate2 = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate2 = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd HH:mm:ss");

            // Act
            int bookingId1 = _bookingBLL.CreateBooking(idCard, roomId, checkInDate1, checkOutDate1, 1, "testadmin");
            bookingId1.Should().BeGreaterThan(0);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate2, checkOutDate2, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Phòng đã được đặt trong khoảng thời gian này"));
        }

        [Test]
        public void CheckIn_NonExistentBooking_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn("999", idCard, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại"));
        }

        [Test]
        public void CheckOut_NonExistentBooking_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckOut("999", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại"));
        }

        [Test]
        public void ExtendBooking_NonExistentBooking_ShouldThrowException()
        {
            // Arrange
            DateTime newEndDate = DateTime.Now.AddDays(5);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.ExtendBooking("999", newEndDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Đặt phòng không tồn tại"));
        }

        #endregion
    }
}