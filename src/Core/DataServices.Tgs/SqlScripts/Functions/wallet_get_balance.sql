CREATE OR REPLACE FUNCTION dbo.wallet_get_balance(wallet_id uuid)
RETURNS decimal
LANGUAGE plpgsql
AS $$
declare
	bal decimal;
begin

	select w.balance into bal
	from dbo.wallets w
	where w.id = $1;

	return bal;

END;
$$
;
