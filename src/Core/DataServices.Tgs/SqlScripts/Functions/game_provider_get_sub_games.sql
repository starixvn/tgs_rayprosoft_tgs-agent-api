CREATE OR REPLACE FUNCTION dbo.game_provider_get_sub_games(game_provider_id uuid)
RETURNS TABLE(
	"Id" int,
	"ExternalId" varchar,
    "PlatformType" smallint)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select
        g.id as "Id",
        g.external_id as "ExternalId",
        g.platform_type as "PlatformType"
    from dbo.games g 
    where g.game_provider_id = $1
    and g.sub_game = true;
END;
$$
