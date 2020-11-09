SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Licensee_GetLanguagesByExternalCode]
      @licenseeId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [LicenseeCultureCode] AS [Key], [CultureCode] AS [Value]
    FROM [dbo].[LicenseeLanguages]
    WHERE [LicenseeId] = @licenseeId
END
GO