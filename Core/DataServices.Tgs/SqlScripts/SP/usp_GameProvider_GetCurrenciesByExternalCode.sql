SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameProvider_GetCurrenciesByExternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [GameProviderCurrencyCode] AS [Key], [CurrencyCode] AS [Value]
    FROM [dbo].[GameProviderCurrencies]
    WHERE [GameProviderId] = @gameProviderId
END
GO