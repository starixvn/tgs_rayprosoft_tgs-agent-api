SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGamesForCache]
      @brandId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT 
		g.[GameProviderId],
		g.[Id] AS [GameId],
		g.[ExternalId] AS [ExternalGameId],
		g.[PlatformType],
		g.[SubGame],
		bg.[Status] AS [BrandGameStatus]
	FROM dbo.[BrandGames] bg
	JOIN dbo.[Games] g ON bg.[GameId] = g.[Id]
	WHERE bg.[BrandId] = @brandId    
END
GO