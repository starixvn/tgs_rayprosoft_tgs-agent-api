SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Round_UpdateRound') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Round_UpdateRound AS');
GO

ALTER PROCEDURE [dbo].[usp_Round_UpdateRound]
        @roundId            uniqueidentifier,        
        @status             smallint,        
        @closedOn           datetimeoffset,
        @updatedOn          datetimeoffset,
        @externalClosedOn   datetimeoffset NULL,
        @betAmount          decimal(18, 6) NULL,
        @winLoss            decimal(18, 6) NULL,
        @tieBetAdjustment   decimal(18, 6) NULL,        
        @commission         decimal(18, 6) NULL,        
        @roundDetail        nvarchar(max) NULL,
        @isReopen           bit,
        @isClosed           bit
AS 
BEGIN
    SET NOCOUNT ON
    
    IF @isReopen = 1
    BEGIN
         UPDATE [dbo].[Rounds]
         SET [Commission] = @commission,
             [Status] = @status,
             [UpdatedOn] = @updatedOn
         WHERE [Id] = @roundId
    END
    ELSE IF @isClosed = 1
    BEGIN
         UPDATE [dbo].[Rounds]
         SET [Commission] = @commission,
             [Status] = @status,             
             [BetAmount] = @betAmount,
             [WinLoss] = @winLoss,
             [TieBetAdjustment] = @tieBetAdjustment,
             [UpdatedOn] = @updatedOn,
             [ClosedOn] = @closedOn,
             [ExternalClosedOn] = @externalClosedOn
         WHERE [Id] = @roundId
    END
    ELSE
    BEGIN   
        UPDATE [dbo].[Rounds]
        SET [Commission] = @commission,
            [UpdatedOn] = @updatedOn
        WHERE [Id] = @roundId
    END

    IF ISNULL(@roundDetail, '') <> ''
    BEGIN
        
        UPDATE [dbo].[RoundDetails]
        SET [AdditionalData] = @roundDetail
        WHERE [RoundId] = @roundId

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO [dbo].[RoundDetails]
               ([RoundId]
               ,[AdditionalData])
            VALUES
               (@roundId           
               ,@roundDetail); 
        END
    END

END
GO