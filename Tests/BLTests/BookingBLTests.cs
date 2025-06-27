using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;

namespace HotelManagementSystem.Tests.BLTests
{
    /// <summary>
    /// Test class cho BookingBL
    /// Kiểm tra tất cả các phương thức trong business logic layer của Booking Management
    /// </summary>
    [TestFixture]
    public class BookingBLTests : BaseTestClass
    {
        private BookingBLL _bookingBLL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _bookingBLL = new BookingBLL();
            SeedTestData(); // Thêm dữ liệu test cơ bản
            SeedBookingTestData(); // Thêm dữ liệu test cho booking
        }

        /// <summary>
        /// Thêm dữ liệu test cần thiết cho booking
        /// </summary>
        private void SeedBookingTestData()
        {
            // Thêm booking table
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
                    UpdatedByUsername VARCHAR(50) NULL
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
                _bookingBLL.CreateBooking(idCard, "invalid", checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID phòng không hợp lệ"));
        }

        [Test]
        public void CreateBooking_InvalidDateRange_ShouldThrowException()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"); // Check-out before check-in

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
            string checkInDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"); // Past date
            string checkOutDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Ngày check-in không thể trước thời gian hiện tại"));
        }

        #endregion

        #region CancelBooking Tests

        [Test]
        public void CancelBooking_ValidBookingId_ShouldSucceed()
        {
            // Arrange - First create a booking
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.CancelBooking(bookingId.ToString(), 1, "testadmin"));
        }

        [Test]
        public void CancelBooking_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CancelBooking("invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID đặt phòng không hợp lệ"));
        }

        [Test]
        public void CancelBooking_EmptyBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CancelBooking("", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID đặt phòng không hợp lệ"));
        }

        #endregion

        #region CheckIn Tests

        [Test]
        public void CheckIn_ValidBookingId_ShouldSucceed()
        {
            // Arrange - Create a booking for today
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.CheckIn(bookingId.ToString(), 1, "testadmin"));
        }

        [Test]
        public void CheckIn_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn("invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID đặt phòng không hợp lệ"));
        }

        [Test]
        public void CheckIn_FutureCheckInDate_ShouldThrowException()
        {
            // Arrange - Create a booking for future date
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn(bookingId.ToString(), 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Không thể check-in trước ngày"));
        }

        #endregion

        #region CheckOut Tests

        [Test]
        public void CheckOut_ValidActiveBooking_ShouldSucceed()
        {
            // Arrange - Create and check-in a booking
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");
            _bookingBLL.CheckIn(bookingId.ToString(), 1, "testadmin");

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.CheckOut(bookingId.ToString(), 1, "testadmin"));
        }

        [Test]
        public void CheckOut_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckOut("invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID đặt phòng không hợp lệ"));
        }

        #endregion

        #region ExtendBooking Tests

        [Test]
        public void ExtendBooking_ValidInput_ShouldSucceed()
        {
            // Arrange - Create a booking
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate, checkOutDate, 1, "testadmin");
            DateTime newEndDate = DateTime.Now.AddDays(5);

            // Act & Assert
            Assert.DoesNotThrow(() => _bookingBLL.ExtendBooking(bookingId.ToString(), newEndDate, 1, "testadmin"));
        }

        [Test]
        public void ExtendBooking_InvalidBookingId_ShouldThrowException()
        {
            // Arrange
            DateTime newEndDate = DateTime.Now.AddDays(5);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.ExtendBooking("invalid", newEndDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID đặt phòng không hợp lệ"));
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
            DateTime pastEndDate = DateTime.Now.AddDays(-1); // Past date

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.ExtendBooking(bookingId.ToString(), pastEndDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Ngày gia hạn phải lớn hơn ngày hiện tại"));
        }

        #endregion

        #region GetBookingHistory Tests

        [Test]
        public void GetBookingHistory_ValidCustomerId_ShouldReturnData()
        {
            // Arrange
            int customerId = 1;
            
            // Create a booking for the customer
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
            
            // Create bookings for the customer
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
        public void CreateBooking_WithNonExistentCustomer_ShouldThrowException()
        {
            // Arrange
            string nonExistentIdCard = "999999999";
            string roomId = "1";
            string checkInDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _bookingBLL.CreateBooking(nonExistentIdCard, roomId, checkInDate, checkOutDate, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Lỗi trong logic nghiệp vụ khi tạo đặt phòng"));
        }

        [Test]
        public void CheckIn_NonExistentBooking_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _bookingBLL.CheckIn("999", 1, "testadmin"));
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
            Assert.That(exception?.Message, Does.Contain("Không thể gia hạn đặt phòng"));
        }

        [Test]
        public void CreateBooking_MultipleBookingsForSameRoom_ShouldHandleConflicts()
        {
            // Arrange
            string idCard = "123456789";
            string roomId = "1";
            string checkInDate1 = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            string checkOutDate1 = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
            string checkInDate2 = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"); // Overlapping dates
            string checkOutDate2 = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd HH:mm:ss");

            // Act - Create first booking
            int bookingId1 = _bookingBLL.CreateBooking(idCard, roomId, checkInDate1, checkOutDate1, 1, "testadmin");
            bookingId1.Should().BeGreaterThan(0);

            // Act & Assert - Try to create overlapping booking
            var exception = Assert.Throws<Exception>(() => 
                _bookingBLL.CreateBooking(idCard, roomId, checkInDate2, checkOutDate2, 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Lỗi trong logic nghiệp vụ khi tạo đặt phòng"));
        }

        #endregion
    }
}
