SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Brand_GetBrandGameProviderConfigurationForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Brand_GetBrandGameProviderConfigurationForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_Brand_GetBrandGameProviderConfigurationForCache]
      @brandId			UNIQUEIDENTIFIER,
	  @gameProviderId	UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT
      [DefaultLanguageCode], 
      [BrandLogoUrl],
      [DesktopQueryString],
      [MobileQueryString],
      [GameProviderConfigurationId]
    FROM dbo.[BrandGameProviderConfigurations]
    WHERE [BrandId] = @brandId 
    AND [GameProviderId] = @gameProviderId
END
GO