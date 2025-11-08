using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;

namespace Tests.Application.OrdemServico
{
    public class AlterarStatusUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AlterarStatusUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando alterar status da ordem de serviço")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoAlterarStatus()
        {
            var ordemServico = new OrdemServicoBuilder().ProntoParaOrcamento().ComServicos().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ordemServico.Id,
                StatusOrdemServicoEnum.AguardandoAprovacao,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ordemServicoId,
                StatusOrdemServicoEnum.Cancelada,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando DomainException ocorrer")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainExceptionOcorrer()
        {
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();
            var domainException = new DomainException(message: "Regra de domínio quebrada.", ErrorType.DomainRuleBroken);

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(domainException);

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ordemServicoId,
                StatusOrdemServicoEnum.Cancelada,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro(domainException.Message, domainException.ErrorType);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro inesperado"));

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ordemServicoId,
                StatusOrdemServicoEnum.Cancelada,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }
    }
}
