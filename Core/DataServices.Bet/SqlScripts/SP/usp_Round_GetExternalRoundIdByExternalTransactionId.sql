SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Round_GetExternalRoundIdByExternalTransactionId]
	  @externalTransactionId   NVARCHAR(255) 
AS 
BEGIN
    SET NOCOUNT ON
    
    SELECT 
        r.[ExternalRoundId]
    FROM dbo.[Rounds] r
    JOIN dbo.[GameActions] ga ON ga.[RoundId] = r.Id
    WHERE ga.[ExternalTransactionId] = @externalTransactionId
END
GO