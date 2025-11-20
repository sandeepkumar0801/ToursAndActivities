-- =============================================
-- Tours and Activities API - Sample Database Objects
-- This file demonstrates the SQL Server technologies used in the project
-- =============================================

-- =============================================
-- Sample Stored Procedure: Get Activity Details
-- Used by: Activity search and detail endpoints
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_get_Activity_detail]
    @ServiceID INT,
    @LanguageCode VARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.ActivityID,
        a.ActivityName,
        a.Description,
        a.Duration,
        a.CategoryID,
        a.SupplierID,
        a.Rating,
        a.ReviewCount,
        a.IsActive,
        a.CreatedDate,
        a.ModifiedDate
    FROM Activities a
    WHERE a.ActivityID = @ServiceID
        AND a.IsActive = 1;
        
    -- Get pricing information
    SELECT 
        p.PriceID,
        p.ActivityID,
        p.TravelerType,
        p.Price,
        p.Currency,
        p.ValidFrom,
        p.ValidTo
    FROM ActivityPrices p
    WHERE p.ActivityID = @ServiceID
        AND p.ValidFrom <= GETDATE()
        AND (p.ValidTo IS NULL OR p.ValidTo >= GETDATE());
        
    -- Get availability
    SELECT 
        av.AvailabilityID,
        av.ActivityID,
        av.AvailableDate,
        av.TimeSlot,
        av.Capacity,
        av.BookedCount,
        (av.Capacity - av.BookedCount) AS RemainingSpots
    FROM ActivityAvailability av
    WHERE av.ActivityID = @ServiceID
        AND av.AvailableDate >= CAST(GETDATE() AS DATE);
END
GO

-- =============================================
-- Sample Stored Procedure: Search Products
-- Full-text search implementation
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_get_search_product_fulltext]
    @Keyword NVARCHAR(255),
    @DestinationID INT = NULL,
    @CategoryID INT = NULL,
    @MinPrice DECIMAL(10,2) = NULL,
    @MaxPrice DECIMAL(10,2) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    WITH SearchResults AS (
        SELECT 
            a.ActivityID,
            a.ActivityName,
            a.Description,
            a.Duration,
            a.Rating,
            a.ReviewCount,
            d.DestinationName,
            c.CategoryName,
            MIN(p.Price) AS MinPrice,
            MAX(p.Price) AS MaxPrice,
            ROW_NUMBER() OVER (ORDER BY a.Rating DESC, a.ReviewCount DESC) AS RowNum
        FROM Activities a
        INNER JOIN Destinations d ON a.DestinationID = d.DestinationID
        INNER JOIN Categories c ON a.CategoryID = c.CategoryID
        LEFT JOIN ActivityPrices p ON a.ActivityID = p.ActivityID
        WHERE a.IsActive = 1
            AND (@Keyword IS NULL OR 
                 a.ActivityName LIKE '%' + @Keyword + '%' OR 
                 a.Description LIKE '%' + @Keyword + '%' OR
                 d.DestinationName LIKE '%' + @Keyword + '%')
            AND (@DestinationID IS NULL OR a.DestinationID = @DestinationID)
            AND (@CategoryID IS NULL OR a.CategoryID = @CategoryID)
        GROUP BY 
            a.ActivityID, a.ActivityName, a.Description, a.Duration,
            a.Rating, a.ReviewCount, d.DestinationName, c.CategoryName
    )
    SELECT 
        ActivityID,
        ActivityName,
        Description,
        Duration,
        Rating,
        ReviewCount,
        DestinationName,
        CategoryName,
        MinPrice,
        MaxPrice,
        (SELECT COUNT(*) FROM SearchResults) AS TotalCount
    FROM SearchResults
    WHERE (@MinPrice IS NULL OR MinPrice >= @MinPrice)
        AND (@MaxPrice IS NULL OR MaxPrice <= @MaxPrice)
        AND RowNum > @Offset
        AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END
GO

-- =============================================
-- Sample Stored Procedure: Create Booking
-- Handles booking creation with transaction
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_create_booking]
    @ActivityID INT,
    @BookingDate DATE,
    @TimeSlot VARCHAR(10),
    @CustomerEmail VARCHAR(255),
    @CustomerName VARCHAR(255),
    @TotalAmount DECIMAL(10,2),
    @Currency VARCHAR(3),
    @BookingID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Insert booking
        INSERT INTO Bookings (
            ActivityID, BookingDate, TimeSlot, CustomerEmail, 
            CustomerName, TotalAmount, Currency, BookingStatus, 
            CreatedDate, ConfirmationNumber
        )
        VALUES (
            @ActivityID, @BookingDate, @TimeSlot, @CustomerEmail,
            @CustomerName, @TotalAmount, @Currency, 'Pending',
            GETDATE(), 'BK' + CAST(NEWID() AS VARCHAR(36))
        );
        
        SET @BookingID = SCOPE_IDENTITY();
        
        -- Update availability
        UPDATE ActivityAvailability
        SET BookedCount = BookedCount + 1
        WHERE ActivityID = @ActivityID
            AND AvailableDate = @BookingDate
            AND TimeSlot = @TimeSlot;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

