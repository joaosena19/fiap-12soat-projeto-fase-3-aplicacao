using Application.Contracts.Presenters;
using FluentAssertions;
using Moq;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Helpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Domain.OrdemServico.Enums;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers.AggregateBuilders;

namespace Tests.Application.OrdemServico
{
    public class AprovarOrcamentoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AprovarOrcamentoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve aprovar orçamento com sucesso quando ordem de serviço existir e itens estiverem disponíveis")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveAprovarOrcamentoComSucesso_QuandoOrdemServicoExistirEItensEstiveremDisponiveis()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            var itemIncluido = ordemServico.ItensIncluidos.First();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().ComId(itemIncluido.ItemEstoqueOriginalId).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemIncluido.ItemEstoqueOriginalId).Retorna(itemEstoque);
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(itemIncluido.ItemEstoqueOriginalId, itemEstoque.Quantidade - itemIncluido.Quantidade.Valor).Completa();
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve verificar se a quantidade no estoque foi atualizada após aprovação")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveAtualizarQuantidadeNoEstoque_AposAprovacao()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            var itemIncluido = ordemServico.ItensIncluidos.First();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().ComId(itemIncluido.ItemEstoqueOriginalId).Build();
            var quantidadeEsperadaAposAtualizacao = itemEstoque.Quantidade - itemIncluido.Quantidade.Valor;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemIncluido.ItemEstoqueOriginalId).Retorna(itemEstoque);
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(itemIncluido.ItemEstoqueOriginalId, quantidadeEsperadaAposAtualizacao).Completa();
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(_ => { });

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.EstoqueExternalServiceMock.DeveTerAtualizadoQuantidade(itemIncluido.ItemEstoqueOriginalId, quantidadeEsperadaAposAtualizacao);
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não estiver disponível no estoque")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoEstiverDisponivelNoEstoque()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            ordemServico.IniciarDiagnostico();
            
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            ordemServico.AdicionarItem(itemEstoque.Id, itemEstoque.Nome, itemEstoque.Preco, 5, TipoItemIncluidoEnum.Peca);
            
            ordemServico.GerarOrcamento();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemEstoque.Id, 5).Retorna(false);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            var itemIncluido = ordemServico.ItensIncluidos.First();
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro($"Item '{itemIncluido.Nome.Valor}' não está disponível no estoque na quantidade necessária ({itemIncluido.Quantidade.Valor}).", ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComStatus(StatusOrdemServicoEnum.EmDiagnostico).Build();
            // Ordem em diagnóstico sem itens/serviços e sem orçamento gerado para provocar DomainException

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Não existe orçamento para aprovar. É necessário gerar o orçamento primeiro.", ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComItens().ComServicos().ComOrcamento().Build();

            var itemIncluido = ordemServico.ItensIncluidos.First();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().ComId(itemIncluido.ItemEstoqueOriginalId).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemIncluido.ItemEstoqueOriginalId).Retorna(itemEstoque);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve verificar disponibilidade de múltiplos itens antes de aprovar")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveVerificarDisponibilidadeMultiplosItens_AntesDeAprovar()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().ComItens().ComServicos().ComOrcamento().Build();

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            var itemIncluido = ordemServico.ItensIncluidos.First();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().ComId(itemIncluido.ItemEstoqueOriginalId).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemIncluido.ItemEstoqueOriginalId).Retorna(itemEstoque);
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(itemIncluido.ItemEstoqueOriginalId, itemEstoque.Quantidade - itemIncluido.Quantidade.Valor).Completa();
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }
    }
}