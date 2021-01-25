CREATE OR REPLACE FUNCTION dbo.localization_get_localization_by_game_provider_id(game_provider_id uuid)
RETURNS TABLE(
    "Context" varchar, 
    "Key" varchar, 
    "CultureCode" varchar, 
    "Transalation" varchar)
LANGUAGE plpgsql
AS $$
begin

	drop table if exists contexts cascade;
    
	create temp table contexts as 
	select id::varchar as "value" 
	from dbo.games g
    where g.game_provider_id = $1
    union
	select $1::varchar;

	return QUERY
    select distinct 
        ls.context as "Context",
        ls."key" as "Key",
        ls.culture_code as "CultureCode",
        ls."translation" as "Transalation"
    from dbo.localized_strings ls 
    join contexts ct on ls."key" = ct."value";
  
END;
$$
;
