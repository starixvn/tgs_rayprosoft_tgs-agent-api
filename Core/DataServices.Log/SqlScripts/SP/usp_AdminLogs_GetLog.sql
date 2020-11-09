SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_AdminLogs_GetLog]
    @trackingId		UNIQUEIDENTIFIER
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
		RequestHeaders,
		RequestContent,
		ResponseHeaders,
		ResponseContent
    FROM dbo.AdminLogs WITH ( NOLOCK )
    WHERE TrackingId = @trackingId

END

GO


