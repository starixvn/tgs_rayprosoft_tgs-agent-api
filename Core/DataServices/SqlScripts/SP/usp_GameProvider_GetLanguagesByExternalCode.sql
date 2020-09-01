SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetLanguagesByExternalCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetLanguagesByExternalCode AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetLanguagesByExternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [GameProviderCultureCode] AS [Key], [CultureCode] AS [Value]
    FROM [dbo].[GameProviderLanguages]
    WHERE [GameProviderId] = @gameProviderId
END
GO