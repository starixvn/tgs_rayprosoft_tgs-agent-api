CREATE OR REPLACE FUNCTION dbo.game_provider_get_sub_game_id_maps(game_provider_id uuid, platform_type smallint)
RETURNS TABLE(
	"Id" int,
	"ExternalId" varchar)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select
        g.id as "Id",
        g.external_id as "ExternalId"
    from dbo.games g 
    where g.game_provider_id = $1 
    and g.platform_type= $2
    and g.sub_game = true;
END;
$$
