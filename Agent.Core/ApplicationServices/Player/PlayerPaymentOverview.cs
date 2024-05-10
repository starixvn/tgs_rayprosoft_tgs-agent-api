namespace TGS.Services.Agent.ApplicationServices.Player;

public record PlayerPaymentOverview(
    string PlayerCode,
    decimal TotalDepositAmount,
    decimal TotalWithdrawAmount);