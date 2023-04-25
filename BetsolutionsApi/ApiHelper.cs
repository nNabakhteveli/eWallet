using System.Security.Cryptography;
using System.Text;
using BetsolutionsApi.Models.Seamless;
using BetsolutionsApi.Models.Transfers;
using EWallet.Domain.Data;
using EWallet.Domain.Models;

namespace BetsolutionsApi;

public class ApiHelper
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionsRepository _transactionsRepository;

    public static string GetSha256(string text)
    {
        var utf8Encoding = new UTF8Encoding();
        var message = utf8Encoding.GetBytes(text);

        var hashString = new SHA256Managed();
        var hex = string.Empty;

        var hashValue = hashString.ComputeHash(message);

        return hashValue.Aggregate(hex, (current, bt) => current + $"{bt:x2}");
    }

    public static TransferResponse GenerateTransferResponse(int statusCode, decimal balance, int transactionId = 0)
    {
        return new TransferResponse
        {
            StatusCode = statusCode,
            Data = new TransferResponseData
            {
                TransactionId = transactionId,
                Balance = balance
            }
        };
    }

    public static int DetermineRequestStatusCode(TransferRequest req, TokenEntity userToken, WalletEntity userWallet)
    {
        if (userToken == null) return 406;

        if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
            userToken.PrivateToken != req.Token) return 401;

        return 200;
    }

    public static int DetermineRequestStatusCode(SeamlessBaseRequest req, TokenEntity userToken,
        WalletEntity userWallet)
    {
        if (userToken == null) return 406;

        if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
            userToken.PrivateToken != req.Token) return 401;

        return 200;
    }
}