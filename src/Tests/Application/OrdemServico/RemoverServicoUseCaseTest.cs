using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;

namespace Tests.Application.OrdemServico
{
    public class RemoverServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public RemoverServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando remover serviço")]
        [Trait("UseCase", "RemoverServico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoRemoverServico()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComServicos().Build();
            var servicoIncluidoId = ordemServico.ServicosIncluidos.First().Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.RemoverServicoUseCase.ExecutarAsync(
                ordemServico.Id,
                servicoIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "RemoverServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var servicoIncluidoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.RemoverServicoUseCase.ExecutarAsync(
                ordemServicoId,
                servicoIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "RemoverServico")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComServicos().Build();
            var servicoIncluidoIdInexistente = Guid.NewGuid(); // ID que não existe na ordem

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.RemoverServicoUseCase.ExecutarAsync(
                ordemServico.Id,
                servicoIncluidoIdInexistente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErroComTipo(ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "RemoverServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComServicos().Build();
            var servicoIncluidoId = ordemServico.ServicosIncluidos.First().Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.RemoverServicoUseCase.ExecutarAsync(
                ordemServico.Id,
                servicoIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }
    }
}