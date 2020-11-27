SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetLanguagesByExternalCode]
      @brandId uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [BrandCultureCode] AS [Key], [CultureCode] AS [Value]
    FROM [dbo].[BrandLanguages]
    WHERE [BrandId] = @brandId
END
GO