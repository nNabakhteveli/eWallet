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
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
    }

    [HttpPost("GetPlayerInfo")]
    public async Task<IActionResult> GetPlayerInfo(GetPlayerInfoRequest req)
    {
        var user = await _userRepository.GetUserByIdAsync(req.Id);

        if (user == null) return StatusCode(406, new { StatusCode = 406 });

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


    [HttpPost("GetRake")]
    public async Task<IActionResult> GetRake(GetRakeRequest req)
    {
        var user = _userRepository.GetUserByIdAsync(req.UserId);

        if (user == null) return StatusCode(406, new { StatusCode = 406 });

        var response = new
        {
            StatusCode = 200,
            Data = new
            {
                RakeData = new List<dynamic>
                {
                    new { Amount = 18, PlayerId = 100020, MerchantPlayerId = 111501, Date = "08-21-2018" },
                    new { Amount = 51, PlayerId = 100020, MerchantPlayerId = 111501, Date = "08-21-2018" }
                }
            }
        };

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("GetGameList")]
    public async Task<IActionResult> GetGameList(GetGameList req)
    {
        var response = new
        {
            StatusCode = 200,
            Data = new
            {
                Products = new List<dynamic>()
                {
                    new
                    {
                        ProductId = 3,
                        Games = new List<dynamic>()
                        {
                            new
                            {
                                GameId = 7000,
                                ProductId = 3,
                                HasFreeplay = false,
                                Name = "High low",
                                LaunchUrl = "http://highlow-staging.betsolutions.com/",
                                RTP = 1,
                                RakePercent = 34,
                                HasMobileDeviceSupport = true,
                                Thumbnails = new List<dynamic>()
                                {
                                    new
                                    {
                                        Url =
                                            "http://static-staging.betsolutions.com/7000/logo-thumbnails/2048x2048.png",
                                        Width = 2048,
                                        Height = 2048,
                                        Lang = "en-US"
                                    },
                                    new
                                    {
                                        Url =
                                            "http://static-staging.betsolutions.com/7000/logo-thumbnails/1920x1080.png",
                                        Width = 1920,
                                        Height = 1080,
                                        Lang = "en-US"
                                    },
                                    new
                                    {
                                        Url =
                                            "http://static-staging.betsolutions.com/7000/logo-thumbnails/1000x1000.png",
                                        Width = 1000,
                                        Height = 1000,
                                        Lang = "en-US"
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };

        return StatusCode(response.StatusCode, response);
    }
}