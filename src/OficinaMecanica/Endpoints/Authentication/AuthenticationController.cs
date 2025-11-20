using API.Dtos;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Authentication;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Autenticação que recebe client credentials e retorna um Bearer token
    /// </summary>
    /// <param name="request">Client credentials</param>
    /// <returns>JWT access token</returns>
    /// <response code="200">Retorna o token JWT</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public ActionResult<TokenResponseDto> GetToken([FromBody] TokenRequestDto request)
    {
        var tokenService = new TokenService(_configuration);
        var authService = new AuthenticationService(_configuration, tokenService);   
        var tokenResponse = authService.ValidateCredentialsAndGenerateToken(request);

        return Ok(tokenResponse);
    }
}
