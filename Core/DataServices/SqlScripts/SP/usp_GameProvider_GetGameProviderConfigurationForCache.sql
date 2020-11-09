SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameProvider_GetGameProviderConfigurationForCache]
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