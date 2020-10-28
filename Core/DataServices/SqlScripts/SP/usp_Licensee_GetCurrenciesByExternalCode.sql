SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Licensee_GetCurrenciesByExternalCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Licensee_GetCurrenciesByExternalCode AS');
GO

ALTER PROCEDURE [dbo].[usp_Licensee_GetCurrenciesByExternalCode]
      @licenseeId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [LicenseeCurrencyCode] AS [Key], [CurrencyCode] AS [Value]
    FROM [dbo].[LicenseeCurrencies]
    WHERE [LicenseeId] = @licenseeId
END
GO