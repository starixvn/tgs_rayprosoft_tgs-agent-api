SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Round_GetRoundByExternalRoundId]
      @gameProviderConfigurationId UNIQUEIDENTIFIER,
      @walletId                    UNIQUEIDENTIFIER,
	  @externalRoundId             NVARCHAR(64) 
AS 
BEGIN
    SET NOCOUNT ON

    EXEC [Bet].[dbo].[usp_Round_GetRoundByExternalRoundId] @gameProviderConfigurationId, @walletId, @externalRoundId
END
GO