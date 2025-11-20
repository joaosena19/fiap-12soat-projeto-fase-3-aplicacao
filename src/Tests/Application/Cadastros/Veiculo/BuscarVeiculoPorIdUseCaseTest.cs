using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculoPorIdUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculoPorIdUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar veículo quando encontrado")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveRetornarVeiculo_QuandoEncontrado()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(veiculo.Id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>(veiculo);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não encontrado")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNaoEncontrado()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.VeiculoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>("Veículo não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.VeiculoGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }
    }
}