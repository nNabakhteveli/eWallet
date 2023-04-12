using EBank;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EWallet.Controllers;

public class MoneyTransferController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IEBank _bank;
    
    public MoneyTransferController(ITransactionsRepository transactionsRepository, IWalletRepository walletRepository,
        IEBank bank)
    {
        _transactionsRepository = transactionsRepository;
        _walletRepository = walletRepository;
        _bank = bank;
    }

    [HttpPost]
    [Route("/Transactions/Api/Deposit")]
    public async Task<JsonResult> Deposit(Deposit transaction)
    {
        transaction.CreateDate = DateTime.Now;

        if (transaction.RecipientId.IsNullOrEmpty())
            ModelState.AddModelError("RecipientId", "RecipientId must not be null or empty");

        if (ModelState.IsValid)
        {
            var createdTransaction = await _transactionsRepository.CreateAsync(transaction);

            // imitation of a bank
            var transactionStatus = await _bank.ProcessDeposit(transaction, createdTransaction);
            createdTransaction.Status = transactionStatus;

            await _transactionsRepository.UpdateAsync(createdTransaction);

            return Json(new { success = true });
        }

        transaction.Status = 2;

        await _transactionsRepository.CreateAsync(transaction);
        return Json(new { success = false });
    }


    [HttpPost]
    [Route("/Transactions/Api/Withdraw")]
    // MUST RETURN JSON
    public async Task<JsonResult> Withdraw(TransactionEntity transaction)
    {
        transaction.CreateDate = DateTime.Now;
        
        if (ModelState.IsValid)
        {
            var createdTransaction = await _transactionsRepository.CreateAsync(transaction);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);

            userWallet.CurrentBalance -= transaction.Amount;

            await _walletRepository.UpdateWalletAsync(userWallet);

            var transactionStatus = await _bank.ProcessWithdraw(transaction, createdTransaction);
            createdTransaction.Status = transactionStatus;

            await _transactionsRepository.UpdateAsync(createdTransaction);

            return Json(new { success = true });
        }

        transaction.Status = 2;

        await _transactionsRepository.CreateAsync(transaction);
        return Json(new { success = false });
    }
}