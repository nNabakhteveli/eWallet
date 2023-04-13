using EWallet.Domain.Data;
using EWallet.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class TransactionsController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;

    public TransactionsController(ITransactionsRepository transactionsRepository)
    {
        _transactionsRepository = transactionsRepository;
    }
    
    public async Task<IActionResult> TableView()
    {
        return View(await _transactionsRepository.GetAllAsync());
    }
}