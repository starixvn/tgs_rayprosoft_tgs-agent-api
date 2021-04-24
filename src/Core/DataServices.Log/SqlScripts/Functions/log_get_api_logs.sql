CREATE OR REPLACE FUNCTION dbo.log_get_api_logs(
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
	 "TrackingId" uuid, 
	 "Id" uuid, 
	 "Ip" character varying, 
	 "Path" character varying, 
	 "Method" character varying,
	 "Prefix" character varying,
	 "ServerName" character varying,
	 "ResponseCode" smallint, 
	 "ExecutionTimeMs" integer, 
	 "RequestHeaders" text, 
	 "RequestContent" text, 
	 "ResponseHeaders" text, 
	 "ResponseContent" text,
	 "TotalCount" bigint) 
LANGUAGE plpgsql
AS $$
BEGIN

    RETURN QUERY 
	select
        a.recorded_on as "RecordedOn", 
        a.tracking_id as "TrackingId", 
        a.id as "Id",
        a.ip as "Ip",
        a.path as "Path",
        a.method as "Method",
		a.prefix as "Prefix",
		a.server_name as "ServerName",
		a.response_code as "ResponseCode",
		a.execution_time_ms as "ExecutionTimeMs",
		a.request_headers AS "RequestHeaders",
		a.request_content AS "RequestContent",
		a.response_headers AS "ResponseHeaders",
		a.response_content AS "ResponseContent",
		count(*) over() as "TotalCount"
    from dbo.api_logs a
    where a.recorded_on >= _from        
      and a.recorded_on < _to  
      and (_tracking_id is null or a.tracking_id = _tracking_id)
	  and (_ip is null or a.ip = _ip)
      and ((_path is null or _path = '') or a.path ilike '%' || _path || '%')
      and (_method is null or a.method = _method)    
    order by
			case when _is_desc = false AND _sort_by = 'RecordedOn'	then a.recorded_on	end asc,
			case when _is_desc = false AND _sort_by = 'Method'		then a.method		end asc,
			case when _is_desc = false AND _sort_by = 'Ip'			then a.ip			end asc,
			case when _is_desc = false AND _sort_by = 'Path'		then a.path			end asc,
			case when _is_desc = true AND _sort_by = 'RecordedOn'	then a.recorded_on	end desc,
			case when _is_desc = true AND _sort_by = 'Method'		then a.method		end desc,
			case when _is_desc = true AND _sort_by = 'Ip'			then a.ip			end desc,
			case when _is_desc = true AND _sort_by = 'Path'			then a.path			end desc
    offset (_page_number-1) * _page_size 
	limit _page_size;
END;
$$
