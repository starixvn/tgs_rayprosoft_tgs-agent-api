CREATE OR REPLACE FUNCTION dbo.promotion_update_amount_used(
    _PROMOTION_ID UUID,
    _AMOUNT decimal)
RETURNS boolean
LANGUAGE PLPGSQL AS $$
declare
    row_count int;
    _old_amount decimal;
    _new_amount decimal;
    _total_amount_limit decimal;
    _unlimited boolean;
BEGIN
    select p.total_amount_limit,p.unlimited into _total_amount_limit, _unlimited from dbo.promotions p where p.id = _promotion_id;

    set lock_timeout to 3000;

    update dbo.promotions n
    set amount_used = o.amount_used + _amount
    from dbo.promotions o
    where n.id = o.id and n.id = _promotion_id
    returning o.amount_used into _old_amount;

    get diagnostics row_count = ROW_COUNT;

    if row_count = 0 then
        raise exception 'promotion not found.' using ERRCODE = '5000';
    end if;

    if _old_amount >= _total_amount_limit AND _unlimited = false then
        ROLLBACK;
    else 
        COMMIT;
    end if;

    return (_old_amount < _total_amount_limit);
END;
$$