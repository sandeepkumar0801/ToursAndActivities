-- =============================================
-- SSRS Report Queries
-- SQL Server Reporting Services (SSRS)
-- Data sources for business intelligence reports
-- =============================================

-- =============================================
-- Daily Booking Summary Report
-- Used by SSRS for daily booking analytics
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSRS_DailyBookingSummary]
    @StartDate DATE,
    @EndDate DATE,
    @SupplierID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CAST(b.CreatedDate AS DATE) AS BookingDate,
        s.SupplierName,
        c.CategoryName,
        d.DestinationName,
        COUNT(DISTINCT b.BookingID) AS TotalBookings,
        SUM(b.TotalAmount) AS TotalRevenue,
        AVG(b.TotalAmount) AS AverageBookingValue,
        COUNT(DISTINCT b.CustomerEmail) AS UniqueCustomers,
        SUM(CASE WHEN b.BookingStatus = 'Confirmed' THEN 1 ELSE 0 END) AS ConfirmedBookings,
        SUM(CASE WHEN b.BookingStatus = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledBookings,
        SUM(CASE WHEN b.BookingStatus = 'Pending' THEN 1 ELSE 0 END) AS PendingBookings
    FROM [dbo].[Bookings] b
    INNER JOIN [dbo].[Activities] a ON b.ActivityID = a.ActivityID
    INNER JOIN [dbo].[Suppliers] s ON a.SupplierID = s.SupplierID
    INNER JOIN [dbo].[Categories] c ON a.CategoryID = c.CategoryID
    INNER JOIN [dbo].[Destinations] d ON a.DestinationID = d.DestinationID
    WHERE CAST(b.CreatedDate AS DATE) BETWEEN @StartDate AND @EndDate
        AND (@SupplierID IS NULL OR a.SupplierID = @SupplierID)
    GROUP BY 
        CAST(b.CreatedDate AS DATE),
        s.SupplierName,
        c.CategoryName,
        d.DestinationName
    ORDER BY BookingDate DESC, TotalRevenue DESC;
END
GO

-- =============================================
-- Revenue by Destination Report
-- Geographic revenue analysis for SSRS
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSRS_RevenueByDestination]
    @Year INT,
    @Month INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.DestinationName,
        d.CountryCode,
        COUNT(DISTINCT b.BookingID) AS TotalBookings,
        SUM(b.TotalAmount) AS TotalRevenue,
        AVG(b.TotalAmount) AS AverageBookingValue,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 1 THEN b.TotalAmount ELSE 0 END) AS Jan,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 2 THEN b.TotalAmount ELSE 0 END) AS Feb,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 3 THEN b.TotalAmount ELSE 0 END) AS Mar,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 4 THEN b.TotalAmount ELSE 0 END) AS Apr,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 5 THEN b.TotalAmount ELSE 0 END) AS May,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 6 THEN b.TotalAmount ELSE 0 END) AS Jun,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 7 THEN b.TotalAmount ELSE 0 END) AS Jul,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 8 THEN b.TotalAmount ELSE 0 END) AS Aug,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 9 THEN b.TotalAmount ELSE 0 END) AS Sep,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 10 THEN b.TotalAmount ELSE 0 END) AS Oct,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 11 THEN b.TotalAmount ELSE 0 END) AS Nov,
        SUM(CASE WHEN MONTH(b.CreatedDate) = 12 THEN b.TotalAmount ELSE 0 END) AS Dec
    FROM [dbo].[Bookings] b
    INNER JOIN [dbo].[Activities] a ON b.ActivityID = a.ActivityID
    INNER JOIN [dbo].[Destinations] d ON a.DestinationID = d.DestinationID
    WHERE YEAR(b.CreatedDate) = @Year
        AND (@Month IS NULL OR MONTH(b.CreatedDate) = @Month)
        AND b.BookingStatus = 'Confirmed'
    GROUP BY d.DestinationName, d.CountryCode
    ORDER BY TotalRevenue DESC;
END
GO

-- =============================================
-- Supplier Performance Report
-- Supplier KPI metrics for SSRS dashboards
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSRS_SupplierPerformance]
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.SupplierName,
        s.SupplierCode,
        COUNT(DISTINCT a.ActivityID) AS TotalActivities,
        COUNT(DISTINCT b.BookingID) AS TotalBookings,
        SUM(b.TotalAmount) AS TotalRevenue,
        AVG(b.TotalAmount) AS AverageBookingValue,
        AVG(a.Rating) AS AverageRating,
        SUM(a.ReviewCount) AS TotalReviews,
        CAST(SUM(CASE WHEN b.BookingStatus = 'Confirmed' THEN 1 ELSE 0 END) AS FLOAT) / 
            NULLIF(COUNT(b.BookingID), 0) * 100 AS ConfirmationRate,
        CAST(SUM(CASE WHEN b.BookingStatus = 'Cancelled' THEN 1 ELSE 0 END) AS FLOAT) / 
            NULLIF(COUNT(b.BookingID), 0) * 100 AS CancellationRate
    FROM [dbo].[Suppliers] s
    LEFT JOIN [dbo].[Activities] a ON s.SupplierID = a.SupplierID
    LEFT JOIN [dbo].[Bookings] b ON a.ActivityID = b.ActivityID 
        AND CAST(b.CreatedDate AS DATE) BETWEEN @StartDate AND @EndDate
    WHERE s.IsActive = 1
    GROUP BY s.SupplierName, s.SupplierCode
    ORDER BY TotalRevenue DESC;
END
GO

-- =============================================
-- Customer Booking History Report
-- Customer analytics for SSRS
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSRS_CustomerBookingHistory]
    @CustomerEmail NVARCHAR(255) = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.CustomerEmail,
        b.CustomerName,
        b.ConfirmationNumber,
        b.BookingDate,
        a.ActivityName,
        d.DestinationName,
        c.CategoryName,
        b.TotalAmount,
        b.Currency,
        b.BookingStatus,
        b.CreatedDate,
        DATEDIFF(DAY, b.CreatedDate, b.BookingDate) AS DaysInAdvance
    FROM [dbo].[Bookings] b
    INNER JOIN [dbo].[Activities] a ON b.ActivityID = a.ActivityID
    INNER JOIN [dbo].[Destinations] d ON a.DestinationID = d.DestinationID
    INNER JOIN [dbo].[Categories] c ON a.CategoryID = c.CategoryID
    WHERE (@CustomerEmail IS NULL OR b.CustomerEmail = @CustomerEmail)
        AND (@StartDate IS NULL OR CAST(b.CreatedDate AS DATE) >= @StartDate)
        AND (@EndDate IS NULL OR CAST(b.CreatedDate AS DATE) <= @EndDate)
    ORDER BY b.CreatedDate DESC;
END
GO

-- =============================================
-- Activity Performance Metrics
-- Activity-level analytics for SSRS
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSRS_ActivityPerformance]
    @CategoryID INT = NULL,
    @DestinationID INT = NULL,
    @TopN INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopN)
        a.ActivityName,
        c.CategoryName,
        d.DestinationName,
        s.SupplierName,
        a.Rating,
        a.ReviewCount,
        COUNT(DISTINCT b.BookingID) AS TotalBookings,
        SUM(b.TotalAmount) AS TotalRevenue,
        AVG(b.TotalAmount) AS AveragePrice,
        MIN(ap.Price) AS MinPrice,
        MAX(ap.Price) AS MaxPrice
    FROM [dbo].[Activities] a
    INNER JOIN [dbo].[Categories] c ON a.CategoryID = c.CategoryID
    INNER JOIN [dbo].[Destinations] d ON a.DestinationID = d.DestinationID
    INNER JOIN [dbo].[Suppliers] s ON a.SupplierID = s.SupplierID
    LEFT JOIN [dbo].[Bookings] b ON a.ActivityID = b.ActivityID
    LEFT JOIN [dbo].[ActivityPrices] ap ON a.ActivityID = ap.ActivityID AND ap.IsActive = 1
    WHERE a.IsActive = 1
        AND (@CategoryID IS NULL OR a.CategoryID = @CategoryID)
        AND (@DestinationID IS NULL OR a.DestinationID = @DestinationID)
    GROUP BY 
        a.ActivityName, c.CategoryName, d.DestinationName, 
        s.SupplierName, a.Rating, a.ReviewCount
    ORDER BY TotalRevenue DESC;
END
GO

