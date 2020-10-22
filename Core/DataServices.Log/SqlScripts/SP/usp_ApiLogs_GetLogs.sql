SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_ApiLogs_GetLogs') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_ApiLogs_GetLogs] AS');
GO

ALTER PROCEDURE [dbo].[usp_ApiLogs_GetLogs]
    @from			datetimeoffset(7),
	@to				datetimeoffset(7),
    @trackingId		UNIQUEIDENTIFIER NULL,
	@ip				NVARCHAR(15),
	@path			NVARCHAR(255),
    @method			NVARCHAR(255),
    @pageSize		INT,
	@pageNumber		INT,
	@sortBy			NVARCHAR(255),
	@isDesc			BIT
AS
BEGIN

    SET NOCOUNT ON;

    create table #temptable (
		RecordedOn datetimeoffset NOT NULL, 
		TrackingId UNIQUEIDENTIFIER NOT NULL,
		Id UNIQUEIDENTIFIER NOT NULL,
		Ip NVarchar(50) NOT NULL,
		Path NVarchar(255),
		Method NVarchar(255) NOT NULL,
		Prefix NVarchar(255) NOT NULL,
		ServerName NVarchar(30) NOT NULL,
		RequestHeaders NVarchar(4000) NOT NULL,
		RequestContent NVarchar(4000) NOT NULL,
		ResponseHeaders NVarchar(4000) NOT NULL,
		ResponseContent NVarchar(4000) NOT NULL,
		ResponseCode smallint NOT NULL,
		ExecutionTimeMs int NOT NULL,
		TotalCount int NOT NULL
		);

	WITH Main_CTE AS(
    SELECT
        a.RecordedOn, 
        a.TrackingId, 
        a.Id,
        a.Ip,
        a.Path,
        a.Method,
        a.Prefix,
		a.ServerName,
		a.RequestHeaders,
		a.RequestContent,
		a.ResponseHeaders,
		a.ResponseContent,
		a.ResponseCode,
		a.ExecutionTimeMs
    FROM dbo.ApiLogs a WITH ( NOLOCK )
    WHERE a.RecordedOn >= @from        
      AND a.RecordedOn < @to  
      AND (@trackingId IS NULL OR a.TrackingId = @trackingId)
	  AND (@ip IS NULL OR a.Ip = @ip)
      AND ((@path IS NULL OR @path = '') OR a.Path LIKE '%'+@path+'%')
      AND (@method IS NULL OR a.TrackingId = @method)    
)
, Count_CTE AS(
	SELECT COUNT(*) AS TotalCount		
		FROM Main_CTE
)
	INSERT INTO #temptable
    SELECT * FROM Main_CTE,Count_CTE
    ORDER BY
			CASE WHEN @isDesc = 0 AND @sortBy = 'RecordedOn'			THEN RecordedOn		END ASC,
			CASE WHEN @isDesc = 0 AND @sortBy = 'Method'			THEN Method		END ASC,
			CASE WHEN @isDesc = 0 AND @sortBy = 'Ip'			THEN Ip		END ASC,
			CASE WHEN @isDesc = 0 AND @sortBy = 'Path'			THEN Path		END ASC,
			CASE WHEN @isDesc = 1 AND @sortBy = 'RecordedOn'			THEN RecordedOn		END DESC,
			CASE WHEN @isDesc = 1 AND @sortBy = 'Method'			THEN Method		END DESC,
			CASE WHEN @isDesc = 1 AND @sortBy = 'Ip'			THEN Ip		END DESC,
			CASE WHEN @isDesc = 1 AND @sortBy = 'Path'			THEN Path		END DESC
            OFFSET @pageSize * (@pageNumber - 1) ROWS FETCH NEXT @pageSize ROWS ONLY

	SELECT RecordedOn, 
        TrackingId, 
        Id,
        Ip,
        Path,
        Method,
		ServerName,
		RequestHeaders,
		RequestContent,
		ResponseHeaders,
		ResponseContent,
		ResponseCode,
		ExecutionTimeMs from #temptable

	SELECT TOP 1 TotalCount AS [TotalCount] FROM #temptable
END


GO


