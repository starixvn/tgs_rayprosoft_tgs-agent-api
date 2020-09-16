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
    
    EXEC [Bet].[dbo].[usp_Round_UpdateRound] 
        @roundId,        
        @status,        
        @closedOn,
        @updatedOn,
        @externalClosedOn,
        @betAmount,
        @winLoss,
        @tieBetAdjustment,        
        @commission,        
        @roundDetail,
        @isReopen,
        @isClosed   

END
GO