using EWallet.Domain.Models;
using EWallet.Domain.Data;

namespace EBank;

public class EBank : IEBank
{
    public int ValidateTransfer()
    {
        var random = new Random();
        return random.Next(1, 3);
    }
}