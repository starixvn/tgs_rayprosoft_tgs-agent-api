SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGameProviderConfigurationForCache]
      @brandId  		UNIQUEIDENTIFIER,
	  @gameProviderId	UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT
        [DefaultLanguageCode],
        [DesktopQueryString],
        [MobileQueryString],
        [GameProviderConfigurationId]
    FROM dbo.[BrandGameProviders]
    WHERE [BrandId] = @brandId
    AND [GameProviderId] = @gameProviderId
END
GO