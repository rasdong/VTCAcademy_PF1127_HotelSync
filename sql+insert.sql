-- Drop the database if it exists
DROP DATABASE IF EXISTS hotel_management;

-- Create the database
CREATE DATABASE hotel_management;
USE hotel_management;

-- Users Table
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(10) NOT NULL,
    RoleID INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

-- Roles Table
CREATE TABLE Roles (
    RoleID INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE,
    Permissions JSON NOT NULL,
    CreatedAt VARCHAR(255),
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy INT NULL,
    UpdatedByUsername VARCHAR(50) NULL 
);

-- Add foreign key constraints for Users and Roles
ALTER TABLE Users
    ADD CONSTRAINT fk_users_roleid FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_users_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE Roles
    ADD CONSTRAINT fk_roles_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

-- Insert sample data for Roles
INSERT INTO Roles (RoleName, Permissions, UpdatedBy, UpdatedByUsername, CreatedAt) VALUES
('Admin', '["manage_rooms", "manage_customers", "manage_bookings", "manage_invoices", "manage_services", "manage_staff", "manage_users", "view_reports"]', NULL, NULL, '2024-10-15 10:00:00'),
('Receptionist', '["manage_customers", "manage_bookings", "manage_invoices"]', NULL, NULL, '2024-10-15 10:00:00'),
('Housekeeping', '["manage_rooms", "manage_services"]', NULL, NULL, '2024-10-15 10:00:00');

-- Rooms Table
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

-- Add foreign key and indexes for Rooms
ALTER TABLE Rooms
    ADD CONSTRAINT fk_rooms_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_room_number ON Rooms(RoomNumber);
CREATE INDEX idx_room_status_type ON Rooms(Status, RoomType);

-- Customers Table
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

-- Add foreign key and indexes for Customers
ALTER TABLE Customers
    ADD CONSTRAINT fk_customers_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_customer_idcard ON Customers(IDCard);
CREATE INDEX idx_customer_phone ON Customers(Phone);

-- Bookings Table (Modified to include IDCard)
CREATE TABLE Bookings (
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
    UpdatedByUsername VARCHAR(50) NULL,
    FOREIGN KEY (IDCard) REFERENCES Customers(IDCard) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Add foreign key constraints and checks for Bookings
ALTER TABLE Bookings
    ADD CONSTRAINT fk_bookings_roomid FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_bookings_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_bookings_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE,
    ADD CONSTRAINT chk_dates CHECK (CheckInDate < CheckOutDate);

CREATE INDEX idx_booking_dates ON Bookings(CheckInDate, CheckOutDate);
CREATE INDEX idx_booking_customer ON Bookings(CustomerID);
CREATE INDEX idx_booking_room ON Bookings(RoomID);
CREATE INDEX idx_booking_status ON Bookings(Status);

-- Invoices Table
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

-- Add foreign key and indexes for Invoices
ALTER TABLE Invoices
    ADD CONSTRAINT fk_invoices_bookingid FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_invoices_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_invoices_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_invoice_booking ON Invoices(BookingID);
CREATE INDEX idx_invoice_customer ON Invoices(CustomerID);
CREATE INDEX idx_invoice_payment_status ON Invoices(PaymentStatus);

-- Services Table
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

-- Add foreign key and indexes for Services
ALTER TABLE Services
    ADD CONSTRAINT fk_services_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_service_type ON Services(Type);

-- ServiceUsage Table
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

-- Add foreign key and indexes for ServiceUsage
ALTER TABLE ServiceUsage
    ADD CONSTRAINT fk_serviceusage_bookingid FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_serviceid FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_customerid FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT ON UPDATE CASCADE,
    ADD CONSTRAINT fk_serviceusage_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_service_usage_booking ON ServiceUsage(BookingID);
CREATE INDEX idx_service_usage_service ON ServiceUsage(ServiceID);
CREATE INDEX idx_service_usage_date ON ServiceUsage(Date);
CREATE INDEX idx_service_usage_payment_status ON ServiceUsage(PaymentStatus); 

-- Staff Table
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

-- Add foreign key and indexes for Staff
ALTER TABLE Staff
    ADD CONSTRAINT fk_staff_updatedby FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE;

CREATE INDEX idx_staff_role ON Staff(Role);

-- Logs Table
CREATE TABLE Logs (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Action VARCHAR(255) NOT NULL,
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedByUsername VARCHAR(50) NULL 
);

-- Add foreign key and indexes for Logs
ALTER TABLE Logs
    ADD CONSTRAINT fk_logs_userid FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE RESTRICT ON UPDATE CASCADE;

CREATE INDEX idx_log_user ON Logs(UserID);
CREATE INDEX idx_log_timestamp ON Logs(Timestamp);

-- Stored Procedure: Add Room
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

-- Stored Procedure: Update Customer
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

-- Stored Procedure: Generate Report
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

-- Stored Procedure: Get Booking History
DELIMITER //
CREATE PROCEDURE getBookingHistory(
    IN p_CustomerID INT,
    IN p_RoomID INT
)
BEGIN
    SELECT BookingID, CustomerID, RoomID, CheckInDate, CheckOutDate, Status
    FROM Bookings
    WHERE CustomerID = p_CustomerID
    AND (p_RoomID IS NULL OR RoomID = p_RoomID);
END //
DELIMITER ;

-- Stored Procedure: Check Room Availability
DELIMITER //
CREATE PROCEDURE checkRoomAvailability(
    IN p_StartDate DATETIME,
    IN p_EndDate DATETIME
)
BEGIN
    SELECT r.RoomID, r.RoomNumber, r.RoomType, r.Price, r.Status
    FROM Rooms r
    WHERE r.Status = 'Available'
    AND NOT EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.RoomID = r.RoomID
        AND b.Status = 'Active'
        AND (p_StartDate < b.CheckOutDate AND p_EndDate > b.CheckInDate)
    );
END //
DELIMITER ;





-- Stored Procedure: Get All Rooms
DELIMITER //
CREATE PROCEDURE getAllRooms()
BEGIN
    SELECT RoomID, RoomNumber, RoomType, Price, Status, Amenities
    FROM Rooms;
END //
DELIMITER ;

-- Stored Procedure: Search Rooms
DELIMITER //
CREATE PROCEDURE searchRooms(
    IN p_Status VARCHAR(50),
    IN p_RoomType VARCHAR(50),
    IN p_MinPrice DECIMAL(10,2),
    IN p_MaxPrice DECIMAL(10,2)
)
BEGIN
    SELECT RoomID, RoomNumber, RoomType, Price, Status, Amenities
    FROM Rooms
    WHERE (p_Status IS NULL OR Status = p_Status)
    AND (p_RoomType IS NULL OR RoomType = p_RoomType)
    AND (p_MinPrice IS NULL OR Price >= p_MinPrice)
    AND (p_MaxPrice IS NULL OR Price <= p_MaxPrice);
END //
DELIMITER ;

-- Stored Procedure: Clean Room
DELIMITER //
CREATE PROCEDURE cleanRoom(
    IN p_RoomID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Rooms
    SET Status = 'Available',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;
END //
DELIMITER ;

-- Stored Procedure Transaction: Add Room
DELIMITER //
CREATE PROCEDURE addRoomWithTransaction(
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType ENUM('Single', 'Double', 'Suite'),
    IN p_Price DECIMAL(10,2),
    IN p_Amenities JSON,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi thêm phòng';
    END;

    START TRANSACTION;

    -- Check if RoomNumber already exists
    IF EXISTS (SELECT 1 FROM Rooms WHERE RoomNumber = p_RoomNumber) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Số phòng đã tồn tại';
    END IF;

    -- Insert new room
    INSERT INTO Rooms (RoomNumber, RoomType, Price, Amenities, UpdatedBy, UpdatedByUsername)
    VALUES (p_RoomNumber, p_RoomType, p_Price, p_Amenities, p_UpdatedBy, p_UpdatedByUsername);

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Thêm phòng mới: ', p_RoomNumber), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Stored Procedure Transaction: Delete Room
DELIMITER //
CREATE PROCEDURE deleteRoomWithTransaction(
    IN p_RoomID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi xóa phòng';
    END;

    START TRANSACTION;

    -- Check if room has active bookings
    IF EXISTS (
        SELECT 1 FROM Bookings
        WHERE RoomID = p_RoomID AND Status = 'Active'
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Không thể xóa phòng vì đang có đặt phòng hoạt động';
    END IF;

    -- Delete room
    DELETE FROM Rooms WHERE RoomID = p_RoomID;

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xóa phòng ID ', p_RoomID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Stored Procedure Transaction: Update Room
DELIMITER //
CREATE PROCEDURE updateRoomWithTransaction(
    IN p_RoomID INT,
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType ENUM('Single', 'Double', 'Suite'),
    IN p_Price DECIMAL(10,2),
    IN p_Amenities JSON,
    IN p_Status ENUM('Available', 'Occupied', 'Under Maintenance'),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi cập nhật phòng';
    END;

    START TRANSACTION;

    -- Check if RoomNumber is duplicated (excluding current room)
    IF EXISTS (SELECT 1 FROM Rooms WHERE RoomNumber = p_RoomNumber AND RoomID != p_RoomID) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Số phòng đã tồn tại';
    END IF;

    -- Update room
    UPDATE Rooms
    SET RoomNumber = p_RoomNumber,
        RoomType = p_RoomType,
        Price = p_Price,
        Amenities = p_Amenities,
        Status = p_Status,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Cập nhật phòng ID ', p_RoomID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Stored Procedure Transaction: Clean Room
DELIMITER //
CREATE PROCEDURE cleanRoomWithTransaction(
    IN p_RoomID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi dọn phòng';
    END;

    START TRANSACTION;

    -- Kiểm tra trạng thái phòng phải là 'Uncleaned'
    IF NOT EXISTS (
        SELECT 1 FROM Rooms WHERE RoomID = p_RoomID AND Status = 'Uncleaned'
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Chỉ có thể dọn phòng ở trạng thái Uncleaned';
    END IF;

    -- Cập nhật trạng thái phòng
    UPDATE Rooms
    SET Status = 'Available',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;

    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Dọn phòng ID ', p_RoomID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;


-- Stored Procedure Transaction: Create Booking
DELIMITER //
CREATE PROCEDURE createBookingWithTransaction(
    IN p_IDCard VARCHAR(20),
    IN p_RoomID INT,
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50),
    OUT p_BookingID INT
)
BEGIN
    DECLARE v_CustomerID INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi tạo đặt phòng';
    END;

    START TRANSACTION;

    -- Get CustomerID from IDCard
    SELECT CustomerID INTO v_CustomerID
    FROM Customers
    WHERE IDCard = p_IDCard;

    -- Check if room is available
    IF EXISTS (
        SELECT 1 FROM Bookings
        WHERE RoomID = p_RoomID
        AND Status = 'Active'
        AND (p_CheckInDate < CheckOutDate AND p_CheckOutDate > CheckInDate)
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Phòng đã được đặt trong khoảng thời gian này';
    END IF;

    -- Check if customer and room exist
    IF v_CustomerID IS NULL THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Khách hàng không tồn tại';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM Rooms WHERE RoomID = p_RoomID) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Phòng không tồn tại';
    END IF;

    -- Insert booking
    INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, UpdatedBy, UpdatedByUsername)
    VALUES (v_CustomerID, p_IDCard, p_RoomID, p_CheckInDate, p_CheckOutDate, p_UpdatedBy, p_UpdatedByUsername);

    -- Set output parameter to the newly created BookingID
    SET p_BookingID = LAST_INSERT_ID();

    -- Update room status
    UPDATE Rooms
    SET Status = 'Occupied',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Tạo đặt phòng ID ', p_BookingID, ' cho phòng ID ', p_RoomID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Stored Procedure Transaction: Cancel Booking
DELIMITER //
CREATE PROCEDURE cancelBookingWithTransaction(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_rowsAffected INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi hủy đặt phòng';
    END;

    START TRANSACTION;

    -- Check if booking exists and is valid
    IF NOT EXISTS (SELECT 1 FROM Bookings WHERE BookingID = p_BookingID AND Status = 'Active') THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Đặt phòng không tồn tại hoặc không hợp lệ để hủy';
    END IF;

    -- Get RoomID
    SELECT RoomID INTO v_RoomID FROM Bookings WHERE BookingID = p_BookingID;

    -- Cancel booking
    UPDATE Bookings
    SET Status = 'Cancelled',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Hủy đặt phòng thất bại do không ảnh hưởng dòng nào';
    END IF;

    -- Update room status
    UPDATE Rooms
    SET Status = 'Available',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = v_RoomID;

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Hủy đặt phòng ID ', p_BookingID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Stored Procedure Transaction: Check-in
DELIMITER //
CREATE PROCEDURE checkInWithTransaction(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_rowsAffected INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi check-in';
    END;

    START TRANSACTION;

    -- Chỉ kiểm tra tồn tại và trạng thái Active
    IF NOT EXISTS (
        SELECT 1 FROM Bookings 
        WHERE BookingID = p_BookingID 
        AND Status = 'Active'
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Đặt phòng không tồn tại hoặc không hợp lệ để check-in';
    END IF;

    -- Lấy RoomID
    SELECT RoomID INTO v_RoomID FROM Bookings WHERE BookingID = p_BookingID;

    -- Cập nhật trạng thái booking (không cần thay đổi Status vì đã Active)
    UPDATE Bookings
    SET UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Check-in thất bại: Không tìm thấy hoặc không cập nhật được bản ghi';
    END IF;

    -- Cập nhật trạng thái phòng thành 'Occupied'
    UPDATE Rooms
    SET Status = 'Occupied',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = v_RoomID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Check-in thất bại: Không tìm thấy phòng để cập nhật';
    END IF;

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Check-in đặt phòng ID ', p_BookingID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

	-- Stored Procedure Transaction: Check-out

	DELIMITER //
CREATE PROCEDURE checkOutWithTransaction(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_rowsAffected INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi check-out';
    END;

    START TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM Bookings WHERE BookingID = p_BookingID AND Status = 'Active') THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Đặt phòng không tồn tại hoặc không hợp lệ để check-out';
    END IF;

    SELECT RoomID INTO v_RoomID FROM Bookings WHERE BookingID = p_BookingID;

    UPDATE Bookings
    SET Status = 'Completed',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Check-out thất bại: Không tìm thấy hoặc không cập nhật được bản ghi';
    END IF;

    UPDATE Rooms
    SET Status = 'Uncleaned',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = v_RoomID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Check-out thất bại: Không tìm thấy phòng để cập nhật';
    END IF;

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Check-out đặt phòng ID ', p_BookingID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Stored Procedure Transaction: Extend Booking
DELIMITER //
CREATE PROCEDURE extendBookingWithTransaction(
    IN p_BookingID INT,
    IN p_NewEndDate DATE,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_CurrentEndDate DATE;
    DECLARE v_rowsAffected INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi gia hạn đặt phòng';
    END;

    START TRANSACTION;

    -- Check if booking exists and is valid
    IF NOT EXISTS (SELECT 1 FROM Bookings WHERE BookingID = p_BookingID AND Status = 'Active') THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Đặt phòng không tồn tại hoặc không hợp lệ để gia hạn';
    END IF;

    -- Get RoomID and current EndDate
    SELECT RoomID, EndDate INTO v_RoomID, v_CurrentEndDate
    FROM Bookings WHERE BookingID = p_BookingID;

    -- Check if new end date is valid
    IF p_NewEndDate <= v_CurrentEndDate THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ngày gia hạn phải lớn hơn ngày kết thúc hiện tại';
    END IF;

    -- Check for conflicting bookings
    IF EXISTS (
        SELECT 1 FROM Bookings
        WHERE RoomID = v_RoomID AND Status = 'Active'
        AND BookingID != p_BookingID
        AND StartDate <= p_NewEndDate AND EndDate >= CURDATE()
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Phòng đã được đặt trong khoảng thời gian gia hạn';
    END IF;

    -- Update booking
    UPDATE Bookings
    SET EndDate = p_NewEndDate,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Gia hạn đặt phòng thất bại do không ảnh hưởng dòng nào';
    END IF;

    -- Log action
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Gia hạn đặt phòng ID ', p_BookingID, ' đến ', p_NewEndDate), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

-- Trigger: Check before deleting a room
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

-- Trigger: Update room status after booking insert
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

-- Trigger: Update room status after booking update
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

-- Trigger: Calculate total price before inserting ServiceUsage
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

-- Trigger: Calculate total price before updating ServiceUsage
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

-- Trigger: Log after inserting an invoice
DELIMITER //
CREATE TRIGGER after_invoice_insert
AFTER INSERT ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo hóa đơn ID ', NEW.InvoiceID, ' cho đặt phòng ID ', NEW.BookingID), NEW.UpdatedByUsername);
END //
DELIMITER ;

-- Trigger: Check before deleting a user
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

-- Trigger: Log after updating room status
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

-- Trigger: Log after inserting a customer
DELIMITER //
CREATE TRIGGER after_customer_insert
AFTER INSERT ON Customers
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo khách hàng ID ', NEW.CustomerID, ' (', NEW.Name, ')'), NEW.UpdatedByUsername);
END //
DELIMITER ;

-- Trigger: Log after inserting a booking
DELIMITER //
CREATE TRIGGER after_booking_log_insert
AFTER INSERT ON Bookings
FOR EACH ROW
BEGIN
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (NEW.UpdatedBy, CONCAT('Tạo đặt phòng ID ', NEW.BookingID, ' cho phòng ID ', NEW.RoomID), NEW.UpdatedByUsername);
END //
DELIMITER ;

-- Trigger: Check permission before inserting staff
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

-- Trigger: Check permission before updating staff
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

-- Trigger: Check permission before deleting staff
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

-- Sample Data
INSERT INTO Users (Username, Password, RoleID) VALUES
('admin', 'admin123', 1),
('receptionist', 'recep123', 2),
('housekeeping', 'house123', 3);

INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) VALUES
('Nguyễn Văn A', '123456789', '0901234567', 'nva@example.com', 'Vietnam', 1, 'admin'),
('Trần Thị B', '987654321', '0907654321', 'ttb@example.com', 'Vietnam', 1, 'admin');

INSERT INTO Rooms (RoomID, RoomNumber, RoomType, Price, Amenities) VALUES
(1, '101', 'Single', 500000, '[]'),
(2, '102', 'Double', 750000, '[]');

INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername) VALUES
('Phòng đơn', 'Other', 500000, 1, 'admin'),
('Ăn sáng', 'Food', 100000, 1, 'admin');

INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, UpdatedBy, UpdatedByUsername) 
VALUES (1, '123456789', 1, '2025-06-20 14:00:00', '2025-06-21 12:00:00', 1, 'admin');

INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, 700000, 1, 'admin');

INSERT INTO ServiceUsage (BookingID, ServiceID, CustomerID, Quantity, Date, TotalPrice, PaymentStatus, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, 1, 1, '2025-06-15', 500000, 'Unpaid', 1, 'admin'),
(1, 2, 1, 2, '2025-06-15', 200000, 'Unpaid', 1, 'admin');

INSERT INTO Staff (Name, Role, Phone, UpdatedBy, UpdatedByUsername) VALUES
('Nguyen Van C', 'Receptionist', '0905555555', 1, 'admin'),
('Tran Thi D', 'Housekeeping', '0906666666', 1, 'admin');

INSERT INTO Logs (UserID, Action, UpdatedByUsername) VALUES
(1, 'Khởi tạo hệ thống', 'admin');
SELECT * FROM Bookings  ;
SELECT * FROM Rooms  ;
SELECT RoomID FROM Bookings WHERE BookingID = 1;
SELECT RoomID, Status FROM Rooms WHERE RoomID = (SELECT RoomID FROM Bookings WHERE BookingID = 1);
ALTER TABLE Rooms MODIFY COLUMN Status ENUM('Available', 'Occupied', 'Under Maintenance', 'Uncleaned') NOT NULL DEFAULT 'Available';
DELIMITER //

-- Thêm khách hàng (transaction-safe)
CREATE PROCEDURE addCustomerWithTransaction(
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
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi thêm khách hàng';
    END;

    START TRANSACTION;

    IF EXISTS (SELECT 1 FROM Customers WHERE CustomerID = p_CustomerID OR IDCard = p_IDCard) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Khách hàng đã tồn tại (ID hoặc IDCard trùng)';
    END IF;

    INSERT INTO Customers (CustomerID, Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername)
    VALUES (p_CustomerID, p_Name, p_IDCard, p_Phone, p_Email, p_Nationality, p_UpdatedBy, p_UpdatedByUsername);

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Thêm khách hàng mới: ', p_Name), p_UpdatedByUsername);

    COMMIT;
END //

-- Cập nhật khách hàng (transaction-safe)
CREATE PROCEDURE updateCustomerWithTransaction(
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
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi cập nhật khách hàng';
    END;

    START TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerID = p_CustomerID) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Khách hàng không tồn tại';
    END IF;

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

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Cập nhật khách hàng ID ', p_CustomerID), p_UpdatedByUsername);

    COMMIT;
END //

-- Xóa khách hàng 
CREATE PROCEDURE deleteCustomerWithTransaction(
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi xóa khách hàng';
    END;

    START TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerID = p_CustomerID) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Không tìm thấy khách hàng để xóa';
    END IF;

    DELETE FROM Customers WHERE CustomerID = p_CustomerID;

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xóa khách hàng ID ', p_CustomerID), p_UpdatedByUsername);

    COMMIT;
    SELECT 1 AS Result;
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

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Tìm kiếm khách hàng ID ', p_CustomerID), p_UpdatedByUsername);
END //

-- Lịch sử đặt phòng của khách hàng
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
    JOIN Rooms r ON b.RoomID = r.RoomID
    LEFT JOIN Invoices i ON b.BookingID = i.BookingID
    WHERE b.CustomerID = p_CustomerID;

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xem lịch sử đặt phòng khách hàng ID ', p_CustomerID), p_UpdatedByUsername);
END //

--  Thêm nhân viên (transaction-safe)
DELIMITER //
CREATE PROCEDURE addEmployeeWithTransaction(
    IN p_StaffID INT,
    IN p_Name VARCHAR(100),
    IN p_Role ENUM('Receptionist', 'Housekeeping', 'Manager'),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi thêm nhân viên';
    END;

    START TRANSACTION;

    IF EXISTS (SELECT 1 FROM Staff WHERE StaffID = p_StaffID) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Staff ID đã tồn tại';
    END IF;

    INSERT INTO Staff (StaffID, Name, Role, UpdatedBy, UpdatedByUsername)
    VALUES (p_StaffID, p_Name, p_Role, p_UpdatedBy, p_UpdatedByUsername);

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Thêm nhân viên ID ', p_StaffID), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;

--  Xóa nhân viên (transaction-safe)
DELIMITER //
CREATE PROCEDURE deleteEmployeeWithTransaction(
    IN p_StaffID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_rows_deleted INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi xóa nhân viên';
    END;

    START TRANSACTION;

    DELETE FROM Staff WHERE StaffID = p_StaffID;
    SET v_rows_deleted = ROW_COUNT();

    IF v_rows_deleted = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Không tìm thấy nhân viên để xóa';
    END IF;

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xóa nhân viên ID ', p_StaffID), p_UpdatedByUsername);

    COMMIT;
    SELECT 1 AS Result;
END //
DELIMITER ;

--  Gán vai trò (transaction-safe)
DELIMITER //
CREATE PROCEDURE assignEmployeeRoleWithTransaction(
    IN p_StaffID INT,
    IN p_NewRole ENUM('Receptionist', 'Housekeeping', 'Manager'),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_rows INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi cập nhật vai trò nhân viên';
    END;

    START TRANSACTION;

    UPDATE Staff
    SET Role = p_NewRole,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE StaffID = p_StaffID;

    SET v_rows = ROW_COUNT();

    IF v_rows = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Không tìm thấy nhân viên để cập nhật';
    END IF;

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Cập nhật vai trò nhân viên ID ', p_StaffID, ' thành ', p_NewRole), p_UpdatedByUsername);

    COMMIT;
    SELECT 1 AS Result;
END //
DELIMITER ;

--  Lấy danh sách nhân viên theo vai trò
DELIMITER //
CREATE PROCEDURE getEmployeesByRole(
    IN p_Role VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    IF p_Role IS NULL OR p_Role = 'All' THEN
        SELECT StaffID, Name, Role FROM Staff;
    ELSE
        SELECT StaffID, Name, Role FROM Staff WHERE Role = p_Role;
    END IF;

    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xem danh sách nhân viên theo vai trò: ', IFNULL(p_Role, 'All')), p_UpdatedByUsername);
END //
DELIMITER ;
