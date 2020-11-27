SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGameProvidersForDetail]
      @brandId    UNIQUEIDENTIFIER,
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
	brandGames
	AS
    (
		SELECT g.*, bgp.[Status] 
		FROM games g
		JOIN dbo.[BrandGameProviders] bgp ON bgp.[GameProviderId] = g.[GameProviderId]
        JOIN dbo.[BrandGames] bg ON bg.[GameId] = g.[Id] AND bg.[BrandId] = bgp.[BrandId]
        WHERE bg.[BrandId] = @brandId
    )
	SELECT DISTINCT
		gp.[Id],
        gp.[Code],
        gp.[Name],
        gp.[Status],
        gp.[Description],
		g.[Type] AS [GameType]
    FROM dbo.[GameProviders] gp
    JOIN brandGames g ON g.[GameProviderId] = gp.[Id]
    JOIN [GameProviderCurrencies] c ON c.[GameProviderId] = gp.[Id]
	WHERE c.CurrencyCode = @currencyCode
END
GO