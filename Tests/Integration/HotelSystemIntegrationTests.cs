using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;

namespace HotelManagementSystem.Tests.Integration
{
    /// <summary>
    /// Integration tests kiểm tra tương tác giữa các modules
    /// </summary>
    [TestFixture]
    public class HotelSystemIntegrationTests : BaseTestClass
    {
        private RoomBLL _roomBLL = null!;
        private BookingBLL _bookingBLL = null!;
        // private CustomerBL _customerBL = null!;
        // private InvoiceBL _invoiceBL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _roomBLL = new RoomBLL();
            _bookingBLL = new BookingBLL();
            // _customerBL = new CustomerBL();
            // _invoiceBL = new InvoiceBL();
            SeedTestData();
            SeedIntegrationTestData();
        }

        private void SeedIntegrationTestData()
        {
            // Tạo các bảng cần thiết cho integration tests
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

            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Invoices (
                    InvoiceID INT AUTO_INCREMENT PRIMARY KEY,
                    BookingID INT NOT NULL,
                    CustomerID INT NOT NULL,
                    TotalAmount DECIMAL(10, 2) NOT NULL,
                    IssueDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    PaymentStatus ENUM('Paid', 'Unpaid') NOT NULL DEFAULT 'Unpaid',
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    UpdatedBy INT NULL,
                    UpdatedByUsername VARCHAR(50) NULL 
                );");
        }

        #region Room-Booking Integration Tests

        [Test]
        public void BookingWorkflow_CreateBookingAndCheckAvailability_ShouldWork()
        {
            // Arrange - Add a room first
            _roomBLL.AddRoom("201", "Double", "800000", "[\"TV\", \"WiFi\"]", 1, "testadmin");
            
            // Get the room ID
            var rooms = ExecuteQuery("SELECT RoomID FROM Rooms WHERE RoomNumber = '201'");
            string roomId = rooms.Rows[0]["RoomID"].ToString()!;

            DateTime checkInDate = DateTime.Now.AddDays(1);
            DateTime checkOutDate = DateTime.Now.AddDays(3);

            // Act - Check availability before booking
            DataTable availableRooms = _roomBLL.CheckRoomAvailability(checkInDate, checkOutDate);
            
            // Assert - Room should be available
            availableRooms.Rows.Count.Should().BeGreaterThan(0);
            bool roomIsAvailable = false;
            foreach (DataRow row in availableRooms.Rows)
            {
                if (row["RoomID"].ToString() == roomId)
                {
                    roomIsAvailable = true;
                    break;
                }
            }
            roomIsAvailable.Should().BeTrue();

            // Act - Create booking
            string idCard = "123456789";
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate.ToString("yyyy-MM-dd HH:mm:ss"), checkOutDate.ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");

            // Assert - Booking should be created
            bookingId.Should().BeGreaterThan(0);

            // Act - Check availability after booking
            DataTable availableRoomsAfter = _roomBLL.CheckRoomAvailability(checkInDate, checkOutDate);

            // Assert - Room should not be available anymore
            bool roomStillAvailable = false;
            foreach (DataRow row in availableRoomsAfter.Rows)
            {
                if (row["RoomID"].ToString() == roomId)
                {
                    roomStillAvailable = true;
                    break;
                }
            }
            roomStillAvailable.Should().BeFalse("Room should not be available after booking");
        }

        [Test]
        public void CheckInCheckOutWorkflow_ShouldUpdateRoomStatus()
        {
            // Arrange - Create a booking
            string idCard = "123456789";
            string roomId = "1";
            DateTime checkInDate = DateTime.Now;
            DateTime checkOutDate = DateTime.Now.AddDays(2);
            
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate.ToString("yyyy-MM-dd HH:mm:ss"), checkOutDate.ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");

            // Act - Check in
            _bookingBLL.CheckIn(bookingId.ToString(), idCard, 1, "testadmin");


            // Assert - Room status should be 'Occupied'
            var rooms = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = {roomId}");
            rooms.Rows[0]["Status"].ToString().Should().Be("Occupied");

            // Act - Check out
            _bookingBLL.CheckOut(bookingId.ToString(), 1, "testadmin");

            // Assert - Room status should be 'Uncleaned'
            var roomsAfterCheckout = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = {roomId}");
            roomsAfterCheckout.Rows[0]["Status"].ToString().Should().Be("Uncleaned");

            // Act - Clean room
            _roomBLL.CleanRoom(roomId, 1, "testadmin");

            // Assert - Room status should be 'Available'
            var roomsAfterCleaning = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = {roomId}");
            roomsAfterCleaning.Rows[0]["Status"].ToString().Should().Be("Available");
        }

        #endregion

        #region Customer-Booking Integration Tests

        [Test]
        public void CustomerBookingHistory_ShouldReturnCorrectData()
        {
            // Arrange - Create multiple bookings for the same customer
            string idCard = "123456789";
            int customerId = 1;
            
            int booking1 = _bookingBLL.CreateBooking(idCard, "1", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");
            int booking2 = _bookingBLL.CreateBooking(idCard, "1", DateTime.Now.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");

            // Act - Get booking history
            DataTable history = _bookingBLL.GetBookingHistory(customerId, null);

            // Assert - Should return all bookings for the customer
            history.Rows.Count.Should().BeGreaterOrEqualTo(2);
            
            foreach (DataRow row in history.Rows)
            {
                Convert.ToInt32(row["CustomerID"]).Should().Be(customerId);
            }
        }

        #endregion

        #region Complete Hotel Workflow Tests

        [Test]
        public void CompleteHotelWorkflow_AddRoomToInvoice_ShouldWork()
        {
            // Arrange - Add a new room
            _roomBLL.AddRoom("301", "Suite", "1500000", "[\"TV\", \"WiFi\", \"Jacuzzi\"]", 1, "testadmin");
            var rooms = ExecuteQuery("SELECT RoomID FROM Rooms WHERE RoomNumber = '301'");
            string roomId = rooms.Rows[0]["RoomID"].ToString()!;

            // Step 1: Create booking
            string idCard = "123456789";
            DateTime checkInDate = DateTime.Now;
            DateTime checkOutDate = DateTime.Now.AddDays(2);
            
            int bookingId = _bookingBLL.CreateBooking(idCard, roomId, checkInDate.ToString("yyyy-MM-dd HH:mm:ss"), checkOutDate.ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");
            bookingId.Should().BeGreaterThan(0);

            // Step 2: Check in
            _bookingBLL.CheckIn(bookingId.ToString(), idCard, 1, "testadmin");

            // Verify room is occupied
            var roomStatus = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = {roomId}");
            roomStatus.Rows[0]["Status"].ToString().Should().Be("Occupied");

            // Step 3: Check out
            _bookingBLL.CheckOut(bookingId.ToString(), 1, "testadmin");
            
            // Verify booking is completed
            var bookingStatus = ExecuteQuery($"SELECT Status FROM Bookings WHERE BookingID = {bookingId}");
            bookingStatus.Rows[0]["Status"].ToString().Should().Be("Completed");

            // Step 4: Clean room
            _roomBLL.CleanRoom(roomId, 1, "testadmin");
            
            // Verify room is available again
            var finalRoomStatus = ExecuteQuery($"SELECT Status FROM Rooms WHERE RoomID = {roomId}");
            finalRoomStatus.Rows[0]["Status"].ToString().Should().Be("Available");

            // Assert - Complete workflow should work without errors
            Assert.Pass("Complete hotel workflow executed successfully");
        }

        [Test]
        public void ExtendBookingWorkflow_ShouldUpdateDatesCorrectly()
        {
            // Arrange - Create a booking
            string idCard = "123456789";
            DateTime originalCheckIn = DateTime.Now.AddDays(1);
            DateTime originalCheckOut = DateTime.Now.AddDays(3);
            DateTime extendedCheckOut = DateTime.Now.AddDays(5);
            
            int bookingId = _bookingBLL.CreateBooking(idCard, "1", originalCheckIn.ToString("yyyy-MM-dd HH:mm:ss"), originalCheckOut.ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");

            // Act - Extend booking
            _bookingBLL.ExtendBooking(bookingId.ToString(), extendedCheckOut, 1, "testadmin");

            // Assert - Check that dates were updated
            var updatedBooking = ExecuteQuery($"SELECT CheckOutDate FROM Bookings WHERE BookingID = {bookingId}");
            DateTime actualCheckOut = Convert.ToDateTime(updatedBooking.Rows[0]["CheckOutDate"]);
            
            // Allow for small time differences due to database precision
            actualCheckOut.Date.Should().Be(extendedCheckOut.Date);
        }

        #endregion

        #region Error Handling Integration Tests

        [Test]
        public void BookingConflict_ShouldPreventOverlappingBookings()
        {
            // Arrange - Create first booking
            string idCard = "123456789";
            DateTime checkIn1 = DateTime.Now.AddDays(1);
            DateTime checkOut1 = DateTime.Now.AddDays(3);
            
            int booking1 = _bookingBLL.CreateBooking(idCard, "1", checkIn1.ToString("yyyy-MM-dd HH:mm:ss"), checkOut1.ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");
            booking1.Should().BeGreaterThan(0);

            // Act & Assert - Try to create overlapping booking
            DateTime checkIn2 = DateTime.Now.AddDays(2); // Overlaps with first booking
            DateTime checkOut2 = DateTime.Now.AddDays(4);
            
            var exception = Assert.Throws<Exception>(() => 
                _bookingBLL.CreateBooking(idCard, "1", checkIn2.ToString("yyyy-MM-dd HH:mm:ss"), checkOut2.ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin"));
            
            Assert.That(exception?.Message, Does.Contain("Lỗi trong logic nghiệp vụ khi tạo đặt phòng"));
        }

        [Test]
        public void DeleteRoomWithActiveBooking_ShouldFail()
        {
            // Arrange - Create booking
            string idCard = "123456789";
            int bookingId = _bookingBLL.CreateBooking(idCard, "1", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");
            bookingId.Should().BeGreaterThan(0);

            // Act & Assert - Try to delete room with active booking
            var exception = Assert.Throws<Exception>(() => _roomBLL.DeleteRoom("1", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Lỗi trong logic nghiệp vụ khi xóa phòng"));
        }

        #endregion

        #region Performance Tests

        [Test]
        public void SearchRooms_WithLargeDataset_ShouldPerformWell()
        {
            // Arrange - Add multiple rooms
            for (int i = 100; i < 200; i++)
            {
                string roomType = i % 3 == 0 ? "Single" : i % 3 == 1 ? "Double" : "Suite";
                string price = (500000 + (i * 10000)).ToString();
                _roomBLL.AddRoom($"R{i}", roomType, price, "[\"TV\", \"WiFi\"]", 1, "testadmin");
            }

            // Act - Measure search performance
            var startTime = DateTime.Now;
            DataTable results = _roomBLL.SearchRooms("Available", "", null, null);
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            // Assert - Should complete within reasonable time (< 5 seconds)
            duration.TotalSeconds.Should().BeLessThan(5, "Search should complete within 5 seconds");
            results.Rows.Count.Should().BeGreaterThan(100, "Should return multiple rooms");
        }

        #endregion

        #region Data Consistency Tests

        [Test]
        public void CancelBooking_ShouldMaintainDataConsistency()
        {
            // Arrange - Create and check in booking
            string idCard = "123456789";
            int bookingId = _bookingBLL.CreateBooking(idCard, "1", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"), 1, "testadmin");
            _bookingBLL.CheckIn(bookingId.ToString(), idCard, 1, "testadmin");

            // Verify room is occupied
            var roomStatus = ExecuteQuery("SELECT Status FROM Rooms WHERE RoomID = 1");
            roomStatus.Rows[0]["Status"].ToString().Should().Be("Occupied");

            // Act - Cancel booking
            _bookingBLL.CancelBooking(bookingId.ToString(), 1, "testadmin");

            // Assert - Room should be available again
            var finalRoomStatus = ExecuteQuery("SELECT Status FROM Rooms WHERE RoomID = 1");
            finalRoomStatus.Rows[0]["Status"].ToString().Should().Be("Available");

            // Booking should be cancelled
            var bookingStatus = ExecuteQuery($"SELECT Status FROM Bookings WHERE BookingID = {bookingId}");
            bookingStatus.Rows[0]["Status"].ToString().Should().Be("Cancelled");
        }

        #endregion
    }
}
