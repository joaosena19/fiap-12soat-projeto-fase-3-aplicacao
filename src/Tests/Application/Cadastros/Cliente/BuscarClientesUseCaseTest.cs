using Application.Contracts.Presenters;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class BuscarClientesUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public BuscarClientesUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve retornar lista de clientes")]
        public async Task ExecutarAsync_DeveRetornarListaDeClientes()
        {
            // Arrange
            var clientes = new List<ClienteAggregate> { new ClienteBuilder().Build(), new ClienteBuilder().Build() };
            _fixture.ClienteGatewayMock.AoObterTodos().Retorna(clientes);

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(_fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>(clientes);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não houver clientes")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoHouverClientes()
        {
            // Arrange
            var listaVazia = new List<ClienteAggregate>();
            _fixture.ClienteGatewayMock.AoObterTodos().Retorna(listaVazia);

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(_fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>(listaVazia);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            _fixture.ClienteGatewayMock.AoObterTodos().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(_fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>("Erro interno do servidor.", Shared.Enums.ErrorType.UnexpectedError);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }
    }
}
