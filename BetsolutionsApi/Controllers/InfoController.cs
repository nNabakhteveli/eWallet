using BetsolutionsApi.Models;
using EWallet.Domain.Data;
using Microsoft.AspNetCore.Mvc;

namespace BetsolutionsApi.Controllers;

[Route("v1")]
[ApiController]
public class InfoController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public InfoController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("GetPlayerInfo")]
    public async Task<IActionResult> GetPlayerInfo(GetPlayerInfoRequest req)
    {
        var rawHash = $"{req.MerchantToken}|{req.PrivateToken}";
        
        if (req.Hash != ApiHelper.GetSha256(rawHash)) return StatusCode(403, CustomHttpResponses.InvalidHash403);
        
        var user = await _userRepository.GetUserByIdAsync(req.Id);

        if (user == null) return StatusCode(406, CustomHttpResponses.UserNotFound406);

        var response = new GetPlayerInfoResponse
        {
            StatusCode = 200,
            Data = new PlayerData
            {
                Id = req.Id,
                WalletId = user.WalletId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            }
        };

        return StatusCode(response.StatusCode, response);
    }
}