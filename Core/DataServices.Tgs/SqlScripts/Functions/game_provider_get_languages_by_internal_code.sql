CREATE OR REPLACE FUNCTION dbo.game_provider_get_languages_by_internal_code(game_provider_id uuid)
RETURNS TABLE(
 "Key" varchar, 
 "Value" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select distinct
        l.culture_code as "Key",
    	l.game_provider_culture_code as "Value"
    from dbo.game_provider_languages l
    where l.game_provider_id = $1;
END;
$$
