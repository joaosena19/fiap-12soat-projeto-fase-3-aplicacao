using Application.Contracts.Presenters;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class AtualizarQuantidadeUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public AtualizarQuantidadeUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve atualizar quantidade do item de estoque com sucesso quando item existir")]
        [Trait("UseCase", "AtualizarQuantidade")]
        public async Task ExecutarAsync_DeveAtualizarQuantidadeComSucesso_QuandoItemExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder()
                .ComQuantidade(10)
                .Build();

            var quantidadeOriginal = itemExistente.Quantidade.Valor;
            var novaQuantidade = 50;

            ItemEstoqueAggregate? itemAtualizado = null;

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);
            _fixture.ItemEstoqueGatewayMock.AoAtualizar().ComCallback(item => itemAtualizado = item);

            // Act
            await _fixture.AtualizarQuantidadeUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                novaQuantidade,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarQuantidadePresenterMock.Object);

            // Assert
            itemAtualizado.Should().NotBeNull();
            itemAtualizado.Quantidade.Valor.Should().Be(novaQuantidade);
            itemAtualizado.Quantidade.Valor.Should().NotBe(quantidadeOriginal);

            _fixture.AtualizarQuantidadePresenterMock.DeveTerApresentadoSucesso<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>(itemAtualizado);
            _fixture.AtualizarQuantidadePresenterMock.NaoDeveTerApresentadoErro<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não existir")]
        [Trait("UseCase", "AtualizarQuantidade")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var novaQuantidade = 20;
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarQuantidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                novaQuantidade,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarQuantidadePresenterMock.Object);

            // Assert
            _fixture.AtualizarQuantidadePresenterMock.DeveTerApresentadoErro<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>($"Item de estoque com ID {itemId} não foi encontrado", ErrorType.ResourceNotFound);
            _fixture.AtualizarQuantidadePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarQuantidade")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var quantidadeInvalida = -5; // Quantidade inválida para provocar DomainException

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.AtualizarQuantidadeUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                quantidadeInvalida,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarQuantidadePresenterMock.Object);

            // Assert
            _fixture.AtualizarQuantidadePresenterMock.DeveTerApresentadoErro<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>("Quantidade não pode ser negativa", ErrorType.InvalidInput);
            _fixture.AtualizarQuantidadePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarQuantidade")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var novaQuantidade = 30;

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);
            _fixture.ItemEstoqueGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarQuantidadeUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                novaQuantidade,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarQuantidadePresenterMock.Object);

            // Assert
            _fixture.AtualizarQuantidadePresenterMock.DeveTerApresentadoErro<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarQuantidadePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de acesso negado quando cliente tentar atualizar quantidade do estoque")]
        [Trait("UseCase", "AtualizarQuantidade")]
        public async Task ExecutarAsync_DeveApresentarErroAcessoNegado_QuandoClienteTentarAtualizarQuantidadeEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemId = Guid.NewGuid();
            var novaQuantidade = 30;

            // Act
            await _fixture.AtualizarQuantidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                novaQuantidade,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarQuantidadePresenterMock.Object);

            // Assert
            _fixture.AtualizarQuantidadePresenterMock.DeveTerApresentadoErro<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>("Acesso negado. Apenas administradores podem gerenciar estoque.", ErrorType.NotAllowed);
            _fixture.AtualizarQuantidadePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarQuantidadePresenter, ItemEstoqueAggregate>();
        }
    }
}