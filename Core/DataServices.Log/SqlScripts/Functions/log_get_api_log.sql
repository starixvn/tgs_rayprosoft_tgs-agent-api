CREATE OR REPLACE FUNCTION dbo.log_get_api_log(_tracking_id uuid)
RETURNS TABLE(
	 "RequestHeaders" text, 
	 "RequestContent" text, 
	 "ResponseHeaders" text, 
	 "ResponseContent" text)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
		a.request_headers AS "RequestHeaders",
		a.request_content AS "RequestContent",
		a.response_headers AS "ResponseHeaders",
		a.response_content AS "ResponseContent"
    FROM dbo.api_logs a
    WHERE a.tracking_id = $1;

END
$$

