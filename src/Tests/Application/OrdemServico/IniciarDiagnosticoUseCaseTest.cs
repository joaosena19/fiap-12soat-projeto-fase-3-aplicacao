using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;

namespace Tests.Application.OrdemServico
{
    public class IniciarDiagnosticoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public IniciarDiagnosticoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando iniciar diagnóstico")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoIniciarDiagnostico()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();
            var domainException = new DomainException("Não é possível iniciar diagnóstico neste status.", ErrorType.DomainRuleBroken);

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(domainException);

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro(domainException.Message, domainException.ErrorType);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }
    }
}