SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Licensee_GetLicenseeGameProviders]
      @licenseeId    UNIQUEIDENTIFIER,
	  @currencyCode  NVARCHAR(6),
	  @platformType  SMALLINT
AS 
BEGIN
    SET NOCOUNT ON
    
    ;WITH games
    AS
	(
		SELECT 
			g.Id,
			g.GameProviderId,
			g.[Type]
		FROM dbo.[Games] g
		WHERE PlatformType = @platformType
	),
	licenseeGames
	AS
    (
		SELECT g.*
		FROM games g
		JOIN dbo.[LicenseeGameProviders] lgp ON lgp.[GameProviderId] = g.[GameProviderId]
        JOIN dbo.[LicenseeGames] lg ON lg.[GameId] = g.[Id] AND lg.[LicenseeId] = lgp.[LicenseeId]
        WHERE lg.[LicenseeId] = @licenseeId
    )
	SELECT DISTINCT
		gp.[Id],
        gp.[Code],
        gp.[Name],
        gp.[Status],
        gp.[Description],
		g.[Type] AS [GameType]
    FROM dbo.[GameProviders] gp
    JOIN licenseeGames g ON g.[GameProviderId] = gp.[Id]
    JOIN [GameProviderCurrencies] c ON c.[GameProviderId] = gp.[Id]
	WHERE c.CurrencyCode = currencyCode
END
GO