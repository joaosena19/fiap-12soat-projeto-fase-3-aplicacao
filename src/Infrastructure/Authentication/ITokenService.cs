namespace Infrastructure.Authentication
{
    public interface ITokenService
    {
        string GenerateToken(string clientId);
    }
}