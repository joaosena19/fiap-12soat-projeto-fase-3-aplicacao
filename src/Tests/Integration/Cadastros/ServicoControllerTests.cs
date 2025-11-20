using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Application.Cadastros.Dtos;
using Infrastructure.Database;

namespace Tests.Integration.Cadastros
{
    public class ServicoControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ServicoControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Serviço no banco de dados.")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirServico()
        {
            // Arrange
            var dto = new { Nome = "Troca de óleo", Preco = 150.50M };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/servicos", dto);
            var servicoEntity = await context.Servicos.FirstOrDefaultAsync(s => s.Nome.Valor == "Troca de óleo");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            servicoEntity.Should().NotBeNull();
            servicoEntity.Nome.Valor.Should().Be("Troca de óleo");
            servicoEntity.Preco.Valor.Should().Be(150.50M);
        }

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Serviço existente no banco de dados.")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarServico()
        {
            // Arrange
            var criarDto = new { Nome = "Alinhamento", Preco = 80.00M };
            var atualizarDto = new { Nome = "Alinhamento e Balanceamento", Preco = 120.00M };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create service first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/servicos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var servicoCriado = await context.Servicos.FirstOrDefaultAsync(s => s.Nome.Valor == "Alinhamento");
            servicoCriado.Should().NotBeNull();

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/cadastros/servicos/{servicoCriado!.Id}", atualizarDto);
            
            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var servicoAtualizado = await context.Servicos.FirstOrDefaultAsync(s => s.Id == servicoCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            servicoAtualizado.Should().NotBeNull();
            servicoAtualizado!.Nome.Valor.Should().Be("Alinhamento e Balanceamento");
            servicoAtualizado.Preco.Valor.Should().Be(120.00M);
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de serviços")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeServicos()
        {
            // Arrange
            var servico1 = new { Nome = "Revisão completa", Preco = 300.00M };
            var servico2 = new { Nome = "Troca de pneus", Preco = 250.00M };

            // Create test services
            await _client.PostAsJsonAsync("/api/cadastros/servicos", servico1);
            await _client.PostAsJsonAsync("/api/cadastros/servicos", servico2);

            // Act
            var response = await _client.GetAsync("/api/cadastros/servicos");
            var servicos = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoServicoDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            servicos.Should().NotBeNull();
            servicos.Should().HaveCountGreaterThanOrEqualTo(2);
            servicos.Should().Contain(s => s.Nome == "Revisão completa" && s.Preco == 300.00M);
            servicos.Should().Contain(s => s.Nome == "Troca de pneus" && s.Preco == 250.00M);
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há serviços")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_QuandoNaoHaServicos()
        {
            // Arrange - Clear database
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Servicos.RemoveRange(context.Servicos);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/cadastros/servicos");
            var servicos = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoServicoDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            servicos.Should().NotBeNull();
            servicos.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET /{id} deve retornar 200 OK e serviço específico")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar200OK_E_ServicoEspecifico()
        {
            // Arrange
            var criarDto = new { Nome = "Diagnóstico eletrônico", Preco = 180.00M };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create service first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/servicos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var servicoCriado = await context.Servicos.FirstOrDefaultAsync(s => s.Nome.Valor == "Diagnóstico eletrônico");
            servicoCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/servicos/{servicoCriado!.Id}");
            var servico = await response.Content.ReadFromJsonAsync<RetornoServicoDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            servico.Should().NotBeNull();
            servico.Id.Should().Be(servicoCriado.Id);
            servico.Nome.Should().Be("Diagnóstico eletrônico");
            servico.Preco.Should().Be(180.00M);
        }
    }
}
