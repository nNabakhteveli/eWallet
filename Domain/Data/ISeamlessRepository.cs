using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface ISeamlessRepository
{
    BetResult Bet(SeamlessBetRequest req); 
    BetResult Win(SeamlessBetRequest req);
    BetResult CancelBet(SeamlessCancelBetRequest req);
    BetResult ChangeWin(SeamlessCancelBetRequest req);
    BetResult GetBalance(SeamlessBetRequest req);
}