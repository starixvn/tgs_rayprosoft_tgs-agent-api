CREATE OR REPLACE FUNCTION dbo.log_get_log_records(
	_from timestamptz, 
	_to timestamptz, 
	_tracking_id UUID, 
	_server_name text,
	_level integer,
	_page_size integer,
	_page_number integer, 
	_sort_by text, 
	_is_desc boolean
	)
RETURNS TABLE (
	 "RecordedOn" timestamptz, 
	 "TrackingId" UUID, 
	 "Id" UUID, 
	 "ServerName" character varying,
	 "Level" integer,
	 "Logger" text,
	 "Message" text,
	 "Exception" text,
	 "TotalCount" bigint) 
AS $$
BEGIN
    RETURN QUERY 
	SELECT
        a.recorded_on as RecordedOn, 
        a.tracking_id as TrackingId, 
        a.id as Id,
		a.server_name as ServerName,
		a.level as Level,
		a.logger as Logger,
		a.message as Message,
		a.exception as Exception,
		count(*) OVER() AS TotalCount
    FROM dbo.log_records a
    WHERE a.recorded_on >= _from        
      AND a.recorded_on < _to  
      AND (_tracking_id IS NULL OR a.tracking_id = _tracking_id)
	  AND (_level IS NULL OR a.level = _level)
      AND ((_server_name IS NULL OR _server_name = '') OR lower(a.server_name) LIKE '%' || lower(_server_name) || '%')
    ORDER BY
			CASE WHEN _is_desc = false AND _sort_by = 'RecordedOn'	THEN recorded_on	END ASC,
			CASE WHEN _is_desc = false AND _sort_by = 'Level'		THEN level			END ASC,
			CASE WHEN _is_desc = true AND _sort_by = 'RecordedOn'	THEN recorded_on	END DESC,
			CASE WHEN _is_desc = true AND _sort_by = 'Level'		THEN level			END DESC
    Offset (_page_number-1) * _page_size 
	Limit _page_size;
END
$$ LANGUAGE plpgsql;
