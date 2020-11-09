SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameProvider_GetLanguagesByExternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [GameProviderCultureCode] AS [Key], [CultureCode] AS [Value]
    FROM [dbo].[GameProviderLanguages]
    WHERE [GameProviderId] = @gameProviderId
END
GO