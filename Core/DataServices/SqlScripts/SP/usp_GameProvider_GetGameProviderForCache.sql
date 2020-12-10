SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameProvider_GetGameProviderForCache]
      @gameProviderId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT 
        [Id],
        [Code],
        [Name],
        [Status],
        [Description],
        [AutoCloseRound],
        [SupportTestPlayer],
        [PlaceBetTimeout],
        [SettleBetTimeout],
        [AllowKickPlayer]
    FROM [dbo].[GameProviders]  
    WHERE [Id] = @gameProviderId
END
GO