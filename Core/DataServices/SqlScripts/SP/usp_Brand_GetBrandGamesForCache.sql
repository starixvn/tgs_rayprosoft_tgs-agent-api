SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Brand_GetBrandGamesForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Brand_GetBrandGamesForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGamesForCache]
      @brandId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT 
		g.[GameProviderId],
		g.[Id] AS [GameId],
		g.[ExternalId] as [ExternalGameId],
		g.[SubGame]
	FROM dbo.[BrandGames] bg
	JOIN dbo.[Games] g ON bg.[GameId] = g.[Id]
	WHERE bg.[BrandId] = @brandId      
END
GO