USE [Log]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[usp_AdminLogs_GetLogs]
		@from = '2020-10-22 16:50:00 +08:00',
		@to = '2020-10-22 17:59:00 +08:00',
		@trackingId = NULL,
		@ip = NULL,
		@path = NULL,
		@method = NULL,
		@pageSize = 10,
		@pageNumber = 1,
		@sortBy = N'recordedOn',
		@isDesc = 0
GO