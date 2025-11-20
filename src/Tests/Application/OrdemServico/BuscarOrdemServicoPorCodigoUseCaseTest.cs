using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscarOrdemServicoPorCodigoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscarOrdemServicoPorCodigoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando ordem de serviço existir")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoOrdemServicoExistir()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(ordemServico.Codigo.Valor).Retorna(ordemServico);

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ordemServico.Codigo.Valor,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var codigo = "OS-INEXISTENTE";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).NaoRetornaNada();

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                codigo,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var codigo = "OS-12345";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                codigo,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }
    }
}