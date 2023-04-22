using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetsolutionsApi.Models.Seamless;
using BetsolutionsApi.Models.Transfers;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BetsolutionsApi.Controllers
{
    [Route("v1/Seamless")]
    [ApiController]
    public class SeamlessController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionsRepository _transactionsRepository;

        public SeamlessController(ITokenRepository tokenRepository, IWalletRepository walletRepository,
            ITransactionsRepository transactionsRepository)
        {
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _transactionsRepository = transactionsRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Bet(BetRequest betReq)
        {
            var userToken = await _tokenRepository.GetByUserToken(betReq.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);

            if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

            if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
                userToken.PrivateToken != betReq.Token)
                return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

            if (userWallet.CurrentBalance < betReq.Amount)
                return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });

            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Bet",
                Amount = betReq.Amount,
                Currency = betReq.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };
            
            userWallet.CurrentBalance -= betReq.Amount;
            await _walletRepository.UpdateWalletAsync(userWallet);
            var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

            var responseData = new TransferResponse
            {
                StatusCode = 200,
                Data = new TransferResponseData
                {
                    TransactionId = createdTransaction.Id,
                    Balance = userWallet.CurrentBalance
                }
            };

            return StatusCode(StatusCodes.Status200OK, responseData);
        }

        [HttpPost]
        public async Task<IActionResult> Win(WinRequest winReq)
        {
            var userToken = await _tokenRepository.GetByUserToken(winReq.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);

            if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

            if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
                userToken.PrivateToken != winReq.Token)
                return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

            if (userWallet.CurrentBalance < winReq.Amount)
                return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });

            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Win",
                Amount = winReq.Amount,
                Currency = winReq.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            userWallet.CurrentBalance += winReq.Amount;
            await _walletRepository.UpdateWalletAsync(userWallet);
            var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

            var responseData = new TransferResponse
            {
                StatusCode = 200,
                Data = new TransferResponseData
                {
                    TransactionId = createdTransaction.Id,
                    Balance = userWallet.CurrentBalance
                }
            };

            return StatusCode(StatusCodes.Status200OK, responseData);
        }
        
        [HttpPost]
        public async Task<IActionResult> CancelBet(CancelBet req)
        {
            var userToken = await _tokenRepository.GetByUserToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);

            if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

            if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
                userToken.PrivateToken != req.Token)
                return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

            if (userWallet.CurrentBalance < req.Amount)
                return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });

            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Cancel Bet",
                Amount = req.Amount,
                Currency = req.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            userWallet.CurrentBalance += req.Amount;
            await _walletRepository.UpdateWalletAsync(userWallet);
            var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

            var responseData = new TransferResponse
            {
                StatusCode = 200,
                Data = new TransferResponseData
                {
                    TransactionId = createdTransaction.Id,
                    Balance = userWallet.CurrentBalance
                }
            };

            return StatusCode(StatusCodes.Status200OK, responseData);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeWin(ChangeWin req)
        {
            var userToken = await _tokenRepository.GetByUserToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);

            if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

            if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
                userToken.PrivateToken != req.Token)
                return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

            if (userWallet.CurrentBalance < req.Amount)
                return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });

            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Change Win",
                Amount = req.Amount,
                Currency = req.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

            var responseData = new TransferResponse
            {
                StatusCode = 200,
                Data = new TransferResponseData
                {
                    TransactionId = createdTransaction.Id,
                    Balance = userWallet.CurrentBalance
                }
            };

            return StatusCode(StatusCodes.Status200OK, responseData);
        }
        
        [HttpPost]
        public async Task<IActionResult> GetBalance(ChangeWin req)
        {
            var userToken = await _tokenRepository.GetByUserToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);

            if (userToken == null) return StatusCode(StatusCodes.Status406NotAcceptable, new { StatusCode = 406 });

            if (userToken.PrivateTokenStatus != "active" || userToken.PublicTokenStatus != "active" ||
                userToken.PrivateToken != req.Token)
                return StatusCode(StatusCodes.Status401Unauthorized, new { StatusCode = 401 });

            if (userWallet.CurrentBalance < req.Amount)
                return StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, new { StatusCode = 407 });
            
            var responseData = new TransferResponse
            {
                StatusCode = 200,
                Data = new TransferResponseData
                {
                    CurrentBalance = userWallet.CurrentBalance
                }
            };

            return StatusCode(StatusCodes.Status200OK, responseData);
        }
    }
}