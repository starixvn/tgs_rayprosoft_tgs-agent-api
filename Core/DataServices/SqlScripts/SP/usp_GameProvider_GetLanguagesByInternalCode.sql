SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetLanguagesByInternalCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetLanguagesByInternalCode AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetLanguagesByInternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CultureCode] AS [Key], [GameProviderCultureCode] AS [Value]
    FROM [dbo].[GameProviderLanguages]
    WHERE [GameProviderId] = @gameProviderId
END
GO