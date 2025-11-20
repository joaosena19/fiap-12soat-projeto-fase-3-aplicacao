using Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Enums;
using Shared.Exceptions;

namespace Tests.Other.Authentication;

public class AuthenticationServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _tokenServiceMock = new Mock<ITokenService>();
        _authenticationService = new AuthenticationService(_configurationMock.Object, _tokenServiceMock.Object);
    }

    [Fact(DisplayName = "Deve retornar TokenResponseDto quando credenciais são válidas")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_RetornarTokenResponseDto_Quando_CredenciaisSaoValidas()
    {
        // Arrange
        var clientId = "valid-client-id";
        var clientSecret = "valid-client-secret";
        var expectedToken = "generated-jwt-token";
        var request = new TokenRequestDto(clientId, clientSecret);

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(clientId);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(clientSecret);
        _tokenServiceMock.Setup(t => t.GenerateToken(clientId)).Returns(expectedToken);

        // Act
        var result = _authenticationService.ValidateCredentialsAndGenerateToken(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().Be(3600);
        _tokenServiceMock.Verify(t => t.GenerateToken(clientId), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar DomainException com InvalidInput quando ClientId é nulo")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientIdEhNulo()
    {
        // Arrange
        var request = new TokenRequestDto(null!, "client-secret");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
        exception.ErrorType.Should().Be(ErrorType.InvalidInput);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com InvalidInput quando ClientId é vazio")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientIdEhVazio()
    {
        // Arrange
        var request = new TokenRequestDto(string.Empty, "client-secret");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
        exception.ErrorType.Should().Be(ErrorType.InvalidInput);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com InvalidInput quando ClientSecret é nulo")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientSecretEhNulo()
    {
        // Arrange
        var request = new TokenRequestDto("client-id", null!);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
        exception.ErrorType.Should().Be(ErrorType.InvalidInput);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com InvalidInput quando ClientSecret é vazio")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientSecretEhVazio()
    {
        // Arrange
        var request = new TokenRequestDto("client-id", string.Empty);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
        exception.ErrorType.Should().Be(ErrorType.InvalidInput);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ClientId configurado está ausente")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientIdConfiguradoEstaAusente()
    {
        // Arrange
        var request = new TokenRequestDto("client-id", "client-secret");

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns((string?)null);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns("configured-secret");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ClientId configurado é vazio")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientIdConfiguradoEhVazio()
    {
        // Arrange
        var request = new TokenRequestDto("client-id", "client-secret");

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(string.Empty);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns("configured-secret");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ClientSecret configurado está ausente")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientSecretConfiguradoEstaAusente()
    {
        // Arrange
        var request = new TokenRequestDto("client-id", "client-secret");

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns("configured-id");
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns((string?)null);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ClientSecret configurado é vazio")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientSecretConfiguradoEhVazio()
    {
        // Arrange
        var request = new TokenRequestDto("client-id", "client-secret");

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns("configured-id");
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(string.Empty);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ClientId é inválido")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientIdEhInvalido()
    {
        // Arrange
        var request = new TokenRequestDto("invalid-client-id", "valid-client-secret");
        var configuredClientId = "valid-client-id";
        var configuredClientSecret = "valid-client-secret";

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(configuredClientId);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(configuredClientSecret);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ClientSecret é inválido")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_ClientSecretEhInvalido()
    {
        // Arrange
        var request = new TokenRequestDto("valid-client-id", "invalid-client-secret");
        var configuredClientId = "valid-client-id";
        var configuredClientSecret = "valid-client-secret";

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(configuredClientId);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(configuredClientSecret);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar DomainException com Unauthorized quando ambas credenciais são inválidas")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_AmbasCredenciaisSaoInvalidas()
    {
        // Arrange
        var request = new TokenRequestDto("invalid-client-id", "invalid-client-secret");
        var configuredClientId = "valid-client-id";
        var configuredClientSecret = "valid-client-secret";

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(configuredClientId);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(configuredClientSecret);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("Credenciais inválidas.");
        exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Theory(DisplayName = "Deve lançar DomainException com InvalidInput quando credenciais são nulas ou vazias")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("", null)]
    public void ValidateCredentialsAndGenerateToken_Deve_LancarDomainException_Quando_CredenciaisSaoNulasOuVazias(
        string? clientId, string? clientSecret)
    {
        // Arrange
        var request = new TokenRequestDto(clientId!, clientSecret!);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            _authenticationService.ValidateCredentialsAndGenerateToken(request));

        exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
        exception.ErrorType.Should().Be(ErrorType.InvalidInput);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Deve chamar TokenService com ClientId correto")]
    [Trait("Método", "ValidateCredentialsAndGenerateToken")]
    public void ValidateCredentialsAndGenerateToken_Deve_ChamarTokenService_Com_ClientIdCorreto()
    {
        // Arrange
        var clientId = "test-client-id";
        var clientSecret = "test-client-secret";
        var expectedToken = "test-token";
        var request = new TokenRequestDto(clientId, clientSecret);

        _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(clientId);
        _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(clientSecret);
        _tokenServiceMock.Setup(t => t.GenerateToken(clientId)).Returns(expectedToken);

        // Act
        _authenticationService.ValidateCredentialsAndGenerateToken(request);

        // Assert
        _tokenServiceMock.Verify(t => t.GenerateToken(clientId), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.Is<string>(s => s == clientId)), Times.Once);
    }
}
