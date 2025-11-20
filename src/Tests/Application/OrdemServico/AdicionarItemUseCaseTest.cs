using Application.Contracts.Presenters;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class AdicionarItemUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AdicionarItemUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve adicionar item com sucesso quando ordem de serviço existir e item de estoque for válido")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveAdicionarItemComSucesso_QuandoOrdemServicoExistirEItemEstoqueForValido()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidade = 2;

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ordemServico.Id,
                itemEstoque.Id,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object);

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.ItensIncluidos.Should().HaveCount(1);

            var itemAdicionado = ordemServicoAtualizada.ItensIncluidos.First();
            itemAdicionado.ItemEstoqueOriginalId.Should().Be(itemEstoque.Id);
            itemAdicionado.Nome.Valor.Should().Be(itemEstoque.Nome);
            itemAdicionado.Preco.Valor.Should().Be(itemEstoque.Preco);
            itemAdicionado.Quantidade.Valor.Should().Be(quantidade);

            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>(ordemServicoAtualizada);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var itemEstoqueId = Guid.NewGuid();
            var quantidade = 1;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ordemServicoId,
                itemEstoqueId,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object);

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item de estoque não existir")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemEstoqueNaoExistir()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoqueId = Guid.NewGuid();
            var quantidade = 1;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoqueId).NaoRetornaNada();

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ordemServico.Id,
                itemEstoqueId,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object);

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>($"Item de estoque com ID {itemEstoqueId} não encontrado.", ErrorType.ReferenceNotFound);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidadeInvalida = -1; // Quantidade inválida para provocar DomainException

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ordemServico.Id,
                itemEstoque.Id,
                quantidadeInvalida,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object);

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("A quantidade deve ser maior que zero.", ErrorType.InvalidInput);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidade = 1;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ordemServico.Id,
                itemEstoque.Id,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object);

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }
    }
}