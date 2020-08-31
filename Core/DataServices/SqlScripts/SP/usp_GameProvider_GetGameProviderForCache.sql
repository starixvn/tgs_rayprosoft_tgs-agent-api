SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetGameProviderForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetGameProviderForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetGameProviderForCache]
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
        [SettleBetTimeout]
    FROM [dbo].[GameProviders]  
    WHERE [Id] = @gameProviderId
END
GO