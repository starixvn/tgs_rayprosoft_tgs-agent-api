CREATE OR REPLACE FUNCTION dbo.brand_get_brand_games_for_cache(brand_id uuid)
RETURNS TABLE(
    "GameProviderId" uuid, 
    "GameId" int,
    "ExternalGameId" varchar, 
    "PlatformType" smallint, 
    "BrandGameStatus" smallint,
    "Tag" smallint)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select 
    	g.game_provider_id as "GameProviderId",
    	g.Id as "GameId",
    	g.external_id as "ExternalGameId",
    	g.platform_type as "PlatformType",
    	bg.status as "BrandGameStatus",
    	bg.tag as "Tag"
    from dbo.brand_games as bg
    join dbo.games as g on bg.game_id = g.id
    where bg.brand_id = $1;
END;
$$