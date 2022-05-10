CREATE OR REPLACE FUNCTION dbo.wallet_update_wallet_balance(
    player_code varchar, 
    wallet_id uuid, 
    amount decimal, 
    updated_on timestamptz)
RETURNS numeric
LANGUAGE plpgsql
AS $$
DECLARE 
    updatedCount int;
    bal decimal;
BEGIN
    
    SET LOCK_TIMEOUT TO 3000;
    
    UPDATE dbo.wallets w
    SET 
        balance = w.balance + $3,
        updated_on = $4
    WHERE 
        w.id = $2 
        AND 
        w.player_code = $1
    RETURNING balance INTO bal;

    GET diagnostics updatedCount = ROW_COUNT;

    IF updatedCount = 0 THEN
        RAISE EXCEPTION 'wallet not found.' USING ERRCODE = '5000';
    END IF;
    
    RETURN bal;
    
END;
$$