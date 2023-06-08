using BetsolutionsApi.Models;
using EWallet.Domain.Data;
using Microsoft.AspNetCore.Mvc;

namespace BetsolutionsApi.Controllers;

[Route("v1")]
[ApiController]
public class InfoController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;

    public InfoController(IUserRepository userRepository, ITokenRepository tokenRepository)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
    }

    [HttpPost("GetPlayerInfo")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> GetPlayerInfo([FromForm] GetPlayerInfoRequest req)
    {
        var rawHash = $"{req.MerchantToken}|{req.PrivateToken}";

        if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);
        try
        {
            var tokenData = await _tokenRepository.GetByPrivateToken(req.PrivateToken);
            var user = await _userRepository.GetUserByIdAsync(tokenData.UserId);

            if (user == null) return StatusCode(406, CustomHttpResponses.UserNotFound406);

            var response = new GetPlayerInfoResponse
            {
                StatusCode = 200,
                Data = new PlayerData
                {
                    Id = user.Id,
                    WalletId = user.WalletId,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                }
            };

            return StatusCode(response.StatusCode, response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, CustomHttpResponses.GeneralError500);
        }
    }
}