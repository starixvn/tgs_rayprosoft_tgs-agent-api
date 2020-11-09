SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Licensee_GetLanguagesByInternalCode]
      @licenseeId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CultureCode] AS [Key], [LicenseeCultureCode] AS [Value]
    FROM [dbo].[LicenseeLanguages]
    WHERE [LicenseeId] = @licenseeId
END
GO