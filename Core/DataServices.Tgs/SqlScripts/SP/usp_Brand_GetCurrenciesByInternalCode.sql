SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetCurrenciesByInternalCode]
      @brandId uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CurrencyCode] AS [Key], [BrandCurrencyCode] AS [Value]
    FROM [dbo].[BrandCurrencies]
    WHERE [BrandId] = @brandId
END
GO