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
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Auth([FromForm] Auth auth)
    {
        var userToken = await _tokenRepository.GetByPublicToken(auth.PublicKey);
        var user = await _userRepository.GetUserByIdAsync(auth.UserId);
        
        if (userToken == null) return StatusCode(StatusCodes.Status411LengthRequired, CustomHttpResponses.InvalidRequest411);
        if (user == null) return StatusCode(StatusCodes.Status406NotAcceptable, CustomHttpResponses.UserNotFound406);
        
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