using FluentAssertions;
using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Tests.Other.Authentication;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _tokenService = new TokenService(_configurationMock.Object);
    }

    [Fact(DisplayName = "Deve gerar token JWT válido quando configuração está presente")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_GerarTokenJwtValido_Quando_ConfiguracaoEstaPresente()
    {
        // Arrange
        var clientId = "test-client-id";
        var key = "minha-chave-secreta-muito-longa-para-jwt-com-256-bits";
        var issuer = "test-issuer";
        var audience = "test-audience";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        // Act
        var token = _tokenService.GenerateToken(clientId);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.CanReadToken(token).Should().BeTrue();

        var jwtToken = tokenHandler.ReadJwtToken(token);
        jwtToken.Issuer.Should().Be(issuer);
        jwtToken.Audiences.Should().Contain(audience);
        jwtToken.Subject.Should().Be(clientId);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == clientId);
        jwtToken.Claims.Should().Contain(c => c.Type == "client_id" && c.Value == clientId);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Iat);
    }

    [Fact(DisplayName = "Deve gerar token com expiração de 1 hora")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_GerarToken_Com_ExpiracaoDeUmaHora()
    {
        // Arrange
        var clientId = "test-client-id";
        var key = "minha-chave-secreta-muito-longa-para-jwt-com-256-bits";
        var issuer = "test-issuer";
        var audience = "test-audience";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _tokenService.GenerateToken(clientId);

        // Assert
        var afterGeneration = DateTime.UtcNow;
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddMinutes(59));
        jwtToken.ValidTo.Should().BeBefore(afterGeneration.AddMinutes(61));
    }

    [Fact(DisplayName = "Deve gerar tokens únicos para mesmo clientId")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_GerarTokensUnicos_Para_MesmoClientId()
    {
        // Arrange
        var clientId = "test-client-id";
        var key = "minha-chave-secreta-muito-longa-para-jwt-com-256-bits";
        var issuer = "test-issuer";
        var audience = "test-audience";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        // Act
        var token1 = _tokenService.GenerateToken(clientId);
        var token2 = _tokenService.GenerateToken(clientId);

        // Assert
        token1.Should().NotBe(token2);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken1 = tokenHandler.ReadJwtToken(token1);
        var jwtToken2 = tokenHandler.ReadJwtToken(token2);

        var jti1 = jwtToken1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = jwtToken2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        jti1.Should().NotBe(jti2);
    }

    [Fact(DisplayName = "Deve assinar token com chave HMAC SHA256")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_AssinarToken_Com_ChaveHmacSha256()
    {
        // Arrange
        var clientId = "test-client-id";
        var key = "minha-chave-secreta-muito-longa-para-jwt-com-256-bits";
        var issuer = "test-issuer";
        var audience = "test-audience";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        // Act
        var token = _tokenService.GenerateToken(clientId);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);

        // Verificar se o token pode ser validado com a chave correta
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        var action = () => tokenHandler.ValidateToken(token, validationParameters, out _);
        action.Should().NotThrow();
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando chave JWT está ausente")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_ChaveJwtEstaAusente()
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando chave JWT é vazia")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_ChaveJwtEhVazia()
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(string.Empty);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando issuer JWT está ausente")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_IssuerJwtEstaAusente()
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando issuer JWT é vazio")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_IssuerJwtEhVazio()
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(string.Empty);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando audience JWT está ausente")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_AudienceJwtEstaAusente()
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns((string?)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando audience JWT é vazio")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_AudienceJwtEhVazio()
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(string.Empty);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Theory(DisplayName = "Deve lançar InvalidOperationException quando configurações JWT estão ausentes ou vazias")]
    [Trait("Método", "GenerateToken")]
    [InlineData(null, "issuer", "audience")]
    [InlineData("", "issuer", "audience")]
    [InlineData("key", null, "audience")]
    [InlineData("key", "", "audience")]
    [InlineData("key", "issuer", null)]
    [InlineData("key", "issuer", "")]
    [InlineData(null, null, null)]
    [InlineData("", "", "")]
    public void GenerateToken_Deve_LancarInvalidOperationException_Quando_ConfiguracoesJwtEstaoAusentesOuVazias(
        string? key, string? issuer, string? audience)
    {
        // Arrange
        var clientId = "test-client-id";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateToken(clientId));

        exception.Message.Should().Contain("JWT").And.Contain("ausente");
    }

    [Fact(DisplayName = "Deve incluir clientId em claims do token")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_IncluirClientId_Em_ClaimsDoToken()
    {
        // Arrange
        var clientId = "specific-client-id-123";
        var key = "minha-chave-secreta-muito-longa-para-jwt-com-256-bits";
        var issuer = "test-issuer";
        var audience = "test-audience";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        // Act
        var token = _tokenService.GenerateToken(clientId);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        var clientIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "client_id");

        subClaim.Should().NotBeNull();
        subClaim!.Value.Should().Be(clientId);

        clientIdClaim.Should().NotBeNull();
        clientIdClaim!.Value.Should().Be(clientId);
    }

    [Fact(DisplayName = "Deve incluir timestamp IAT no token")]
    [Trait("Método", "GenerateToken")]
    public void GenerateToken_Deve_IncluirTimestampIat_No_Token()
    {
        // Arrange
        var clientId = "test-client-id";
        var key = "minha-chave-secreta-muito-longa-para-jwt-com-256-bits";
        var issuer = "test-issuer";
        var audience = "test-audience";

        _configurationMock.Setup(c => c["Jwt:Key"]).Returns(key);
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);

        var beforeGeneration = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Act
        var token = _tokenService.GenerateToken(clientId);

        // Assert
        var afterGeneration = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var iatClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
        iatClaim.Should().NotBeNull();

        var iatValue = long.Parse(iatClaim!.Value);
        iatValue.Should().BeGreaterThanOrEqualTo(beforeGeneration);
        iatValue.Should().BeLessThanOrEqualTo(afterGeneration);
    }
}
