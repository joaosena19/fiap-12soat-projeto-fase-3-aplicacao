using Application.Contracts.Presenters;
using Bogus;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class AtualizarClienteUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public AtualizarClienteUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve atualizar cliente com sucesso quando cliente existir")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveAtualizarClienteComSucesso_QuandoClienteExistir()
        {
            // Arrange
            var clienteExistente = new ClienteBuilder().Build();
            var nomeOriginal = clienteExistente.Nome.Valor;
            var novoNome = new Faker("pt_BR").Person.FullName;

            ClienteAggregate? clienteAtualizado = null;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);
            _fixture.ClienteGatewayMock.AoAtualizar().ComCallback(cliente => clienteAtualizado = cliente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object);

            // Assert
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Valor.Should().Be(novoNome);

            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IAtualizarClientePresenter, ClienteAggregate>();
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var nomeAtualizado = new Faker("pt_BR").Person.FullName;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(clienteId, nomeAtualizado, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Cliente não encontrado.", ErrorType.ResourceNotFound);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var clienteExistente = new ClienteBuilder().Build();
            var nomeInvalido = ""; // Nome inválido para provocar DomainException

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(clienteExistente.Id, nomeInvalido, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var clienteExistente = new ClienteBuilder().Build();
            var novoNome = new Faker("pt_BR").Person.FullName;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);
            _fixture.ClienteGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }
    }
}