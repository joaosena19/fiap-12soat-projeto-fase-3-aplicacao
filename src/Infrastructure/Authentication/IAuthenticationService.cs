namespace Infrastructure.Authentication
{
    public interface IAuthenticationService
    {
        TokenResponseDto ValidateCredentialsAndGenerateToken(TokenRequestDto request);
    }
}