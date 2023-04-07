using EWallet.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class MoneyTransferController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IConfiguration _configuration;
    private string errorPageUrl = "/InvalidRequest";

    public MoneyTransferController(IConfiguration configuration, ITransactionsRepository transactionsRepository, IWalletRepository walletRepository)
    {
        _transactionsRepository = transactionsRepository;
        _walletRepository = walletRepository;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("/Transactions/Deposit")]
    public async Task Deposit(TransactionEntity transaction)
    {
        var bank = new EBank.EBank(_configuration.GetConnectionString("DefaultConnection"));
        if (ModelState.IsValid)
        {
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);
            
            transaction.Status = 0;
    
            var createdtransaction = await _transactionsRepository.CreateAsync(transaction);
    
            userWallet.CurrentBalance -= transaction.Amount;
    
            var recipientWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.RecipientId);
            recipientWallet.CurrentBalance += transaction.Amount;
    
            await _walletRepository.UpdateWalletAsync(userWallet);
            await _walletRepository.UpdateWalletAsync(recipientWallet);
            Redirect("/Identity/Account/Wallet");
        }
    
        transaction.Status = 2;
    
        await _transactionsRepository.CreateAsync(transaction);
        Redirect(errorPageUrl);
        
        bank.ProcessDeposit(1, 1, "");
    }
    // public async Task<IActionResult> Deposit(TransactionEntity transaction)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         var userWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.UserId);
    //
    //         if (userWallet.CurrentBalance < transaction.Amount)
    //         {
    //             transaction.Status = 2;
    //             await _transactionsRepository.CreateAsync(transaction);
    //
    //             return Redirect(errorPageUrl);
    //         }
    //
    //         transaction.Status = 1;
    //
    //         var createdtransaction = await _transactionsRepository.CreateAsync(transaction);
    //
    //         userWallet.CurrentBalance -= transaction.Amount;
    //
    //         var recipientWallet = await _walletRepository.GetWalletByUserIdAsync(transaction.RecipientId);
    //         recipientWallet.CurrentBalance += transaction.Amount;
    //
    //         await _walletRepository.UpdateWalletAsync(userWallet);
    //         await _walletRepository.UpdateWalletAsync(recipientWallet);
    //         return Redirect("/Identity/Account/Wallet");
    //     }
    //
    //     transaction.Status = 2;
    //
    //     await _transactionsRepository.CreateAsync(transaction);
    //     return Redirect(errorPageUrl);
    // }

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