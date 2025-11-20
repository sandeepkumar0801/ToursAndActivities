-- =============================================
-- SSIS Data Synchronization Scripts
-- SQL Server Integration Services (SSIS)
-- Used for ETL processes and data synchronization
-- =============================================

-- =============================================
-- Supplier Data Sync Stored Procedure
-- Called by SSIS packages for data synchronization
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSIS_SyncSupplierData]
    @SupplierID INT,
    @SyncType NVARCHAR(50) = 'Full',
    @LastSyncDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StartTime DATETIME2 = GETUTCDATE();
    DECLARE @RecordsProcessed INT = 0;
    DECLARE @ErrorMessage NVARCHAR(MAX);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Log sync start
        INSERT INTO [dbo].[SyncLog] (SupplierID, SyncType, StartTime, Status)
        VALUES (@SupplierID, @SyncType, @StartTime, 'Running');
        
        DECLARE @SyncLogID INT = SCOPE_IDENTITY();
        
        -- Sync activities based on type
        IF @SyncType = 'Full'
        BEGIN
            -- Full sync: Truncate and reload
            DELETE FROM [dbo].[StagingActivities] WHERE SupplierID = @SupplierID;
            
            -- Data will be loaded by SSIS Data Flow Task
            -- This procedure prepares the environment
        END
        ELSE IF @SyncType = 'Incremental'
        BEGIN
            -- Incremental sync: Only new/modified records
            -- SSIS will filter based on @LastSyncDate
            SELECT @LastSyncDate = ISNULL(@LastSyncDate, DATEADD(DAY, -1, GETUTCDATE()));
        END
        
        -- Update sync log
        UPDATE [dbo].[SyncLog]
        SET EndTime = GETUTCDATE(),
            Status = 'Completed',
            RecordsProcessed = @RecordsProcessed
        WHERE SyncLogID = @SyncLogID;
        
        COMMIT TRANSACTION;
        
        SELECT 'Success' AS Result, @RecordsProcessed AS RecordsProcessed;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @ErrorMessage = ERROR_MESSAGE();
        
        -- Log error
        INSERT INTO [dbo].[SyncErrorLog] (SupplierID, ErrorMessage, ErrorDate)
        VALUES (@SupplierID, @ErrorMessage, GETUTCDATE());
        
        SELECT 'Error' AS Result, @ErrorMessage AS ErrorMessage;
    END CATCH
END
GO

-- =============================================
-- SSIS Staging to Production Merge
-- Merges staging data into production tables
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSIS_MergeStagingToProduction]
    @SupplierID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Merge Activities
        MERGE [dbo].[Activities] AS Target
        USING [dbo].[StagingActivities] AS Source
        ON Target.SupplierActivityID = Source.SupplierActivityID 
           AND Target.SupplierID = Source.SupplierID
        WHEN MATCHED THEN
            UPDATE SET
                Target.ActivityName = Source.ActivityName,
                Target.Description = Source.Description,
                Target.Duration = Source.Duration,
                Target.Rating = Source.Rating,
                Target.ModifiedDate = GETUTCDATE()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (ActivityName, Description, Duration, SupplierID, CategoryID, 
                    DestinationID, Rating, CreatedDate)
            VALUES (Source.ActivityName, Source.Description, Source.Duration, 
                    Source.SupplierID, Source.CategoryID, Source.DestinationID,
                    Source.Rating, GETUTCDATE())
        WHEN NOT MATCHED BY SOURCE AND Target.SupplierID = @SupplierID THEN
            UPDATE SET Target.IsActive = 0, Target.ModifiedDate = GETUTCDATE();
        
        -- Merge Prices
        MERGE [dbo].[ActivityPrices] AS Target
        USING [dbo].[StagingPrices] AS Source
        ON Target.ActivityID = Source.ActivityID 
           AND Target.TravelerType = Source.TravelerType
        WHEN MATCHED THEN
            UPDATE SET
                Target.Price = Source.Price,
                Target.Currency = Source.Currency,
                Target.ValidFrom = Source.ValidFrom,
                Target.ValidTo = Source.ValidTo
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (ActivityID, TravelerType, Price, Currency, ValidFrom, ValidTo)
            VALUES (Source.ActivityID, Source.TravelerType, Source.Price, 
                    Source.Currency, Source.ValidFrom, Source.ValidTo);
        
        COMMIT TRANSACTION;
        
        SELECT 'Success' AS Result;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMsg NVARCHAR(MAX) = ERROR_MESSAGE();
        SELECT 'Error' AS Result, @ErrorMsg AS ErrorMessage;
        THROW;
    END CATCH
END
GO

-- =============================================
-- SSIS Data Quality Check
-- Validates data before loading
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_SSIS_DataQualityCheck]
    @TableName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @ErrorCount INT = 0;
    
    -- Check for NULL values in required fields
    IF @TableName = 'StagingActivities'
    BEGIN
        SELECT @ErrorCount = COUNT(*)
        FROM [dbo].[StagingActivities]
        WHERE ActivityName IS NULL 
           OR SupplierID IS NULL
           OR CategoryID IS NULL;
        
        IF @ErrorCount > 0
        BEGIN
            RAISERROR('Data quality check failed: %d records with NULL required fields', 16, 1, @ErrorCount);
            RETURN;
        END
    END
    
    -- Check for duplicate records
    SELECT @ErrorCount = COUNT(*)
    FROM (
        SELECT SupplierActivityID, SupplierID, COUNT(*) AS DuplicateCount
        FROM [dbo].[StagingActivities]
        GROUP BY SupplierActivityID, SupplierID
        HAVING COUNT(*) > 1
    ) AS Duplicates;
    
    IF @ErrorCount > 0
    BEGIN
        RAISERROR('Data quality check failed: %d duplicate records found', 16, 1, @ErrorCount);
        RETURN;
    END
    
    SELECT 'Success' AS Result, 'Data quality check passed' AS Message;
END
GO

