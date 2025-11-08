using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;

namespace Tests.Application.OrdemServico
{
    public class DesaprovarOrcamentoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public DesaprovarOrcamentoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando desaprovar orçamento")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoDesaprovarOrcamento()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build(); // Ordem sem orçamento

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErroComTipo(ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }
    }
}