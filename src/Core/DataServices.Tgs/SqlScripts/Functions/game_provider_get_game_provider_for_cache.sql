CREATE OR REPLACE FUNCTION dbo.game_provider_get_game_provider_for_cache(game_provider_id uuid)
RETURNS TABLE(
	"Id" uuid,
	"Code" varchar,
	"Name" varchar,
	"Status" smallint,
	"Description" varchar,
	"AutoCloseRound" bool,
	"SupportTestPlayer" bool,
	"PlaceBetTimeout" smallint,
	"SettleBetTimeout" smallint,
	"AllowKickPlayer" bool,
    "IsTransferWallet" bool)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    select
        gp.id as "Id",
        gp.code as "Code",
        gp."name"  as "Name",
        gp.status as "Status",
        gp.description as "Description",
        gp.auto_close_round as "AutoCloseRound",
        gp.support_test_player as "SupportTestPlayer",
        gp.place_bet_timeout as "PlaceBetTimeout",
        gp.settle_bet_timeout as "SettleBetTimeout",
        gp.allow_kick_player as "AllowKickPlayer",
        gp.transfer_wallet as "IsTransferWallet"
    from dbo.game_providers gp
    where gp.id = $1;
END;
$$
