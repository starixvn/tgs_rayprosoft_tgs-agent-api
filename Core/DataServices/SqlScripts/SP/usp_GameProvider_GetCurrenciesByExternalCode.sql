SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetCurrenciesByExternalCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetCurrenciesByExternalCode AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetCurrenciesByExternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [GameProviderCurrencyCode] AS [Key], [CurrencyCode] AS [Value]
    FROM [dbo].[GameProviderCurrencies]
    WITH (NOLOCK)
    WHERE [GameProviderId] = @gameProviderId
END
GO