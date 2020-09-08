SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Round_GetRoundByExternalRoundId') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Round_GetRoundByExternalRoundId AS');
GO

ALTER PROCEDURE [dbo].[usp_Round_GetRoundByExternalRoundId]
      @gameProviderConfigurationId UNIQUEIDENTIFIER,
      @walletId                    UNIQUEIDENTIFIER,
	  @externalRoundId             NVARCHAR(64) 
AS 
BEGIN
    SET NOCOUNT ON

    EXEC [Bet].[dbo].[usp_Round_GetRoundByExternalRoundId] @gameProviderConfigurationId, @walletId, @externalRoundId
END
GO