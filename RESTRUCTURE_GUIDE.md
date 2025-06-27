# Tài Liệu Tái Cấu Trúc ConsoleUI - Hotel Management System

## Tổng Quan

ConsoleUI ban đầu đã trở nên quá dài và khó quản lý với hơn 2800 dòng code. Để giải quyết vấn đề này, chúng ta đã tái cấu trúc và chia nhỏ thành các class riêng biệt theo từng module chức năng.

## Cấu Trúc Mới

### 1. BaseUI.cs
- **Mục đích**: Class trừu tượng chứa các method chung cho tất cả UI
- **Chức năng**:
  - Vẽ header và box
  - Xử lý input và password
  - Hiển thị thông báo (success, error, info)
  - Quản lý thông tin user hiện tại

### 2. MainConsoleUI.cs
- **Mục đích**: Class chính điều khiển flow của ứng dụng
- **Chức năng**:
  - Màn hình khởi tạo
  - Đăng nhập/Đăng ký
  - Menu chính
  - Điều hướng đến các module khác

### 3. UserManagementUI.cs
- **Mục đích**: UI cho quản lý người dùng (đã tích hợp UserBL)
- **Chức năng**:
  - Thêm/Sửa/Xóa người dùng
  - Đổi mật khẩu
  - Kích hoạt/Vô hiệu hóa tài khoản
  - Xem danh sách người dùng

### 4. UserBL.cs + UserDAL.cs
- **Mục đích**: Business Logic và Data Access Layer cho User
- **Chức năng**:
  - Validate dữ liệu input
  - Xử lý logic nghiệp vụ
  - Tương tác với database

## Cách Sử Dụng

### Chạy Ứng Dụng
```bash
cd "c:\Users\dong\Desktop\New folder\VTCAcademy_PF1127_HotelSync"
dotnet run
```

### Luồng Hoạt Động
1. **Khởi động**: MainConsoleUI được khởi tạo trong Program.cs
2. **Màn hình chính**: Hiển thị tùy chọn Đăng nhập/Đăng ký/Thoát
3. **Đăng nhập**: Sử dụng UserManagementUI để xác thực
4. **Menu chính**: Hiển thị tùy chọn theo vai trò (Admin/Receptionist/Housekeeping)
5. **Chọn module**: Điều hướng đến UI tương ứng

### Thêm Module Mới

Để thêm module mới (ví dụ: CustomerManagementUI), làm theo các bước:

1. **Tạo UI Class**:
```csharp
public class CustomerManagementUI : BaseUI
{
    private readonly CustomerBLL _customerBLL = new CustomerBLL();
    
    public CustomerManagementUI(string? username, string? role, int? userId) 
        : base(username, role, userId)
    {
    }
    
    public void ShowCustomerManagement()
    {
        // Implementation here
    }
}
```

2. **Thêm vào MainConsoleUI**:
```csharp
private CustomerManagementUI? _customerManagementUI;

private void InitializeCustomerManagementUI()
{
    if (_customerManagementUI == null)
    {
        _customerManagementUI = new CustomerManagementUI(currentUsername, currentRole, currentUserId);
    }
}

// Trong HandleOption method:
case "Quản lý khách hàng":
    InitializeCustomerManagementUI();
    _customerManagementUI?.ShowCustomerManagement();
    break;
```

## Ưu Điểm Của Cấu Trúc Mới

### 1. Tách Biệt Trách Nhiệm
- Mỗi class chỉ chịu trách nhiệm cho một module cụ thể
- Code dễ đọc, dễ hiểu và dễ bảo trì

### 2. Khả Năng Mở Rộng
- Dễ dàng thêm module mới mà không ảnh hưởng đến code hiện tại
- Có thể phát triển song song nhiều module

### 3. Tái Sử Dụng Code
- BaseUI chứa các method chung, giảm code trùng lặp
- Các UI class khác kế thừa và sử dụng lại

### 4. Quản Lý Tốt Hơn
- Mỗi file có kích thước hợp lý (200-400 dòng)
- Dễ tracking và debug lỗi

## Cấu Trúc Thư Mục

```
ConsoleUI/
├── BaseUI.cs              # Base class cho tất cả UI
├── MainConsoleUI.cs       # Main UI và flow control
├── UserManagementUI.cs    # User management UI
├── RoomManagementUI.cs    # (Sẽ tạo)
├── BookingManagementUI.cs # (Sẽ tạo)
└── ... (các UI khác)

UserManagement/
├── UserBL.cs              # Business Logic
└── UserDAL.cs             # Data Access Layer
```

## Các Module Cần Tích Hợp Tiếp Theo

1. **RoomManagementUI** - Quản lý phòng
2. **BookingManagementUI** - Quản lý đặt phòng  
3. **InvoiceManagementUI** - Quản lý hóa đơn
4. **ServiceManagementUI** - Quản lý dịch vụ
5. **StaffManagementUI** - Quản lý nhân viên
6. **ReportUI** - Báo cáo và thống kê

## Lưu Ý Quan Trọng

1. **Session Management**: Thông tin user được truyền qua constructor và shared giữa các UI
2. **Error Handling**: Mỗi UI có method riêng để xử lý lỗi
3. **Validation**: Business Logic validation được tách riêng trong BL layer
4. **Database**: Tất cả database operations đều thông qua DAL layer

## Kết Luận

Việc tái cấu trúc này giúp:
- Giảm complexity của ConsoleUI từ 2800+ dòng xuống còn ~400 dòng
- Tách biệt concerns rõ ràng
- Dễ dàng maintain và extend
- Team có thể làm việc song song trên các module khác nhau
- Code quality tốt hơn và ít bug hơn
