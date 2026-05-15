-- ============================================================
--  STARRY NIGHTS MOTEL — SEED DATA
--  Run this after 01_schema.sql
-- ============================================================

USE MotelBookingDb;
GO

-- ─── ROOM TYPES ──────────────────────────────────────────────
INSERT INTO RoomTypes (Name, Description) VALUES
('Standard',  'Comfortable standard rooms with all essential amenities for a relaxing stay.'),
('Deluxe',    'Spacious rooms with premium furnishings, enhanced views, and upgraded amenities.'),
('Suite',     'Luxurious suites with separate living areas, premium bedding, and panoramic views.'),
('Executive', 'Business-class rooms with work desk, high-speed internet, and premium services.'),
('Family',    'Spacious multi-bedroom rooms perfect for families with children.');

-- ─── AMENITIES ───────────────────────────────────────────────
INSERT INTO Amenities (Name, Icon, Category) VALUES
('Free WiFi',        'wifi',           'Technology'),
('Smart TV',         'tv',             'Technology'),
('Air Conditioning', 'ac_unit',        'Comfort'),
('Mini Bar',         'local_bar',      'Food & Drink'),
('Coffee Maker',     'coffee',         'Food & Drink'),
('Private Balcony',  'balcony',        'Views'),
('City View',        'location_city',  'Views'),
('King Bed',         'king_bed',       'Sleeping'),
('Queen Bed',        'bed',            'Sleeping'),
('Jacuzzi',          'hot_tub',        'Bathroom'),
('Work Desk',        'desk',           'Business'),
('In-Room Safe',     'lock',           'Security'),
('Valet Parking',    'local_parking',  'Transport'),
('Room Service',     'room_service',   'Services'),
('Iron & Board',     'iron',           'Comfort'),
('Hair Dryer',       'dry',            'Bathroom');

-- ─── ADMIN USER ──────────────────────────────────────────────
-- Password: Admin@123! (BCrypt hash — change in production)
INSERT INTO Users (FullName, Email, PasswordHash, [Role], IsEmailVerified)
VALUES (
    'System Administrator',
    'admin@motelbooking.com',
    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQyCgK3lUGnbDKQd3U7YQ8yHi',
    'Admin',
    1
);

-- ─── ROOMS ───────────────────────────────────────────────────
DECLARE @StandardId INT = (SELECT Id FROM RoomTypes WHERE Name = 'Standard');
DECLARE @DeluxeId   INT = (SELECT Id FROM RoomTypes WHERE Name = 'Deluxe');
DECLARE @SuiteId    INT = (SELECT Id FROM RoomTypes WHERE Name = 'Suite');
DECLARE @ExecId     INT = (SELECT Id FROM RoomTypes WHERE Name = 'Executive');
DECLARE @FamilyId   INT = (SELECT Id FROM RoomTypes WHERE Name = 'Family');

INSERT INTO Rooms (Name, Description, PricePerNight, MaxOccupancy, FloorNumber, RoomNumber, SizeInSqFt, RoomTypeId, IsFeatured, AverageRating, TotalReviews,
    ThumbnailUrl, [Status])
VALUES
('Classic Standard Room',
 'A cozy, well-appointed room perfect for solo travelers or couples seeking comfortable accommodation at an unbeatable value.',
 79.00, 2, 1, '101', 280, @StandardId, 0, 4.2, 48,
 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800', 0),

('Premium Standard Room',
 'An elevated standard experience with upgraded linens, blackout curtains, and a refreshed bathroom suite.',
 95.00, 2, 1, '102', 300, @StandardId, 0, 4.3, 31,
 'https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=800', 0),

('Deluxe King Room',
 'Indulge in elevated comfort with stunning city views, premium king-sized bedding, and a range of luxury amenities.',
 129.00, 2, 2, '201', 420, @DeluxeId, 1, 4.7, 134,
 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800', 0),

('Deluxe Twin Room',
 'Two comfortable queen beds with all the amenities of our Deluxe collection — ideal for friends or colleagues.',
 119.00, 3, 2, '202', 380, @DeluxeId, 0, 4.5, 62,
 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800', 0),

('Executive Suite',
 'Our flagship Executive Suite blends business functionality with luxury comfort. Features a separate living area, private balcony, jacuzzi, and panoramic city views.',
 229.00, 3, 3, '301', 680, @SuiteId, 1, 4.9, 89,
 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800', 0),

('Business Executive Room',
 'Designed for the discerning business traveler. High-speed internet, ergonomic work desk, and premium amenities for maximum productivity.',
 159.00, 2, 4, '401', 480, @ExecId, 1, 4.6, 72,
 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800', 0),

('Family Suite',
 'Spacious and welcoming, our Family Suite features two bedrooms, a shared living space, and everything your family needs for a memorable stay.',
 189.00, 5, 5, '501', 720, @FamilyId, 0, 4.5, 56,
 'https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=800', 0),

('Luxury Penthouse Suite',
 'The pinnacle of luxury. Our Penthouse Suite offers breathtaking 360° views, a private rooftop terrace, full butler service, and world-class interior design.',
 499.00, 4, 6, '601', 1200, @SuiteId, 1, 5.0, 23,
 'https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800', 0);

-- ─── ROOM IMAGES ─────────────────────────────────────────────
INSERT INTO RoomImages (RoomId, ImageUrl, IsPrimary, SortOrder)
SELECT r.Id,
       'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=1200', 1, 0
FROM Rooms r WHERE r.RoomNumber = '101';

INSERT INTO RoomImages (RoomId, ImageUrl, IsPrimary, SortOrder)
SELECT r.Id,
       'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=1200', 1, 0
FROM Rooms r WHERE r.RoomNumber = '201';

INSERT INTO RoomImages (RoomId, ImageUrl, IsPrimary, SortOrder)
SELECT r.Id, 'https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=1200', 0, 1
FROM Rooms r WHERE r.RoomNumber = '201';

INSERT INTO RoomImages (RoomId, ImageUrl, IsPrimary, SortOrder)
SELECT r.Id,
       'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=1200', 1, 0
FROM Rooms r WHERE r.RoomNumber = '301';

INSERT INTO RoomImages (RoomId, ImageUrl, IsPrimary, SortOrder)
SELECT r.Id,
       'https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=1200', 1, 0
FROM Rooms r WHERE r.RoomNumber = '601';

-- ─── ROOM AMENITIES ──────────────────────────────────────────
DECLARE @WifiId     INT = (SELECT Id FROM Amenities WHERE Name = 'Free WiFi');
DECLARE @TvId       INT = (SELECT Id FROM Amenities WHERE Name = 'Smart TV');
DECLARE @AcId       INT = (SELECT Id FROM Amenities WHERE Name = 'Air Conditioning');
DECLARE @MiniBarId  INT = (SELECT Id FROM Amenities WHERE Name = 'Mini Bar');
DECLARE @CoffeeId   INT = (SELECT Id FROM Amenities WHERE Name = 'Coffee Maker');
DECLARE @BalconyId  INT = (SELECT Id FROM Amenities WHERE Name = 'Private Balcony');
DECLARE @CityId     INT = (SELECT Id FROM Amenities WHERE Name = 'City View');
DECLARE @KingId     INT = (SELECT Id FROM Amenities WHERE Name = 'King Bed');
DECLARE @QueenId    INT = (SELECT Id FROM Amenities WHERE Name = 'Queen Bed');
DECLARE @JacuzziId  INT = (SELECT Id FROM Amenities WHERE Name = 'Jacuzzi');
DECLARE @DeskId     INT = (SELECT Id FROM Amenities WHERE Name = 'Work Desk');
DECLARE @RsvcId     INT = (SELECT Id FROM Amenities WHERE Name = 'Room Service');

-- Standard Room (101) amenities
INSERT INTO RoomAmenities (RoomId, AmenityId)
SELECT r.Id, a.Id FROM Rooms r CROSS JOIN Amenities a
WHERE r.RoomNumber = '101' AND a.Id IN (@WifiId, @TvId, @AcId, @QueenId, @CoffeeId);

-- Deluxe King Room (201) amenities
INSERT INTO RoomAmenities (RoomId, AmenityId)
SELECT r.Id, a.Id FROM Rooms r CROSS JOIN Amenities a
WHERE r.RoomNumber = '201' AND a.Id IN (@WifiId, @TvId, @AcId, @KingId, @CityId, @MiniBarId, @CoffeeId, @RsvcId);

-- Executive Suite (301) amenities
INSERT INTO RoomAmenities (RoomId, AmenityId)
SELECT r.Id, a.Id FROM Rooms r CROSS JOIN Amenities a
WHERE r.RoomNumber = '301' AND a.Id IN (@WifiId, @TvId, @AcId, @KingId, @BalconyId, @CityId, @MiniBarId, @JacuzziId, @DeskId, @RsvcId);

-- Business Executive (401) amenities
INSERT INTO RoomAmenities (RoomId, AmenityId)
SELECT r.Id, a.Id FROM Rooms r CROSS JOIN Amenities a
WHERE r.RoomNumber = '401' AND a.Id IN (@WifiId, @TvId, @AcId, @KingId, @DeskId, @CoffeeId, @RsvcId);

-- Penthouse (601) amenities
INSERT INTO RoomAmenities (RoomId, AmenityId)
SELECT r.Id, a.Id FROM Rooms r CROSS JOIN Amenities a
WHERE r.RoomNumber = '601' AND a.Id IN (@WifiId, @TvId, @AcId, @KingId, @BalconyId, @CityId, @MiniBarId, @JacuzziId, @DeskId, @CoffeeId, @RsvcId);

-- ─── DEMO USER ───────────────────────────────────────────────
-- Password: Demo@123!
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, [Role], IsEmailVerified)
VALUES (
    'Alex Johnson',
    'demo@motelbooking.com',
    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQyCgK3lUGnbDKQd3U7YQ8yHi',
    '+1 (555) 234-5678',
    'Customer',
    1
);

PRINT 'Seed data inserted successfully.';
GO

-- ─── USEFUL VIEWS ────────────────────────────────────────────
CREATE OR ALTER VIEW vw_RoomDetails AS
SELECT
    r.Id, r.Name, r.Description, r.PricePerNight,
    r.MaxOccupancy, r.FloorNumber, r.RoomNumber,
    r.SizeInSqFt, r.[Status], r.ThumbnailUrl,
    r.AverageRating, r.TotalReviews, r.IsFeatured,
    rt.Name AS RoomTypeName,
    (SELECT COUNT(*) FROM Bookings b
     WHERE b.RoomId = r.Id AND b.[Status] NOT IN (4) -- Not cancelled
       AND b.CheckInDate <= SYSUTCDATETIME()
       AND b.CheckOutDate >= SYSUTCDATETIME()) AS ActiveBookings
FROM Rooms r
JOIN RoomTypes rt ON r.RoomTypeId = rt.Id
WHERE r.IsDeleted = 0;
GO

CREATE OR ALTER VIEW vw_BookingSummary AS
SELECT
    b.Id, b.ConfirmationNumber,
    u.FullName AS GuestName, u.Email AS GuestEmail,
    r.Name AS RoomName, r.RoomNumber,
    b.CheckInDate, b.CheckOutDate,
    DATEDIFF(day, b.CheckInDate, b.CheckOutDate) AS NightsCount,
    b.NumberOfGuests, b.TotalAmount, b.TaxAmount,
    b.[Status] AS BookingStatus,
    p.[Status] AS PaymentStatus,
    b.CreatedAt
FROM Bookings b
JOIN Users u ON b.UserId = u.Id
JOIN Rooms r ON b.RoomId = r.Id
LEFT JOIN Payments p ON p.BookingId = b.Id
WHERE b.IsDeleted = 0;
GO

CREATE OR ALTER VIEW vw_RevenueByMonth AS
SELECT
    YEAR(CreatedAt)  AS RevenueYear,
    MONTH(CreatedAt) AS RevenueMonth,
    DATENAME(MONTH, CreatedAt) AS MonthName,
    COUNT(*)         AS BookingCount,
    SUM(TotalAmount) AS TotalRevenue,
    AVG(TotalAmount) AS AvgBookingValue
FROM Bookings
WHERE IsDeleted = 0 AND [Status] <> 4  -- Exclude cancelled
GROUP BY YEAR(CreatedAt), MONTH(CreatedAt), DATENAME(MONTH, CreatedAt);
GO

PRINT 'Views created successfully.';
GO
