using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ISeamlessRepository
{
    BetResult Bet(SeamlessBetRequest req); 
    BetResult Win(SeamlessBetRequest req);
    Task<BetResult> CancelBet(SeamlessCancelBetRequest req);
    Task<BetResult> ChangeWin(SeamlessCancelBetRequest req);
    Task<BetResult> GetBalance(SeamlessBetRequest req);
}