using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class BuscarClientePorIdUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public BuscarClientePorIdUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve retornar cliente quando encontrado")]
        public async Task ExecutarAsync_DeveRetornarCliente_QuandoEncontrado()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(cliente.Id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>(cliente);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não encontrado")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNaoEncontrado()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.ClienteGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>("Cliente não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.ClienteGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }
    }
}
