SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Round_GetRoundByExternalRoundId]
      @gameProviderConfigurationId  UNIQUEIDENTIFIER,
      @walletId                     UNIQUEIDENTIFIER,
	  @externalRoundId              NVARCHAR(255) 
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