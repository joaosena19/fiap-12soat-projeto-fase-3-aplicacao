using Microsoft.Extensions.Configuration;
using Shared.Exceptions;
using Shared.Enums;

namespace Infrastructure.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IConfiguration configuration, ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public TokenResponseDto ValidateCredentialsAndGenerateToken(TokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
                throw new DomainException("ClientId e ClientSecret requeridos.", ErrorType.InvalidInput);

            var configuredClientId = _configuration["ApiCredentials:ClientId"];
            var configuredClientSecret = _configuration["ApiCredentials:ClientSecret"];

            if (string.IsNullOrEmpty(configuredClientId) || string.IsNullOrEmpty(configuredClientSecret))
                throw new DomainException("Credenciais inválidas.", ErrorType.Unauthorized);

            if (request.ClientId != configuredClientId || request.ClientSecret != configuredClientSecret)
                throw new DomainException("Credenciais inválidas.", ErrorType.Unauthorized);

            var token = _tokenService.GenerateToken(request.ClientId);
            return new TokenResponseDto(token);
        }
    }
}