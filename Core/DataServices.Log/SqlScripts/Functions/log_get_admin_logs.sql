CREATE OR REPLACE FUNCTION dbo.log_get_admin_logs(
	_from timestamptz, 
	_to timestamptz, 
	_tracking_id UUID, 
	_ip text,
	_path text,
	_method text, 
	_page_size integer,
	_page_number integer, 
	_sort_by text, 
	_is_desc boolean
	)
RETURNS TABLE (
	 "RecordedOn" timestamptz, 
	 "TrackingId" UUID, 
	 "Id" UUID, 
	 "Ip" character varying, 
	 "Path" character varying, 
	 "Method" character varying,
	 "Description" character varying,
	 "Exception" text,
	 "StackTrace" text,	 
	 "ServerName" character varying,
	 "ResponseCode" smallint, 
	 "ExecutionTimeMs" integer, 
	 "TotalCount" bigint) 
AS $$
BEGIN
    RETURN QUERY 
	SELECT
        a.recorded_on as RecordedOn, 
        a.tracking_id as TrackingId, 
        a.id as Id,
        a.ip as Ip,
        a.path as Path,
        a.method as Method,
        a.description as Description,
		a.exception as Exception,
		a.stack_trace as StackTrace,
		a.server_name as ServerName,
		a.response_code as ResponseCode,
		a.execution_time_ms as ExecutionTimeMs,
		count(*) OVER() AS TotalCount
    FROM dbo.admin_logs a
    WHERE a.recorded_on >= _from        
      AND a.recorded_on < _to  
      AND (_tracking_id IS NULL OR a.tracking_id = _tracking_id)
	  AND (_ip IS NULL OR a.ip = _ip)
      AND ((_path IS NULL OR _path = '') OR lower(a.path) LIKE '%' || lower(_path) || '%')
      AND (_method IS NULL OR a.method = _method)    
    ORDER BY
			CASE WHEN _is_desc = false AND _sort_by = 'RecordedOn'	THEN recorded_on	END ASC,
			CASE WHEN _is_desc = false AND _sort_by = 'Method'		THEN method			END ASC,
			CASE WHEN _is_desc = false AND _sort_by = 'Ip'			THEN ip				END ASC,
			CASE WHEN _is_desc = false AND _sort_by = 'Path'			THEN path			END ASC,
			CASE WHEN _is_desc = true AND _sort_by = 'RecordedOn'	THEN recorded_on	END DESC,
			CASE WHEN _is_desc = true AND _sort_by = 'Method'		THEN method			END DESC,
			CASE WHEN _is_desc = true AND _sort_by = 'Ip'			THEN ip				END DESC,
			CASE WHEN _is_desc = true AND _sort_by = 'Path'			THEN path			END DESC
    Offset (_page_number-1) * _page_size 
	Limit _page_size;
END
$$ LANGUAGE plpgsql;


