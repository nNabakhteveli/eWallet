using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using BetsolutionsApi.Models.Transfers;

namespace BetsolutionsApi.Controllers;

[Route("v1/Wallet")]
[ApiController]
public class TransferController : ControllerBase
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionsRepository _transactionsRepository;

    public TransferController(ITokenRepository tokenRepository, IWalletRepository walletRepository,
        ITransactionsRepository transactionsRepository)
    {
        _tokenRepository = tokenRepository;
        _walletRepository = walletRepository;
        _transactionsRepository = transactionsRepository;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(TransferRequest req)
    {
        var rawHash = $"{req.Amount}|{req.Currency}|{req.MerchantId}|{req.TransactionId}|{req.Token}|{req.UserId}|{req.Key}";
        if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, new { StatusCode = 403 });

        var userWallet = await _walletRepository.GetWalletByUserIdAsync(req.UserId);
        var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
        var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);
        
        if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

        var newTransaction = new TransactionEntity
        {
            UserId = userWallet.UserId,
            PaymentType = "Deposit",
            Amount = (decimal)req.Amount,
            Currency = req.Currency,
            CreateDate = DateTime.Now,
            Status = 1
        };

        var newCalculatedBalance = userWallet.CurrentBalance + req.Amount;
        userWallet.CurrentBalance = (decimal)newCalculatedBalance;

        try
        {
            await _walletRepository.UpdateWalletAsync(userWallet);

            var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

            return StatusCode(statusCode,
                ApiHelper.GenerateTransferResponse(statusCode, (decimal)newCalculatedBalance, createdTransaction.Id));
        }
        catch (Exception)
        {
            statusCode = 500;
            return StatusCode(statusCode, new { statusCode });
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(TransferRequest req)
    {
        var rawHash = $"{req.Amount}|{req.Currency}|{req.MerchantId}|{req.TransactionId}|{req.Token}|{req.UserId}|{req.Key}";
        if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, new { StatusCode = 403 });
        
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(req.UserId);
        var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
        var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

        if (userWallet.CurrentBalance < req.Amount) statusCode = 407;
        if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

        var newTransaction = new TransactionEntity
        {
            UserId = userWallet.UserId,
            PaymentType = "Withdraw",
            Amount = (decimal)req.Amount,
            Currency = req.Currency,
            CreateDate = DateTime.Now,
            Status = 1
        };

        var newCalculatedBalance = userWallet.CurrentBalance - req.Amount;
        userWallet.CurrentBalance = (decimal)newCalculatedBalance;

        try
        {
            await _walletRepository.UpdateWalletAsync(userWallet);
            var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

            return StatusCode(statusCode,
                ApiHelper.GenerateTransferResponse(statusCode, (decimal)newCalculatedBalance, createdTransaction.Id));
        }
        catch (Exception)
        {
            statusCode = 500;
            return StatusCode(statusCode, new { statusCode });
        }
    }

    [HttpPost("GetBalance")]
    public async Task<IActionResult> GetBalance(TransferRequest req)
    {
        var rawHash = $"{req.Currency}|{req.MerchantId}|{req.Token}|{req.UserId}|{req.Key}";
        if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, new { StatusCode = 403 });
        
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(req.UserId);
        var userToken = await _tokenRepository.GetByPrivateToken(req.Token);

        var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

        return statusCode != 200
            ? StatusCode(statusCode, new { statusCode })
            : StatusCode(StatusCodes.Status200OK,
                ApiHelper.GenerateTransferResponse(statusCode, balance: userWallet.CurrentBalance));
    }
}