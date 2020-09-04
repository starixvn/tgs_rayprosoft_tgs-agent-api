SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Player_GetPlayerLoginRecords') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Player_GetPlayerLoginRecords AS');
GO

ALTER PROCEDURE [dbo].[usp_Player_GetPlayerLoginRecords]
      @playerCode    NVARCHAR(16),
	  @statuses      IntValuesType READONLY,
      @from          DATETIMEOFFSET
AS 
BEGIN
    SET NOCOUNT ON

    SELECT 
        [BrandId], [PlatformType], [CreatedOn], [ExpiredOn]
    FROM dbo.[PlayerLoginRecords]
    WHERE [PlayerCode] = @playerCode
    AND [CreatedOn] >= @from
    AND [Status] IN (SELECT [Value] FROM @statuses)
END
GO