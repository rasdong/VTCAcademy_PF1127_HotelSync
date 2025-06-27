-- Drop the database if it exists
DROP DATABASE IF EXISTS Hotel_management;

-- Create the database
CREATE DATABASE Hotel_management;
USE Hotel_management;

-- Users Table
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL, -- Increased size for BCrypt hash
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
    Status ENUM('Available', 'Occupied', 'Under Maintenance','Uncleaned') NOT NULL DEFAULT 'Available',
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


-- Add simple deleteRoom procedure that calls deleteRoomWithTransaction


-- Add simple updateRoom procedure  


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

-- Simple procedures that call transaction versions for backwards compatibility

-- Create Booking (calls transaction version)
DELIMITER //
CREATE PROCEDURE createBooking(
    IN p_IDCard VARCHAR(20),
    IN p_RoomID INT,
    IN p_CheckInDate DATETIME,
    IN p_CheckOutDate DATETIME,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50),
    OUT p_BookingID INT
)
BEGIN
    CALL createBookingWithTransaction(p_IDCard, p_RoomID, p_CheckInDate, p_CheckOutDate, p_UpdatedBy, p_UpdatedByUsername, p_BookingID);
END //
DELIMITER ;

-- Cancel Booking (calls transaction version)
DELIMITER //
CREATE PROCEDURE cancelBooking(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    CALL cancelBookingWithTransaction(p_BookingID, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

-- Check In (simple procedure)
DELIMITER //
CREATE PROCEDURE checkIn(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Bookings
    SET Status = 'Active',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    
    -- Update room status to Occupied
    UPDATE Rooms r
    JOIN Bookings b ON r.RoomID = b.RoomID
    SET r.Status = 'Occupied',
        r.UpdatedBy = p_UpdatedBy,
        r.UpdatedByUsername = p_UpdatedByUsername,
        r.UpdatedAt = CURRENT_TIMESTAMP
    WHERE b.BookingID = p_BookingID;
END //
DELIMITER ;

-- Check Out (simple procedure)
DELIMITER //
CREATE PROCEDURE checkOut(
    IN p_BookingID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Bookings
    SET Status = 'Completed',
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    
    -- Update room status to Under Maintenance for cleaning
    UPDATE Rooms r
    JOIN Bookings b ON r.RoomID = b.RoomID
    SET r.Status = 'Under Maintenance',
        r.UpdatedBy = p_UpdatedBy,
        r.UpdatedByUsername = p_UpdatedByUsername,
        r.UpdatedAt = CURRENT_TIMESTAMP
    WHERE b.BookingID = p_BookingID;
END //
DELIMITER ;

-- Extend Booking (calls transaction version)
DELIMITER //
CREATE PROCEDURE extendBooking(
    IN p_BookingID INT,
    IN p_NewEndDate DATETIME,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    CALL extendBookingWithTransaction(p_BookingID, p_NewEndDate, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

-- Sample data for testing
-- Insert sample Users
INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername) VALUES
('admin', 'admin123', 1, NULL, NULL),
('receptionist1', 'recep123', 2, 1, 'admin'),
('housekeeper1', 'house123', 3, 1, 'admin');

-- Insert sample Customers
INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) VALUES
('Nguyễn Văn A', '123456789', '0901234567', 'nguyenvana@email.com', 'Vietnam', 1, 'admin'),
('Trần Thị B', '987654321', '0987654321', 'tranthib@email.com', 'Vietnam', 1, 'admin'),
('John Smith', '111222333', '0912345678', 'johnsmith@email.com', 'USA', 1, 'admin');

-- Insert sample Rooms
INSERT INTO Rooms (RoomNumber, RoomType, Price, Status, Amenities, UpdatedBy, UpdatedByUsername) VALUES
('101', 'Single', 500000.00, 'Available', '["TV", "WiFi", "Air Conditioning"]', 1, 'admin'),
('102', 'Single', 500000.00, 'Available', '["TV", "WiFi", "Air Conditioning"]', 1, 'admin'),
('201', 'Double', 800000.00, 'Available', '["TV", "WiFi", "Air Conditioning", "Mini Bar"]', 1, 'admin'),
('202', 'Double', 800000.00, 'Occupied', '["TV", "WiFi", "Air Conditioning", "Mini Bar"]', 1, 'admin'),
('301', 'Suite', 1500000.00, 'Available', '["TV", "WiFi", "Air Conditioning", "Mini Bar", "Balcony", "Jacuzzi"]', 1, 'admin'),
('302', 'Suite', 1500000.00, 'Under Maintenance', '["TV", "WiFi", "Air Conditioning", "Mini Bar", "Balcony", "Jacuzzi"]', 1, 'admin');

-- Insert sample Services
INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername) VALUES
('Room Service Breakfast', 'Food', 150000.00, 1, 'admin'),
('Laundry Service', 'Laundry', 100000.00, 1, 'admin'),
('Spa Massage', 'Spa', 300000.00, 1, 'admin'),
('Airport Transfer', 'Other', 200000.00, 1, 'admin');

-- Insert sample Staff
INSERT INTO Staff (Name, Role, Phone, UpdatedBy, UpdatedByUsername) VALUES
('Nguyễn Văn C', 'Receptionist', '0901111111', 1, 'admin'),
('Trần Thị D', 'Housekeeping', '0902222222', 1, 'admin'),
('Lê Văn E', 'Manager', '0903333333', 1, 'admin');

-- Insert sample active booking for Room 202
INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, Status, UpdatedBy, UpdatedByUsername) VALUES
(1, '123456789', 4, '2024-12-01 14:00:00', '2024-12-05 12:00:00', 'Active', 1, 'admin');

-- Additional Customer Management Procedures
DELIMITER //
CREATE PROCEDURE addCustomer(
    IN p_Name VARCHAR(100),
    IN p_IDCard VARCHAR(20),
    IN p_Phone VARCHAR(15),
    IN p_Email VARCHAR(100),
    IN p_Nationality VARCHAR(50),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername)
    VALUES (p_Name, p_IDCard, p_Phone, p_Email, p_Nationality, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE deleteCustomer(
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DELETE FROM Customers WHERE CustomerID = p_CustomerID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getAllCustomers()
BEGIN
    SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, CreatedAt, UpdatedAt
    FROM Customers;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE searchCustomers(
    IN p_Name VARCHAR(100),
    IN p_IDCard VARCHAR(20),
    IN p_Phone VARCHAR(15)
)
BEGIN
    SELECT CustomerID, Name, IDCard, Phone, Email, Nationality, CreatedAt, UpdatedAt
    FROM Customers
    WHERE (p_Name IS NULL OR Name LIKE CONCAT('%', p_Name, '%'))
    AND (p_IDCard IS NULL OR IDCard = p_IDCard)
    AND (p_Phone IS NULL OR Phone = p_Phone);
END //
DELIMITER ;

-- Service Management Procedures
DELIMITER //
CREATE PROCEDURE addService(
    IN p_ServiceName VARCHAR(50),
    IN p_Type ENUM('Food', 'Laundry', 'Spa', 'Other'),
    IN p_Price DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername)
    VALUES (p_ServiceName, p_Type, p_Price, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE updateService(
    IN p_ServiceID INT,
    IN p_ServiceName VARCHAR(50),
    IN p_Type ENUM('Food', 'Laundry', 'Spa', 'Other'),
    IN p_Price DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Services
    SET ServiceName = p_ServiceName,
        Type = p_Type,
        Price = p_Price,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE ServiceID = p_ServiceID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE deleteService(
    IN p_ServiceID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DELETE FROM Services WHERE ServiceID = p_ServiceID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getAllServices()
BEGIN
    SELECT ServiceID, ServiceName, Type, Price, CreatedAt, UpdatedAt
    FROM Services;
END //
DELIMITER ;

-- Staff Management Procedures
DELIMITER //
CREATE PROCEDURE addStaff(
    IN p_Name VARCHAR(100),
    IN p_Role ENUM('Receptionist', 'Housekeeping', 'Manager'),
    IN p_Phone VARCHAR(15),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Staff (Name, Role, Phone, UpdatedBy, UpdatedByUsername)
    VALUES (p_Name, p_Role, p_Phone, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE updateStaff(
    IN p_StaffID INT,
    IN p_Name VARCHAR(100),
    IN p_Role ENUM('Receptionist', 'Housekeeping', 'Manager'),
    IN p_Phone VARCHAR(15),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Staff
    SET Name = p_Name,
        Role = p_Role,
        Phone = p_Phone,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE StaffID = p_StaffID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE deleteStaff(
    IN p_StaffID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DELETE FROM Staff WHERE StaffID = p_StaffID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getAllStaff()
BEGIN
    SELECT StaffID, Name, Role, Phone, CreatedAt, UpdatedAt
    FROM Staff;
END //
DELIMITER ;

-- User Management Procedures
DELIMITER //
CREATE PROCEDURE addUser(
    IN p_Username VARCHAR(50),
    IN p_Password VARCHAR(255),
    IN p_RoleID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Users (Username, Password, RoleID, UpdatedBy, UpdatedByUsername)
    VALUES (p_Username, p_Password, p_RoleID, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE updateUser(
    IN p_UserID INT,
    IN p_Username VARCHAR(50),
    IN p_Password VARCHAR(255),
    IN p_RoleID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Users
    SET Username = p_Username,
        Password = p_Password,
        RoleID = p_RoleID,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE UserID = p_UserID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE deleteUser(
    IN p_UserID INT
)
BEGIN
    DELETE FROM Users WHERE UserID = p_UserID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getAllUsers()
BEGIN
    SELECT u.UserID, u.Username, r.RoleName, u.CreatedAt, u.UpdatedAt
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID;
END //
DELIMITER ;

-- Invoice Management Procedures
DELIMITER //
CREATE PROCEDURE createInvoice(
    IN p_BookingID INT,
    IN p_CustomerID INT,
    IN p_TotalAmount DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, UpdatedBy, UpdatedByUsername)
    VALUES (p_BookingID, p_CustomerID, p_TotalAmount, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE updateInvoicePaymentStatus(
    IN p_InvoiceID INT,
    IN p_PaymentStatus ENUM('Paid', 'Unpaid'),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Invoices
    SET PaymentStatus = p_PaymentStatus,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE InvoiceID = p_InvoiceID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getAllInvoices()
BEGIN
    SELECT i.InvoiceID, i.BookingID, c.Name as CustomerName, i.TotalAmount, 
           i.IssueDate, i.PaymentStatus, i.CreatedAt, i.UpdatedAt
    FROM Invoices i
    JOIN Customers c ON i.CustomerID = c.CustomerID;
END //
DELIMITER ;

-- Service Usage Procedures
DELIMITER //
CREATE PROCEDURE addServiceUsage(
    IN p_BookingID INT,
    IN p_ServiceID INT,
    IN p_CustomerID INT,
    IN p_Quantity INT,
    IN p_Date DATE,
    IN p_TotalPrice DECIMAL(10,2),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO ServiceUsage (BookingID, ServiceID, CustomerID, Quantity, Date, TotalPrice, UpdatedBy, UpdatedByUsername)
    VALUES (p_BookingID, p_ServiceID, p_CustomerID, p_Quantity, p_Date, p_TotalPrice, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getServiceUsageByBooking(
    IN p_BookingID INT
)
BEGIN
    SELECT su.UsageID, s.ServiceName, su.Quantity, s.Price as UnitPrice, 
           su.TotalPrice, su.Date, su.PaymentStatus
    FROM ServiceUsage su
    JOIN Services s ON su.ServiceID = s.ServiceID
    JOIN Bookings b ON su.BookingID = b.BookingID
    JOIN Invoices i ON i.BookingID = b.BookingID
    WHERE i.InvoiceID = p_InvoiceID;
END //
DELIMITER ;

-- Reports and Analytics Procedures
DELIMITER //
CREATE PROCEDURE getOccupancyReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    SELECT 
        DATE(b.CheckInDate) as Date,
        COUNT(DISTINCT b.RoomID) as OccupiedRooms,
        (SELECT COUNT(*) FROM Rooms) as TotalRooms,
        ROUND(COUNT(DISTINCT b.RoomID) * 100.0 / (SELECT COUNT(*) FROM Rooms), 2) as OccupancyRate
    FROM Bookings b
    WHERE b.Status = 'Active'
    AND DATE(b.CheckInDate) BETWEEN p_StartDate AND p_EndDate
    GROUP BY DATE(b.CheckInDate)
    ORDER BY Date;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE getRevenueReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    SELECT 
        DATE(i.IssueDate) as Date,
        SUM(i.TotalAmount) as TotalRevenue,
        COUNT(i.InvoiceID) as TotalInvoices
    FROM Invoices i
    WHERE i.PaymentStatus = 'Paid'
    AND DATE(i.IssueDate) BETWEEN p_StartDate AND p_EndDate
    GROUP BY DATE(i.IssueDate)
    ORDER BY Date;
END //
DELIMITER ;

-- =============================================
-- STORED PROCEDURES BỔ SUNG - HOÀN THIỆN CLONE.SQL
-- =============================================

-- 1. Room Management - Basic procedures
DELIMITER //
CREATE PROCEDURE addRoom(
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType VARCHAR(50),
    IN p_Price DECIMAL(10,2),
    IN p_Status VARCHAR(20),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    INSERT INTO Rooms (RoomNumber, RoomType, Price, Status, UpdatedBy, UpdatedByUsername)
    VALUES (p_RoomNumber, p_RoomType, p_Price, p_Status, p_UpdatedBy, p_UpdatedByUsername);
END //
DELIMITER ;

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
    
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Dọn phòng ID ', p_RoomID), p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE deleteRoom(
    IN p_RoomID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DELETE FROM Rooms WHERE RoomID = p_RoomID;
    
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xóa phòng ID ', p_RoomID), p_UpdatedByUsername);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE updateRoom(
    IN p_RoomID INT,
    IN p_RoomNumber VARCHAR(10),
    IN p_RoomType VARCHAR(50),
    IN p_Price DECIMAL(10,2),
    IN p_Status VARCHAR(20),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    UPDATE Rooms 
    SET RoomNumber = p_RoomNumber,
        RoomType = p_RoomType,
        Price = p_Price,
        Status = p_Status,
        UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE RoomID = p_RoomID;
    
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Cập nhật phòng ID ', p_RoomID), p_UpdatedByUsername);
END //
DELIMITER ;

-- 2. Customer Management - Advanced procedures
DELIMITER //
CREATE PROCEDURE deleteCustomerWithTransaction(
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_BookingCount INT DEFAULT 0;
    DECLARE v_ActiveBookingCount INT DEFAULT 0;
    DECLARE v_Error VARCHAR(255) DEFAULT '';
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Kiểm tra xem khách hàng có booking nào không
    SELECT COUNT(*) INTO v_BookingCount
    FROM Bookings
    WHERE CustomerID = p_CustomerID;
    
    -- Kiểm tra xem khách hàng có booking đang hoạt động không
    SELECT COUNT(*) INTO v_ActiveBookingCount
    FROM Bookings
    WHERE CustomerID = p_CustomerID 
    AND Status IN ('Active', 'Confirmed');
    
    -- Nếu có booking đang hoạt động, không cho phép xóa
    IF v_ActiveBookingCount > 0 THEN
        SET v_Error = CONCAT('Không thể xóa khách hàng có booking đang hoạt động. Số booking đang hoạt động: ', v_ActiveBookingCount);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_Error;
    END IF;
    
    -- Nếu có booking đã hoàn thành, cập nhật thành NULL thay vì xóa
    IF v_BookingCount > 0 THEN
        UPDATE Bookings 
        SET CustomerID = NULL,
            UpdatedBy = p_UpdatedBy,
            UpdatedByUsername = p_UpdatedByUsername,
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE CustomerID = p_CustomerID;
        
        -- Cập nhật Invoice
        UPDATE Invoices 
        SET CustomerID = NULL,
            UpdatedBy = p_UpdatedBy,
            UpdatedByUsername = p_UpdatedByUsername,
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE CustomerID = p_CustomerID;
    END IF;
    
    -- Xóa khách hàng
    DELETE FROM Customers WHERE CustomerID = p_CustomerID;
    
    -- Ghi log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Xóa khách hàng ID ', p_CustomerID, ' với ', v_BookingCount, ' booking(s)'), p_UpdatedByUsername);
    
    COMMIT;
END //
DELIMITER ;

-- 3. Invoice Management - Advanced procedures
DELIMITER //
CREATE PROCEDURE searchInvoices(
    IN p_CustomerName VARCHAR(100),
    IN p_StartDate DATE,
    IN p_EndDate DATE,
    IN p_PaymentStatus VARCHAR(20),
    IN p_MinAmount DECIMAL(10,2),
    IN p_MaxAmount DECIMAL(10,2)
)
BEGIN
    SELECT 
        i.InvoiceID, i.BookingID, i.CustomerID,
        c.Name as CustomerName, c.IDCard, c.PhoneNumber,
        i.TotalAmount, i.PaymentStatus, i.IssueDate,
        i.UpdatedBy, i.UpdatedByUsername, i.UpdatedAt
    FROM Invoices i
    LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
    WHERE (p_CustomerName IS NULL OR c.Name LIKE CONCAT('%', p_CustomerName, '%'))
    AND (p_StartDate IS NULL OR DATE(i.IssueDate) >= p_StartDate)
    AND (p_EndDate IS NULL OR DATE(i.IssueDate) <= p_EndDate)
    AND (p_PaymentStatus IS NULL OR i.PaymentStatus = p_PaymentStatus)
    AND (p_MinAmount IS NULL OR i.TotalAmount >= p_MinAmount)
    AND (p_MaxAmount IS NULL OR i.TotalAmount <= p_MaxAmount)
    ORDER BY i.IssueDate DESC;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE printInvoice(
    IN p_InvoiceID INT
)
BEGIN
    -- Thông tin hóa đơn chính
    SELECT 
        i.InvoiceID, i.BookingID, i.TotalAmount, i.PaymentStatus, i.IssueDate,
        c.Name as CustomerName, c.IDCard, c.PhoneNumber, c.Address, c.Nationality,
        r.RoomNumber, r.RoomType, r.Price as RoomPrice,
        b.CheckInDate, b.CheckOutDate, b.Status as BookingStatus,
        DATEDIFF(b.CheckOutDate, b.CheckInDate) as StayDays,
        (DATEDIFF(b.CheckOutDate, b.CheckInDate) * r.Price) as RoomCost
    FROM Invoices i
    LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
    LEFT JOIN Bookings b ON i.BookingID = b.BookingID
    LEFT JOIN Rooms r ON b.RoomID = r.RoomID
    WHERE i.InvoiceID = p_InvoiceID;
    
    -- Chi tiết dịch vụ sử dụng
    SELECT s.ServiceName, su.Quantity, s.Price as UnitPrice, 
           su.TotalPrice, su.Date, su.PaymentStatus
    FROM ServiceUsage su
    JOIN Services s ON su.ServiceID = s.ServiceID
    JOIN Bookings b ON su.BookingID = b.BookingID
    JOIN Invoices i ON i.BookingID = b.BookingID
    WHERE i.InvoiceID = p_InvoiceID;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE calculateInvoiceAmount(
    IN p_BookingID INT,
    OUT p_TotalAmount DECIMAL(10,2)
)
BEGIN
    DECLARE v_RoomCost DECIMAL(10,2) DEFAULT 0;
    DECLARE v_ServiceCost DECIMAL(10,2) DEFAULT 0;
    
    -- Calculate room cost
    SELECT (DATEDIFF(b.CheckOutDate, b.CheckInDate) * r.Price) INTO v_RoomCost
    FROM Bookings b
    JOIN Rooms r ON b.RoomID = r.RoomID
    WHERE b.BookingID = p_BookingID;
    
    -- Calculate service cost
    SELECT COALESCE(SUM(TotalPrice), 0) INTO v_ServiceCost
    FROM ServiceUsage
    WHERE BookingID = p_BookingID;
    
    SET p_TotalAmount = v_RoomCost + v_ServiceCost;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE createInvoiceWithCalculation(
    IN p_BookingID INT,
    IN p_CustomerID INT,
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50),
    OUT p_InvoiceID INT
)
BEGIN
    DECLARE v_TotalAmount DECIMAL(10,2);
    
    -- Calculate total amount
    CALL calculateInvoiceAmount(p_BookingID, v_TotalAmount);
    
    -- Create invoice
    INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, UpdatedBy, UpdatedByUsername)
    VALUES (p_BookingID, p_CustomerID, v_TotalAmount, p_UpdatedBy, p_UpdatedByUsername);
    
    SET p_InvoiceID = LAST_INSERT_ID();
END //
DELIMITER ;

-- 4. Staff Management - Advanced procedures
DELIMITER //
CREATE PROCEDURE assignStaffToTask(
    IN p_StaffID INT,
    IN p_TaskType VARCHAR(50), -- 'cleaning', 'maintenance', 'service'
    IN p_RoomID INT,
    IN p_BookingID INT,
    IN p_AssignedBy INT,
    IN p_AssignedByUsername VARCHAR(50)
)
BEGIN
    -- Create a simple task assignment log
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_AssignedBy, 
            CONCAT('Gán nhân viên ID ', p_StaffID, ' làm ', p_TaskType, 
                   CASE 
                       WHEN p_RoomID IS NOT NULL THEN CONCAT(' cho phòng ID ', p_RoomID)
                       WHEN p_BookingID IS NOT NULL THEN CONCAT(' cho booking ID ', p_BookingID)
                       ELSE ''
                   END), 
            p_AssignedByUsername);
            
    -- Update room status if it's a cleaning task
    IF p_TaskType = 'cleaning' AND p_RoomID IS NOT NULL THEN
        UPDATE Rooms 
        SET Status = 'Under Maintenance',
            UpdatedBy = p_AssignedBy,
            UpdatedByUsername = p_AssignedByUsername,
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE RoomID = p_RoomID;
    END IF;
END //
DELIMITER ;

-- 5. Reporting - Customer statistics
DELIMITER //
CREATE PROCEDURE getCustomerReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    -- Customer statistics by nationality
    SELECT 
        c.Nationality,
        COUNT(DISTINCT c.CustomerID) as TotalCustomers,
        COUNT(b.BookingID) as TotalBookings,
        SUM(CASE WHEN b.Status = 'Completed' THEN 1 ELSE 0 END) as CompletedBookings,
        AVG(DATEDIFF(b.CheckOutDate, b.CheckInDate)) as AvgStayDays
    FROM Customers c
    LEFT JOIN Bookings b ON c.CustomerID = b.CustomerID
    WHERE (p_StartDate IS NULL OR DATE(b.CheckInDate) >= p_StartDate)
    AND (p_EndDate IS NULL OR DATE(b.CheckInDate) <= p_EndDate)
    GROUP BY c.Nationality
    ORDER BY TotalBookings DESC;

    -- Returning customers (customers with more than 1 booking)
    SELECT 
        c.CustomerID, c.Name, c.IDCard, c.Nationality,
        COUNT(b.BookingID) as BookingCount,
        MIN(b.CheckInDate) as FirstVisit,
        MAX(b.CheckInDate) as LastVisit
    FROM Customers c
    JOIN Bookings b ON c.CustomerID = b.CustomerID
    WHERE (p_StartDate IS NULL OR DATE(b.CheckInDate) >= p_StartDate)
    AND (p_EndDate IS NULL OR DATE(b.CheckInDate) <= p_EndDate)
    GROUP BY c.CustomerID, c.Name, c.IDCard, c.Nationality
    HAVING COUNT(b.BookingID) > 1
    ORDER BY BookingCount DESC;
END //
DELIMITER ;

-- 6. Reporting - Service usage statistics
DELIMITER //
CREATE PROCEDURE getServiceReport(
    IN p_StartDate DATE,
    IN p_EndDate DATE
)
BEGIN
    -- Most used services
    SELECT 
        s.ServiceID, s.ServiceName, s.Type,
        COUNT(su.UsageID) as UsageCount,
        SUM(su.Quantity) as TotalQuantity,
        SUM(su.TotalPrice) as TotalRevenue,
        AVG(su.TotalPrice) as AvgRevenuePerUsage
    FROM Services s
    LEFT JOIN ServiceUsage su ON s.ServiceID = su.ServiceID
    WHERE (p_StartDate IS NULL OR su.Date >= p_StartDate)
    AND (p_EndDate IS NULL OR su.Date <= p_EndDate)
    GROUP BY s.ServiceID, s.ServiceName, s.Type
    ORDER BY TotalRevenue DESC;

    -- Service revenue by type
    SELECT 
        s.Type,
        COUNT(su.UsageID) as UsageCount,
        SUM(su.TotalPrice) as TotalRevenue,
        AVG(su.TotalPrice) as AvgRevenue
    FROM Services s
    LEFT JOIN ServiceUsage su ON s.ServiceID = su.ServiceID
    WHERE (p_StartDate IS NULL OR su.Date >= p_StartDate)
    AND (p_EndDate IS NULL OR su.Date <= p_EndDate)
    GROUP BY s.Type
    ORDER BY TotalRevenue DESC;
END //
DELIMITER ;

-- 7. Dashboard and Summary
DELIMITER //
CREATE PROCEDURE getDashboardSummary()
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM Rooms) as TotalRooms,
        (SELECT COUNT(*) FROM Rooms WHERE Status = 'Available') as AvailableRooms,
        (SELECT COUNT(*) FROM Rooms WHERE Status = 'Occupied') as OccupiedRooms,
        (SELECT COUNT(*) FROM Rooms WHERE Status = 'Under Maintenance') as MaintenanceRooms,
        (SELECT COUNT(*) FROM Bookings WHERE Status = 'Active') as ActiveBookings,
        (SELECT COUNT(*) FROM Bookings WHERE DATE(CheckInDate) = CURDATE()) as TodayCheckIns,
        (SELECT COUNT(*) FROM Bookings WHERE DATE(CheckOutDate) = CURDATE()) as TodayCheckOuts,
        (SELECT COUNT(*) FROM Customers) as TotalCustomers,
        (SELECT COALESCE(SUM(TotalAmount), 0) FROM Invoices WHERE PaymentStatus = 'Paid' AND DATE(IssueDate) = CURDATE()) as TodayRevenue,
        (SELECT COALESCE(SUM(TotalAmount), 0) FROM Invoices WHERE PaymentStatus = 'Unpaid') as PendingPayments;
END //
DELIMITER ;

-- =============================================
-- KẾT THÚC BỔ SUNG STORED PROCEDURES
-- =============================================

-- Check-in with ID Card verification (Enhanced Security)
DELIMITER //
CREATE PROCEDURE checkInWithIDVerification(
    IN p_BookingID INT,
    IN p_IDCard VARCHAR(20),
    IN p_UpdatedBy INT,
    IN p_UpdatedByUsername VARCHAR(50)
)
BEGIN
    DECLARE v_RoomID INT;
    DECLARE v_BookingIDCard VARCHAR(20);
    DECLARE v_rowsAffected INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lỗi khi check-in với xác thực căn cước';
    END;

    START TRANSACTION;

    -- Kiểm tra booking tồn tại và trạng thái Active
    IF NOT EXISTS (
        SELECT 1 FROM Bookings 
        WHERE BookingID = p_BookingID 
        AND Status = 'Active'
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Đặt phòng không tồn tại hoặc không hợp lệ để check-in';
    END IF;

    -- Lấy thông tin booking để xác thực căn cước
    SELECT RoomID, IDCard INTO v_RoomID, v_BookingIDCard 
    FROM Bookings 
    WHERE BookingID = p_BookingID;

    -- Xác thực căn cước công dân
    IF v_BookingIDCard != p_IDCard THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Căn cước công dân không khớp với thông tin đặt phòng';
    END IF;

    -- Cập nhật trạng thái booking
    UPDATE Bookings
    SET UpdatedBy = p_UpdatedBy,
        UpdatedByUsername = p_UpdatedByUsername,
        UpdatedAt = CURRENT_TIMESTAMP
    WHERE BookingID = p_BookingID;
    SET v_rowsAffected = ROW_COUNT();
    IF v_rowsAffected = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Check-in thất bại: Không tìm thấy hoặc không cập nhật được booking';
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

    -- Log action với thông tin căn cước
    INSERT INTO Logs (UserID, Action, UpdatedByUsername)
    VALUES (p_UpdatedBy, CONCAT('Check-in đặt phòng ID ', p_BookingID, ' với xác thực căn cước ', p_IDCard), p_UpdatedByUsername);

    COMMIT;
END //
DELIMITER ;