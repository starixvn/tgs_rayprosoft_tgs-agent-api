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
	  @externalRoundId             NVARCHAR(255) 
AS 
BEGIN
    SET NOCOUNT ON
    
    SELECT 
        [Id] AS [RoundId],
        [ExternalRoundId],
        [Status]
    FROM dbo.[Rounds]
    WHERE [ExternalRoundId] = @externalRoundId
    AND [WalletId] = @walletId
    AND [GameProviderConfigurationId] = @gameProviderConfigurationId
END
GO