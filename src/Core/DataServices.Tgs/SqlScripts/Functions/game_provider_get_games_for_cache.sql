CREATE OR REPLACE FUNCTION dbo.game_provider_get_games_for_cache(game_provider_id uuid)
RETURNS TABLE(
 "Id" int, 
 "Code" varchar, 
 "Name" varchar, 
 "Description" varchar, 
 "Status" smallint, 
 "ExternalId" varchar, 
 "SubGame" bool, 
 "RuleUrl" varchar, 
 "EndpointPath" varchar, 
 "Type" smallint, 
 "PlatformType" smallint, 
 "HasTurnoverOnTieBet" bool,
 "AvailableCurrencies" varchar)
LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    select
        g.id as "Id",
        g.code as "Code",
        g."name"  as "Name",
        g.description as "Description",
        g.status as "Status",
        g.external_id as "ExternalId",
        g.sub_game as "SubGame",
        g.rule_url as "RuleUrl",
        g.endpoint_path as "EndpointPath" ,
        g."type" as "Type",
        g.platform_type as "PlatformType",
        g.has_turnover_on_tie_bet as "HasTurnoverOnTieBet",
        g.available_currencies as "AvailableCurrencies"
    from dbo.games g 
    where g.game_provider_id = $1;
END;
$function$
;
