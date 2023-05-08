using EWallet.Domain.Data;
using EWallet.Domain.Models;
using EWallet.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

public class TokenController : Controller
{
    private readonly ITokenRepository _tokenRepository;

    public TokenController(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    [HttpPost("/token/generate")]
    public async Task<JsonResult> GeneratePublicToken([FromBody] GenerateTokenDto user)
    {
        try
        {
            var token = new TokenEntity
            {
                UserId = user.UserId,
                PublicToken = Guid.NewGuid(),
                PublicTokenStatus = "active",
                PrivateToken = Guid.Empty,
                PrivateTokenStatus = string.Empty
            };
            
            var result = await _tokenRepository.CreateAsync(token);
            
            return Json(new { Success = true, publicToken = result.PublicToken, privateToken = result.PrivateToken });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Json(new { Success = false });
        }
    }
}