using EBank;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class WithdrawController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IEBank _bank;

    public WithdrawController(ITransactionsRepository transactionsRepository,
        IEBank bank)
    {
        _transactionsRepository = transactionsRepository;
        _bank = bank;
    }

    [HttpPost]
    [Route("/Transactions/Api/Withdraw")]
    public async Task<JsonResult> Withdraw(TransactionEntity transaction)
    {
        try
        {
            transaction.CreateDate = DateTime.Now;
            
            var result = await _transactionsRepository.InitialWithdraw(transaction);
            return Json(new
                { Success = true, Amount = transaction.Amount, TransactionId = result.TransactionId });
        }
        catch (Exception e)
        {
            return Json(new { Success = false });
        }
    }

    [HttpPost]
    [Route("/Transactions/Api/AcceptWithdraw")]
    public async Task<JsonResult> AcceptWithdraw(AppTransaction transaction)
    {
        try
        {
            var transactionStatus = _bank.ValidateTransfer();

            if (transactionStatus == 1)
            {
                await _transactionsRepository.AcceptWithdraw(transaction.TransactionId, transactionStatus);
                return Json(new { Success = true });
            }

            // If transaction got denied by bank
            await _transactionsRepository.RejectWithdraw(transaction.TransactionId);
            return Json(new { Success = false });
        }
        catch (Exception e)
        {
            return Json(new { Success = false });
        }
    }

    [HttpPost]
    [Route("/Transactions/Api/RejectWithdraw")]
    public async Task<JsonResult> RejectWithdraw(AppTransaction transaction)
    {
        await _transactionsRepository.RejectWithdraw(transaction.TransactionId);

        return Json(new { Success = false });
    }
}