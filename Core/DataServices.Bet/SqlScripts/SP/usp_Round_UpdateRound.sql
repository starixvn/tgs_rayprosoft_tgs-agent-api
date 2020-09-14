SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Round_UpdateRound') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Round_UpdateRound AS');
GO

ALTER PROCEDURE [dbo].[usp_Round_UpdateRound]
        @id                 uniqueidentifier,        
        @status             smallint,        
        @closedOn           datetimeoffset,
        @updatedOn          datetimeoffset,
        @externalClosedOn   datetimeoffset NULL,
        @betAmount          decimal(18, 6) NULL,
        @winAmount          decimal(18, 6) NULL,
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
         SET [Commssion] = @commission,
             [Status] = @status,
             [UpdatedOn] = @updatedOn
         WHERE [Id] = @id
    END
    ELSE IF @isClosed = 1
    BEGIN
         UPDATE [dbo].[Rounds]
         SET [Commssion] = @commission,
             [Status] = @status,
             [ClosedOn] = @closedOn,
             [ExternalClosedOn] = @externalClosedOn,
             [BetAmount] = @betAmount,
             [WinAmount] = @winAmount,
             [WinLoss] = @winLoss,
             [TieBetAdjustment] = @tieBetAdjustment,
             [UpdatedOn] = @updatedOn
         WHERE [Id] = @id
    END
    ELSE
    BEGIN   
        UPDATE [dbo].[Rounds]
        SET [Commssion] = @commission,
            [UpdatedOn] = @updatedOn
        WHERE [Id] = @id
    END

    IF ISNULL(@roundDetail, '') <> ''
    BEGIN
        
        UPDATE [dbo].[RoundDetails]
        SET [AdditionalData] = @roundDetail
        WHERE [RoundId] = @RoundId

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