using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;
using HotelManagementSystem.InvoiceManagement;

namespace HotelManagementSystem.Tests.BLTests
{
    /// <summary>
    /// Test class cho InvoiceBL
    /// Kiểm tra tất cả các phương thức trong business logic layer của Invoice Management
    /// </summary>
    [TestFixture]
    public class InvoiceBLTests : BaseTestClass
    {
        private InvoiceBL _invoiceBL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _invoiceBL = new InvoiceBL();
            SeedTestData(); // Thêm dữ liệu test cơ bản
            SeedInvoiceTestData(); // Thêm dữ liệu test cho invoice
        }

        /// <summary>
        /// Thêm dữ liệu test cần thiết cho invoice
        /// </summary>
        private void SeedInvoiceTestData()
        {
            // Thêm invoice table
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

            // Thêm bookings table để test
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

            // Thêm booking test data
            ExecuteNonQuery(@"
                INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, Status, UpdatedBy, UpdatedByUsername) 
                VALUES (1, '123456789', 1, NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY), 'Active', 1, 'testadmin');");
        }

        #region CreateInvoice Tests

        [Test]
        public void CreateInvoice_ValidInput_ShouldSucceed()
        {
            // Arrange
            string bookingId = "1";
            string customerId = "1";
            string totalAmount = "1000000";
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act & Assert
            Assert.DoesNotThrow(() => _invoiceBL.CreateInvoice(bookingId, customerId, totalAmount, updatedBy, updatedByUsername));
            
            // Verify invoice was created
            var invoices = ExecuteQuery($"SELECT * FROM Invoices WHERE BookingID = {bookingId} AND CustomerID = {customerId}");
            invoices.Rows.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public void CreateInvoice_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoice("invalid", "1", "1000000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID booking phải là số nguyên dương"));
        }

        [Test]
        public void CreateInvoice_ZeroBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoice("0", "1", "1000000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID booking phải là số nguyên dương"));
        }

        [Test]
        public void CreateInvoice_InvalidCustomerId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoice("1", "invalid", "1000000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID khách hàng phải là số nguyên dương"));
        }

        [Test]
        public void CreateInvoice_NegativeAmount_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoice("1", "1", "-1000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Tổng tiền phải là số không âm"));
        }

        [Test]
        public void CreateInvoice_InvalidAmountFormat_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoice("1", "1", "invalid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Tổng tiền phải là số không âm"));
        }

        [Test]
        public void CreateInvoice_ExcessiveAmount_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoice("1", "1", "200000000", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Tổng tiền không được vượt quá 100,000,000 VND"));
        }

        [Test]
        public void CreateInvoice_ValidMaxAmount_ShouldSucceed()
        {
            // Arrange
            string maxAmount = "100000000"; // Exactly 100 million

            // Act & Assert
            Assert.DoesNotThrow(() => _invoiceBL.CreateInvoice("1", "1", maxAmount, 1, "testadmin"));
        }

        #endregion

        #region CreateInvoiceWithAutoCalculation Tests

        [Test]
        public void CreateInvoiceWithAutoCalculation_ValidInput_ShouldSucceed()
        {
            // Arrange
            string bookingId = "1";
            string customerId = "1";
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act & Assert
            Assert.DoesNotThrow(() => _invoiceBL.CreateInvoiceWithAutoCalculation(bookingId, customerId, updatedBy, updatedByUsername));
            
            // Verify invoice was created with calculated amount
            var invoices = ExecuteQuery($"SELECT * FROM Invoices WHERE BookingID = {bookingId} AND CustomerID = {customerId}");
            invoices.Rows.Count.Should().BeGreaterThan(0);
            
            // Amount should be calculated automatically (greater than 0)
            decimal amount = Convert.ToDecimal(invoices.Rows[0]["TotalAmount"]);
            amount.Should().BeGreaterThan(0);
        }

        [Test]
        public void CreateInvoiceWithAutoCalculation_InvalidBookingId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.CreateInvoiceWithAutoCalculation("invalid", "1", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID booking phải là số nguyên dương"));
        }

        #endregion

        #region UpdatePaymentStatus Tests

        [Test]
        public void UpdatePaymentStatus_ValidInput_ShouldSucceed()
        {
            // Arrange - Create an invoice first
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            var invoices = ExecuteQuery("SELECT InvoiceID FROM Invoices ORDER BY InvoiceID DESC LIMIT 1");
            string invoiceId = invoices.Rows[0]["InvoiceID"].ToString()!;

            // Act & Assert
            Assert.DoesNotThrow(() => _invoiceBL.UpdatePaymentStatus(invoiceId, "Paid", 1, "testadmin"));
            
            // Verify payment status was updated
            var updatedInvoices = ExecuteQuery($"SELECT PaymentStatus FROM Invoices WHERE InvoiceID = {invoiceId}");
            updatedInvoices.Rows[0]["PaymentStatus"].ToString().Should().Be("Paid");
        }

        [Test]
        public void UpdatePaymentStatus_InvalidInvoiceId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.UpdatePaymentStatus("invalid", "Paid", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("ID hóa đơn phải là số nguyên dương"));
        }

        [Test]
        public void UpdatePaymentStatus_InvalidStatus_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.UpdatePaymentStatus("1", "InvalidStatus", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Trạng thái thanh toán phải là 'Paid' hoặc 'Unpaid'"));
        }

        [Test]
        public void UpdatePaymentStatus_EmptyStatus_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _invoiceBL.UpdatePaymentStatus("1", "", 1, "testadmin"));
            Assert.That(exception?.Message, Does.Contain("Trạng thái thanh toán không được để trống"));
        }

        #endregion

        #region GetAllInvoices Tests

        [Test]
        public void ViewAllInvoices_ShouldNotThrowException()
        {
            // Arrange - Create an invoice first
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.ViewAllInvoices());
        }

        [Test]
        public void ViewAllInvoices_EmptyDatabase_ShouldNotThrowException()
        {
            // Arrange - Clear all invoices
            ExecuteNonQuery("DELETE FROM Invoices");

            // Act & Assert - Method should handle empty database gracefully
            Assert.DoesNotThrow(() => _invoiceBL.ViewAllInvoices());
        }

        #endregion

        #region SearchInvoices Tests

        [Test]
        public void SearchInvoices_ByCustomerName_ShouldNotThrowException()
        {
            // Arrange - Create an invoice first
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            string customerName = "Test Customer"; // From SeedTestData

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.SearchInvoices(customerName, "", "", "", "", ""));
        }

        [Test]
        public void SearchInvoices_ByPaymentStatus_ShouldNotThrowException()
        {
            // Arrange - Create invoices with different payment statuses
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            var invoices = ExecuteQuery("SELECT InvoiceID FROM Invoices ORDER BY InvoiceID DESC LIMIT 1");
            string invoiceId = invoices.Rows[0]["InvoiceID"].ToString()!;
            _invoiceBL.UpdatePaymentStatus(invoiceId, "Paid", 1, "testadmin");

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.SearchInvoices("", "", "", "Paid", "", ""));
        }

        [Test]
        public void SearchInvoices_ByDateRange_ShouldNotThrowException()
        {
            // Arrange - Create an invoice
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            string startDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string endDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd");

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.SearchInvoices("", startDate, endDate, "", "", ""));
        }

        [Test]
        public void SearchInvoices_ByAmountRange_ShouldNotThrowException()
        {
            // Arrange - Create invoices with different amounts
            _invoiceBL.CreateInvoice("1", "1", "500000", 1, "testadmin");
            _invoiceBL.CreateInvoice("1", "1", "1500000", 1, "testadmin");

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.SearchInvoices("", "", "", "", "1000000", "2000000"));
        }

        #endregion

        #region ViewInvoiceDetails Tests

        [Test]
        public void ViewInvoiceDetails_ValidId_ShouldNotThrowException()
        {
            // Arrange - Create an invoice first
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            var invoices = ExecuteQuery("SELECT InvoiceID FROM Invoices ORDER BY InvoiceID DESC LIMIT 1");
            string invoiceId = invoices.Rows[0]["InvoiceID"].ToString()!;

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.ViewInvoiceDetails(invoiceId));
        }

        [Test]
        public void ViewInvoiceDetails_InvalidId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _invoiceBL.ViewInvoiceDetails("invalid"));
            Assert.That(exception?.Message, Does.Contain("ID hóa đơn phải là số nguyên dương"));
        }

        [Test]
        public void ViewInvoiceDetails_NonExistentId_ShouldNotThrowException()
        {
            // Act & Assert - Should handle gracefully 
            Assert.DoesNotThrow(() => _invoiceBL.ViewInvoiceDetails("999"));
        }

        #endregion

        #region PrintInvoice Tests

        [Test]
        public void PrintInvoice_ValidId_ShouldNotThrowException()
        {
            // Arrange - Create an invoice first
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            var invoices = ExecuteQuery("SELECT InvoiceID FROM Invoices ORDER BY InvoiceID DESC LIMIT 1");
            string invoiceId = invoices.Rows[0]["InvoiceID"].ToString()!;

            // Act & Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.PrintInvoice(invoiceId));
        }

        [Test]
        public void PrintInvoice_InvalidId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _invoiceBL.PrintInvoice("invalid"));
            Assert.That(exception?.Message, Does.Contain("ID hóa đơn phải là số nguyên dương"));
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Test]
        public void CreateInvoice_ZeroAmount_ShouldSucceed()
        {
            // Arrange - Zero amount should be valid (free services)
            string totalAmount = "0";

            // Act & Assert
            Assert.DoesNotThrow(() => _invoiceBL.CreateInvoice("1", "1", totalAmount, 1, "testadmin"));
            
            // Verify invoice was created with zero amount
            var invoices = ExecuteQuery("SELECT * FROM Invoices WHERE TotalAmount = 0");
            invoices.Rows.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public void InvoiceLifecycle_CreateUpdatePrint_ShouldWorkCorrectly()
        {
            // Arrange & Act - Create
            _invoiceBL.CreateInvoice("1", "1", "1000000", 1, "testadmin");
            var invoices = ExecuteQuery("SELECT InvoiceID FROM Invoices ORDER BY InvoiceID DESC LIMIT 1");
            string invoiceId = invoices.Rows[0]["InvoiceID"].ToString()!;

            // Act - Update payment status
            Assert.DoesNotThrow(() => _invoiceBL.UpdatePaymentStatus(invoiceId, "Paid", 1, "testadmin"));

            // Act - Print invoice
            Assert.DoesNotThrow(() => _invoiceBL.PrintInvoice(invoiceId));

            // Assert - Verify payment status was updated
            var updatedInvoices = ExecuteQuery($"SELECT PaymentStatus FROM Invoices WHERE InvoiceID = {invoiceId}");
            updatedInvoices.Rows[0]["PaymentStatus"].ToString().Should().Be("Paid");
        }

        [Test]
        public void CreateMultipleInvoices_ForSameBooking_ShouldSucceed()
        {
            // Act - Create multiple invoices for the same booking (e.g., partial payments)
            _invoiceBL.CreateInvoice("1", "1", "500000", 1, "testadmin");
            _invoiceBL.CreateInvoice("1", "1", "300000", 1, "testadmin");

            // Assert - Both invoices should be created
            var invoices = ExecuteQuery("SELECT * FROM Invoices WHERE BookingID = 1");
            invoices.Rows.Count.Should().BeGreaterOrEqualTo(2);
        }

        [Test]
        public void SearchInvoices_ComplexCriteria_ShouldNotThrowException()
        {
            // Arrange - Create invoices with different properties
            _invoiceBL.CreateInvoice("1", "1", "800000", 1, "testadmin");
            _invoiceBL.CreateInvoice("1", "1", "1200000", 1, "testadmin");
            
            var invoices = ExecuteQuery("SELECT InvoiceID FROM Invoices ORDER BY InvoiceID DESC LIMIT 1");
            string invoiceId = invoices.Rows[0]["InvoiceID"].ToString()!;
            _invoiceBL.UpdatePaymentStatus(invoiceId, "Paid", 1, "testadmin");

            // Act - Search with multiple criteria
            string startDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string endDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd");
            
            // Assert - Method should not throw exception
            Assert.DoesNotThrow(() => _invoiceBL.SearchInvoices("Test", startDate, endDate, "Paid", "1000000", "2000000"));
        }

        #endregion
    }
}
