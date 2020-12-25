SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Round_GetExternalRoundIdByExternalTransactionId]
	  @externalTransactionId   NVARCHAR(255) 
AS 
BEGIN
    SET NOCOUNT ON

    EXEC [Bet].[dbo].[usp_Round_GetExternalRoundIdByExternalTransactionId] @externalTransactionId
END
GO