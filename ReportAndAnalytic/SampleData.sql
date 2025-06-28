-- Demo data for Report and Analytics Testing
-- Run this after main database creation

USE Hotel_management;

-- Insert additional sample data for better reporting

-- More Customers with different nationalities
INSERT INTO Customers (Name, IDCard, Phone, Email, Nationality, UpdatedBy, UpdatedByUsername) VALUES
('Michael Johnson', '444555666', '0123456789', 'michael@email.com', 'USA', 1, 'admin'),
('Zhang Wei', '777888999', '0234567890', 'zhang@email.com', 'China', 1, 'admin'),
('Emma Wilson', '111000222', '0345678901', 'emma@email.com', 'UK', 1, 'admin'),
('Pierre Dubois', '333444555', '0456789012', 'pierre@email.com', 'France', 1, 'admin'),
('Yuki Tanaka', '666777888', '0567890123', 'yuki@email.com', 'Japan', 1, 'admin');

-- More Bookings for reporting
INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, Status, UpdatedBy, UpdatedByUsername) VALUES
(2, '987654321', 1, '2024-12-10 14:00:00', '2024-12-15 12:00:00', 'Completed', 1, 'admin'),
(3, '111222333', 2, '2024-12-12 14:00:00', '2024-12-18 12:00:00', 'Active', 1, 'admin'),
(4, '444555666', 3, '2024-12-08 14:00:00', '2024-12-12 12:00:00', 'Completed', 1, 'admin'),
(5, '777888999', 5, '2024-12-15 14:00:00', '2024-12-20 12:00:00', 'Active', 1, 'admin'),
(6, '111000222', 1, '2024-11-20 14:00:00', '2024-11-25 12:00:00', 'Completed', 1, 'admin'),
(7, '333444555', 2, '2024-11-15 14:00:00', '2024-11-20 12:00:00', 'Completed', 1, 'admin'),
(8, '666777888', 3, '2024-11-10 14:00:00', '2024-11-15 12:00:00', 'Completed', 1, 'admin');

-- More Invoices
INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, IssueDate, PaymentStatus, UpdatedBy, UpdatedByUsername) VALUES
(2, 2, 2500000.00, '2024-12-10 14:00:00', 'Paid', 1, 'admin'),
(3, 3, 4800000.00, '2024-12-12 14:00:00', 'Paid', 1, 'admin'),
(4, 4, 6000000.00, '2024-12-08 14:00:00', 'Paid', 1, 'admin'),
(5, 5, 7500000.00, '2024-12-15 14:00:00', 'Unpaid', 1, 'admin'),
(6, 6, 2500000.00, '2024-11-20 14:00:00', 'Paid', 1, 'admin'),
(7, 7, 4000000.00, '2024-11-15 14:00:00', 'Paid', 1, 'admin'),
(8, 8, 7500000.00, '2024-11-10 14:00:00', 'Paid', 1, 'admin');

-- More Services
INSERT INTO Services (ServiceName, Type, Price, UpdatedBy, UpdatedByUsername) VALUES
('Dinner Service', 'Food', 200000.00, 1, 'admin'),
('Dry Cleaning', 'Laundry', 80000.00, 1, 'admin'),
('Taxi Service', 'Other', 150000.00, 1, 'admin'),
('Gym Access', 'Other', 50000.00, 1, 'admin'),
('Conference Room', 'Other', 500000.00, 1, 'admin');

-- Service Usage
INSERT INTO ServiceUsage (BookingID, ServiceID, CustomerID, Quantity, Date, TotalPrice, PaymentStatus, UpdatedBy, UpdatedByUsername) VALUES
(1, 1, 1, 2, '2024-12-02', 300000.00, 'Paid', 1, 'admin'),
(1, 2, 1, 1, '2024-12-03', 100000.00, 'Paid', 1, 'admin'),
(2, 3, 2, 1, '2024-12-11', 300000.00, 'Paid', 1, 'admin'),
(3, 1, 3, 3, '2024-12-13', 450000.00, 'Paid', 1, 'admin'),
(4, 4, 4, 1, '2024-12-09', 200000.00, 'Paid', 1, 'admin'),
(5, 2, 5, 2, '2024-12-16', 200000.00, 'Unpaid', 1, 'admin'),
(6, 5, 6, 1, '2024-11-21', 150000.00, 'Paid', 1, 'admin'),
(7, 6, 7, 1, '2024-11-16', 50000.00, 'Paid', 1, 'admin'),
(8, 7, 8, 1, '2024-11-11', 500000.00, 'Paid', 1, 'admin');

-- Additional bookings for trend analysis (previous months)
INSERT INTO Bookings (CustomerID, IDCard, RoomID, CheckInDate, CheckOutDate, Status, UpdatedBy, UpdatedByUsername) VALUES
(1, '123456789', 1, '2024-10-15 14:00:00', '2024-10-20 12:00:00', 'Completed', 1, 'admin'),
(2, '987654321', 2, '2024-10-10 14:00:00', '2024-10-15 12:00:00', 'Completed', 1, 'admin'),
(3, '111222333', 3, '2024-09-20 14:00:00', '2024-09-25 12:00:00', 'Completed', 1, 'admin'),
(4, '444555666', 4, '2024-09-15 14:00:00', '2024-09-20 12:00:00', 'Completed', 1, 'admin');

-- Corresponding invoices for trend analysis
INSERT INTO Invoices (BookingID, CustomerID, TotalAmount, IssueDate, PaymentStatus, UpdatedBy, UpdatedByUsername) VALUES
(9, 1, 2500000.00, '2024-10-15 14:00:00', 'Paid', 1, 'admin'),
(10, 2, 4000000.00, '2024-10-10 14:00:00', 'Paid', 1, 'admin'),
(11, 3, 7500000.00, '2024-09-20 14:00:00', 'Paid', 1, 'admin'),
(12, 4, 6000000.00, '2024-09-15 14:00:00', 'Paid', 1, 'admin');

-- Update some room statuses for better status reporting
UPDATE Rooms SET Status = 'Under Maintenance' WHERE RoomID = 6;
UPDATE Rooms SET Status = 'Uncleaned' WHERE RoomID IN (3, 4) AND Status = 'Available';

COMMIT;
