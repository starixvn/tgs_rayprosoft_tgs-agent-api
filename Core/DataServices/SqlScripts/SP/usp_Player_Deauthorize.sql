SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Player_Deauthorize') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Player_Deauthorize AS');
GO

ALTER PROCEDURE [dbo].[usp_Player_Deauthorize]
      @playerCode  NVARCHAR(16)
AS 
BEGIN
    SET NOCOUNT ON
    
    UPDATE [dbo].[PlayerLoginRecords]
    SET [dbo].[PlayerLoginRecords].[Status] = 1
    FROM [dbo].[PlayerLoginRecords] record
    WHERE record.[Status] = 0 
    AND record.PlayerCode = @playerCode
    AND record.ExpiredOn >= SYSDATETIMEOFFSET()
    
END
GO