SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Player_GetPlayerLatestLoginRecord') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Player_GetPlayerLatestLoginRecord AS');
GO

ALTER PROCEDURE [dbo].[usp_Player_GetPlayerLatestLoginRecord]
      @playerCode    NVARCHAR(16),
	  @platformType  SMALLINT
AS 
BEGIN
    SET NOCOUNT ON

    DECLARE @from DATETIMEOFFSET = DATEADD(day, -1, SYSDATETIMEOFFSET())
    DECLARE @now DATETIMEOFFSET = SYSDATETIMEOFFSET()
    
    SELECT 
        TOP 1 [BrandId], [PlatformType], [CreatedOn], [ExpiredOn]
    FROM dbo.[PlayerLoginRecords]
    WHERE [PlayerCode] = @playerCode
    AND [CreatedOn] >= @from
    AND [ExpiredOn] >= @now
    AND	([PlatformType] = @platformType OR @platformType IS NULL)
    AND [Status] = '0' -- Enabled
    ORDER BY [ExpiredON] DESC

END
GO