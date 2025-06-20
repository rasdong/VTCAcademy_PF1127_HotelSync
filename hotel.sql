CREATE DATABASE hotel_management;
USE hotel_management;

CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

CREATE TABLE Roles (
    RoleID INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE,
    Permissions JSON NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE Users
    ADD CONSTRAINT fk_users_roleid FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_users_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE Roles
    ADD CONSTRAINT fk_roles_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

INSERT INTO Roles (RoleName, Permissions, UpdatedBy, UpdatedByUsername) VALUES
('Admin', '["manage_rooms", "manage_customers", "manage_bookings", "manage_invoices", "manage_services", "manage_staff", "manage_users", "view_reports"]', NULL, NULL),
('Receptionist', '["manage_customers", "manage_bookings", "manage_invoices"]', NULL, NULL),
('Housekeeping', '["manage_rooms", "manage_services"]', NULL, NULL);

CREATE TABLE Rooms (
    RoomID INT AUTO_INCREMENT PRIMARY KEY,
    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
    RoomType ENUM('Single', 'Double', 'Suite') NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Status ENUM('Available', 'Occupied', 'Under Maintenance') NOT NULL DEFAULT 'Available',
    Amenities JSON NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE Rooms
    ADD CONSTRAINT fk_rooms_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_room_number ON Rooms(RoomNumber);
CREATE INDEX idx_room_status_type ON Rooms(Status, RoomType);

CREATE TABLE Customers (
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
);

ALTER TABLE Customers
    ADD CONSTRAINT fk_customers_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_customer_idcard ON Customers(IDCard);
CREATE INDEX idx_customer_phone ON Customers(Phone);

CREATE TABLE Bookings (
    BookingID INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID INT NOT NULL,
    RoomID INT NOT NULL,
    CheckInDate DATETIME NOT NULL,
    CheckOutDate DATETIME NOT NULL,
    Status ENUM('Active', 'Completed', 'Cancelled') NOT NULL DEFAULT 'Active',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE Bookings
    ADD CONSTRAINT fk_bookings_roomid FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_bookings_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_bookings_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE,
    ADD CONSTRAINT chk_dates CHECK (CheckInDate < CheckOutDate);

CREATE INDEX idx_booking_dates ON Bookings(CheckInDate, CheckOutDate);
CREATE INDEX idx_booking_customer ON Bookings(CustomerID);
CREATE INDEX idx_booking_room ON Bookings(RoomID);
CREATE INDEX idx_booking_status ON Bookings(Status);

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

ALTER TABLE Invoices
    ADD CONSTRAINT fk_invoices_bookingid FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_invoices_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_invoices_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_invoice_booking ON Invoices(BookingID);
CREATE INDEX idx_invoice_customer ON Invoices(CustomerID);
CREATE INDEX idx_invoice_payment_status ON Invoices(PaymentStatus);

CREATE TABLE Services (
    ServiceID INT AUTO_INCREMENT PRIMARY KEY,
    ServiceName VARCHAR(50) NOT NULL,
    Type ENUM('Food', 'Laundry', 'Spa', 'Other') NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE Services
    ADD CONSTRAINT fk_services_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_service_type ON Services(Type);

CREATE TABLE ServiceUsage (
    UsageID INT AUTO_INCREMENT PRIMARY KEY,
    BookingID INT NOT NULL,
    ServiceID INT NOT NULL,
    CustomerID INT NOT NULL, 
    Quantity INT NOT NULL DEFAULT 1,
    Date DATE NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    PaymentStatus ENUM('Paid', 'Unpaid') NOT NULL DEFAULT 'Unpaid', 
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, 
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE ServiceUsage
    ADD CONSTRAINT fk_serviceusage_bookingid FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_serviceid FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_service_usage_booking ON ServiceUsage(BookingID);
CREATE INDEX idx_service_usage_service ON ServiceUsage(ServiceID);
CREATE INDEX idx_service_usage_date ON ServiceUsage(Date);
CREATE INDEX idx_service_usage_payment_status ON ServiceUsage(PaymentStatus); 

CREATE TABLE Staff (
    StaffID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Role ENUM('Receptionist', 'Housekeeping', 'Manager') NOT NULL,
    Phone VARCHAR(15),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE Staff
    ADD CONSTRAINT fk_staff_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_staff_role ON Staff(Role);

CREATE TABLE Logs (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Action VARCHAR(255) NOT NULL,
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedByUsername VARCHAR(50) NULL 
);

ALTER TABLE Logs
    ADD CONSTRAINT fk_logs_userid FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT ON UPDATE CASCADE;

CREATE INDEX idx_log_user ON Logs(UserID);
CREATE INDEX idx_log_timestamp ON Logs(Timestamp);

DELIMITER //

-- Thêm khách hàng
CREATE PROCEDURE addCustomer(
    IN p_CustomerID INT,
    IN p_Name VARCHAR(100),
    IN p_IDCard VARCHAR(20),
    IN p_Phone VARCHAR(15),
    IN p_Email VARCHAR(100),
    IN p_Nationality VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Customers (CustomerID, Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername)
    VALUES (p_CustomerID, p_Name, p_IDCard, p_Phone, p_Email, p_Nationality, p_UpdatedBy, p_UpdatedByUsername);
END //

-- Cập nhật thông tin khách hàng
CREATE PROCEDURE updateCustomer(
    IN p_CustomerID INT,
    IN p_Name VARCHAR(100),
    IN p_IDCard VARCHAR(20),
    IN p_Phone VARCHAR(15),
    IN p_Email VARCHAR(100),
    IN p_Nationality VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Customers 
    SET Name = p_Name,
        IDCard = p_IDCard,
        Phone = p_Phone,
        Email = p_Email,
        Nationality = p_Nationality,
        UpdatedAt = CURRENT_TIMESTAMP,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername
    WHERE CustomerID = p_CustomerID;
END //

-- Xóa khách hàng
CREATE PROCEDURE deleteCustomer(
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Bookings
        WHERE CustomerID = p_CustomerID
        AND Status = 'Active'
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể xóa khách hàng có đặt phòng đang hoạt động';
    END IF;
    
    DELETE FROM Customers WHERE CustomerID = p_CustomerID;
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy khách hàng để xóa.';
    END IF;
END //

-- Tìm kiếm khách hàng
CREATE PROCEDURE searchCustomer(
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    SELECT CustomerID, Name, IDCard, Phone, Email, Nationality
    FROM Customers
    WHERE CustomerID = p_CustomerID;
END //

-- Lịch sử đặt phòng
CREATE PROCEDURE getCustomerBookingHistory(
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    SELECT 
        b.BookingID,
        b.CheckInDate,
        b.CheckOutDate,
        b.Status,
        r.RoomNumber,
        i.InvoiceID,
        i.TotalAmount,
        i.PaymentStatus
    FROM Bookings b
    LEFT JOIN Rooms r ON b.RoomID = r.RoomID
    LEFT JOIN Invoices i ON b.BookingID = i.BookingID
    WHERE b.CustomerID = p_CustomerID;
END //

-- Thêm nhân viên
CREATE PROCEDURE addEmployee(
    IN p_StaffID INT,
    IN p_Name VARCHAR(255),
    IN p_Role VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Staff (StaffID, Name, Role, UpdatedBy, UpdatedByUsername, UpdatedAt)
    VALUES (p_StaffID, p_Name, p_Role, p_UpdatedBy, p_UpdatedByUsername, NOW())
    ON DUPLICATE KEY UPDATE
        Name = p_Name,
        Role = p_Role,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = NOW();
END //

-- Xóa nhân viên
CREATE PROCEDURE deleteEmployee(
    IN p_StaffID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DELETE FROM Staff
    WHERE StaffID = p_StaffID;
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Staff not found';
    END IF;
END //

-- Gán vai trò mới cho nhân viên
CREATE PROCEDURE assignEmployeeRole(
    IN p_StaffID INT,
    IN p_NewRole VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Staff
    SET Role = p_NewRole,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = NOW()
    WHERE StaffID = p_StaffID;
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Staff not found';
    END IF;
END //

-- Xem danh sách nhân viên theo vai trò
CREATE PROCEDURE getEmployeesByRole(
    IN p_Role VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    IF p_Role IS NULL THEN
        SELECT StaffID, Name, Role
        FROM Staff
        ORDER BY Role, Name;
    ELSE
        SELECT StaffID, Name, Role
        FROM Staff
        WHERE Role = p_Role
        ORDER BY Name;
    END IF;
END //

-- Thêm phòng
CREATE PROCEDURE addRoom(
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType ENUM('Single', 'Double', 'Suite'),
    IN p_Price DECIMAL(10,2),
    IN p_Amenities JSON,
    IN p_UpdatedBy INT, 
    IN p_UpdatedByUsername VARCHAR(50) 
)
BEGIN
    INSERT INTO Rooms (RoomNumber, RoomType, Price, Amenities, UpdatedBy, UpdatedByUsername)
    VALUES (p_RoomNumber, p_RoomType, p_Price, p_Amenities, p_UpdatedBy, p_UpdatedByUsername);
END //

-- Tạo đặt phòng
CREATE PROCEDURE createBooking(
    IN p_CustomerID INT,
    IN p_RoomID INT,
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME,
    IN p_UpdatedBy INT, 
    IN p_UpdatedByUsername VARCHAR(50) 
)
BEGIN
    INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate, UpdatedBy, UpdatedByUsername)
    VALUES (p_CustomerID, p_RoomID, p_CheckInDate, p_CheckOutDate, p_UpdatedBy, p_UpdatedByUsername);
END //

-- Tạo báo cáo
CREATE PROCEDURE generateReport(
    IN p_ReportType VARCHAR(50),
    IN p_UpdatedBy INT, 
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Tạo báo cáo ', p_ReportType), p_UpdatedByUsername);
END //

-- Trigger trước khi xóa phòng
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

-- Trigger sau khi thêm đặt phòng
CREATE TRIGGER after_booking_insert
AFTER INSERT ON Bookings
FOR EACH ROW
BEGIN
    UPDATE Rooms
    SET Status = 'Occupied',
        UpdatedBy = NEW.UpdatedBy,
        UpdatedByUsername = NEW.UpdatedByUsername
    WHERE RoomID = NEW.RoomID
    AND NEW.Status = 'Active';
END //

-- Trigger sau khi cập nhật đặt phòng
CREATE TRIGGER after_booking_update
AFTER UPDATE ON Bookings
FOR EACH ROW
BEGIN
    IF NEW.Status IN ('Cancelled', 'Completed') THEN
        UPDATE Rooms
        SET Status = 'Available',
            UpdatedBy = NEW.UpdatedBy,
            UpdatedByUsername = NEW.UpdatedByUsername
        WHERE RoomID = NEW.RoomID;
    END IF;
END //

-- Trigger trước khi thêm sử dụng dịch vụ
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

-- Trigger trước khi cập nhật sử dụng dịch vụ
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

-- Trigger sau khi thêm hóa đơn
CREATE TRIGGER after_invoice_insert
AFTER INSERT ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo hóa đơn ID ', NEW.InvoiceID, ' cho đặt phòng ID ', NEW.BookingID), NEW.UpdatedByUsername);
END //

-- Trigger trước khi xóa người dùng
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

-- Trigger sau khi cập nhật trạng thái phòng
CREATE TRIGGER after_room_status_update
AFTER UPDATE ON Rooms
FOR EACH ROW
BEGIN
    IF OLD.Status != NEW.Status THEN
        INSERT INTO Logs (UserID, Action, UpdatedByUsername)
        VALUES (NEW.UpdatedBy, CONCAT('Thay đổi trạng thái phòng ', NEW.RoomNumber, ' từ ', OLD.Status, ' sang ', NEW.Status), NEW.UpdatedByUsername);
    END IF;
END //

-- Trigger sau khi thêm khách hàng
CREATE TRIGGER after_customer_insert
AFTER INSERT ON Customers
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo khách hàng ID ', NEW.CustomerID, ' (', NEW.Name, ')'), NEW.UpdatedByUsername);
END //

-- Trigger sau khi thêm log đặt phòng
CREATE TRIGGER after_booking_log_insert
AFTER INSERT ON Bookings
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo đặt phòng ID ', NEW.BookingID, ' cho phòng ID ', NEW.RoomID), NEW.UpdatedByUsername);
END //

DELIMITER //

-- Stored procedure để tìm phòng trống trong khoảng thời gian
CREATE PROCEDURE findAvailableRooms(
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME
)
BEGIN
    SELECT r.RoomID, r.RoomNumber, r.RoomType, r.Price, r.Status
    FROM Rooms r
    WHERE r.RoomID NOT IN (
        SELECT b.RoomID
        FROM Bookings b
        WHERE (b.CheckInDate <= p_CheckOutDate AND b.CheckOutDate >= p_CheckInDate)
        AND b.Status = 'Active'
    )
    AND r.Status = 'Available';
END //

-- Stored procedure để cập nhật trạng thái phòng
CREATE PROCEDURE updateRoomStatus(
    IN p_RoomID INT,
    IN p_Status VARCHAR(20),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Rooms
    SET Status = p_Status,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;
    
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy phòng!';
    END IF;
END //

-- Stored procedure để cập nhật giá phòng
CREATE PROCEDURE updateRoomPrice(
    IN p_RoomID INT,
    IN p_Price DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    IF p_Price <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Giá phòng phải lớn hơn 0!';
    END IF;

    UPDATE Rooms
    SET Price = p_Price,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;
    
    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy phòng!';
    END IF;
END //

-- Stored procedure để xem danh sách phòng
CREATE PROCEDURE viewAllRooms()
BEGIN
    SELECT r.RoomID, r.RoomNumber, r.RoomType, r.Price, r.Status, r.Amenities, r.CreatedAt, r.UpdatedAt, r.UpdatedBy, r.UpdatedByUsername
    FROM Rooms r
    ORDER BY r.RoomNumber;
END //

-- Stored procedure để thêm đặt phòng mới với transaction
CREATE PROCEDURE addBookingWithTransaction(
    IN p_CustomerID INT,
    IN p_RoomID INT,
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;
    
    -- Kiểm tra xem phòng có trống trong khoảng thời gian này không
    IF EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.RoomID = p_RoomID
        AND b.Status = 'Active'
        AND (b.CheckInDate <= p_CheckOutDate AND b.CheckOutDate >= p_CheckInDate)
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Phòng đã được đặt trong khoảng thời gian này!';
    END IF;

    -- Kiểm tra trạng thái phòng
    IF NOT EXISTS (
        SELECT 1
        FROM Rooms r
        WHERE r.RoomID = p_RoomID
        AND r.Status = 'Available'
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Phòng không khả dụng!';
    END IF;

    -- Thêm đặt phòng mới
    INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate, Status, UpdatedBy, UpdatedByUsername)
    VALUES (p_CustomerID, p_RoomID, p_CheckInDate, p_CheckOutDate, 'Active', p_UpdatedBy, p_UpdatedByUsername);

    -- Cập nhật trạng thái phòng
    UPDATE Rooms
    SET Status = 'Occupied',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;

    COMMIT;
END //

-- Stored procedure để cập nhật trạng thái đặt phòng với transaction
CREATE PROCEDURE updateBookingStatus(
    IN p_BookingID INT,
    IN p_Status VARCHAR(20),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy RoomID từ booking
    SELECT RoomID INTO v_RoomID
    FROM Bookings
    WHERE BookingID = p_BookingID;

    IF v_RoomID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng!';
    END IF;

    -- Cập nhật trạng thái đặt phòng
    UPDATE Bookings
    SET Status = p_Status,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;

    -- Cập nhật trạng thái phòng nếu đặt phòng được hoàn thành hoặc hủy
    IF p_Status IN ('Completed', 'Cancelled') THEN
        UPDATE Rooms
        SET Status = 'Available',
            UpdatedBy = p_UpdatedBy,
            UpdatedByUsername = p_UpdatedByUsername,
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE RoomID = v_RoomID;
    END IF;

    COMMIT;
END //

-- Stored procedure để xem danh sách đặt phòng
CREATE PROCEDURE viewAllBookings()
BEGIN
    SELECT 
        b.BookingID,
        c.Name AS CustomerName,
        r.RoomNumber,
        b.CheckInDate,
        b.CheckOutDate,
        b.Status,
        b.CreatedAt,
        b.UpdatedAt,
        b.UpdatedByUsername
    FROM Bookings b
    JOIN Customers c ON b.CustomerID = c.CustomerID
    JOIN Rooms r ON b.RoomID = r.RoomID
    ORDER BY b.CreatedAt DESC;
END //

-- Stored procedure để xóa phòng với kiểm tra
CREATE PROCEDURE deleteRoom(
    IN p_RoomID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Kiểm tra xem phòng có đang được đặt không
    IF EXISTS (
        SELECT 1
        FROM Bookings
        WHERE RoomID = p_RoomID
        AND Status = 'Active'
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể xóa phòng đang có đặt phòng hoạt động!';
    END IF;

    -- Xóa phòng
    DELETE FROM Rooms
    WHERE RoomID = p_RoomID;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy phòng để xóa!';
    END IF;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Đã xóa phòng ID: ', p_RoomID), p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để cập nhật thông tin phòng với transaction
CREATE PROCEDURE updateRoom(
    IN p_RoomID INT,
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType ENUM('Single', 'Double', 'Suite'),
    IN p_Price DECIMAL(10,2),
    IN p_Status ENUM('Available', 'Occupied', 'Under Maintenance'),
    IN p_Amenities JSON,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE current_status VARCHAR(20);
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Kiểm tra giá phòng
    IF p_Price <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Giá phòng phải lớn hơn 0!';
    END IF;

    -- Lấy trạng thái hiện tại của phòng
    SELECT Status INTO current_status
    FROM Rooms
    WHERE RoomID = p_RoomID;

    IF current_status IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy phòng!';
    END IF;

    -- Kiểm tra nếu phòng đang được đặt và muốn chuyển sang trạng thái Under Maintenance
    IF current_status = 'Occupied' AND p_Status = 'Under Maintenance' THEN
        IF EXISTS (
            SELECT 1
            FROM Bookings
            WHERE RoomID = p_RoomID
            AND Status = 'Active'
        ) THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Không thể chuyển phòng đang có khách sang trạng thái bảo trì!';
        END IF;
    END IF;

    -- Cập nhật thông tin phòng
    UPDATE Rooms
    SET RoomNumber = p_RoomNumber,
        RoomType = p_RoomType,
        Price = p_Price,
        Status = p_Status,
        Amenities = p_Amenities,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Cập nhật thông tin phòng ID: ', p_RoomID, 
                   ', Số phòng: ', p_RoomNumber,
                   ', Trạng thái: ', p_Status),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để tìm kiếm phòng theo nhiều tiêu chí
CREATE PROCEDURE searchRooms(
    IN p_RoomType ENUM('Single', 'Double', 'Suite'),
    IN p_Status ENUM('Available', 'Occupied', 'Under Maintenance'),
    IN p_MinPrice DECIMAL(10,2),
    IN p_MaxPrice DECIMAL(10,2),
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME
)
BEGIN
    IF p_CheckInDate IS NOT NULL AND p_CheckOutDate IS NOT NULL THEN
        -- Tìm phòng trống trong khoảng thời gian
        SELECT DISTINCT r.*
        FROM Rooms r
        WHERE (p_RoomType IS NULL OR r.RoomType = p_RoomType)
        AND (p_Status IS NULL OR r.Status = p_Status)
        AND (p_MinPrice IS NULL OR r.Price >= p_MinPrice)
        AND (p_MaxPrice IS NULL OR r.Price <= p_MaxPrice)
        AND r.RoomID NOT IN (
            SELECT b.RoomID
            FROM Bookings b
            WHERE b.Status = 'Active'
            AND (b.CheckInDate <= p_CheckOutDate AND b.CheckOutDate >= p_CheckInDate)
        )
        ORDER BY r.RoomNumber;
    ELSE
        -- Tìm phòng theo các tiêu chí khác
        SELECT r.*
        FROM Rooms r
        WHERE (p_RoomType IS NULL OR r.RoomType = p_RoomType)
        AND (p_Status IS NULL OR r.Status = p_Status)
        AND (p_MinPrice IS NULL OR r.Price >= p_MinPrice)
        AND (p_MaxPrice IS NULL OR r.Price <= p_MaxPrice)
        ORDER BY r.RoomNumber;
    END IF;
END //

-- Stored procedure để kiểm tra tình trạng phòng theo thời gian thực
CREATE PROCEDURE checkRoomStatus(
    IN p_RoomID INT
)
BEGIN
    SELECT 
        r.RoomID,
        r.RoomNumber,
        r.RoomType,
        r.Status,
        r.Price,
        r.Amenities,
        CASE 
            WHEN b.BookingID IS NOT NULL THEN 'Đã đặt'
            WHEN r.Status = 'Under Maintenance' THEN 'Đang bảo trì'
            ELSE 'Trống'
        END AS CurrentStatus,
        b.CheckInDate,
        b.CheckOutDate,
        c.Name AS CustomerName
    FROM Rooms r
    LEFT JOIN Bookings b ON r.RoomID = b.RoomID AND b.Status = 'Active'
    LEFT JOIN Customers c ON b.CustomerID = c.CustomerID
    WHERE r.RoomID = p_RoomID;
END //

-- Stored procedure để lấy thống kê sử dụng phòng
CREATE PROCEDURE getRoomUsageStatistics(
    IN p_RoomID INT,
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    SELECT 
        r.RoomNumber,
        r.RoomType,
        COUNT(DISTINCT b.BookingID) as TotalBookings,
        SUM(DATEDIFF(b.CheckOutDate, b.CheckInDate)) as TotalDaysBooked,
        SUM(i.TotalAmount) as TotalRevenue,
        AVG(DATEDIFF(b.CheckOutDate, b.CheckInDate)) as AverageStayDuration
    FROM Rooms r
    LEFT JOIN Bookings b ON r.RoomID = b.RoomID
    LEFT JOIN Invoices i ON b.BookingID = i.BookingID
    WHERE r.RoomID = p_RoomID
    AND (b.CheckInDate IS NULL OR b.CheckInDate BETWEEN p_StartDate AND p_EndDate)
    GROUP BY r.RoomID, r.RoomNumber, r.RoomType;
END //

DELIMITER //

-- Stored procedure để check-in với transaction
CREATE PROCEDURE checkIn(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_Status VARCHAR(20);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy thông tin booking
    SELECT RoomID, Status INTO v_RoomID, v_Status
    FROM Bookings
    WHERE BookingID = p_BookingID;

    IF v_RoomID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng!';
    END IF;

    IF v_Status != 'Active' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Đặt phòng này không trong trạng thái active!';
    END IF;

    -- Kiểm tra trạng thái phòng
    IF EXISTS (
        SELECT 1
        FROM Rooms
        WHERE RoomID = v_RoomID
        AND Status != 'Available'
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Phòng không khả dụng để check-in!';
    END IF;

    -- Cập nhật trạng thái phòng
    UPDATE Rooms
    SET Status = 'Occupied',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = v_RoomID;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Check-in cho đặt phòng ID: ', p_BookingID), p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để check-out và tạo hóa đơn với transaction
CREATE PROCEDURE checkOut(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_CustomerID INT;
    DECLARE v_TotalAmount DECIMAL(10,2);
    DECLARE v_ServiceTotal DECIMAL(10,2);
    DECLARE v_RoomPrice DECIMAL(10,2);
    DECLARE v_Days INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy thông tin booking
    SELECT 
        b.RoomID, 
        b.CustomerID,
        r.Price,
        DATEDIFF(b.CheckOutDate, b.CheckInDate) as Days
    INTO v_RoomID, v_CustomerID, v_RoomPrice, v_Days
    FROM Bookings b
    JOIN Rooms r ON b.RoomID = r.RoomID
    WHERE b.BookingID = p_BookingID
    AND b.Status = 'Active';

    IF v_RoomID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng active!';
    END IF;

    -- Tính tổng tiền dịch vụ
    SELECT COALESCE(SUM(TotalPrice), 0)
    INTO v_ServiceTotal
    FROM ServiceUsage
    WHERE BookingID = p_BookingID
    AND PaymentStatus = 'Unpaid';

    -- Tính tổng tiền (phòng + dịch vụ)
    SET v_TotalAmount = (v_RoomPrice * v_Days) + v_ServiceTotal;

    -- Cập nhật trạng thái đặt phòng
    UPDATE Bookings
    SET Status = 'Completed',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;

    -- Cập nhật trạng thái phòng
    UPDATE Rooms
    SET Status = 'Available',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = v_RoomID;

    -- Tạo hóa đơn
    INSERT INTO Invoices (
        BookingID,
        CustomerID,
        TotalAmount,
        PaymentStatus,
        UpdatedBy,
        UpdatedByUsername
    )
    VALUES (
        p_BookingID,
        v_CustomerID,
        v_TotalAmount,
        'Unpaid',
        p_UpdatedBy,
        p_UpdatedByUsername
    );

    -- Cập nhật trạng thái thanh toán dịch vụ
    UPDATE ServiceUsage
    SET PaymentStatus = 'Paid',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID
    AND PaymentStatus = 'Unpaid';

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Check-out và tạo hóa đơn cho đặt phòng ID: ', p_BookingID, 
                   ', Tổng tiền: ', v_TotalAmount),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để gia hạn đặt phòng
CREATE PROCEDURE extendBooking(
    IN p_BookingID INT,
    IN p_NewCheckOutDate DATETIME,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_CurrentCheckOutDate DATETIME;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy thông tin booking hiện tại
    SELECT RoomID, CheckOutDate
    INTO v_RoomID, v_CurrentCheckOutDate
    FROM Bookings
    WHERE BookingID = p_BookingID
    AND Status = 'Active';

    IF v_RoomID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng active!';
    END IF;

    IF p_NewCheckOutDate <= v_CurrentCheckOutDate THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ngày check-out mới phải sau ngày check-out hiện tại!';
    END IF;

    -- Kiểm tra xem phòng có được đặt trong khoảng thời gian gia hạn không
    IF EXISTS (
        SELECT 1
        FROM Bookings
        WHERE RoomID = v_RoomID
        AND BookingID != p_BookingID
        AND Status = 'Active'
        AND CheckInDate <= p_NewCheckOutDate
        AND CheckOutDate >= v_CurrentCheckOutDate
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể gia hạn vì phòng đã được đặt trong khoảng thời gian này!';
    END IF;

    -- Cập nhật ngày check-out
    UPDATE Bookings
    SET CheckOutDate = p_NewCheckOutDate,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Gia hạn đặt phòng ID: ', p_BookingID, 
                   ' đến ngày ', DATE_FORMAT(p_NewCheckOutDate, '%Y-%m-%d')),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để hủy đặt phòng với lý do
CREATE PROCEDURE cancelBooking(
    IN p_BookingID INT,
    IN p_CancellationReason VARCHAR(255),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy thông tin booking
    SELECT RoomID INTO v_RoomID
    FROM Bookings
    WHERE BookingID = p_BookingID
    AND Status = 'Active';

    IF v_RoomID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng active!';
    END IF;

    -- Cập nhật trạng thái đặt phòng
    UPDATE Bookings
    SET Status = 'Cancelled',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;

    -- Cập nhật trạng thái phòng
    UPDATE Rooms
    SET Status = 'Available',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = v_RoomID;

    -- Ghi log với lý do hủy
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Hủy đặt phòng ID: ', p_BookingID, 
                   '. Lý do: ', p_CancellationReason),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để tạo hóa đơn chi tiết
CREATE PROCEDURE createDetailedInvoice(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_CustomerID INT;
    DECLARE v_RoomTotal DECIMAL(10,2);
    DECLARE v_ServiceTotal DECIMAL(10,2);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy thông tin booking và tính tiền phòng
    SELECT 
        b.CustomerID,
        (DATEDIFF(b.CheckOutDate, b.CheckInDate) * r.Price) as RoomTotal
    INTO v_CustomerID, v_RoomTotal
    FROM Bookings b
    JOIN Rooms r ON b.RoomID = r.RoomID
    WHERE b.BookingID = p_BookingID;

    IF v_CustomerID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng!';
    END IF;

    -- Tính tổng tiền dịch vụ
    SELECT COALESCE(SUM(TotalPrice), 0)
    INTO v_ServiceTotal
    FROM ServiceUsage
    WHERE BookingID = p_BookingID
    AND PaymentStatus = 'Unpaid';

    -- Tạo hóa đơn
    INSERT INTO Invoices (
        BookingID,
        CustomerID,
        TotalAmount,
        PaymentStatus,
        UpdatedBy,
        UpdatedByUsername
    )
    VALUES (
        p_BookingID,
        v_CustomerID,
        v_RoomTotal + v_ServiceTotal,
        'Unpaid',
        p_UpdatedBy,
        p_UpdatedByUsername
    );

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Tạo hóa đơn cho đặt phòng ID: ', p_BookingID, 
                   '. Tổng tiền: ', v_RoomTotal + v_ServiceTotal),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để cập nhật trạng thái thanh toán hóa đơn
CREATE PROCEDURE updateInvoicePaymentStatus(
    IN p_InvoiceID INT,
    IN p_PaymentStatus ENUM('Paid', 'Unpaid'),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_BookingID INT;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Lấy BookingID từ hóa đơn
    SELECT BookingID INTO v_BookingID
    FROM Invoices
    WHERE InvoiceID = p_InvoiceID;

    IF v_BookingID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy hóa đơn!';
    END IF;

    -- Cập nhật trạng thái thanh toán hóa đơn
    UPDATE Invoices
    SET PaymentStatus = p_PaymentStatus,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE InvoiceID = p_InvoiceID;

    -- Nếu đã thanh toán, cập nhật trạng thái thanh toán của các dịch vụ
    IF p_PaymentStatus = 'Paid' THEN
        UPDATE ServiceUsage
        SET PaymentStatus = 'Paid',
            UpdatedBy = p_UpdatedBy,
            UpdatedByUsername = p_UpdatedByUsername,
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE BookingID = v_BookingID
        AND PaymentStatus = 'Unpaid';
    END IF;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Cập nhật trạng thái thanh toán hóa đơn ID: ', p_InvoiceID, 
                   ' thành ', p_PaymentStatus),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để thêm dịch vụ mới
CREATE PROCEDURE addService(
    IN p_ServiceName VARCHAR(50),
    IN p_Type ENUM('Food', 'Laundry', 'Spa', 'Other'),
    IN p_Price DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    IF p_Price <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Giá dịch vụ phải lớn hơn 0!';
    END IF;

    INSERT INTO Services (
        ServiceName,
        Type,
        Price,
        UpdatedBy,
        UpdatedByUsername
    )
    VALUES (
        p_ServiceName,
        p_Type,
        p_Price,
        p_UpdatedBy,
        p_UpdatedByUsername
    );

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Thêm dịch vụ mới: ', p_ServiceName, 
                   ', Loại: ', p_Type,
                   ', Giá: ', p_Price),
            p_UpdatedByUsername);
END //

-- Stored procedure để ghi nhận sử dụng dịch vụ
CREATE PROCEDURE recordServiceUsage(
    IN p_BookingID INT,
    IN p_ServiceID INT,
    IN p_Quantity INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_CustomerID INT;
    DECLARE v_ServicePrice DECIMAL(10,2);
    DECLARE v_TotalPrice DECIMAL(10,2);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Kiểm tra booking có tồn tại và active
    SELECT CustomerID INTO v_CustomerID
    FROM Bookings
    WHERE BookingID = p_BookingID
    AND Status = 'Active';

    IF v_CustomerID IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy đặt phòng active!';
    END IF;

    -- Lấy giá dịch vụ
    SELECT Price INTO v_ServicePrice
    FROM Services
    WHERE ServiceID = p_ServiceID;

    IF v_ServicePrice IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy dịch vụ!';
    END IF;

    -- Tính tổng tiền
    SET v_TotalPrice = v_ServicePrice * p_Quantity;

    -- Ghi nhận sử dụng dịch vụ
    INSERT INTO ServiceUsage (
        BookingID,
        ServiceID,
        CustomerID,
        Quantity,
        Date,
        TotalPrice,
        UpdatedBy,
        UpdatedByUsername
    )
    VALUES (
        p_BookingID,
        p_ServiceID,
        v_CustomerID,
        p_Quantity,
        CURDATE(),
        v_TotalPrice,
        p_UpdatedBy,
        p_UpdatedByUsername
    );

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Ghi nhận sử dụng dịch vụ - Booking ID: ', p_BookingID, 
                   ', Dịch vụ ID: ', p_ServiceID,
                   ', Số lượng: ', p_Quantity,
                   ', Tổng tiền: ', v_TotalPrice),
            p_UpdatedByUsername);

    COMMIT;
END //

-- Stored procedure để xem danh sách dịch vụ theo loại
CREATE PROCEDURE viewServicesByType(
    IN p_Type ENUM('Food', 'Laundry', 'Spa', 'Other')
)
BEGIN
    IF p_Type IS NULL THEN
        SELECT * FROM Services ORDER BY Type, ServiceName;
    ELSE
        SELECT * FROM Services WHERE Type = p_Type ORDER BY ServiceName;
    END IF;
END //

-- Stored procedure để cập nhật thông tin dịch vụ
CREATE PROCEDURE updateService(
    IN p_ServiceID INT,
    IN p_ServiceName VARCHAR(50),
    IN p_Type ENUM('Food', 'Laundry', 'Spa', 'Other'),
    IN p_Price DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    IF p_Price <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Giá dịch vụ phải lớn hơn 0!';
    END IF;

    UPDATE Services
    SET ServiceName = p_ServiceName,
        Type = p_Type,
        Price = p_Price,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE ServiceID = p_ServiceID;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không tìm thấy dịch vụ!';
    END IF;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, 
            CONCAT('Cập nhật thông tin dịch vụ ID: ', p_ServiceID,
                   ', Tên: ', p_ServiceName,
                   ', Giá mới: ', p_Price),
            p_UpdatedByUsername);
END //

-- Stored procedure để tạo báo cáo doanh thu theo khoảng thời gian
CREATE PROCEDURE generateRevenueReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    -- Báo cáo doanh thu tổng quan
    SELECT 
        COUNT(DISTINCT b.BookingID) as TotalBookings,
        COUNT(DISTINCT r.RoomID) as RoomsUsed,
        SUM(i.TotalAmount) as TotalRevenue,
        AVG(i.TotalAmount) as AverageRevenuePerBooking,
        SUM(su.TotalPrice) as TotalServiceRevenue
    FROM Bookings b
    JOIN Rooms r ON b.RoomID = r.RoomID
    LEFT JOIN Invoices i ON b.BookingID = i.BookingID
    LEFT JOIN ServiceUsage su ON b.BookingID = su.BookingID
    WHERE b.CreatedAt BETWEEN p_StartDate AND p_EndDate
    AND b.Status != 'Cancelled';

    -- Chi tiết doanh thu theo loại phòng
    SELECT 
        r.RoomType,
        COUNT(DISTINCT b.BookingID) as Bookings,
        SUM(i.TotalAmount) as Revenue,
        AVG(i.TotalAmount) as AverageRevenue
    FROM Bookings b
    JOIN Rooms r ON b.RoomID = r.RoomID
    LEFT JOIN Invoices i ON b.BookingID = i.BookingID
    WHERE b.CreatedAt BETWEEN p_StartDate AND p_EndDate
    AND b.Status != 'Cancelled'
    GROUP BY r.RoomType;

    -- Chi tiết doanh thu theo loại dịch vụ
    SELECT 
        s.Type as ServiceType,
        COUNT(su.UsageID) as TimesUsed,
        SUM(su.TotalPrice) as Revenue,
        AVG(su.TotalPrice) as AverageRevenue
    FROM ServiceUsage su
    JOIN Services s ON su.ServiceID = s.ServiceID
    WHERE su.Date BETWEEN p_StartDate AND p_EndDate
    GROUP BY s.Type;
END //

-- Stored procedure để tạo báo cáo tỷ lệ lấp đầy phòng
CREATE PROCEDURE generateOccupancyReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    DECLARE v_TotalRooms INT;
    DECLARE v_TotalDays INT;

    -- Lấy tổng số phòng
    SELECT COUNT(*) INTO v_TotalRooms FROM Rooms;
    
    -- Tính số ngày trong khoảng thời gian
    SET v_TotalDays = DATEDIFF(p_EndDate, p_StartDate) + 1;

    -- Tỷ lệ lấp đầy tổng thể
    SELECT 
        COUNT(DISTINCT b.BookingID) as TotalBookings,
        SUM(DATEDIFF(LEAST(b.CheckOutDate, p_EndDate), 
                    GREATEST(b.CheckInDate, p_StartDate)) + 1) as OccupiedRoomDays,
        (SUM(DATEDIFF(LEAST(b.CheckOutDate, p_EndDate), 
                     GREATEST(b.CheckInDate, p_StartDate)) + 1) / 
         (v_TotalRooms * v_TotalDays) * 100) as OccupancyRate
    FROM Bookings b
    WHERE b.Status != 'Cancelled'
    AND b.CheckInDate <= p_EndDate
    AND b.CheckOutDate >= p_StartDate;

    -- Tỷ lệ lấp đầy theo loại phòng
    SELECT 
        r.RoomType,
        COUNT(DISTINCT r.RoomID) as TotalRooms,
        COUNT(DISTINCT b.BookingID) as Bookings,
        (COUNT(DISTINCT b.BookingID) / (COUNT(DISTINCT r.RoomID) * v_TotalDays) * 100) as OccupancyRate
    FROM Rooms r
    LEFT JOIN Bookings b ON r.RoomID = b.RoomID
    AND b.Status != 'Cancelled'
    AND b.CheckInDate <= p_EndDate
    AND b.CheckOutDate >= p_StartDate
    GROUP BY r.RoomType;
END //

-- Stored procedure để tạo báo cáo thống kê khách hàng
CREATE PROCEDURE generateCustomerReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    -- Thống kê khách hàng theo quốc tịch
    SELECT 
        c.Nationality,
        COUNT(DISTINCT c.CustomerID) as TotalCustomers,
        COUNT(DISTINCT b.BookingID) as TotalBookings,
        AVG(i.TotalAmount) as AverageSpending
    FROM Customers c
    LEFT JOIN Bookings b ON c.CustomerID = b.CustomerID
    AND b.CreatedAt BETWEEN p_StartDate AND p_EndDate
    LEFT JOIN Invoices i ON b.BookingID = i.BookingID
    GROUP BY c.Nationality
    ORDER BY TotalCustomers DESC;

    -- Khách hàng thường xuyên
    SELECT 
        c.CustomerID,
        c.Name,
        c.Nationality,
        COUNT(b.BookingID) as TotalBookings,
        SUM(i.TotalAmount) as TotalSpending
    FROM Customers c
    JOIN Bookings b ON c.CustomerID = b.CustomerID
    JOIN Invoices i ON b.BookingID = i.BookingID
    WHERE b.CreatedAt BETWEEN p_StartDate AND p_EndDate
    GROUP BY c.CustomerID, c.Name, c.Nationality
    HAVING COUNT(b.BookingID) > 1
    ORDER BY TotalBookings DESC, TotalSpending DESC;
END //

-- Stored procedure để tạo báo cáo dịch vụ
CREATE PROCEDURE generateServiceReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    -- Thống kê sử dụng dịch vụ
    SELECT 
        s.ServiceID,
        s.ServiceName,
        s.Type,
        COUNT(su.UsageID) as TimesUsed,
        SUM(su.Quantity) as TotalQuantity,
        SUM(su.TotalPrice) as TotalRevenue,
        AVG(su.TotalPrice) as AverageRevenue
    FROM Services s
    LEFT JOIN ServiceUsage su ON s.ServiceID = su.ServiceID
    AND su.Date BETWEEN p_StartDate AND p_EndDate
    GROUP BY s.ServiceID, s.ServiceName, s.Type
    ORDER BY TotalRevenue DESC;

    -- Thống kê theo loại dịch vụ
    SELECT 
        s.Type,
        COUNT(DISTINCT s.ServiceID) as NumberOfServices,
        COUNT(su.UsageID) as TimesUsed,
        SUM(su.TotalPrice) as TotalRevenue
    FROM Services s
    LEFT JOIN ServiceUsage su ON s.ServiceID = su.ServiceID
    AND su.Date BETWEEN p_StartDate AND p_EndDate
    GROUP BY s.Type
    ORDER BY TotalRevenue DESC;

    -- Top khách hàng sử dụng dịch vụ nhiều nhất
    SELECT 
        c.CustomerID,
        c.Name,
        COUNT(su.UsageID) as ServiceUsages,
        SUM(su.TotalPrice) as TotalSpent
    FROM Customers c
    JOIN ServiceUsage su ON c.CustomerID = su.CustomerID
    WHERE su.Date BETWEEN p_StartDate AND p_EndDate
    GROUP BY c.CustomerID, c.Name
    ORDER BY TotalSpent DESC
    LIMIT 10;
END //

DELIMITER //

-- ===================== DỮ LIỆU MẪU =====================
-- Insert Users (10 users, dùng RoleID 1,2,3)
INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername) VALUES
('admin1', 'adminpass1', 1, NULL, NULL),
('admin2', 'adminpass2', 1, NULL, NULL),
('recept1', 'receptpass1', 2, NULL, NULL),
('recept2', 'receptpass2', 2, NULL, NULL),
('house1', 'housepass1', 3, NULL, NULL),
('house2', 'housepass2', 3, NULL, NULL),
('user7', 'userpass7', 2, NULL, NULL),
('user8', 'userpass8', 3, NULL, NULL),
('user9', 'userpass9', 1, NULL, NULL),
('user10', 'userpass10', 2, NULL, NULL);

-- Insert Staff (10 staff, dùng các UserID đã tạo)
INSERT INTO Staff (Name, Role, Phone, UpdatedBy, UpdatedByUsername) VALUES
('Nguyen Van A', 'Receptionist', '0901000001', 1, 'admin1'),
('Tran Thi B', 'Housekeeping', '0901000002', 2, 'admin2'),
('Le Van C', 'Manager', '0901000003', 3, 'recept1'),
('Pham Thi D', 'Receptionist', '0901000004', 4, 'recept2'),
('Hoang Van E', 'Housekeeping', '0901000005', 5, 'house1'),
('Do Thi F', 'Manager', '0901000006', 6, 'house2'),
('Bui Van G', 'Receptionist', '0901000007', 7, 'user7'),
('Vu Thi H', 'Housekeeping', '0901000008', 8, 'user8'),
('Ngo Van I', 'Manager', '0901000009', 9, 'user9'),
('Dang Thi K', 'Receptionist', '0901000010', 10, 'user10');

-- Insert Customers (10 customers)
INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) VALUES
('Nguyen Thanh', '0123456781', '0912000001', 'thanh1@email.com', 'Vietnam', 1, 'admin1'),
('Tran Binh', '0123456782', '0912000002', 'binh2@email.com', 'Vietnam', 2, 'admin2'),
('Le Hoa', '0123456783', '0912000003', 'hoa3@email.com', 'Vietnam', 3, 'recept1'),
('Pham Son', '0123456784', '0912000004', 'son4@email.com', 'Vietnam', 4, 'recept2'),
('Hoang Mai', '0123456785', '0912000005', 'mai5@email.com', 'Vietnam', 5, 'house1'),
('Doan Kien', '0123456786', '0912000006', 'kien6@email.com', 'Vietnam', 6, 'house2'),
('Bui Lan', '0123456787', '0912000007', 'lan7@email.com', 'Vietnam', 7, 'user7'),
('Vu Quang', '0123456788', '0912000008', 'quang8@email.com', 'Vietnam', 8, 'user8'),
('Ngo Hieu', '0123456789', '0912000009', 'hieu9@email.com', 'Vietnam', 9, 'user9'),
('Dang Tuan', '0123456790', '0912000010', 'tuan10@email.com', 'Vietnam', 10, 'user10');

-- Insert Rooms (10 rooms)
INSERT INTO Rooms (RoomNumber, RoomType, Price, Status, Amenities, UpdatedBy, UpdatedByUsername) VALUES
('101', 'Single', 500000, 'Available', '["WiFi","TV"]', 1, 'admin1'),
('102', 'Double', 700000, 'Occupied', '["WiFi","TV","Mini Bar"]', 2, 'admin2'),
('103', 'Suite', 1200000, 'Available', '["WiFi","TV","Bathtub"]', 3, 'recept1'),
('104', 'Single', 520000, 'Available', '["WiFi"]', 4, 'recept2'),
('105', 'Double', 750000, 'Occupied', '["WiFi","TV","Balcony"]', 5, 'house1'),
('106', 'Suite', 1300000, 'Available', '["WiFi","TV","Jacuzzi"]', 6, 'house2'),
('107', 'Single', 510000, 'Available', '["WiFi","TV"]', 7, 'user7'),
('108', 'Double', 730000, 'Available', '["WiFi","TV","Mini Bar"]', 8, 'user8'),
('109', 'Suite', 1250000, 'Available', '["WiFi","TV","Bathtub"]', 9, 'user9'),
('110', 'Single', 540000, 'Available', '["WiFi"]', 10, 'user10');

-- Insert Bookings (10 bookings, dùng CustomerID 1-10, RoomID 1-10)
INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate, Status, UpdatedBy, UpdatedByUsername) VALUES
(1, 2, '2025-06-10 14:00:00', '2025-06-12 12:00:00', 'Completed', 1, 'admin1'),
(2, 5, '2025-06-15 14:00:00', '2025-06-18 12:00:00', 'Active', 2, 'admin2'),
(3, 3, '2025-06-11 14:00:00', '2025-06-13 12:00:00', 'Completed', 3, 'recept1'),
(4, 4, '2025-06-16 14:00:00', '2025-06-19 12:00:00', 'Cancelled', 4, 'recept2'),
(5, 1, '2025-06-12 14:00:00', '2025-06-14 12:00:00', 'Completed', 5, 'house1'),
(6, 6, '2025-06-17 14:00:00', '2025-06-20 12:00:00', 'Active', 6, 'house2'),
(7, 7, '2025-06-13 14:00:00', '2025-06-15 12:00:00', 'Completed', 7, 'user7'),
(8, 8, '2025-06-18 14:00:00', '2025-06-21 12:00:00', 'Active', 8, 'user8'),
(9, 9, '2025-06-14 14:00:00', '2025-06-16 12:00:00', 'Completed', 9, 'user9'),
(10, 10, '2025-06-19 14:00:00', '2025-06-22 12:00:00', 'Active', 10, 'user10');

-- Insert Services (10 services)
INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername) VALUES
('Buffet Sáng', 'Food', 150000, 1, 'admin1'),
('Giặt ủi', 'Laundry', 50000, 1, 'admin1'),
('Spa Relax', 'Spa', 300000, 1, 'admin1'),
('Đồ uống Mini Bar', 'Food', 80000, 1, 'admin1'),
('Dọn phòng', 'Other', 40000, 1, 'admin1'),
('Bữa tối', 'Food', 200000, 1, 'admin1'),
('Massage', 'Spa', 350000, 1, 'admin1'),
('Giặt khô', 'Laundry', 70000, 1, 'admin1'),
('Trái cây phòng', 'Food', 60000, 1, 'admin1'),
('Xông hơi', 'Spa', 250000, 1, 'admin1');

-- Insert Invoices (10 invoices, BookingID 1-10, CustomerID 1-10)
INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, PaymentStatus, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, 1200000, 'Paid', 1, 'admin1'),
(2, 2, 1400000, 'Paid', 1, 'admin1'),
(3, 3, 2400000, 'Paid', 1, 'admin1'),
(4, 4, 1000000, 'Paid', 1, 'admin1'),
(5, 5, 1400000, 'Paid', 1, 'admin1'),
(6, 6, 2400000, 'Paid', 1, 'admin1'),
(7, 7, 1000000, 'Paid', 1, 'admin1'),
(8, 8, 1400000, 'Paid', 1, 'admin1'),
(9, 9, 2400000, 'Paid', 1, 'admin1'),
(10, 10, 1000000, 'Unpaid', 1, 'admin1');
-- =================== KẾT THÚC DỮ LIỆU MẪU ===================
