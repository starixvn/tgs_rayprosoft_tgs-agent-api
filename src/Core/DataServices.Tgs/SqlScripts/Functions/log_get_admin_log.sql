CREATE OR REPLACE FUNCTION log.log_get_admin_log(tracking_id uuid)
RETURNS TABLE(
	 "RequestHeaders" text, 
	 "RequestContent" text, 
	 "ResponseHeaders" text, 
	 "ResponseContent" text)
LANGUAGE plpgsql
AS $$
BEGIN

    RETURN QUERY
    select
		a.request_headers AS "RequestHeaders",
		a.request_content AS "RequestContent",
		a.response_headers AS "ResponseHeaders",
		a.response_content AS "ResponseContent"
    from log.admin_logs a
    where a.tracking_id = $1;

END;
$$


