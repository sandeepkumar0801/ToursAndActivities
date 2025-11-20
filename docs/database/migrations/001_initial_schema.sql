-- =============================================
-- Initial Database Schema for Tours and Activities API
-- SQL Server 2019+
-- =============================================

USE [ToursActivitiesDB]
GO

-- =============================================
-- Create Activities Table
-- =============================================
CREATE TABLE [dbo].[Activities] (
    [ActivityID] INT IDENTITY(1,1) PRIMARY KEY,
    [ActivityName] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [ShortDescription] NVARCHAR(500) NULL,
    [Duration] NVARCHAR(100) NULL,
    [CategoryID] INT NOT NULL,
    [DestinationID] INT NOT NULL,
    [SupplierID] INT NOT NULL,
    [Rating] DECIMAL(3,2) NULL DEFAULT 0.00,
    [ReviewCount] INT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NULL,
    [CreatedBy] NVARCHAR(100) NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    CONSTRAINT [FK_Activities_Categories] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories]([CategoryID]),
    CONSTRAINT [FK_Activities_Destinations] FOREIGN KEY ([DestinationID]) REFERENCES [dbo].[Destinations]([DestinationID]),
    CONSTRAINT [FK_Activities_Suppliers] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Suppliers]([SupplierID])
);
GO

CREATE INDEX [IX_Activities_CategoryID] ON [dbo].[Activities]([CategoryID]);
CREATE INDEX [IX_Activities_DestinationID] ON [dbo].[Activities]([DestinationID]);
CREATE INDEX [IX_Activities_SupplierID] ON [dbo].[Activities]([SupplierID]);
CREATE INDEX [IX_Activities_IsActive] ON [dbo].[Activities]([IsActive]) WHERE [IsActive] = 1;
GO

-- =============================================
-- Create Bookings Table
-- =============================================
CREATE TABLE [dbo].[Bookings] (
    [BookingID] INT IDENTITY(1,1) PRIMARY KEY,
    [ActivityID] INT NOT NULL,
    [ConfirmationNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [BookingDate] DATE NOT NULL,
    [TimeSlot] NVARCHAR(20) NULL,
    [CustomerEmail] NVARCHAR(255) NOT NULL,
    [CustomerName] NVARCHAR(255) NOT NULL,
    [CustomerPhone] NVARCHAR(50) NULL,
    [TotalAmount] DECIMAL(10,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [BookingStatus] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [PaymentStatus] NVARCHAR(50) NULL,
    [SupplierBookingRef] NVARCHAR(100) NULL,
    [VoucherURL] NVARCHAR(500) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NULL,
    CONSTRAINT [FK_Bookings_Activities] FOREIGN KEY ([ActivityID]) REFERENCES [dbo].[Activities]([ActivityID])
);
GO

CREATE INDEX [IX_Bookings_ActivityID] ON [dbo].[Bookings]([ActivityID]);
CREATE INDEX [IX_Bookings_BookingDate] ON [dbo].[Bookings]([BookingDate]);
CREATE INDEX [IX_Bookings_CustomerEmail] ON [dbo].[Bookings]([CustomerEmail]);
CREATE INDEX [IX_Bookings_ConfirmationNumber] ON [dbo].[Bookings]([ConfirmationNumber]);
GO

-- =============================================
-- Create Categories Table
-- =============================================
CREATE TABLE [dbo].[Categories] (
    [CategoryID] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoryName] NVARCHAR(100) NOT NULL,
    [CategorySlug] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [IconURL] NVARCHAR(500) NULL,
    [DisplayOrder] INT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- =============================================
-- Create Destinations Table
-- =============================================
CREATE TABLE [dbo].[Destinations] (
    [DestinationID] INT IDENTITY(1,1) PRIMARY KEY,
    [DestinationName] NVARCHAR(255) NOT NULL,
    [CountryCode] NVARCHAR(3) NOT NULL,
    [CityCode] NVARCHAR(10) NULL,
    [Latitude] DECIMAL(10,8) NULL,
    [Longitude] DECIMAL(11,8) NULL,
    [TimeZone] NVARCHAR(50) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE INDEX [IX_Destinations_CountryCode] ON [dbo].[Destinations]([CountryCode]);
GO

-- =============================================
-- Create Suppliers Table
-- =============================================
CREATE TABLE [dbo].[Suppliers] (
    [SupplierID] INT IDENTITY(1,1) PRIMARY KEY,
    [SupplierName] NVARCHAR(255) NOT NULL,
    [SupplierCode] NVARCHAR(50) NOT NULL UNIQUE,
    [APIEndpoint] NVARCHAR(500) NULL,
    [APIKey] NVARCHAR(500) NULL,
    [APISecret] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NULL
);
GO

-- =============================================
-- Create Activity Prices Table
-- =============================================
CREATE TABLE [dbo].[ActivityPrices] (
    [PriceID] INT IDENTITY(1,1) PRIMARY KEY,
    [ActivityID] INT NOT NULL,
    [TravelerType] NVARCHAR(50) NOT NULL,
    [Price] DECIMAL(10,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [ValidFrom] DATE NOT NULL,
    [ValidTo] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_ActivityPrices_Activities] FOREIGN KEY ([ActivityID]) REFERENCES [dbo].[Activities]([ActivityID])
);
GO

CREATE INDEX [IX_ActivityPrices_ActivityID] ON [dbo].[ActivityPrices]([ActivityID]);
CREATE INDEX [IX_ActivityPrices_ValidFrom_ValidTo] ON [dbo].[ActivityPrices]([ValidFrom], [ValidTo]);
GO

-- =============================================
-- Create Activity Availability Table
-- =============================================
CREATE TABLE [dbo].[ActivityAvailability] (
    [AvailabilityID] INT IDENTITY(1,1) PRIMARY KEY,
    [ActivityID] INT NOT NULL,
    [AvailableDate] DATE NOT NULL,
    [TimeSlot] NVARCHAR(20) NULL,
    [Capacity] INT NOT NULL DEFAULT 0,
    [BookedCount] INT NOT NULL DEFAULT 0,
    [IsAvailable] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ActivityAvailability_Activities] FOREIGN KEY ([ActivityID]) REFERENCES [dbo].[Activities]([ActivityID])
);
GO

CREATE INDEX [IX_ActivityAvailability_ActivityID_Date] ON [dbo].[ActivityAvailability]([ActivityID], [AvailableDate]);
GO

