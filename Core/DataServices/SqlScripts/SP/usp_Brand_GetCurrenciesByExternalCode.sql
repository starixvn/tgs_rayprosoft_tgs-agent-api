SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetCurrenciesByExternalCode]
      @brandId uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [BrandCurrencyCode] AS [Key], [CurrencyCode] AS [Value]
    FROM [dbo].[BrandCurrencies]
    WHERE [BrandId] = @brandId
END
GO