# Tài Liệu Hướng Dẫn Sử Dụng Module Quản Lý Hóa Đơn (Invoice Management)

## Tổng Quan
Module Quản lý hóa đơn đã được tích hợp hoàn chỉnh vào Hệ thống Quản lý Khách sạn HotelSync với đầy đủ các chức năng nghiệp vụ và giao diện người dùng.

## Cấu Trúc Module

### 1. InvoiceBL.cs (Business Logic Layer)
- **Vị trí**: `InvoiceManagement/InvoiceBL.cs`
- **Chức năng**: Xử lý logic nghiệp vụ, validation, và điều phối các thao tác với hóa đơn

### 2. InvoiceDAL.cs (Data Access Layer)  
- **Vị trí**: `InvoiceManagement/InvoiceDAL.cs`
- **Chức năng**: Tương tác trực tiếp với database thông qua stored procedures

### 3. Tích hợp với ConsoleUI
- **Vị trí**: `ConsoleUI/ConsoleUI.cs`
- **Chức năng**: Giao diện người dùng cho tất cả các chức năng quản lý hóa đơn

## Các Tính Năng Đã Triển Khai

### 1. Tạo Hóa Đơn
#### Tạo hóa đơn thủ công
- Nhập ID booking, ID khách hàng, và tổng tiền
- Validation đầy đủ cho tất cả input
- Giới hạn tổng tiền tối đa 100,000,000 VND

#### Tạo hóa đơn tự động
- Tính toán tự động dựa on booking ID
- Tổng hợp chi phí phòng và dịch vụ
- Trả về ID hóa đơn mới được tạo

### 2. Xem Danh Sách Hóa Đơn
- Hiển thị tất cả hóa đơn với thông tin cơ bản
- Định dạng bảng đẹp với căn chỉnh cột
- Thông tin: ID, Booking ID, Khách hàng, Tổng tiền, Ngày tạo, Trạng thái

### 3. Xem Chi Tiết Hóa Đơn
- Thông tin đầy đủ về hóa đơn
- Chi tiết khách hàng và thông tin phòng
- Danh sách dịch vụ đã sử dụng với chi phí
- Tổng hợp tiền phòng và tiền dịch vụ

### 4. Tìm Kiếm Hóa Đơn
Hỗ trợ tìm kiếm theo nhiều tiêu chí:
- **Tên khách hàng**: Tìm kiếm theo tên (có thể để trống)
- **Khoảng thời gian**: Từ ngày - đến ngày (định dạng dd/MM/yyyy)
- **Trạng thái thanh toán**: Pending/Paid/Cancelled
- **Khoảng giá**: Số tiền tối thiểu và tối đa
- Validation đầy đủ cho tất cả input

### 5. Cập Nhật Trạng Thái Thanh Toán
- Thay đổi trạng thái: Pending → Paid → Cancelled
- Validation trạng thái hợp lệ
- Ghi log thay đổi

### 6. In Hóa Đơn
- Xuất hóa đơn định dạng đẹp ra console
- Header khách sạn chuyên nghiệp
- Thông tin chi tiết khách hàng và phòng
- Breakdown chi phí phòng và dịch vụ
- Tổng tiền và trạng thái thanh toán

### 7. Tính Toán Số Tiền Booking
- Tính tổng chi phí dựa trên booking ID
- Kết hợp tiền phòng (số đêm × giá phòng/đêm)
- Tổng hợp tất cả dịch vụ đã sử dụng

### 8. Xóa Hóa Đơn (Admin Only)
- Chỉ Admin mới có quyền xóa
- Xác nhận trước khi xóa
- Ghi log hành động xóa

## Stored Procedures Liên Quan

### 1. createInvoice
```sql
DELIMITER //
CREATE PROCEDURE createInvoice(
    IN p_BookingID INT,
    IN p_CustomerID INT,
    IN p_TotalAmount DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
```

### 2. createInvoiceWithCalculation
```sql
DELIMITER //
CREATE PROCEDURE createInvoiceWithCalculation(
    IN p_BookingID INT,
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50),
    OUT p_InvoiceID INT
)
```

### 3. calculateInvoiceAmount
```sql
DELIMITER //
CREATE PROCEDURE calculateInvoiceAmount(
    IN p_BookingID INT,
    OUT p_TotalAmount DECIMAL(10,2)
)
```

### 4. updateInvoicePaymentStatus
### 5. getAllInvoices
### 6. searchInvoices
### 7. printInvoice

## Cách Sử Dụng

### 1. Truy Cập Module
1. Chạy ứng dụng: `dotnet run --project VTCAcademy_PF1127_HotelSync\prj2.csproj`
2. Đăng nhập với tài khoản có quyền
3. Chọn "Quản lý hóa đơn" từ menu chính

### 2. Quyền Truy Cập
- **Admin**: Tất cả chức năng (bao gồm xóa hóa đơn)
- **Receptionist**: Tất cả chức năng (trừ xóa hóa đơn)
- **Housekeeping**: Không có quyền truy cập

### 3. Workflow Điển Hình
1. **Tạo hóa đơn**: Sử dụng "Tạo hóa đơn tự động" để tính toán tự động
2. **Kiểm tra chi tiết**: Dùng "Xem chi tiết hóa đơn" để xem breakdown
3. **Cập nhật trạng thái**: Thay đổi từ Pending → Paid khi khách thanh toán
4. **In hóa đơn**: Xuất hóa đơn cho khách hàng

## Validation và Error Handling

### Input Validation
- ID phải là số nguyên dương
- Số tiền phải là số không âm
- Ngày tháng đúng định dạng dd/MM/yyyy
- Trạng thái thanh toán trong danh sách cho phép
- Khoảng thời gian và giá tiền hợp lý

### Error Messages
- Thông báo lỗi rõ ràng và hướng dẫn khắc phục
- Validation business logic (ví dụ: booking phải tồn tại)
- Exception handling cho database errors

## Database Schema

### Bảng Invoices
```sql
CREATE TABLE Invoices (
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
);
```

### Liên Kết với Các Bảng Khác
- **Bookings**: Lấy thông tin đặt phòng
- **Customers**: Thông tin khách hàng
- **Rooms**: Chi tiết phòng và giá
- **ServiceUsage**: Dịch vụ đã sử dụng
- **Services**: Thông tin chi tiết dịch vụ

## Testing và Debugging

### Test Cases Đã Thực Hiện
1. ✓ Build thành công với 0 errors
2. ✓ Ứng dụng chạy và hiển thị menu
3. ✓ Tích hợp đầy đủ với ConsoleUI
4. ✓ Validation logic hoạt động

### Debugging Tips
- Check database connection string trong DataHelper.cs
- Đảm bảo MySQL service đang chạy
- Verify stored procedures đã được tạo đúng
- Check user permissions trong database

## Mở Rộng Tương Lai

### Tính Năng Có Thể Thêm
1. **Export hóa đơn ra file PDF/Excel**
2. **Email tự động gửi hóa đơn cho khách**
3. **Báo cáo doanh thu theo thời gian**
4. **Tính thuế và phí phụ trội**
5. **Multi-currency support**
6. **Discount và promotion codes**

### Cải Tiến Kỹ Thuật
1. **Async/await operations** cho database calls
2. **Caching** cho frequently accessed data
3. **Pagination** cho large datasets
4. **Transaction logging** chi tiết hơn
5. **Performance optimization** cho complex queries

---

**Tác giả**: GitHub Copilot  
**Ngày tạo**: 27/06/2025  
**Phiên bản**: 1.0  
**Trạng thái**: Hoàn thành và đã test
