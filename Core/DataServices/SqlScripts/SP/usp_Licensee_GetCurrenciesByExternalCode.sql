SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Licensee_GetCurrenciesByExternalCode]
      @licenseeId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [LicenseeCurrencyCode] AS [Key], [CurrencyCode] AS [Value]
    FROM [dbo].[LicenseeCurrencies]
    WHERE [LicenseeId] = @licenseeId
END
GO