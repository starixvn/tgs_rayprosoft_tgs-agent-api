SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetCurrenciesByInternalCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetCurrenciesByInternalCode AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetCurrenciesByInternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CurrencyCode] AS [Key], [GameProviderCurrencyCode] AS [Value]
    FROM [dbo].[GameProviderCurrencies]
    WITH (NOLOCK)
    WHERE [GameProviderId] = @gameProviderId
END
GO