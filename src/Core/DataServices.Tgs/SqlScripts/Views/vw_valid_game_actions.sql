CREATE OR REPLACE VIEW bet.vw_valid_game_actions
AS SELECT ga.id,
    ga.external_transaction_id,
    ga.type,
    ga.round_id,
    ga.referenced_game_action_id,
    ga.game_action_batch_id,
    ga.recorded_on,
    ga.external_bet_id,
    ga.sent_on,
    ga.player_ip,
    ga.turnover,
    ga.win_loss,
    ga.unsettled_bets,
    ga.is_closing_round,
    ga.closing_round_explicit_request,
    ga.amount,
    ga.external_game_action_group_id,
    ga.balance,
    ga.bet_type,
    ga.description
FROM bet.game_actions ga
WHERE ga.type <> 5 AND NOT (EXISTS 
        ( SELECT 1
          FROM bet.game_actions ref
          WHERE ref.type = 5 AND ref.referenced_game_action_id = ga.id AND ref.round_id = ga.round_id 
          AND NOT (EXISTS 
                ( SELECT 1 FROM bet.game_actions cancel
                  WHERE cancel.type = 5 AND cancel.referenced_game_action_id = ref.id 
                  AND cancel.round_id = ref.round_id))));