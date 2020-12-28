SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameProvider_GetCurrenciesByInternalCode]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CurrencyCode] AS [Key], [GameProviderCurrencyCode] AS [Value]
    FROM [dbo].[GameProviderCurrencies]
    WHERE [GameProviderId] = @gameProviderId
END
GO