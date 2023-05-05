using EWallet.Domain.Data;
using EWallet.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class TransactionsTableController : Controller
{
    private readonly ITransactionsRepository _transactionsRepository;

    public TransactionsTableController(ITransactionsRepository transactionsRepository)
    {
        _transactionsRepository = transactionsRepository;
    }

    public async Task<IActionResult> TableView()
    {
        return View(await _transactionsRepository.GetAllAsync());
    }

    public async Task<JsonResult> GetTransactions() => Json(new { Data = await _transactionsRepository.GetAllAsync() });

    public async Task<JsonResult> FilterInRange(DateInRange dates) => Json(new
        { Data = await _transactionsRepository.FilterInRange(dates.StartDate, dates.EndDate) });
}