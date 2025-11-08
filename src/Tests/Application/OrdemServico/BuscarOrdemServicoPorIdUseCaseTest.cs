using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscarOrdemServicoPorIdUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscarOrdemServicoPorIdUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando ordem de serviço existir")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoOrdemServicoExistir()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var id = ordemServico.Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).Retorna(ordemServico);

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var id = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }
    }
}