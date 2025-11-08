using Application.Contracts.Presenters;
using Bogus;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculoPorPlacaUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculoPorPlacaUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve buscar veículo com sucesso quando veículo existir")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveBuscarVeiculoComSucesso_QuandoVeiculoExistir()
        {
            // Arrange
            var veiculoExistente = new VeiculoBuilder().Build();
            var placa = veiculoExistente.Placa.Valor;

            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).Retorna(veiculoExistente);

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>(veiculoExistente);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não existir")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var placa = new Faker("pt_BR").Random.Replace("???-####");

            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>("Veículo não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var placa = new Faker("pt_BR").Random.Replace("???-####");

            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }
    }
}