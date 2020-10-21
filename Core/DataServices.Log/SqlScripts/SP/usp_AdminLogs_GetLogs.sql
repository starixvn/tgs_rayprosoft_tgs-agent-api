SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_AdminLogs_GetLogs') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_AdminLogs_GetLogs] AS');
GO

ALTER PROCEDURE [dbo].[usp_AdminLogs_GetLogs]
    @from		datetimeoffset(7),
	@to			datetimeoffset(7),
    @trackingId UNIQUEIDENTIFIER NULL,
	@ip         NVARCHAR(15),
	@path       NVARCHAR(255),
    @method     NVARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;
    
    SELECT
        a.RecordedOn, 
        a.TrackingId, 
        a.Id,
        a.Ip,
        a.Path,
        a.Method,
        a.Description,
		a.Exception,
		a.StackTrace,
		a.ServerName,
		a.RequestHeaders,
		a.RequestContent,
		a.ResponseHeaders,
		a.ResponseContent,
		a.ResponseCode,
		a.ExecutionTimeMs
    FROM dbo.AdminLogs a WITH ( NOLOCK )
    WHERE a.RecordedOn >= @from        
      AND a.RecordedOn < @to  
      AND (@trackingId IS NULL OR a.TrackingId = @trackingId)
	  AND (@ip IS NULL OR a.Ip = @ip)
      AND ((@path IS NULL OR @path = '') OR a.Path LIKE '%'+@path+'%')
      AND (@method IS NULL OR a.TrackingId = @method) 	
END

GO


