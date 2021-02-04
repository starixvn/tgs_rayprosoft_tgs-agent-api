CREATE OR REPLACE FUNCTION dbo.game_provider_get_currencies_by_external_code(game_provider_id uuid)
RETURNS TABLE(
 "Key" varchar, 
 "Value" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select distinct
    	c.game_provider_currency_code as "Key",
    	c.currency_code as "Value"    	
    from dbo.game_provider_currencies c
    where c.game_provider_id = $1;
END;
$$
