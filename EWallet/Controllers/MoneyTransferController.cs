using EWallet.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class MoneyTransferController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IWalletRepository _walletRepository;
    private string errorPageUrl = "/InvalidRequest";

    public MoneyTransferController(ITransactionsRepository transactionsRepository, IWalletRepository walletRepository)
    {
        _transactionsRepository = transactionsRepository;
        _walletRepository = walletRepository;
    }

    [HttpPost]
    [Route("/Transactions/Deposit")]
    public async Task<IActionResult> Deposit(TransactionEntity transaction)
    {
        if (ModelState.IsValid)
        {
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);

            if (userWallet.CurrentBalance < transaction.Amount)
            {
                transaction.Status = 2;
                await _transactionsRepository.CreateAsync(transaction);

                return Redirect(errorPageUrl);
            }

            transaction.Status = 1;

            var createdtransaction = await _transactionsRepository.CreateAsync(transaction);

            userWallet.CurrentBalance -= transaction.Amount;

            var recipientWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.RecipientId);
            recipientWallet.CurrentBalance += transaction.Amount;

            await _walletRepository.UpdateWalletAsync(userWallet);
            await _walletRepository.UpdateWalletAsync(recipientWallet);
            return Redirect("/Identity/Account/Wallet");
        }

        transaction.Status = 2;

        await _transactionsRepository.CreateAsync(transaction);
        return Redirect(errorPageUrl);
    }

    [HttpPost]
    [Route("/Transactions/Withdraw")]
    public async Task<IActionResult> Withdraw(TransactionEntity transaction)
    {
        if (ModelState.IsValid)
        {
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);

            if (userWallet.CurrentBalance < transaction.Amount)
            {
                transaction.Status = 2;
                await _transactionsRepository.CreateAsync(transaction);

                return Redirect(errorPageUrl);
            }

            transaction.Status = 1;

            await _transactionsRepository.CreateAsync(transaction);

            userWallet.CurrentBalance -= transaction.Amount;

            await _walletRepository.UpdateWalletAsync(userWallet);
            return Redirect("/Identity/Account/Wallet");
        }

        transaction.Status = 2;

        await _transactionsRepository.CreateAsync(transaction);
        return Redirect(errorPageUrl);
    }
}