using BetsolutionsApi.Models;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BetsolutionsApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenRepository _tokenRepository;

    public AuthController(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    [HttpPost("/auth/auth")]
    public async Task<IActionResult> Auth(Auth auth)
    {
        var userToken = await _tokenRepository.GetByUserIdAsync(auth.UserId);

        if (userToken != null) return StatusCode(StatusCodes.Status411LengthRequired, new { StatusCode = 411 });

        var newToken = new TokenEntity
        {
            UserId = auth.UserId,
            PublicToken = auth.PublicKey,
            PrivateToken = Guid.NewGuid(),
        };

        try
        {
            await _tokenRepository.CreateAsync(newToken);
            return Ok(new { newToken.PublicToken, newToken.PrivateToken });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status411LengthRequired);
        }
    }
}