SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetGamesForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetGamesForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetGamesForCache]
      @gameProviderId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT
        [Id],
        [Code],
        [Name],
        [Description],
        [Status],
        [ExternalId],
        [SubGame],
        [RuleUrl],
        [EndpointPath],
        [Type],
        [PlatformType],
        [HasTurnoverOnTieBet]
    FROM dbo.[Games]
    WHERE [GameProviderId] = @gameProviderId
END
GO