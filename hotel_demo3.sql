-- Tạo cơ sở dữ liệu quản lý khách sạn
CREATE DATABASE hotel_management;
USE hotel_management;

-- Bảng Users: Quản lý người dùng hệ thống
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Bảng Roles: Quản lý vai trò và quyền
CREATE TABLE Roles (
    RoleID INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE,
    Permissions JSON NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại cho Users và Roles
ALTER TABLE Users
    ADD CONSTRAINT fk_users_roleid FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_users_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE Roles
    ADD CONSTRAINT fk_roles_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Thêm vai trò ban đầu
-- Admin: Toàn quyền
-- Receptionist: Quản lý khách hàng, đặt phòng, hóa đơn
-- Housekeeping: Quản lý phòng và dịch vụ
INSERT INTO Roles (RoleName, Permissions) VALUES
('Admin', '["manage_rooms", "manage_customers", "manage_bookings", "manage_invoices", "manage_services", "manage_staff", "manage_users", "view_reports"]'),
('Receptionist', '["manage_customers", "manage_bookings", "manage_invoices"]'),
('Housekeeping', '["manage_rooms", "manage_services"]');

-- Bảng Rooms: Lưu thông tin phòng
CREATE TABLE Rooms (
    RoomID INT AUTO_INCREMENT PRIMARY KEY,
    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
    RoomType ENUM('Single', 'Double', 'Suite') NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Status ENUM('Available', 'Occupied', 'Under Maintenance') NOT NULL DEFAULT 'Available',
    Amenities JSON NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại cho Rooms
ALTER TABLE Rooms
    ADD CONSTRAINT fk_rooms_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Index cho truy vấn phòng
CREATE INDEX idx_room_number ON Rooms(RoomNumber);
CREATE INDEX idx_room_status_type ON Rooms(Status, RoomType);

-- Bảng Customers: Lưu thông tin khách hàng
CREATE TABLE Customers (
    CustomerID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    IDCard VARCHAR(20) NOT NULL UNIQUE,
    Phone VARCHAR(15),
    Email VARCHAR(100),
    Nationality VARCHAR(50) DEFAULT 'Vietnam',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại cho Customers
ALTER TABLE Customers
    ADD CONSTRAINT fk_customers_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Index cho truy vấn khách hàng
CREATE INDEX idx_customer_idcard ON Customers(IDCard);
CREATE INDEX idx_customer_phone ON Customers(Phone);

-- Bảng Bookings: Quản lý đặt phòng
CREATE TABLE Bookings (
    BookingID INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID INT NOT NULL,
    RoomID INT NOT NULL,
    CheckInDate DATETIME NOT NULL,
    CheckOutDate DATETIME NOT NULL,
    Status ENUM('Active', 'Completed', 'Cancelled') NOT NULL DEFAULT 'Active',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại và ràng buộc cho Bookings
ALTER TABLE Bookings
    ADD CONSTRAINT fk_bookings_roomid FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_bookings_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_bookings_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE,
    ADD CONSTRAINT chk_dates CHECK (CheckInDate < CheckOutDate);

-- Index cho truy vấn đặt phòng
CREATE INDEX idx_booking_dates ON Bookings(CheckInDate, CheckOutDate);
CREATE INDEX idx_booking_customer ON Bookings(CustomerID);
CREATE INDEX idx_booking_room ON Bookings(RoomID);
CREATE INDEX idx_booking_status ON Bookings(Status);

-- Bảng Invoices: Quản lý hóa đơn
CREATE TABLE Invoices (
    InvoiceID INT AUTO_INCREMENT PRIMARY KEY,
    BookingID INT NOT NULL,
    CustomerID INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    IssueDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PaymentStatus ENUM('Paid', 'Unpaid') NOT NULL DEFAULT 'Unpaid',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại cho Invoices
ALTER TABLE Invoices
    ADD CONSTRAINT fk_invoices_bookingid FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_invoices_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_invoices_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Index cho truy vấn hóa đơn
CREATE INDEX idx_invoice_booking ON Invoices(BookingID);
CREATE INDEX idx_invoice_customer ON Invoices(CustomerID);
CREATE INDEX idx_invoice_payment_status ON Invoices(PaymentStatus);

-- Bảng Services: Lưu thông tin dịch vụ
CREATE TABLE Services (
    ServiceID INT AUTO_INCREMENT PRIMARY KEY,
    ServiceName VARCHAR(50) NOT NULL,
    Type ENUM('Food', 'Laundry', 'Spa', 'Other') NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại cho Services
ALTER TABLE Services
    ADD CONSTRAINT fk_services_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Index cho truy vấn dịch vụ
CREATE INDEX idx_service_type ON Services(Type);

-- Bảng ServiceUsage: Theo dõi sử dụng dịch vụ
CREATE TABLE ServiceUsage (
    UsageID INT AUTO_INCREMENT PRIMARY KEY,
    BookingID INT NOT NULL,
    ServiceID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    Date DATE NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Thêm khóa ngoại cho ServiceUsage
ALTER TABLE ServiceUsage
    ADD CONSTRAINT fk_serviceusage_bookingid FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_serviceid FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID) ON DELETE RESTRICT ON UPDATE CASCADE;

-- Index cho truy vấn sử dụng dịch vụ
CREATE INDEX idx_service_usage_booking ON ServiceUsage(BookingID);
CREATE INDEX idx_service_usage_service ON ServiceUsage(ServiceID);
CREATE INDEX idx_service_usage_date ON ServiceUsage(Date);

-- Bảng Staff: Quản lý nhân viên
CREATE TABLE Staff (
    StaffID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Role ENUM('Receptionist', 'Housekeeping', 'Manager') NOT NULL,
    Phone VARCHAR(15),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL
);

-- Thêm khóa ngoại cho Staff
ALTER TABLE Staff
    ADD CONSTRAINT fk_staff_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Index cho truy vấn nhân viên
CREATE INDEX idx_staff_role ON Staff(Role);

-- Bảng Logs: Ghi lại hành động hệ thống
CREATE TABLE Logs (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Action VARCHAR(255) NOT NULL,
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Thêm khóa ngoại cho Logs
ALTER TABLE Logs
    ADD CONSTRAINT fk_logs_userid FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT ON UPDATE CASCADE;

-- Index cho truy vấn log
CREATE INDEX idx_log_user ON Logs(UserID);
CREATE INDEX idx_log_timestamp ON Logs(Timestamp);

-- Thủ tục: Thêm phòng mới
DELIMITER //
CREATE PROCEDURE addRoom(
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType ENUM('Single', 'Double', 'Suite'),
    IN p_Price DECIMAL(10,2),
    IN p_Amenities JSON
)
BEGIN
    INSERT INTO Rooms (RoomNumber, RoomType, Price, Amenities)
    VALUES (p_RoomNumber, p_RoomType, p_Price, p_Amenities);
END //
DELIMITER ;

-- Thủ tục: Tạo đặt phòng
DELIMITER //
CREATE PROCEDURE createBooking(
    IN p_CustomerID INT,
    IN p_RoomID INT,
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME
)
BEGIN
    INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate)
    VALUES (p_CustomerID, p_RoomID, p_CheckInDate, p_CheckOutDate);
END //
DELIMITER ;

-- Thủ tục: Cập nhật thông tin khách hàng
DELIMITER //
CREATE PROCEDURE updateCustomer(
    IN p_CustomerID INT,
    IN p_Name VARCHAR(100),
    IN p_IDCard VARCHAR(20),
    IN p_Phone VARCHAR(15),
    IN p_Email VARCHAR(100),
    IN p_Nationality VARCHAR(50)
)
BEGIN
    UPDATE Customers 
    SET Name = p_Name,
        IDCard = p_IDCard,
        Phone = p_Phone,
        Email = p_Email,
        Nationality = p_Nationality,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE CustomerID = p_CustomerID;
END //
DELIMITER ;

-- Thủ tục: Tạo báo cáo
DELIMITER //
CREATE PROCEDURE generateReport(IN p_ReportType VARCHAR(50))
BEGIN
    INSERT INTO Logs (UserID, Action)
    VALUES (1, CONCAT('Tạo báo cáo ', p_ReportType));
END //
DELIMITER ;

-- Trigger: Ngăn xóa phòng có đặt phòng đang hoạt động
DELIMITER //
CREATE TRIGGER before_room_delete
BEFORE DELETE ON Rooms
FOR EACH ROW
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Bookings
        WHERE RoomID = OLD.RoomID
        AND Status = 'Active'
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể xóa phòng có đặt phòng đang hoạt động';
    END IF;
END //
DELIMITER ;

-- Trigger: Cập nhật trạng thái phòng sau khi thêm đặt phòng
DELIMITER //
CREATE TRIGGER after_booking_insert
AFTER INSERT ON Bookings
FOR EACH ROW
BEGIN
    UPDATE Rooms
    SET Status = 'Occupied'
    WHERE RoomID = NEW.RoomID
    AND NEW.Status = 'Active';
END //
DELIMITER ;

-- Trigger: Cập nhật trạng thái phòng sau khi cập nhật đặt phòng
DELIMITER //
CREATE TRIGGER after_booking_update
AFTER UPDATE ON Bookings
FOR EACH ROW
BEGIN
    IF NEW.Status IN ('Cancelled', 'Completed') THEN
        UPDATE Rooms
        SET Status = 'Available'
        WHERE RoomID = NEW.RoomID;
    END IF;
END //
DELIMITER ;

-- Trigger: Tính tổng giá trước khi thêm sử dụng dịch vụ
DELIMITER //
CREATE TRIGGER before_service_usage_insert
BEFORE INSERT ON ServiceUsage
FOR EACH ROW
BEGIN
    DECLARE service_price DECIMAL(10, 2);
    SELECT Price INTO service_price
    FROM Services
    WHERE ServiceID = NEW.ServiceID;
    SET NEW.TotalPrice = NEW.Quantity * service_price;
END //
DELIMITER ;

-- Trigger: Tính lại tổng giá trước khi cập nhật sử dụng dịch vụ
DELIMITER //
CREATE TRIGGER before_service_usage_update
BEFORE UPDATE ON ServiceUsage
FOR EACH ROW
BEGIN
    DECLARE service_price DECIMAL(10, 2);
    SELECT Price INTO service_price
    FROM Services
    WHERE ServiceID = NEW.ServiceID;
    SET NEW.TotalPrice = NEW.Quantity * service_price;
END //
DELIMITER ;

-- Trigger: Ghi log khi tạo hóa đơn
DELIMITER //
CREATE TRIGGER after_invoice_insert
AFTER INSERT ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action)
    VALUES (1, CONCAT('Tạo hóa đơn ID ', NEW.InvoiceID, ' cho đặt phòng ID ', NEW.BookingID));
END //
DELIMITER ;

-- Trigger: Ngăn xóa người dùng có log liên quan
DELIMITER //
CREATE TRIGGER before_user_delete
BEFORE DELETE ON Users
FOR EACH ROW
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Logs
        WHERE UserID = OLD.UserID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể xóa người dùng có log liên quan';
    END IF;
END //
DELIMITER ;

-- Trigger: Ghi log khi thay đổi trạng thái phòng
DELIMITER //
CREATE TRIGGER after_room_status_update
AFTER UPDATE ON Rooms
FOR EACH ROW
BEGIN
    IF OLD.Status != NEW.Status THEN
        INSERT INTO Logs (UserID, Action)
        VALUES (1, CONCAT('Thay đổi trạng thái phòng ', NEW.RoomNumber, ' từ ', OLD.Status, ' sang ', NEW.Status));
    END IF;
END //
DELIMITER ;

-- Trigger: Ghi log khi tạo khách hàng
DELIMITER //
CREATE TRIGGER after_customer_insert
AFTER INSERT ON Customers
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action)
    VALUES (1, CONCAT('Tạo khách hàng ID ', NEW.CustomerID, ' (', NEW.Name, ')'));
END //
DELIMITER ;

-- Trigger: Ghi log khi tạo đặt phòng
DELIMITER //
CREATE TRIGGER after_booking_log_insert
AFTER INSERT ON Bookings
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action)
    VALUES (1, CONCAT('Tạo đặt phòng ID ', NEW.BookingID, ' cho phòng ID ', NEW.RoomID));
END //
DELIMITER ;

-- Trigger: Hạn chế quản lý nhân viên khi là Receptionist/Housekeeping (thêm)
DELIMITER //
CREATE TRIGGER before_staff_insert
BEFORE INSERT ON Staff
FOR EACH ROW
BEGIN
    DECLARE user_role VARCHAR(50);
    SELECT r.RoleName INTO user_role
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.UserID = CURRENT_USER();
    IF user_role IN ('Receptionist', 'Housekeeping') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Lễ tân hoặc Buồng phòng không thể quản lý nhân viên';
    END IF;
END //
DELIMITER ;

-- Trigger: Hạn chế quản lý nhân viên khi là Receptionist/Housekeeping (cập nhật)
DELIMITER //
CREATE TRIGGER before_staff_update
BEFORE UPDATE ON Staff
FOR EACH ROW
BEGIN
    DECLARE user_role VARCHAR(50);
    SELECT r.RoleName INTO user_role
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.UserID = CURRENT_USER();
    IF user_role IN ('Receptionist', 'Housekeeping') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Lễ tân hoặc Buồng phòng không thể quản lý nhân viên';
    END IF;
END //
DELIMITER ;

-- Trigger: Hạn chế quản lý nhân viên khi là Receptionist/Housekeeping (xóa)
DELIMITER //
CREATE TRIGGER before_staff_delete
BEFORE DELETE ON Staff
FOR EACH ROW
BEGIN
    DECLARE user_role VARCHAR(50);
    SELECT r.RoleName INTO user_role
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.UserID = CURRENT_USER();
    IF user_role IN ('Receptionist', 'Housekeeping') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Lễ tân hoặc Buồng phòng không thể quản lý nhân viên';
    END IF;
END //
DELIMITER ;