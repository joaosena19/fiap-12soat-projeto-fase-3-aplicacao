using Application.Identidade.Dtos;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tests.Helpers;

namespace Tests.Integration.Identidade
{
    public class PostEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PostEndpointTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo usuário no banco de dados")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirUsuario()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new 
            { 
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);
            var usuarioEntity = await context.Usuarios.Include(u => u.Roles).FirstOrDefaultAsync(u => u.DocumentoIdentificadorUsuario.Valor == cpf);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            usuarioEntity.Should().NotBeNull();
            usuarioEntity!.DocumentoIdentificadorUsuario.Valor.Should().Be(cpf);
            usuarioEntity.Roles.Should().HaveCount(1);
            usuarioEntity.Roles.First().Nome.Valor.Should().Be("Cliente");
        }

        [Fact(DisplayName = "POST deve retornar 409 Conflict quando usuário já existe")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar409Conflict_QuandoUsuarioJaExiste()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new 
            { 
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };

            // Create user first
            var createResponse = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act - Try to create again
            var response = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact(DisplayName = "POST deve retornar 400 Bad Request para senha inválida")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar400BadRequest_ParaSenhaInvalida()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new 
            { 
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "123", // Senha muito curta
                Roles = new[] { "Cliente" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}