using BetsolutionsApi.Models.Seamless;
using BetsolutionsApi.Models.Transfers;
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

        public SeamlessController(ITokenRepository tokenRepository, IWalletRepository walletRepository,
            ITransactionsRepository transactionsRepository)
        {
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _transactionsRepository = transactionsRepository;
        }

        [HttpPost("Bet")]
        public async Task<IActionResult> Bet(BetRequest req)
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

            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Bet",
                Amount = req.Amount,
                Currency = req.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            userWallet.CurrentBalance -= req.Amount;

            try
            {
                await _walletRepository.UpdateWalletAsync(userWallet);
                var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, userWallet.CurrentBalance, createdTransaction.Id));
            }
            catch (Exception)
            {
                statusCode = 500;
                return StatusCode(statusCode, CustomHttpResponses.GeneralError500);
            }
        }

        [HttpPost("Win")]
        public async Task<IActionResult> Win(WinRequest req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.CampaignId}|{req.CampaignName}|{req.Currency}|{req.GameId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Win",
                Amount = req.Amount,
                Currency = req.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            userWallet.CurrentBalance += req.Amount;

            try
            {
                await _walletRepository.UpdateWalletAsync(userWallet);
                var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, userWallet.CurrentBalance, createdTransaction.Id));
            }
            catch (Exception)
            {
                statusCode = 500;
                return StatusCode(statusCode, CustomHttpResponses.GeneralError500);
            }
        }

        [HttpPost("CancelBet")]
        public async Task<IActionResult> CancelBet(CancelBet req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.BetTransactionId}|{req.BetTypeId}|{req.Currency}|{req.GameId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

            var oldTransaction = await _transactionsRepository.GetTransactionByIdAsync(req.BetTransactionId);
            
            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Cancel Bet",
                Amount = req.Amount,
                Currency = req.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            userWallet.CurrentBalance += oldTransaction.Amount;

            try
            {
                await _walletRepository.UpdateWalletAsync(userWallet);
                var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, userWallet.CurrentBalance, createdTransaction.Id));
            }
            catch (Exception)
            {
                statusCode = 500;
                return StatusCode(statusCode, CustomHttpResponses.GeneralError500);
            }
        }

        [HttpPost("ChangeWin")]
        public async Task<IActionResult> ChangeWin(ChangeWin req)
        {
            // var rawHash =
            //     $"{req.Amount}|{req.ChangeWinTypeId}|{req.Currency}|{req.GameId}|{req.PreviousAmount}|{req.previousTransactionId}|{req.ProductId}|{req.RoundId}|{req.MerchantToken}|{req.TransactionId}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);
            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            if (statusCode != 200) return StatusCode(statusCode, new { statusCode });

            var oldTransaction = await _transactionsRepository.GetTransactionByIdAsync(req.previousTransactionId);
            
            var newTransaction = new TransactionEntity
            {
                UserId = userWallet.UserId,
                PaymentType = "Change Win",
                Amount = req.Amount,
                Currency = req.Currency,
                CreateDate = DateTime.Now,
                Status = 1
            };

            userWallet.CurrentBalance -= oldTransaction.Amount;
            userWallet.CurrentBalance += req.Amount;
            
            try
            {
                await _walletRepository.UpdateWalletAsync(userWallet);
                var createdTransaction = await _transactionsRepository.CreateAsync(newTransaction);

                return StatusCode(statusCode,
                    ApiHelper.GenerateTransferResponse(statusCode, userWallet.CurrentBalance, createdTransaction.Id));
            }
            catch (Exception)
            {
                statusCode = 500;
                return StatusCode(statusCode, CustomHttpResponses.GeneralError500);
            }
        }

        [HttpPost("GetBalance")]
        public async Task<IActionResult> GetBalance(ChangeWin req)
        {
            // var rawHash = $"{req.Currency}|{req.GameId}|{req.ProductId}|{req.MerchantToken}|{req.Token}";
            //
            // if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);

            var userToken = await _tokenRepository.GetByPrivateToken(req.Token);
            var userWallet = await _walletRepository.GetWalletByUserIdAsync(userToken.UserId);

            var statusCode = ApiHelper.DetermineRequestStatusCode(req, userToken, userWallet);

            return statusCode != 200
                ? StatusCode(statusCode, new { statusCode })
                : StatusCode(StatusCodes.Status200OK,
                    ApiHelper.GenerateTransferResponse(statusCode, balance: userWallet.CurrentBalance));
        }
    }
}