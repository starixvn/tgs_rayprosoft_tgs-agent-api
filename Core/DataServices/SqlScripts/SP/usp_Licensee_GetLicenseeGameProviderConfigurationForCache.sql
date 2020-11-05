SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Licensee_GetLicenseeGameProviderConfigurationForCache]
      @licenseeId			UNIQUEIDENTIFIER,
	  @gameProviderId	UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT
      [DefaultLanguageCode], 
      [LicenseeLogoUrl],
      [DesktopQueryString],
      [MobileQueryString],
      [GameProviderConfigurationId]
    FROM dbo.[LicenseeGameProviderConfigurations]
    WHERE [LicenseeId] = @licenseeId 
    AND [GameProviderId] = @gameProviderId
END
GO