using Application.Cadastros.Dtos;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration.Cadastros
{
    public class VeiculoControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public VeiculoControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Veículo no banco de dados.")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirVeiculo()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create a client first
            var clienteDto = new { Nome = "João Silva", DocumentoIdentificador = "22315044057" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "22315044057");
            clienteCriado.Should().NotBeNull();

            var dto = new 
            { 
                ClienteId = clienteCriado!.Id,
                Placa = "ABC4567", 
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Preto", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/veiculos", dto);
            var veiculoEntity = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "ABC4567");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            veiculoEntity.Should().NotBeNull();
            veiculoEntity!.ClienteId.Should().Be(clienteCriado.Id);
            veiculoEntity.Modelo.Valor.Should().Be("Civic");
            veiculoEntity.Marca.Valor.Should().Be("Honda");
            veiculoEntity.Cor.Valor.Should().Be("Preto");
            veiculoEntity.Ano.Valor.Should().Be(2020);
            veiculoEntity.TipoVeiculo.Valor.Should().Be(TipoVeiculoEnum.Carro);
        }

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Veículo existente no banco de dados.")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarVeiculo()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create a client first
            var clienteDto = new { Nome = "Maria Silva", DocumentoIdentificador = "98765432100" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "98765432100");
            clienteCriado.Should().NotBeNull();

            var criarDto = new 
            { 
                ClienteId = clienteCriado!.Id,
                Placa = "XYZ5678", 
                Modelo = "Corolla", 
                Marca = "Toyota", 
                Cor = "Branco", 
                Ano = 2021, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };
            var atualizarDto = new 
            { 
                Modelo = "Corolla Cross", 
                Marca = "Toyota", 
                Cor = "Prata", 
                Ano = 2022, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            // Create vehicle first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var veiculoCriado = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "XYZ5678");
            veiculoCriado.Should().NotBeNull();

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/cadastros/veiculos/{veiculoCriado!.Id}", atualizarDto);
            
            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var veiculoAtualizado = await context.Veiculos.FirstOrDefaultAsync(v => v.Id == veiculoCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculoAtualizado.Should().NotBeNull();
            veiculoAtualizado!.ClienteId.Should().Be(clienteCriado.Id);
            veiculoAtualizado.Modelo.Valor.Should().Be("Corolla Cross");
            veiculoAtualizado.Cor.Valor.Should().Be("Prata");
            veiculoAtualizado.Ano.Valor.Should().Be(2022);
            veiculoAtualizado.Placa.Valor.Should().Be("XYZ5678"); // Placa não deve mudar
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de veículos")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeVeiculos()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create clients first
            var cliente1Dto = new { Nome = "Cliente 1", DocumentoIdentificador = "04663818080" };
            var cliente2Dto = new { Nome = "Cliente 2", DocumentoIdentificador = "98552408040" };
            
            var cliente1Response = await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente1Dto);
            var cliente2Response = await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente2Dto);
            
            cliente1Response.StatusCode.Should().Be(HttpStatusCode.Created);
            cliente2Response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var cliente1 = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "04663818080");
            var cliente2 = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "98552408040");
            
            cliente1.Should().NotBeNull();
            cliente2.Should().NotBeNull();

            var veiculo1 = new 
            { 
                ClienteId = cliente1!.Id,
                Placa = "GET0001", 
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Azul", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };
            var veiculo2 = new 
            { 
                ClienteId = cliente2!.Id,
                Placa = "GET0002", 
                Modelo = "CBR600", 
                Marca = "Honda", 
                Cor = "Vermelho", 
                Ano = 2021, 
                TipoVeiculo = (int)TipoVeiculoEnum.Moto 
            };

            // Create test vehicles
            await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo1);
            await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo2);

            // Act
            var response = await _client.GetAsync("/api/cadastros/veiculos");
            var veiculos = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoVeiculoDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculos.Should().NotBeNull();
            veiculos.Should().HaveCountGreaterThanOrEqualTo(2);
            veiculos.Should().Contain(v => v.Modelo == "Civic" && v.Placa == "GET0001");
            veiculos.Should().Contain(v => v.Modelo == "CBR600" && v.Placa == "GET0002");
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há veículos")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_QuandoNaoHaVeiculos()
        {
            // Arrange - Clear database
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Veiculos.RemoveRange(context.Veiculos);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/cadastros/veiculos");
            var veiculos = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoVeiculoDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculos.Should().NotBeNull();
            veiculos.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET /{id} deve retornar 200 OK e veículo específico")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar200OK_E_VeiculoEspecifico()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create a client first
            var clienteDto = new { Nome = "Cliente GetById", DocumentoIdentificador = "23096067074" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "23096067074");
            clienteCriado.Should().NotBeNull();

            var criarDto = new 
            { 
                ClienteId = clienteCriado!.Id,
                Placa = "GID0001", 
                Modelo = "Fit", 
                Marca = "Honda", 
                Cor = "Branco", 
                Ano = 2019, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            // Create vehicle first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var veiculoCriado = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "GID0001");
            veiculoCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/{veiculoCriado!.Id}");
            var veiculo = await response.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculo.Should().NotBeNull();
            veiculo!.Id.Should().Be(veiculoCriado.Id);
            veiculo.ClienteId.Should().Be(clienteCriado.Id);
            veiculo.Modelo.Should().Be("Fit");
            veiculo.Placa.Should().Be("GID0001");
        }

        [Fact(DisplayName = "GET /{id} deve retornar 404 NotFound quando veículo não existe")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar404NotFound_QuandoVeiculoNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /placa/{placa} deve retornar 200 OK e veículo específico")]
        [Trait("Metodo", "GetByPlaca")]
        public async Task GetByPlaca_Deve_Retornar200OK_E_VeiculoEspecifico()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create a client first
            var clienteDto = new { Nome = "Cliente GetByPlaca", DocumentoIdentificador = "01213944090" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "01213944090");
            clienteCriado.Should().NotBeNull();

            var criarDto = new
            {
                ClienteId = clienteCriado!.Id,
                Placa = "GPL0001",
                Modelo = "Onix",
                Marca = "Chevrolet",
                Cor = "Prata",
                Ano = 2022,
                TipoVeiculo = (int)TipoVeiculoEnum.Carro
            };

            // Create vehicle first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var veiculoCriado = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "GPL0001");
            veiculoCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/placa/GPL0001");
            var veiculo = await response.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculo.Should().NotBeNull();
            veiculo!.Id.Should().Be(veiculoCriado!.Id);
            veiculo.ClienteId.Should().Be(clienteCriado.Id);
            veiculo.Modelo.Should().Be("Onix");
            veiculo.Placa.Should().Be("GPL0001");
        }

        [Fact(DisplayName = "GET /placa/{placa} deve retornar 404 NotFound quando veículo não existe")]
        [Trait("Metodo", "GetByPlaca")]
        public async Task GetByPlaca_Deve_Retornar404NotFound_QuandoVeiculoNaoExiste()
        {
            // Arrange
            var placaInexistente = "XXX9999";

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/placa/{placaInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "POST deve salvar placa sempre em uppercase independente do input")]
        [Trait("Case insenstive", "Placa")]
        public async Task Post_DeveSalvarPlacaSempreEmUppercase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente primeiro
            var clienteDto = new { Nome = "Cliente Teste Placa", DocumentoIdentificador = "23882227028" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "23882227028");
            clienteCriado.Should().NotBeNull();

            var dto = new 
            { 
                ClienteId = clienteCriado!.Id,
                Placa = "abc1234", // lowercase
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Preto", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/veiculos", dto);
            var veiculoEntity = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "ABC1234");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            veiculoEntity.Should().NotBeNull();
            veiculoEntity!.Placa.Valor.Should().Be("ABC1234"); // Deve ser salva em uppercase
        }

        [Fact(DisplayName = "POST deve verificar conflito de placa case insensitive")]
        [Trait("Case insenstive", "Placa")]
        public async Task Post_DeveVerificarConflitoDePlacaCaseInsensitive()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar clientes
            var cliente1Dto = new { Nome = "Cliente 1", DocumentoIdentificador = "27735549067" };
            var cliente2Dto = new { Nome = "Cliente 2", DocumentoIdentificador = "83587959048" };
            
            var cliente1Response = await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente1Dto);
            var cliente2Response = await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente2Dto);
            
            cliente1Response.StatusCode.Should().Be(HttpStatusCode.Created);
            cliente2Response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var cliente1 = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "27735549067");
            var cliente2 = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "83587959048");

            // Criar veículo com placa em uppercase
            var veiculo1Dto = new 
            { 
                ClienteId = cliente1!.Id,
                Placa = "DEF5678", // uppercase
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Preto", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            var firstResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo1Dto);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Tentar criar segundo veículo com versão lowercase da mesma placa
            var veiculo2Dto = new 
            { 
                ClienteId = cliente2!.Id,
                Placa = "def5678", // lowercase - deve gerar conflito
                Modelo = "Corolla", 
                Marca = "Toyota", 
                Cor = "Branco", 
                Ano = 2021, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            // Act
            var secondResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo2Dto);

            // Assert
            secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact(DisplayName = "GET /placa/{placa} deve encontrar veículo independente do case da placa")]
        [Trait("Case insenstive", "Placa")]
        public async Task GetByPlaca_DeveEncontrarVeiculoIndependenteDoCase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar um cliente primeiro
            var clienteDto = new { Nome = "Cliente GetByPlaca Case", DocumentoIdentificador = "45503206053" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "45503206053");
            clienteCriado.Should().NotBeNull();

            var criarDto = new
            {
                ClienteId = clienteCriado!.Id,
                Placa = "GHI9012", // uppercase
                Modelo = "Onix",
                Marca = "Chevrolet",
                Cor = "Prata",
                Ano = 2022,
                TipoVeiculo = (int)TipoVeiculoEnum.Carro
            };

            // Criar veículo primeiro
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act - Tentar encontrar com placa em lowercase
            var response = await _client.GetAsync($"/api/cadastros/veiculos/placa/ghi9012");
            var veiculo = await response.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculo.Should().NotBeNull();
            veiculo!.Placa.Should().Be("GHI9012"); // Deve retornar uppercase
        }

        [Fact(DisplayName = "GET cliente/{clienteId} deve retornar 200 OK e lista de veículos do cliente")]
        [Trait("Metodo", "GetByClienteId")]
        public async Task GetByClienteId_ComClienteComVeiculos_DeveRetornar200EListaVeiculos()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente primeiro
            var clienteDto = new { Nome = "Carlos Pereira", DocumentoIdentificador = "50153367059" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "50153367059");
            clienteCriado.Should().NotBeNull();

            // Criar veículos para clientes
            var veiculo1Dto = new 
            { 
                ClienteId = clienteCriado.Id,
                Placa = "VEI1111", 
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Preto", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            var veiculo2Dto = new 
            { 
                ClienteId = clienteCriado.Id,
                Placa = "VEI2222", 
                Modelo = "Corolla", 
                Marca = "Toyota", 
                Cor = "Branco", 
                Ano = 2021, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo1Dto);
            await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo2Dto);

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/cliente/{clienteCriado.Id}");
            var veiculos = await response.Content.ReadFromJsonAsync<List<RetornoVeiculoDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculos.Should().NotBeNull();
            veiculos!.Should().HaveCount(2);
            veiculos.Should().OnlyContain(v => v.ClienteId == clienteCriado.Id);
            veiculos.Should().Contain(v => v.Placa == "VEI1111");
            veiculos.Should().Contain(v => v.Placa == "VEI2222");
        }

        [Fact(DisplayName = "GET cliente/{clienteId} deve retornar 200 OK e lista vazia quando cliente não tiver veículos")]
        [Trait("Metodo", "GetByClienteId")]
        public async Task GetByClienteId_ComClienteSemVeiculos_DeveRetornar200EListaVazia()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente sem veículos
            var clienteDto = new { Nome = "Ana Costa", DocumentoIdentificador = "94192303094" };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "94192303094");
            clienteCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/cliente/{clienteCriado!.Id}");
            var veiculos = await response.Content.ReadFromJsonAsync<List<RetornoVeiculoDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculos.Should().NotBeNull();
            veiculos!.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET cliente/{clienteId} deve retornar 422 Unprocessable Content quando cliente não existir")]
        [Trait("Metodo", "GetByClienteId")]
        public async Task GetByClienteId_ComClienteInexistente_DeveRetornar422()
        {
            // Arrange
            var clienteIdInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/cliente/{clienteIdInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);
        }
    }
}
