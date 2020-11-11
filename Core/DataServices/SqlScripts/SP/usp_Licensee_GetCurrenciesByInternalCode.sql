SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Licensee_GetCurrenciesByInternalCode]
      @licenseeId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CurrencyCode] AS [Key], [LicenseeCurrencyCode] AS [Value]
    FROM [dbo].[LicenseeCurrencies]
    WHERE [LicenseeId] = @licenseeId
END
GO