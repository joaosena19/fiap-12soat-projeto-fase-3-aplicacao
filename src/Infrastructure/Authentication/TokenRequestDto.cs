namespace Infrastructure.Authentication
{
    public record TokenRequestDto(string ClientId, string ClientSecret);
}