SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGameProvidersForCache]
      @brandId    UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
    SELECT DISTINCT	
        bgp.[GameProviderId],
        gp.[Code] AS [GameProviderCode],
        bgp.[GameProviderConfigurationId],
		bgp.[Status] AS [BrandGameProviderStatus]
    FROM dbo.[GameProviders] gp
	JOIN dbo.[BrandGameProviders] bgp ON bgp.[GameProviderId] = gp.[Id]
	WHERE bgp.[BrandId] = @brandId
END
GO