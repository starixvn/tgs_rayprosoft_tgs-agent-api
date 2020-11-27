SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Player_GetPlayerLoginRecords]
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