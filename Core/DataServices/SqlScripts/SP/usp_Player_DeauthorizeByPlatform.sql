SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Player_DeauthorizeByPlatform]
      @playerCode    NVARCHAR(16),
      @platformType  smallint NULL
AS 
BEGIN
    SET NOCOUNT ON
    
    UPDATE [dbo].[PlayerLoginRecords]
    SET [dbo].[PlayerLoginRecords].[Status] = 1
    FROM [dbo].[PlayerLoginRecords] record
    WHERE record.[Status] = 0 
    AND record.[PlayerCode] = @playerCode
    AND record.[PlatformType] = @platformType OR @platformType IS NULL
    AND record.[ExpiredOn] >= SYSDATETIMEOFFSET()
    
END
GO