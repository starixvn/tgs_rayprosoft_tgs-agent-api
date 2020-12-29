SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetLanguagesByInternalCode]
      @brandId uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT DISTINCT [CultureCode] AS [Key], [BrandCultureCode] AS [Value]
    FROM [dbo].[BrandLanguages]
    WHERE [BrandId] = @brandId
END
GO