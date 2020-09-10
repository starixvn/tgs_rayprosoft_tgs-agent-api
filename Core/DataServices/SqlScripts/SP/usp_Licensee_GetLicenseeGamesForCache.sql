SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Licensee_GetLicenseeGamesForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Licensee_GetLicenseeGamesForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_Licensee_GetLicenseeGamesForCache]
      @licenseeId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT 
		g.[GameProviderId],
		g.[Id] AS [GameId],
		g.[ExternalId] as [ExternalGameId],
		g.[SubGame]
	FROM dbo.[LicenseeGames] bg
	JOIN dbo.[Games] g ON bg.[GameId] = g.[Id]
	WHERE bg.[LicenseeId] = @licenseeId      
END
GO