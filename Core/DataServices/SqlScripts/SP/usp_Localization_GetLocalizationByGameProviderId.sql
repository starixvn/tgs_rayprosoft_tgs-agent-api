SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Localization_GetLocalizationByGameProviderId') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Localization_GetLocalizationByGameProviderId AS');
GO

ALTER PROCEDURE [dbo].[usp_Localization_GetLocalizationByGameProviderId]
      @gameProviderId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
    SELECT          
	    Id As [Value]
	INTO #temp
	FROM dbo.[Games] g
	WHERE g.GameProviderId = @gameProviderId
    UNION ALL
    SELECT @gameProviderId

	SELECT DISTINCT
		Context,
        [Key],
        CultureCode,
        Translation
    FROM dbo.[LocalizedStrings] ls WITH (NOLOCK)
    JOIN #temp t ON ls.[Key] = t.[Value]

	DROP TABLE #temp
	
END
GO