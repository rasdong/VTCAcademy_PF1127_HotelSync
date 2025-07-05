# Module Báo Cáo và Thống Kê (Report and Analytics)

## Tổng quan
Module này cung cấp các báo cáo và thống kê chi tiết để quản lý đánh giá hiệu quả hoạt động của khách sạn.

## Các tính năng chính

### 1. Báo cáo Doanh thu
- **Báo cáo doanh thu theo ngày**: Hiển thị chi tiết doanh thu từ đặt phòng và dịch vụ trong một ngày cụ thể
- **Báo cáo doanh thu theo tháng**: Phân tích doanh thu theo từng tuần trong tháng

### 2. Báo cáo Phòng
- **Báo cáo tình trạng phòng**: Thống kê phòng theo trạng thái và loại
- **Báo cáo tỷ lệ lấp đầy**: Tính phần trăm phòng đã đặt so với tổng số phòng trong khoảng thời gian

### 3. Báo cáo Khách hàng
- **Báo cáo khách hàng theo quốc tịch**: Thống kê số lượng khách theo quốc tịch, tần suất booking
- **Báo cáo khách hàng VIP**: Danh sách khách hàng có nhiều booking (>= 5 lần), phân loại theo mức độ (Silver, Gold, Platinum)

### 4. Báo cáo Dịch vụ
- **Báo cáo dịch vụ phổ biến**: Hiển thị các dịch vụ được sử dụng nhiều nhất và mang lại doanh thu cao nhất

### 5. Thống kê và Phân tích
- **Thống kê booking theo trạng thái**: Phân tích booking theo các trạng thái khác nhau
- **Phân tích xu hướng đặt phòng**: Xem xu hướng booking trong 12 tháng gần đây
- **Báo cáo nhân viên xuất sắc**: Hiển thị thông tin nhân viên hiện tại

### 6. Xuất báo cáo
- **Xuất báo cáo Excel**: Tính năng sẽ được phát triển trong tương lai

## Cấu trúc Code

### ReportDAL.cs
Chứa các query SQL để lấy dữ liệu từ database:
- `GetDailyRevenueReport()`: Lấy doanh thu theo ngày
- `GetMonthlyRevenueReport()`: Lấy doanh thu theo tháng
- `GetOccupancyReport()`: Lấy tỷ lệ lấp đầy phòng
- `GetRoomStatusReport()`: Lấy tình trạng phòng
- `GetCustomerNationalityReport()`: Lấy thống kê khách hàng theo quốc tịch
- `GetVIPCustomerReport()`: Lấy danh sách khách hàng VIP
- `GetPopularServicesReport()`: Lấy dịch vụ phổ biến
- `GetBookingStatusStatistics()`: Lấy thống kê booking
- `GetBookingTrendAnalysis()`: Phân tích xu hướng booking

### ReportBL.cs
Chứa logic nghiệp vụ và validation:
- Validate input (định dạng ngày, khoảng thời gian hợp lệ)
- Xử lý exception
- Utility methods để format tiền tệ, phần trăm, ngày tháng

### ReportAndAnalyticsUI.cs
Giao diện người dùng:
- Menu điều hướng
- Hiển thị báo cáo theo định dạng bảng
- Xử lý input/output console

## Database Schema sử dụng

### Bảng chính:
- `Invoices`: Dữ liệu hóa đơn cho báo cáo doanh thu
- `Bookings`: Dữ liệu đặt phòng cho thống kê booking và xu hướng
- `Rooms`: Dữ liệu phòng cho báo cáo tình trạng và tỷ lệ lấp đầy
- `Customers`: Dữ liệu khách hàng cho báo cáo theo quốc tịch và VIP
- `Services` & `ServiceUsage`: Dữ liệu dịch vụ cho báo cáo dịch vụ phổ biến
- `Staff`: Dữ liệu nhân viên

### Sample Data
File `SampleData.sql` chứa dữ liệu mẫu để test các báo cáo.

## Cách sử dụng

1. **Khởi tạo database**: Chạy script `Hotel_Final.sql` để tạo database
2. **Thêm dữ liệu mẫu**: Chạy `SampleData.sql` để có dữ liệu test
3. **Chạy ứng dụng**: Truy cập menu "Báo cáo & Phân tích" từ màn hình chính
4. **Chọn loại báo cáo**: Nhập số tương ứng với báo cáo muốn xem
5. **Nhập tham số**: Một số báo cáo yêu cầu nhập ngày tháng

## Lưu ý kỹ thuật

- Tất cả ngày tháng nhập theo định dạng `dd/MM/yyyy`
- Báo cáo tỷ lệ lấp đầy không được vượt quá 1 năm
- Khách hàng VIP mặc định >= 5 lần booking
- Connection string có thể cần điều chỉnh trong `ReportDAL.cs`

## Tính năng tương lai

1. **Xuất Excel**: Tích hợp thư viện EPPlus hoặc ClosedXML
2. **Báo cáo Dashboard**: Hiển thị tổng quan tất cả chỉ số
3. **Báo cáo theo thời gian thực**: Auto-refresh dữ liệu
4. **Biểu đồ**: Tích hợp thư viện vẽ chart
5. **Dự báo**: Sử dụng ML để dự báo xu hướng

## Test Cases

### Báo cáo doanh thu theo ngày
- Input: `01/12/2024`
- Expected: Hiển thị doanh thu booking + dịch vụ cho ngày đó

### Báo cáo khách hàng VIP  
- Expected: Hiển thị khách có >= 5 booking, phân loại Silver/Gold/Platinum

### Tỷ lệ lấp đầy phòng
- Input: `01/12/2024` đến `07/12/2024`
- Expected: Hiển thị tỷ lệ % lấp đầy theo từng ngày

## Troubleshooting

### Lỗi kết nối database
- Kiểm tra connection string trong `ReportDAL.cs`
- Đảm bảo MySQL server đang chạy
- Kiểm tra username/password

### Không có dữ liệu
- Chạy `SampleData.sql` để thêm dữ liệu test
- Kiểm tra ngày tháng nhập đúng định dạng
- Đảm bảo có dữ liệu trong khoảng thời gian query

### Lỗi format ngày
- Sử dụng định dạng `dd/MM/yyyy`
- VD: `25/12/2024` thay vì `12/25/2024`
