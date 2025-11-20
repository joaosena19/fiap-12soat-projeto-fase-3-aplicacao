using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculosPorClienteUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculosPorClienteUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar veículos do cliente")]
        public async Task ExecutarAsync_DeveRetornarVeiculosDoCliente()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var veiculos = new List<VeiculoAggregate>
            {
                new VeiculoBuilder().ComClienteId(cliente.Id).Build(),
                new VeiculoBuilder().ComClienteId(cliente.Id).Build()
            };

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).Retorna(veiculos);

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>(veiculos);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var clienteId = Guid.NewGuid();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(clienteId, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>("Cliente não encontrado.", ErrorType.ReferenceNotFound);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando cliente não tem veículos")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoClienteNaoTemVeiculos()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var listaVazia = new List<VeiculoAggregate>();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).Retorna(listaVazia);

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>(listaVazia);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }
    }
}