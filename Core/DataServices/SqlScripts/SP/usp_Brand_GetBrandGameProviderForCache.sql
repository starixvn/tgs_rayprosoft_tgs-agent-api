SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGameProviderForCache]
      @brandId			UNIQUEIDENTIFIER,
	  @gameProviderId   UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON

	SELECT DISTINCT		
        gp.[Code] AS [GameProviderCode],
        bgp.[GameProviderConfigurationId],
		bgp.[Status] AS [BrandGameProviderStatus]
    FROM dbo.[GameProviders] gp
	JOIN dbo.[BrandGameProviders] bgp ON bgp.[GameProviderId] = gp.[Id]
	WHERE bgp.[BrandId] = @brandId
    AND bgp.[GameProviderId] = @gameProviderId
END
GO