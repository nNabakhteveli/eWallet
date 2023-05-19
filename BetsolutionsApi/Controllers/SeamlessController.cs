using BetsolutionsApi.Models.Seamless;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
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
        private readonly ISeamlessRepository _seamlessRepository;

        public SeamlessController(ITokenRepository tokenRepository, IWalletRepository walletRepository,
            ITransactionsRepository transactionsRepository, ISeamlessRepository seamlessRepository)
        {
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _transactionsRepository = transactionsRepository;
            _seamlessRepository = seamlessRepository;
        }

        [HttpPost("Bet")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Bet([FromForm] BetRequest req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.BetTypeId}|{req.CampaignId}|{req.CampaignName}|{req.Currency}|{req.GameId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (userWallet.CurrentBalance < req.Amount) statusCode = 407;
            if (statusCode != 200) return StatusCode(statusCode, CustomHttpResponses.InvalidAmount407);

            // REFACTOR: REMOVE THIS CLASS DEFINITION AND USE req DIRECTLY
            var request = new SeamlessBetRequest
            {
                Token = req.Token,
                Amount = req.Amount,
                Currency = req.Currency,
                PaymentType = "Bet",
                CreateDate = DateTime.Now,
                Status = 1
            };
            
            var result = await _seamlessRepository.Bet(request);
            
            if (result.TransactionId != 0)
            {
                return StatusCode(statusCode,
                ApiHelper.GenerateTransferResponse(statusCode, result.CurrentAmount, result.TransactionId));
            }
            
            return StatusCode(500, CustomHttpResponses.GeneralError500);
        }

        [HttpPost("Win")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Win([FromForm] WinRequest req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.CampaignId}|{req.CampaignName}|{req.Currency}|{req.GameId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (statusCode != 200) return StatusCode(statusCode, new { statusCode });
            
            var newTransaction = new SeamlessBetRequest
            {
                Token = req.Token,
                Amount = req.Amount,
                Currency = req.Currency,
                PaymentType = "Win",
                CreateDate = DateTime.Now,
                Status = 1
            };

            var result = await _seamlessRepository.Win(newTransaction);
            
            if (result.TransactionId != 0)
            {
                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, result.CurrentAmount, result.TransactionId));
            }
            
            return StatusCode(500, CustomHttpResponses.GeneralError500);
        }

        [HttpPost("CancelBet")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> CancelBet([FromForm] CancelBet req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.BetTransactionId}|{req.BetTypeId}|{req.Currency}|{req.GameId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

            // var oldTransaction = await _transactionsRepository.GetTransactionByIdAsync(req.BetTransactionId);
            
            // var newTransaction = new TransactionEntity
            // {
            //     UserId = userWallet.UserId,
            //     PaymentType = "Cancel Bet",
            //     Amount = req.Amount,
            //     Currency = req.Currency,
            //     CreateDate = DateTime.Now,
            //     Status = 1
            // };
            
            var newTransaction = new SeamlessCancelBetRequest
            {
                Token = req.Token,
                Amount = req.Amount,
                Currency = req.Currency,
                PaymentType = "Cancel Bet",
                CreateDate = DateTime.Now,
                OldTransactionId = req.BetTransactionId,
                Status = 1
            };

            var result = await _seamlessRepository.CancelBet(newTransaction);
            
            if (result.TransactionId != 0)
            {
                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, result.CurrentAmount, result.TransactionId));
            }
            
            return StatusCode(500, CustomHttpResponses.GeneralError500);
        }

        [HttpPost("ChangeWin")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ChangeWin([FromForm] ChangeWin req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.ChangeWinTypeId}|{req.Currency}|{req.GameId}|{req.PreviousAmount}|{req.previousTransactionId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

            var oldTransaction = await _transactionsRepository.GetTransactionByIdAsync(req.PreviousTransactionId);
            
            // var newTransaction = new TransactionEntity
            // {
            //     UserId = userWallet.UserId,
            //     PaymentType = "Change Win",
            //     Amount = req.Amount,
            //     Currency = req.Currency,
            //     CreateDate = DateTime.Now,
            //     Status = 1
            // };

            var newTransaction = new SeamlessCancelBetRequest
            {
                Token = req.Token,
                Amount = req.Amount,
                Currency = req.Currency,
                PaymentType = "Change win",
                CreateDate = DateTime.Now,
                OldTransactionId = req.PreviousTransactionId,
                Status = 1
            };

            var result = await _seamlessRepository.ChangeWin(newTransaction);
            
            if (result.TransactionId != 0)
            {
                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, result.CurrentAmount, result.TransactionId));
            }
            
            return StatusCode(500, CustomHttpResponses.GeneralError500);
        }

        [HttpPost("GetBalance")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetBalance([FromForm] ChangeWin req)
        {
            // var rawHash = $"{req.Currency}|{req.GameId}|{req.ProductId}|{req.MerchantToken}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);
            
            var request = new SeamlessBetRequest
            {
                Token = req.Token 
            };
            
            var result = await _seamlessRepository.GetBalance(request);
            
            
            if (result.TransactionId == 0)
            {
                return StatusCode(200,
                    ApiHelper.GenerateTransferResponse(200, result.CurrentAmount, result.TransactionId));
            }
            
            return StatusCode(500, CustomHttpResponses.GeneralError500);
        }
    }
}