using EBank;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class DepositController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IEBank _bank;

    public DepositController(ITransactionsRepository transactionsRepository,
        IEBank bank)
    {
        _transactionsRepository = transactionsRepository;
        _bank = bank;
    }

    [HttpPost]
    [Route("/Transactions/Api/Deposit")]
    public async Task<JsonResult> Deposit(TransactionEntity transaction)
    {
        transaction.CreateDate = DateTime.Now;

        if (!ModelState.IsValid || transaction.Amount < 0)
        {
            transaction.Status = 2;

            await _transactionsRepository.CreateAsync(transaction);
            return Json(new { Success = false });
        }

        var createdTransaction = await _transactionsRepository.CreateAsync(transaction);


        return Json(new { Success = true, Amount = createdTransaction.Amount, TransactionId = createdTransaction.Id });
    }

    [HttpPost]
    [Route("/Transactions/Api/AcceptDeposit")]
    public async Task<JsonResult> AcceptedDeposit(AppTransaction transaction)
    {
        // imitation of a bank
        var transactionStatus = _bank.ValidateTransfer();

        await _transactionsRepository.AcceptDeposit(transaction.TransactionId, transactionStatus);

        if (transactionStatus == 1) return Json(new { success = true });

        return Json(new { success = false });
    }

    [HttpPost]
    [Route("/Transactions/Api/RejectDeposit")]
    public async Task<JsonResult> RejectDeposit(AppTransaction transaction)
    {
        var oldTransaction = await _transactionsRepository.GetTransactionByIdAsync(transaction.TransactionId);

        oldTransaction.Status = 2;
        await _transactionsRepository.UpdateAsync(oldTransaction);

        return Json(new { success = true });
    }
}