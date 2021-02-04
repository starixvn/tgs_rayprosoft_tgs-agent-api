CREATE OR REPLACE FUNCTION dbo.player_get_player_for_cache(player_code varchar)
RETURNS TABLE(
 "BrandId" uuid, 
 "Type" smallint, 
 "Username" varchar, 
 "Status" smallint, 
 "DefaultWalletId" uuid, 
 "BetLimitId" int, 
 "PlayerCode" varchar, 
 "BrandCode" varchar, 
 "DefaultWalletCurrencyCode" varchar, 
 "ExternalCurrencyCode" varchar)
LANGUAGE plpgsql
AS $$
begin

	RETURN QUERY	
	with player as
	(
		select 			
			p.brand_id,
			p."type",
			p.username ,
			p.status ,
			p.default_wallet_id ,
			p.bet_limit_id ,
			p.player_code 			
		from dbo.players p
		where p.player_code = $1
	)
	select
		p.brand_id as "BrandId",
		p."type" as "Type",
		p.username as "Username",
		p.status as "Status",
		p.default_wallet_id as "DefaultWalletId" ,
		p.bet_limit_id as "BetLimitId" ,
		p.player_code as "PlayerCode", 		
		b.code as "BrandCode",
		w.currency_code AS "DefaultWalletCurrencyCode",
		w.external_currency_code as "ExternalCurrencyCode"
	from player p
	join dbo.brands b on  b.id = p.brand_id
	join dbo.wallets w on w.id = p.default_wallet_id;
  
END;
$$
