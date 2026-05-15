-- ============================================================
--  STARRY NIGHTS MOTEL BOOKING SYSTEM
--  MSSQL Database Schema
--  Version: 2.0 | Enterprise Edition
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MotelBookingDb')
BEGIN
    CREATE DATABASE MotelBookingDb
    COLLATE SQL_Latin1_General_CP1_CI_AS;
END
GO

USE MotelBookingDb;
GO

-- ─── ROOM TYPES ──────────────────────────────────────────────
CREATE TABLE RoomTypes (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    IconUrl     NVARCHAR(500) NULL,
    CreatedAt   DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2     NULL,
    IsDeleted   BIT           NOT NULL DEFAULT 0
);

-- ─── AMENITIES ───────────────────────────────────────────────
CREATE TABLE Amenities (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Name      NVARCHAR(100) NOT NULL,
    Icon      NVARCHAR(100) NULL,
    Category  NVARCHAR(100) NULL,
    CreatedAt DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2     NULL,
    IsDeleted BIT           NOT NULL DEFAULT 0
);

-- ─── USERS ───────────────────────────────────────────────────
CREATE TABLE Users (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    FullName         NVARCHAR(100)  NOT NULL,
    Email            NVARCHAR(150)  NOT NULL,
    PasswordHash     NVARCHAR(500)  NOT NULL,
    PhoneNumber      NVARCHAR(20)   NULL,
    Address          NVARCHAR(250)  NULL,
    City             NVARCHAR(100)  NULL,
    [State]          NVARCHAR(100)  NULL,
    ZipCode          NVARCHAR(20)   NULL,
    ProfileImageUrl  NVARCHAR(500)  NULL,
    IsEmailVerified  BIT            NOT NULL DEFAULT 0,
    RefreshToken     NVARCHAR(500)  NULL,
    RefreshTokenExpiry DATETIME2    NULL,
    [Role]           NVARCHAR(50)   NOT NULL DEFAULT 'Customer',
    CreatedAt        DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt        DATETIME2      NULL,
    IsDeleted        BIT            NOT NULL DEFAULT 0,

    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);

CREATE INDEX IX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
CREATE INDEX IX_Users_Role  ON Users([Role]) WHERE IsDeleted = 0;

-- ─── ROOMS ───────────────────────────────────────────────────
CREATE TABLE Rooms (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(150)    NOT NULL,
    Description   NVARCHAR(2000)   NOT NULL,
    PricePerNight DECIMAL(10, 2)   NOT NULL,
    MaxOccupancy  INT              NOT NULL DEFAULT 2,
    FloorNumber   INT              NOT NULL DEFAULT 1,
    RoomNumber    NVARCHAR(20)     NOT NULL,
    SizeInSqFt    FLOAT            NOT NULL DEFAULT 0,
    [Status]      INT              NOT NULL DEFAULT 0,  -- RoomStatus enum
    ThumbnailUrl  NVARCHAR(500)    NULL,
    AverageRating FLOAT            NOT NULL DEFAULT 0,
    TotalReviews  INT              NOT NULL DEFAULT 0,
    IsFeatured    BIT              NOT NULL DEFAULT 0,
    RoomTypeId    INT              NOT NULL,
    CreatedAt     DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt     DATETIME2        NULL,
    IsDeleted     BIT              NOT NULL DEFAULT 0,

    CONSTRAINT FK_Rooms_RoomTypes FOREIGN KEY (RoomTypeId)
        REFERENCES RoomTypes(Id),
    CONSTRAINT UQ_Rooms_RoomNumber UNIQUE (RoomNumber),
    CONSTRAINT CK_Rooms_Price CHECK (PricePerNight > 0),
    CONSTRAINT CK_Rooms_MaxOcc CHECK (MaxOccupancy BETWEEN 1 AND 50)
);

CREATE INDEX IX_Rooms_Status    ON Rooms([Status]) WHERE IsDeleted = 0;
CREATE INDEX IX_Rooms_Featured  ON Rooms(IsFeatured) WHERE IsDeleted = 0;
CREATE INDEX IX_Rooms_Price     ON Rooms(PricePerNight) WHERE IsDeleted = 0;
CREATE INDEX IX_Rooms_RoomType  ON Rooms(RoomTypeId) WHERE IsDeleted = 0;

-- ─── ROOM IMAGES ─────────────────────────────────────────────
CREATE TABLE RoomImages (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    RoomId    INT           NOT NULL,
    ImageUrl  NVARCHAR(500) NOT NULL,
    AltText   NVARCHAR(200) NULL,
    IsPrimary BIT           NOT NULL DEFAULT 0,
    SortOrder INT           NOT NULL DEFAULT 0,
    CreatedAt DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2     NULL,
    IsDeleted BIT           NOT NULL DEFAULT 0,

    CONSTRAINT FK_RoomImages_Rooms FOREIGN KEY (RoomId)
        REFERENCES Rooms(Id) ON DELETE CASCADE
);

CREATE INDEX IX_RoomImages_Room ON RoomImages(RoomId) WHERE IsDeleted = 0;

-- ─── ROOM AMENITIES ──────────────────────────────────────────
CREATE TABLE RoomAmenities (
    RoomId    INT NOT NULL,
    AmenityId INT NOT NULL,

    CONSTRAINT PK_RoomAmenities PRIMARY KEY (RoomId, AmenityId),
    CONSTRAINT FK_RoomAmenities_Rooms     FOREIGN KEY (RoomId)    REFERENCES Rooms(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoomAmenities_Amenities FOREIGN KEY (AmenityId) REFERENCES Amenities(Id) ON DELETE CASCADE
);

-- ─── BOOKINGS ────────────────────────────────────────────────
CREATE TABLE Bookings (
    Id                 INT IDENTITY(1,1) PRIMARY KEY,
    UserId             INT            NOT NULL,
    RoomId             INT            NOT NULL,
    CheckInDate        DATETIME2      NOT NULL,
    CheckOutDate       DATETIME2      NOT NULL,
    NumberOfGuests     INT            NOT NULL DEFAULT 1,
    TotalAmount        DECIMAL(10, 2) NOT NULL,
    TaxAmount          DECIMAL(10, 2) NOT NULL DEFAULT 0,
    DiscountAmount     DECIMAL(10, 2) NOT NULL DEFAULT 0,
    [Status]           INT            NOT NULL DEFAULT 0,  -- BookingStatus enum
    SpecialRequests    NVARCHAR(1000) NULL,
    ConfirmationNumber NVARCHAR(30)   NULL,
    CreatedAt          DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt          DATETIME2      NULL,
    IsDeleted          BIT            NOT NULL DEFAULT 0,

    CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id),
    CONSTRAINT FK_Bookings_Rooms FOREIGN KEY (RoomId)
        REFERENCES Rooms(Id),
    CONSTRAINT CK_Bookings_Dates CHECK (CheckOutDate > CheckInDate),
    CONSTRAINT CK_Bookings_Guests CHECK (NumberOfGuests >= 1),
    CONSTRAINT UQ_Bookings_ConfNum UNIQUE (ConfirmationNumber)
);

CREATE INDEX IX_Bookings_User       ON Bookings(UserId) WHERE IsDeleted = 0;
CREATE INDEX IX_Bookings_Room       ON Bookings(RoomId) WHERE IsDeleted = 0;
CREATE INDEX IX_Bookings_Status     ON Bookings([Status]) WHERE IsDeleted = 0;
CREATE INDEX IX_Bookings_CheckIn    ON Bookings(CheckInDate) WHERE IsDeleted = 0;
CREATE INDEX IX_Bookings_DateRange  ON Bookings(CheckInDate, CheckOutDate) WHERE IsDeleted = 0;

-- ─── PAYMENTS ────────────────────────────────────────────────
CREATE TABLE Payments (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    BookingId     INT            NOT NULL,
    Amount        DECIMAL(10, 2) NOT NULL,
    Currency      NVARCHAR(10)   NOT NULL DEFAULT 'USD',
    [Status]      INT            NOT NULL DEFAULT 0,  -- PaymentStatus enum
    TransactionId NVARCHAR(100)  NULL,
    PaymentMethod NVARCHAR(50)   NULL,
    PaidAt        DATETIME2      NULL,
    FailureReason NVARCHAR(500)  NULL,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt     DATETIME2      NULL,
    IsDeleted     BIT            NOT NULL DEFAULT 0,

    CONSTRAINT FK_Payments_Bookings FOREIGN KEY (BookingId)
        REFERENCES Bookings(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Payments_Booking UNIQUE (BookingId)
);

-- ─── REVIEWS ─────────────────────────────────────────────────
CREATE TABLE Reviews (
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    UserId            INT           NOT NULL,
    RoomId            INT           NOT NULL,
    Rating            INT           NOT NULL,
    Title             NVARCHAR(200) NOT NULL,
    Comment           NVARCHAR(2000) NOT NULL,
    IsVerified        BIT           NOT NULL DEFAULT 0,
    IsApproved        BIT           NOT NULL DEFAULT 0,
    CleanlinessRating INT           NOT NULL DEFAULT 0,
    ServiceRating     INT           NOT NULL DEFAULT 0,
    ValueRating       INT           NOT NULL DEFAULT 0,
    LocationRating    INT           NOT NULL DEFAULT 0,
    CreatedAt         DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt         DATETIME2     NULL,
    IsDeleted         BIT           NOT NULL DEFAULT 0,

    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id),
    CONSTRAINT FK_Reviews_Rooms FOREIGN KEY (RoomId)
        REFERENCES Rooms(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5)
);

CREATE INDEX IX_Reviews_Room ON Reviews(RoomId) WHERE IsDeleted = 0;
CREATE INDEX IX_Reviews_User ON Reviews(UserId) WHERE IsDeleted = 0;

-- ─── NOTIFICATIONS ───────────────────────────────────────────
CREATE TABLE Notifications (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    UserId    INT            NOT NULL,
    Title     NVARCHAR(200)  NOT NULL,
    Message   NVARCHAR(1000) NOT NULL,
    [Type]    NVARCHAR(50)   NOT NULL DEFAULT 'Info',
    IsRead    BIT            NOT NULL DEFAULT 0,
    ActionUrl NVARCHAR(500)  NULL,
    CreatedAt DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2      NULL,
    IsDeleted BIT            NOT NULL DEFAULT 0,

    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Notifications_User   ON Notifications(UserId) WHERE IsDeleted = 0;
CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead) WHERE IsDeleted = 0;

PRINT 'Schema created successfully.';
GO
