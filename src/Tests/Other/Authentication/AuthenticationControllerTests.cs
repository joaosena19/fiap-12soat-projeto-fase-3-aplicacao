using API.Endpoints.Authentication;
using FluentAssertions;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using System.Net;
using System.Net.Http.Json;
using Tests.Integration;

namespace Tests.Other.Authentication
{
    public class AuthenticationControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Usa client sem autenticação

            // Setup for unit tests
            _authServiceMock = new Mock<IAuthenticationService>();
            var configMock = new Mock<IConfiguration>();
            _controller = new AuthenticationController(configMock.Object);
        }

        #region Endpoints que precisam de Authorize

        [Theory]
        // ClienteController endpoints
        [InlineData("GET", "/api/cadastros/clientes")]
        [InlineData("GET", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/clientes/documento/12345678901")]
        [InlineData("POST", "/api/cadastros/clientes")]
        [InlineData("PUT", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        // ServicoController endpoints
        [InlineData("GET", "/api/cadastros/servicos")]
        [InlineData("GET", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/servicos")]
        [InlineData("PUT", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        // VeiculoController endpoints
        [InlineData("GET", "/api/cadastros/veiculos")]
        [InlineData("GET", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/veiculos/placa/ABC1234")]
        [InlineData("GET", "/api/cadastros/veiculos/cliente/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/veiculos")]
        [InlineData("PUT", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        // EstoqueItemController endpoints
        [InlineData("GET", "/api/estoque/itens")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/estoque/itens")]
        [InlineData("PUT", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("PATCH", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/quantidade")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/disponibilidade?quantidadeRequisitada=1")]
        // OrdemServicoController endpoints
        [InlineData("GET", "/api/ordens-servico")]
        [InlineData("GET", "/api/ordens-servico/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/ordens-servico/codigo/OS123")]
        [InlineData("POST", "/api/ordens-servico")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/servicos")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/itens")]
        [InlineData("DELETE", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("DELETE", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/cancelar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/iniciar-diagnostico")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento/aprovar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento/desaprovar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/finalizar-execucao")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/entregar")]
        [InlineData("GET", "/api/ordens-servico/tempo-medio")]
        public async Task Endpoints_SemAutenticacao_DevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Endpoints que não podem ter Authorize

        [Theory]
        [InlineData("POST", "/api/authentication/token")]
        [InlineData("POST", "/api/ordens-servico/busca-publica")]
        public async Task Endpoints_ComAllowAnonymous_NaoDevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Método GetToken 

        [Fact(DisplayName = "GetToken deve retornar 200 OK com TokenResponseDto quando credenciais são válidas")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_Retornar200OK_Com_TokenResponseDto_Quando_CredenciaisSaoValidas()
        {
            // Arrange
            var request = new TokenRequestDto("admin", "admin");
            
            // Setup da configuração mockada
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ApiCredentials:ClientId"]).Returns("admin");
            configMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns("admin");
            configMock.Setup(c => c["Jwt:Key"]).Returns("xmvsLe9QxIR3BWAWJW4wL+5ZfZrYaohxUaRYSkxteiAn5qEAKDd3xCMn1Bk46ndy6sl4gkVXXvEP/1JowbBp/g==");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("OficinaMecanicaApi");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("AuthorizedServices");
            
            var controller = new AuthenticationController(configMock.Object);

            // Act
            var result = controller.GetToken(request);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            var tokenResponse = okResult!.Value as TokenResponseDto;
            tokenResponse.Should().NotBeNull();
            tokenResponse!.Token.Should().NotBeNullOrEmpty();
            tokenResponse.TokenType.Should().Be("Bearer");
            tokenResponse.ExpiresIn.Should().Be(3600);
        }

        [Fact(DisplayName = "GetToken deve lançar DomainException quando request é inválido")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_LancarDomainException_Quando_RequestEhInvalido()
        {
            // Arrange
            var request = new TokenRequestDto("", "");
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ApiCredentials:ClientId"]).Returns("admin");
            configMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns("admin");
            
            var tokenService = new Mock<ITokenService>();
            var authService = new AuthenticationService(configMock.Object, tokenService.Object);
            var controller = new AuthenticationController(configMock.Object);

            // Act & Assert

            var exception = Assert.Throws<DomainException>(() => controller.GetToken(request));
            exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
            exception.ErrorType.Should().Be(ErrorType.InvalidInput);
        }

        [Fact(DisplayName = "GetToken deve lançar DomainException quando credenciais são inválidas")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_LancarDomainException_Quando_CredenciaisSaoInvalidas()
        {
            // Arrange
            var request = new TokenRequestDto("invalid-client-id", "invalid-client-secret");
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ApiCredentials:ClientId"]).Returns("admin");
            configMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns("admin");
            
            var controller = new AuthenticationController(configMock.Object);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => controller.GetToken(request));
            exception.Message.Should().Be("Credenciais inválidas.");
            exception.ErrorType.Should().Be(ErrorType.Unauthorized);
        }

        [Fact(DisplayName = "GetToken deve retornar token quando credenciais são válidas")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_RetornarToken_Quando_CredenciaisSaoValidas()
        {
            // Arrange
            var request = new TokenRequestDto("admin", "admin");
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ApiCredentials:ClientId"]).Returns("admin");
            configMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns("admin");
            configMock.Setup(c => c["Jwt:Key"]).Returns("xmvsLe9QxIR3BWAWJW4wL+5ZfZrYaohxUaRYSkxteiAn5qEAKDd3xCMn1Bk46ndy6sl4gkVXXvEP/1JowbBp/g==");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("OficinaMecanicaApi");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("AuthorizedServices");
            
            var controller = new AuthenticationController(configMock.Object);

            // Act
            var result = controller.GetToken(request);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeOfType<TokenResponseDto>();
        }

        #endregion
    }
}
