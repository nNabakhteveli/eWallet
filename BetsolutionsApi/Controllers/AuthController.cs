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
    private readonly IUserRepository _userRepository;

    public AuthController(ITokenRepository tokenRepository, IUserRepository userRepository)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
    }

    [HttpPost("/auth/auth")]
    public async Task<IActionResult> Auth(Auth auth)
    {
        var userToken = await _tokenRepository.GetByPublicToken(auth.PublicKey);
        var user = await _userRepository.GetUserByIdAsync(auth.UserId);
        
        if (userToken == null) return StatusCode(StatusCodes.Status411LengthRequired, new { StatusCode = 411 });
        if (user == null) return StatusCode(StatusCodes.Status411LengthRequired, new { StatusCode = 406 });
        
        try
        {
            var newToken = new TokenEntity
            {
                UserId = auth.UserId,
                PublicToken = auth.PublicKey,
                PrivateToken = Guid.NewGuid(),
            };
            
            await _tokenRepository.UpdateAsync(newToken);
            return Ok(new { newToken.PublicToken, newToken.PrivateToken });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status411LengthRequired);
        }
    }
}