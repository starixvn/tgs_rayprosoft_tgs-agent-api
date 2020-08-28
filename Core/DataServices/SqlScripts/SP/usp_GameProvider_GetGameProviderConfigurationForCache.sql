SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetGameProviderConfigurationForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetGameProviderConfigurationForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetGameProviderConfigurationForCache]
      @gameProviderConfigurationId 	UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT 
        [Id],
        [Name],
        [Description],
        [Status],
        [SecretKey],
        [GameProviderId],
        [DesktopEndpoint],
        [MobileEndpoint],
        [ConfigurationData],
        [IsDefault]
    FROM[dbo].[GameProviderConfigurations]
    WHERE [Id] = @gameProviderConfigurationId
END
GO