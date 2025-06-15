
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
    ADD CONSTRAINT fk_serviceusage_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE, -- Thêm ràng buộc này
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
DELIMITER ;


DELIMITER //
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
DELIMITER ;


DELIMITER //
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
DELIMITER ;


DELIMITER //
CREATE PROCEDURE generateReport(
    IN p_ReportType VARCHAR(50),
    IN p_UpdatedBy INT, 
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Tạo báo cáo ', p_ReportType), p_UpdatedByUsername);
END //
DELIMITER ;


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


DELIMITER //
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
DELIMITER ;


DELIMITER //
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
DELIMITER ;


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


DELIMITER //
CREATE TRIGGER after_invoice_insert
AFTER INSERT ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo hóa đơn ID ', NEW.InvoiceID, ' cho đặt phòng ID ', NEW.BookingID), NEW.UpdatedByUsername);
END //
DELIMITER ;


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


DELIMITER //
CREATE TRIGGER after_room_status_update
AFTER UPDATE ON Rooms
FOR EACH ROW
BEGIN
    IF OLD.Status != NEW.Status THEN
        INSERT INTO Logs (UserID, Action, UpdatedByUsername)
        VALUES (NEW.UpdatedBy, CONCAT('Thay đổi trạng thái phòng ', NEW.RoomNumber, ' từ ', OLD.Status, ' sang ', NEW.Status), NEW.UpdatedByUsername);
    END IF;
END //
DELIMITER ;


DELIMITER //
CREATE TRIGGER after_customer_insert
AFTER INSERT ON Customers
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo khách hàng ID ', NEW.CustomerID, ' (', NEW.Name, ')'), NEW.UpdatedByUsername);
END //
DELIMITER ;


DELIMITER //
CREATE TRIGGER after_booking_log_insert
AFTER INSERT ON Bookings
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo đặt phòng ID ', NEW.BookingID, ' cho phòng ID ', NEW.RoomID), NEW.UpdatedByUsername);
END //
DELIMITER ;


DELIMITER //
CREATE TRIGGER before_staff_insert
BEFORE INSERT ON Staff
FOR EACH ROW
BEGIN
    DECLARE user_role VARCHAR(50);
    SELECT r.RoleName INTO user_role
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.UserID = NEW.UpdatedBy; 
    IF user_role IN ('Receptionist', 'Housekeeping') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Lễ tân hoặc Buồng phòng không thể quản lý nhân viên';
    END IF;
END //
DELIMITER ;


DELIMITER //
CREATE TRIGGER before_staff_update
BEFORE UPDATE ON Staff
FOR EACH ROW
BEGIN
    DECLARE user_role VARCHAR(50);
    SELECT r.RoleName INTO user_role
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.UserID = NEW.UpdatedBy; 
    IF user_role IN ('Receptionist', 'Housekeeping') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Lễ tân hoặc Buồng phòng không thể quản lý nhân viên';
    END IF;
END //
DELIMITER ;


DELIMITER //
CREATE TRIGGER before_staff_delete
BEFORE DELETE ON Staff
FOR EACH ROW
BEGIN
    DECLARE user_role VARCHAR(50);
    SELECT r.RoleName INTO user_role
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.UserID = OLD.UpdatedBy; 
    IF user_role IN ('Receptionist', 'Housekeeping') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Lễ tân hoặc Buồng phòng không thể quản lý nhân viên';
    END IF;
END //
DELIMITER ;

INSERT INTO Users (Username, Password, RoleID) VALUES
('admin', 'admin123', 1),
('receptionist', 'recep123', 2),
('housekeeping', 'house123', 3);

INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) VALUES
('Nguyễn Văn A', '123456789', '0901234567', 'nva@example.com', 'Vietnam', 1, 'admin'),
('Trần Thị B', '987654321', '0907654321', 'ttb@example.com', 'Vietnam', 1, 'admin');

INSERT INTO Rooms (RoomNumber, RoomType, Price, Amenities) VALUES
('101', 'Single', 500000, '[]'),
('102', 'Double', 750000, '[]');

INSERT INTO Services (ServiceName, Type, Price) VALUES
('Phòng đơn', 'Other', 500000),
('Ăn sáng', 'Food', 100000);

INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, '2025-06-15 14:00:00', '2025-06-17 12:00:00', 1, 'admin');

INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, 700000, 1, 'admin');

INSERT INTO ServiceUsage (BookingID, ServiceID, CustomerID, Quantity, Date, TotalPrice, PaymentStatus, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, 1, 1, '2025-06-15', 500000, 'Unpaid', 1, 'admin'),
(1, 2, 1, 2, '2025-06-15', 200000, 'Unpaid', 1, 'admin');

INSERT INTO Staff (Name, Role, Phone) VALUES
('Nguyen Van C', 'Receptionist', '0905555555'),
('Tran Thi D', 'Housekeeping', '0906666666');

INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES
(1, 'Khởi tạo hệ thống', 'admin');