CREATE OR REPLACE FUNCTION dbo.agent_wallet_update_wallet_balance(
	agent_id uuid, 
	wallet_id uuid, 
	amount decimal, 
	updated_on timestamptz)
RETURNS numeric
LANGUAGE plpgsql
AS $$
declare 
	row_count int;
	bal decimal;
BEGIN
	
	set lock_timeout to 3000;
    
    update dbo.agent_wallets w
    set balance = w.balance + $3,
    	updated_on = $4
    where w.id = $2 and w.agent_id = $1
	returning balance into bal;

	get diagnostics row_count = ROW_COUNT;

	if row_count = 0 then
		raise exception 'agent wallet not found.' using ERRCODE = '5000';
	end if;
	
	return bal;
	
END;
$$