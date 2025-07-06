# Hotel Management System - Test Documentation

## Tổng quan về Test System

Hệ thống test cho Hotel Management System được thiết kế để đảm bảo chất lượng và độ tin cậy của toàn bộ ứng dụng. Hệ thống bao gồm các loại test khác nhau để kiểm tra từng component và tương tác giữa các module.

## Cấu trúc Test

```
Tests/
├── TestHelpers/
│   └── BaseTestClass.cs           # Base class cho tất cả tests
├── BLTests/
│   ├── RoomBLLTests.cs           # Tests cho Room Business Logic
│   ├── BookingBLLTests.cs        # Tests cho Booking Business Logic
│   ├── CustomerBLTests.cs        # Tests cho Customer Business Logic
│   ├── InvoiceBLTests.cs         # Tests cho Invoice Business Logic
│   └── ServiceBLTests.cs         # Tests cho Service Business Logic
├── DALTests/                     # Tests cho Data Access Layer (future)
├── Integration/
│   └── HotelSystemIntegrationTests.cs  # Integration tests
└── TestRunner.cs                 # Custom test runner

```

## Các loại Test

### 1. Unit Tests (BL Tests)
- **Mục đích**: Kiểm tra từng phương thức trong Business Logic Layer
- **Phạm vi**: Validation, business rules, error handling
- **Framework**: NUnit + FluentAssertions

#### RoomBLLTests
- `AddRoom_ValidInput_ShouldSucceed()`: Kiểm tra thêm phòng hợp lệ
- `AddRoom_InvalidInput_ShouldThrowException()`: Kiểm tra validation đầu vào
- `UpdateRoom_ValidInput_ShouldSucceed()`: Kiểm tra cập nhật phòng
- `DeleteRoom_ValidInput_ShouldSucceed()`: Kiểm tra xóa phòng
- `CleanRoom_ValidInput_ShouldSucceed()`: Kiểm tra dọn phòng
- `SearchRooms_ValidCriteria_ShouldReturnFilteredResults()`: Kiểm tra tìm kiếm
- `CheckRoomAvailability_ValidDateRange_ShouldReturnAvailableRooms()`: Kiểm tra tình trạng phòng

#### BookingBLLTests
- `CreateBooking_ValidInput_ShouldSucceed()`: Kiểm tra tạo đặt phòng
- `CancelBooking_ValidBookingId_ShouldSucceed()`: Kiểm tra hủy đặt phòng
- `CheckIn_ValidBookingId_ShouldSucceed()`: Kiểm tra check-in
- `CheckOut_ValidActiveBooking_ShouldSucceed()`: Kiểm tra check-out
- `ExtendBooking_ValidInput_ShouldSucceed()`: Kiểm tra gia hạn đặt phòng
- `GetBookingHistory_ValidCustomerId_ShouldReturnData()`: Kiểm tra lịch sử đặt phòng

#### CustomerBLTests
- `AddCustomer_ValidInput_ShouldSucceed()`: Kiểm tra thêm khách hàng
- `UpdateCustomer_ValidInput_ShouldSucceed()`: Kiểm tra cập nhật thông tin
- `DeleteCustomer_ValidId_ShouldSucceed()`: Kiểm tra xóa khách hàng
- `SearchCustomers_ByName_ShouldReturnFilteredResults()`: Kiểm tra tìm kiếm
- `ValidateCustomerData_InvalidEmail_ShouldReturnError()`: Kiểm tra validation

#### InvoiceBLTests
- `CreateInvoice_ValidInput_ShouldSucceed()`: Kiểm tra tạo hóa đơn
- `CreateInvoiceWithAutoCalculation_ValidInput_ShouldSucceed()`: Kiểm tra tính toán tự động
- `UpdatePaymentStatus_ValidInput_ShouldSucceed()`: Kiểm tra cập nhật trạng thái thanh toán
- `SearchInvoices_ByCustomerName_ShouldReturnFilteredResults()`: Kiểm tra tìm kiếm hóa đơn

#### ServiceBLTests
- `AddService_ValidInput_ShouldSucceed()`: Kiểm tra thêm dịch vụ
- `UpdateService_ValidInput_ShouldSucceed()`: Kiểm tra cập nhật dịch vụ
- `DeleteService_ValidId_ShouldSucceed()`: Kiểm tra xóa dịch vụ
- `SearchServices_ByType_ShouldReturnFilteredResults()`: Kiểm tra tìm kiếm dịch vụ

### 2. Integration Tests
- **Mục đích**: Kiểm tra tương tác giữa các module
- **Phạm vi**: Workflow hoàn chỉnh, data consistency
- **Ví dụ**: 
  - `CompleteHotelWorkflow_AddRoomToInvoice_ShouldWork()`: Test quy trình hoàn chỉnh từ thêm phòng đến tạo hóa đơn
  - `BookingConflict_ShouldPreventOverlappingBookings()`: Test xử lý conflict booking

### 3. Performance Tests
- Kiểm tra hiệu suất với dữ liệu lớn
- Đảm bảo thời gian phản hồi hợp lý

## Cách chạy Tests

### 1. Chạy tất cả tests
```bash
# Từ thư mục gốc của project
dotnet test

# Hoặc chạy trực tiếp
dotnet run --project Tests
```

### 2. Chạy tests cho một module cụ thể
```bash
# Chỉ chạy RoomBLL tests
dotnet test --filter "RoomBLLTests"

# Chỉ chạy integration tests
dotnet test --filter "Integration"
```

### 3. Chạy với custom test runner
```bash
# Compile và chạy test runner
dotnet build
cd Tests
dotnet run TestRunner.cs
```

## Test Configuration

### Database Setup
- Tests sử dụng database riêng: `Hotel_management_test`
- Tự động tạo và dọn dẹp data trước/sau mỗi test
- Connection string có thể cấu hình trong `BaseTestClass.cs`

### Test Data
- `SeedTestData()`: Tạo dữ liệu cơ bản (Users, Roles, Customers, Rooms)
- Mỗi test module có thể có `SeedXXXTestData()` riêng
- Dữ liệu được cleanup tự động

## Quy tắc viết Test

### 1. Naming Convention
```csharp
[Test]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    
    // Act
    
    // Assert
}
```

### 2. Test Structure (AAA Pattern)
- **Arrange**: Chuẩn bị dữ liệu test
- **Act**: Thực hiện action cần test
- **Assert**: Kiểm tra kết quả

### 3. Test Categories
```csharp
[Test]
[Category("Unit")]
public void UnitTest() { }

[Test]
[Category("Integration")]
public void IntegrationTest() { }

[Test]
[Category("Performance")]
public void PerformanceTest() { }
```

## Assertions sử dụng

### FluentAssertions
```csharp
result.Should().NotBeNull();
result.Success.Should().BeTrue();
result.Message.Should().Contain("thành công");
dataTable.Rows.Count.Should().BeGreaterThan(0);
exception.Message.Should().Contain("lỗi");
```

### NUnit Assertions
```csharp
Assert.DoesNotThrow(() => method());
Assert.Throws<Exception>(() => method());
Assert.That(value, Is.EqualTo(expected));
Assert.That(collection, Has.Count.EqualTo(3));
```

## Test Coverage

### Các scenarios được cover:
1. **Happy Path**: Các luồng thành công
2. **Error Handling**: Xử lý lỗi và exception
3. **Edge Cases**: Các trường hợp biên
4. **Data Validation**: Kiểm tra validation đầu vào
5. **Business Rules**: Các quy tắc nghiệp vụ
6. **Integration**: Tương tác giữa modules

### Metrics mong muốn:
- **Line Coverage**: > 80%
- **Branch Coverage**: > 70%
- **Method Coverage**: > 90%

## Troubleshooting

### Lỗi thường gặp:

#### 1. Database Connection Error
```
Lỗi: Cannot connect to MySQL server
Giải pháp: 
- Kiểm tra MySQL server đang chạy
- Cập nhật connection string trong BaseTestClass.cs
- Đảm bảo user có quyền tạo database
```

#### 2. Test Data Conflicts
```
Lỗi: Duplicate entry for key 'PRIMARY'
Giải pháp:
- Kiểm tra CleanupTestData() được gọi đúng cách
- Đảm bảo mỗi test sử dụng data unique
```

#### 3. Timeout Issues
```
Lỗi: Test execution timeout
Giải pháp:
- Tăng timeout trong test configuration
- Tối ưu database queries
- Giảm số lượng test data
```

## Continuous Integration

### Pipeline Steps:
1. **Build**: Compile project
2. **Unit Tests**: Chạy tất cả unit tests
3. **Integration Tests**: Chạy integration tests
4. **Code Coverage**: Generate coverage report
5. **Quality Gate**: Fail nếu coverage < threshold

### Test Reports:
- **JUnit XML**: Cho CI/CD integration
- **HTML Report**: Cho developers
- **Coverage Report**: Code coverage metrics

## Maintenance

### Thêm test mới:
1. Tạo test class trong thư mục phù hợp
2. Kế thừa từ `BaseTestClass`
3. Follow naming convention
4. Thêm test data setup nếu cần

### Update tests khi thay đổi code:
1. Update assertions nếu behavior thay đổi
2. Thêm tests cho features mới
3. Refactor tests khi refactor code
4. Update test data nếu schema thay đổi

---

## Contact & Support

Nếu có vấn đề với test system, vui lòng:
1. Kiểm tra logs trong console output
2. Verify database connection
3. Check test data setup
4. Contact team lead nếu cần hỗ trợ
