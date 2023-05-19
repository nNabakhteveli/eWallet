using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ISeamlessRepository
{
    Task<BetResult> Bet(SeamlessBetRequest req);
    Task<BetResult> Win(SeamlessBetRequest req);
    Task<BetResult> CancelBet(SeamlessCancelBetRequest req);
    Task<BetResult> ChangeWin(SeamlessCancelBetRequest req);
    Task<BetResult> GetBalance(SeamlessBetRequest req);
}