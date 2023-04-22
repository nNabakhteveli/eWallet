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

    public TransferController(ITokenRepository tokenRepository, IWalletRepository walletRepository, ITransactionsRepository transactionsRepository)
    {
        _tokenRepository = tokenRepository;
        _walletRepository = walletRepository;
        _transactionsRepository = transactionsRepository;
    }

    [HttpPost]
    [Route("deposit")]
    public async Task<IActionResult> Deposit(TransferRequest deposit)
    {
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(deposit.UserId);
        var userToken = await _tokenRepository.GetByUserIdAsync(deposit.UserId);

        if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });
        
        if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" || userToken.PrivateToken != deposit.Token)
            return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

        if (userWallet.CurrentBalance < deposit.Amount)
            return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });


        var newTransaction = new TransactionEntity
        {
            UserId = userWallet.UserId,
            PaymentType = "Deposit",
            Amount = (decimal) deposit.Amount,
            Currency = deposit.Currency,
            CreateDate = DateTime.Now,
            Status = 1            
        };
        var newCalculatedBalance = userWallet.CurrentBalance + deposit.Amount;
        
        userWallet.CurrentBalance = (decimal) newCalculatedBalance;
        await _walletRepository.UpdateWalletAsync(userWallet);
        var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

        var responseData = new TransferResponse
        {
            StatusCode = 200,
            Data = new TransferResponseData
            {
                TransactionId = createdTransaction.Id,
                Balance = (decimal) newCalculatedBalance
            }
        };
        
        return StatusCode(StatusCodes.Status200OK, responseData);
    }
    
    
    [HttpPost]
    [Route("withdraw")]
    public async Task<IActionResult> Withdraw(TransferRequest withdraw)
    {
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(withdraw.UserId);
        var userToken = await _tokenRepository.GetByUserIdAsync(withdraw.UserId);

        if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

        if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" || userToken.PrivateToken != withdraw.Token)
            return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });
        
        if (userWallet.CurrentBalance < withdraw.Amount)
            return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });
        
        var newTransaction = new TransactionEntity
        {
            UserId = userWallet.UserId,
            PaymentType = "Withdraw",
            Amount = (decimal) withdraw.Amount,
            Currency = withdraw.Currency,
            CreateDate = DateTime.Now,
            Status = 1            
        };
        var newCalculatedBalance = userWallet.CurrentBalance - withdraw.Amount;
        
        userWallet.CurrentBalance = (decimal) newCalculatedBalance;
        await _walletRepository.UpdateWalletAsync(userWallet);
        var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);
        
        var responseData = new TransferResponse()
        {
            StatusCode = 200,
            Data = new TransferResponseData()
            {
                TransactionId = createdTransaction.Id,
                Balance = (decimal) newCalculatedBalance
            }
        };
        
        return StatusCode(StatusCodes.Status200OK, responseData);
    }
    
    [HttpPost]
    [Route("GetBalance")]
    public async Task<IActionResult> GetBalance(TransferRequest transferReq)
    {
        var userWallet = await _walletRepository.GetWalletByUserIdAsync(transferReq.UserId);
        var userToken = await _tokenRepository.GetByUserIdAsync(transferReq.UserId);

        if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

        if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" || userToken.PrivateToken != transferReq.Token)
            return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

        var responseData = new TransferResponse()
        {
            StatusCode = 200,
            Data = new TransferResponseData()
            {
                Balance = userWallet.CurrentBalance
            }
        };
        
        return StatusCode(StatusCodes.Status200OK, responseData);
    }
}