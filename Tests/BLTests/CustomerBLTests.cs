using NUnit.Framework;
using FluentAssertions;
using System;
using System.Data;
using HotelManagementSystem.Tests.TestHelpers;
using VTCAcademy_PF1127_HotelSync.CustomerManagement;

namespace HotelManagementSystem.Tests.BLTests
{
    /// <summary>
    /// Test class cho CustomerBL - Tests for customer business logic operations
    /// Kiểm tra tất cả các phương thức trong business logic layer của Customer Management
    /// </summary>
    [TestFixture]
    public class CustomerBLTests : BaseTestClass
    {
        private CustomerBL _customerBL = null!;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _customerBL = new CustomerBL();
            SeedTestData(); // Thêm dữ liệu test cơ bản
        }

        #region AddCustomer Tests

        [Test]
        public void AddCustomer_ValidInput_ShouldSucceed()
        {
            // Arrange
            string name = "Nguyen Van Test";
            string idCard = "987654321";
            string phone = "0987654321";
            string email = "test@gmail.com";
            string nationality = "Vietnam";
            int updatedBy = 1;
            string updatedByUsername = "testadmin";

            // Act
            var result = _customerBL.AddCustomer(name, idCard, phone, email, nationality, updatedBy, updatedByUsername);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("Thêm khách hàng thành công");
            result.CustomerId.Should().BeGreaterThan(0);
            
            // Verify customer was added to database
            var customers = ExecuteQuery($"SELECT * FROM Customers WHERE IDCard = '{idCard}'");
            customers.Rows.Count.Should().Be(1);
            customers.Rows[0]["Name"].ToString().Should().Be(name);
            customers.Rows[0]["Phone"].ToString().Should().Be(phone);
            customers.Rows[0]["Email"].ToString().Should().Be(email);
        }

        [Test]
        public void AddCustomer_EmptyName_ShouldReturnError()
        {
            // Act
            var result = _customerBL.AddCustomer("", "987654321", "0987654321", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("không được để trống");
            result.CustomerId.Should().Be(-1);
        }

        [Test]
        public void AddCustomer_EmptyIDCard_ShouldReturnError()
        {
            // Act
            var result = _customerBL.AddCustomer("Test Name", "", "0987654321", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("không được để trống");
            result.CustomerId.Should().Be(-1);
        }

        [Test]
        public void AddCustomer_InvalidPhoneNumber_ShouldReturnError()
        {
            // Act
            var result = _customerBL.AddCustomer("Test Name", "987654321", "12345", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("không hợp lệ");
            result.CustomerId.Should().Be(-1);
        }

        [Test]
        public void AddCustomer_InvalidEmail_ShouldReturnError()
        {
            // Act
            var result = _customerBL.AddCustomer("Test Name", "987654321", "0987654321", "invalid-email", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Email không hợp lệ");
            result.CustomerId.Should().Be(-1);
        }

        [Test]
        public void AddCustomer_DuplicateIDCard_ShouldReturnError()
        {
            // Arrange - IDCard 123456789 already exists from SeedTestData
            string duplicateIDCard = "123456789";

            // Act
            var result = _customerBL.AddCustomer("Test Name", duplicateIDCard, "0987654321", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("đã tồn tại");
            result.CustomerId.Should().Be(-1);
        }

        [Test]
        public void AddCustomer_ValidVietnamesePhoneNumber_ShouldSucceed()
        {
            // Test with various valid Vietnamese phone formats
            var phoneNumbers = new[] { "0901234567", "0123456789", "+84901234567" };
            
            foreach (var phone in phoneNumbers)
            {
                // Arrange
                string idCard = $"TEST{phone.Substring(phone.Length - 6)}"; // Unique IDCard
                
                // Act
                var result = _customerBL.AddCustomer("Test Name", idCard, phone, "test@gmail.com", "Vietnam", 1, "testadmin");

                // Assert
                result.Success.Should().BeTrue($"Phone number {phone} should be valid");
                result.CustomerId.Should().BeGreaterThan(0);
            }
        }

        #endregion

        #region UpdateCustomer Tests

        [Test]
        public void UpdateCustomer_ValidInput_ShouldSucceed()
        {
            // Arrange
            int customerId = 1; // Customer exists from SeedTestData
            string newName = "Updated Name";
            string newPhone = "0999888777";
            string newEmail = "updated@gmail.com";
            string newNationality = "USA";

            // Act
            var result = _customerBL.UpdateCustomer(customerId, newName, newPhone, newEmail, newNationality, 1, "testadmin");

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("Cập nhật thông tin khách hàng thành công");
            
            // Verify customer was updated in database
            var customers = ExecuteQuery($"SELECT * FROM Customers WHERE CustomerID = {customerId}");
            customers.Rows.Count.Should().Be(1);
            customers.Rows[0]["Name"].ToString().Should().Be(newName);
            customers.Rows[0]["Phone"].ToString().Should().Be(newPhone);
            customers.Rows[0]["Email"].ToString().Should().Be(newEmail);
            customers.Rows[0]["Nationality"].ToString().Should().Be(newNationality);
        }

        [Test]
        public void UpdateCustomer_NonExistentId_ShouldReturnError()
        {
            // Act
            var result = _customerBL.UpdateCustomer(999, "Test Name", "0987654321", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Không tìm thấy khách hàng");
        }

        [Test]
        public void UpdateCustomer_EmptyName_ShouldReturnError()
        {
            // Act
            var result = _customerBL.UpdateCustomer(1, "", "0987654321", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("không được để trống");
        }

        [Test]
        public void UpdateCustomer_InvalidPhone_ShouldReturnError()
        {
            // Act
            var result = _customerBL.UpdateCustomer(1, "Test Name", "12345", "test@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("không hợp lệ");
        }

        [Test]
        public void UpdateCustomer_InvalidEmail_ShouldReturnError()
        {
            // Act
            var result = _customerBL.UpdateCustomer(1, "Test Name", "0987654321", "invalid-email", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Email không hợp lệ");
        }

        #endregion

        #region DeleteCustomer Tests - FIXED

        [Test]
        public void DeleteCustomer_ValidId_ShouldSucceed()
        {
            // Arrange - Add a customer to delete
            var addResult = _customerBL.AddCustomer("Delete Test", "DELETE123", "0987654321", "delete@gmail.com", "Vietnam", 1, "testadmin");
            addResult.Success.Should().BeTrue();
            int customerIdToDelete = addResult.CustomerId;

            // Act - FIX: DeleteCustomer chỉ nhận customerId
            var result = _customerBL.DeleteCustomer(customerIdToDelete);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("Xóa khách hàng thành công");
            
            // Verify customer was deleted from database
            var customers = ExecuteQuery($"SELECT * FROM Customers WHERE CustomerID = {customerIdToDelete}");
            customers.Rows.Count.Should().Be(0);
        }

        [Test]
        public void DeleteCustomer_NonExistentId_ShouldReturnError()
        {
            // Act - FIX: DeleteCustomer chỉ nhận customerId
            var result = _customerBL.DeleteCustomer(999);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Không tìm thấy khách hàng");
        }

        [Test]
        public void DeleteCustomer_InvalidId_ShouldReturnError()
        {
            // Act - FIX: DeleteCustomer chỉ nhận customerId  
            var result = _customerBL.DeleteCustomer(0);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Không tìm thấy khách hàng");
        }

        #endregion

        #region GetAllCustomers Tests

        [Test]
        public void GetAllCustomers_ShouldReturnDataTable()
        {
            // Act
            var result = _customerBL.GetAllCustomers();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0); // At least the seeded customer
            result.Columns.Contains("CustomerID").Should().BeTrue();
            result.Columns.Contains("Name").Should().BeTrue();
            result.Columns.Contains("IDCard").Should().BeTrue();
            result.Columns.Contains("Phone").Should().BeTrue();
            result.Columns.Contains("Email").Should().BeTrue();
            result.Columns.Contains("Nationality").Should().BeTrue();
        }

        [Test]
        public void GetAllCustomers_EmptyDatabase_ShouldReturnEmptyDataTable()
        {
            // Arrange - Clear all customers
            ExecuteNonQuery("DELETE FROM Customers");

            // Act
            var result = _customerBL.GetAllCustomers();

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        #endregion

        #region Search Tests - FIXED

        [Test]
        public void SearchCustomersByName_ValidName_ShouldReturnFilteredResults()
        {
            // Arrange
            string searchName = "Test Customer"; // From SeedTestData

            // Act - FIX: Sử dụng SearchCustomersByName thay vì SearchCustomers
            var result = _customerBL.SearchCustomersByName(searchName);

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            
            foreach (DataRow row in result.Rows)
            {
                row["Name"].ToString().Should().Contain(searchName);
            }
        }

        [Test]
        public void SearchCustomersByPhone_ValidPhone_ShouldReturnFilteredResults()
        {
            // Arrange
            string searchPhone = "0901234567"; // From SeedTestData

            // Act - FIX: Sử dụng SearchCustomersByPhone
            var result = _customerBL.SearchCustomersByPhone(searchPhone);

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().BeGreaterThan(0);
            
            foreach (DataRow row in result.Rows)
            {
                row["Phone"].ToString().Should().Be(searchPhone);
            }
        }

        [Test]
        public void SearchCustomersByName_EmptyName_ShouldReturnEmptyResults()
        {
            // Act
            var result = _customerBL.SearchCustomersByName("");

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        [Test]
        public void SearchCustomersByName_TooShort_ShouldReturnEmptyResults()
        {
            // Act
            var result = _customerBL.SearchCustomersByName("A");

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        [Test]
        public void SearchCustomersByPhone_EmptyPhone_ShouldReturnEmptyResults()
        {
            // Act
            var result = _customerBL.SearchCustomersByPhone("");

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        [Test]
        public void SearchCustomersByName_NoMatch_ShouldReturnEmptyResults()
        {
            // Act
            var result = _customerBL.SearchCustomersByName("NonExistentName");

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        #endregion

        #region GetCustomerById Tests - FIXED

        [Test]
        public void GetCustomerById_ValidId_ShouldReturnCustomer()
        {
            // Arrange
            int customerId = 1; // From SeedTestData

            // Act - FIX: GetCustomerById trả về DataRow?, không phải DataTable
            var result = _customerBL.GetCustomerById(customerId);

            // Assert
            result.Should().NotBeNull();
            Convert.ToInt32(result!["CustomerID"]).Should().Be(customerId);
        }

        [Test]
        public void GetCustomerById_InvalidId_ShouldReturnNull()
        {
            // Act - FIX: Trả về null, không phải empty DataTable
            var result = _customerBL.GetCustomerById(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomerById_ZeroId_ShouldReturnNull()
        {
            // Act - FIX: Trả về null, không phải empty DataTable
            var result = _customerBL.GetCustomerById(0);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region NEW Tests - GetCustomerByIdCard

        [Test]
        public void GetCustomerByIdCard_ValidIdCard_ShouldReturnCustomer()
        {
            // Arrange
            string idCard = "123456789"; // From SeedTestData

            // Act
            var result = _customerBL.GetCustomerByIdCard(idCard);

            // Assert
            result.Should().NotBeNull();
            result!["IDCard"].ToString().Should().Be(idCard);
        }

        [Test]
        public void GetCustomerByIdCard_InvalidIdCard_ShouldReturnNull()
        {
            // Act
            var result = _customerBL.GetCustomerByIdCard("NONEXISTENT");

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomerByIdCard_EmptyIdCard_ShouldReturnNull()
        {
            // Act
            var result = _customerBL.GetCustomerByIdCard("");

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomerByIdCard_NullIdCard_ShouldReturnNull()
        {
            // Act
            var result = _customerBL.GetCustomerByIdCard(null!);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region NEW Tests - GetCustomerHistory

        [Test]
        public void GetCustomerHistory_ValidId_ShouldReturnHistory()
        {
            // Arrange
            int customerId = 1; // From SeedTestData

            // Act
            var result = _customerBL.GetCustomerHistory(customerId);

            // Assert
            result.BookingHistory.Should().NotBeNull();
            result.InvoiceHistory.Should().NotBeNull();
            // Note: Empty history is OK for new customers
        }

        [Test]
        public void GetCustomerHistory_InvalidId_ShouldReturnEmptyTables()
        {
            // Act
            var result = _customerBL.GetCustomerHistory(999);

            // Assert
            result.BookingHistory.Should().NotBeNull();
            result.InvoiceHistory.Should().NotBeNull();
            result.BookingHistory.Rows.Count.Should().Be(0);
            result.InvoiceHistory.Rows.Count.Should().Be(0);
        }

        [Test]
        public void GetCustomerHistory_ZeroId_ShouldReturnEmptyTables()
        {
            // Act
            var result = _customerBL.GetCustomerHistory(0);

            // Assert
            result.BookingHistory.Should().NotBeNull();
            result.InvoiceHistory.Should().NotBeNull();
            result.BookingHistory.Rows.Count.Should().Be(0);
            result.InvoiceHistory.Rows.Count.Should().Be(0);
        }

        #endregion

        #region NEW Tests - GetCustomerStatistics

        [Test]
        public void GetCustomerStatistics_ShouldReturnValidStatistics()
        {
            // Act
            var result = _customerBL.GetCustomerStatistics();

            // Assert
            result.TotalCustomers.Should().BeGreaterThanOrEqualTo(0);
            result.CustomersWithBookings.Should().BeGreaterThanOrEqualTo(0);
            result.CustomersWithInvoices.Should().BeGreaterThanOrEqualTo(0);
            result.CustomersWithBookings.Should().BeLessThanOrEqualTo(result.TotalCustomers);
            result.CustomersWithInvoices.Should().BeLessThanOrEqualTo(result.TotalCustomers);
        }

        #endregion

        #region NEW Tests - FormatCustomerInfo

        [Test]
        public void FormatCustomerInfo_ValidCustomer_ShouldReturnFormattedString()
        {
            // Arrange
            var customer = _customerBL.GetCustomerById(1); // From SeedTestData
            customer.Should().NotBeNull();

            // Act
            var result = _customerBL.FormatCustomerInfo(customer!);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("ID:");
            result.Should().Contain("Tên:");
            result.Should().Contain("CMND/Passport:");
            result.Should().Contain("Điện thoại:");
            result.Should().Contain("Email:");
            result.Should().Contain("Quốc tịch:");
        }

        [Test]
        public void FormatCustomerInfo_NullCustomer_ShouldReturnErrorMessage()
        {
            // Act
            var result = _customerBL.FormatCustomerInfo(null!);

            // Assert
            result.Should().Be("Không có thông tin khách hàng.");
        }

        #endregion

        #region Validation Tests

        [Test]
        public void ValidateCustomerData_ValidInput_ShouldReturnValid()
        {
            // This test assumes there's a public validation method
            // If validation is private, we test it indirectly through AddCustomer
            
            // Act - Test through AddCustomer
            var result = _customerBL.AddCustomer("Valid Name", "VALID123", "0901234567", "valid@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeTrue();
        }

        [Test]
        [TestCase("", "Name cannot be empty")]
        [TestCase("A", "Name too short")]
        [TestCase("ThisIsAVeryLongNameThatExceedsTheMaximumAllowedCharacterLimitForCustomerNameFieldInDatabase", "Name too long")]
        public void ValidateCustomerData_InvalidName_ShouldReturnError(string invalidName, string testDescription)
        {
            // Act
            var result = _customerBL.AddCustomer(invalidName, "VALID123", "0901234567", "valid@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse(testDescription);
        }

        [Test]
        [TestCase("invalid-email", "Invalid email format")]
        [TestCase("@gmail.com", "Invalid email format")]
        [TestCase("test@", "Invalid email format")]
        [TestCase("test@.com", "Invalid email format")]
        public void ValidateCustomerData_InvalidEmail_ShouldReturnError(string invalidEmail, string testDescription)
        {
            // Act
            var result = _customerBL.AddCustomer("Valid Name", "VALID123", "0901234567", invalidEmail, "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse(testDescription);
        }

        [Test]
        [TestCase("12345", "Too short")]
        [TestCase("123456789012345678901", "Too long")]
        [TestCase("abcdefghij", "Non-numeric")]
        [TestCase("090-123-4567", "Invalid format with dashes")]
        public void ValidateCustomerData_InvalidPhone_ShouldReturnError(string invalidPhone, string testDescription)
        {
            // Act
            var result = _customerBL.AddCustomer("Valid Name", "VALID123", invalidPhone, "valid@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeFalse(testDescription);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Test]
        public void AddCustomer_WithSpecialCharactersInName_ShouldSucceed()
        {
            // Arrange
            string nameWithSpecialChars = "Nguyễn Văn Đức-Anh";

            // Act
            var result = _customerBL.AddCustomer(nameWithSpecialChars, "SPECIAL123", "0901234567", "special@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeTrue();
            
            // Verify customer was added correctly
            var customers = ExecuteQuery($"SELECT * FROM Customers WHERE IDCard = 'SPECIAL123'");
            customers.Rows.Count.Should().Be(1);
            customers.Rows[0]["Name"].ToString().Should().Be(nameWithSpecialChars);
        }

        [Test]
        public void AddCustomer_WithInternationalPhoneNumber_ShouldSucceed()
        {
            // Arrange
            string internationalPhone = "+84901234567";

            // Act
            var result = _customerBL.AddCustomer("International Customer", "INTL123", internationalPhone, "intl@gmail.com", "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeTrue();
        }

        [Test]
        public void UpdateCustomer_PartialUpdate_ShouldSucceed()
        {
            // Arrange - Get existing customer data
            var existingCustomers = ExecuteQuery("SELECT * FROM Customers WHERE CustomerID = 1");
            existingCustomers.Rows.Count.Should().Be(1);
            string originalEmail = existingCustomers.Rows[0]["Email"].ToString()!;

            // Act - Update only name and phone
            var result = _customerBL.UpdateCustomer(1, "Updated Name Only", "0999888777", originalEmail, "Vietnam", 1, "testadmin");

            // Assert
            result.Success.Should().BeTrue();
            
            // Verify only specified fields were updated
            var updatedCustomers = ExecuteQuery("SELECT * FROM Customers WHERE CustomerID = 1");
            updatedCustomers.Rows[0]["Name"].ToString().Should().Be("Updated Name Only");
            updatedCustomers.Rows[0]["Phone"].ToString().Should().Be("0999888777");
            updatedCustomers.Rows[0]["Email"].ToString().Should().Be(originalEmail);
        }

        [Test]
        public void CustomerLifecycle_AddUpdateDelete_ShouldWorkCorrectly()
        {
            // Arrange
            string name = "Lifecycle Test";
            string idCard = "LIFECYCLE123";

            // Act & Assert - Add
            var addResult = _customerBL.AddCustomer(name, idCard, "0901234567", "lifecycle@gmail.com", "Vietnam", 1, "testadmin");
            addResult.Success.Should().BeTrue();
            int customerId = addResult.CustomerId;

            // Act & Assert - Update
            var updateResult = _customerBL.UpdateCustomer(customerId, "Updated Lifecycle", "0999888777", "updated@gmail.com", "USA", 1, "testadmin");
            updateResult.Success.Should().BeTrue();

            // Act & Assert - Verify Update
            var customer = _customerBL.GetCustomerById(customerId);
            customer.Should().NotBeNull();
            customer!["Name"].ToString().Should().Be("Updated Lifecycle");

            // Act & Assert - Delete
            var deleteResult = _customerBL.DeleteCustomer(customerId);
            deleteResult.Success.Should().BeTrue();

            // Act & Assert - Verify Delete
            var deletedCustomer = _customerBL.GetCustomerById(customerId);
            deletedCustomer.Should().BeNull();
        }

        #endregion
    }
}
