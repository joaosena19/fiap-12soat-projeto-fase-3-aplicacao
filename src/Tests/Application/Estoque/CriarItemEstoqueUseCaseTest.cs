using Application.Contracts.Presenters;
using Domain.Estoque.Enums;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class CriarItemEstoqueUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public CriarItemEstoqueUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve criar item de estoque com sucesso quando não existir item com mesmo nome")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveCriarItemEstoqueComSucesso_QuandoNaoExistirItemComMesmoNome()
        {
            // Arrange
            var itemEsperado = new ItemEstoqueBuilder().Build();

            ItemEstoqueAggregate? itemCriado = null;

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemEsperado.Nome.Valor).NaoRetornaNada();
            _fixture.ItemEstoqueGatewayMock.AoSalvar().ComCallback(item => itemCriado = item);

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                itemEsperado.Nome.Valor,
                itemEsperado.Quantidade.Valor,
                itemEsperado.TipoItemEstoque.Valor,
                itemEsperado.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object);

            // Assert
            itemCriado.Should().NotBeNull();
            itemCriado!.Nome.Valor.Should().Be(itemEsperado.Nome.Valor);
            itemCriado.Quantidade.Valor.Should().Be(itemEsperado.Quantidade.Valor);
            itemCriado.TipoItemEstoque.Valor.Should().Be(itemEsperado.TipoItemEstoque.Valor);
            itemCriado.Preco.Valor.Should().Be(itemEsperado.Preco.Valor);

            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>(itemCriado);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de conflito quando já existir item com mesmo nome")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroConflito_QuandoJaExistirItemComMesmoNome()
        {
            // Arrange
            var itemExistente = new ItemEstoqueBuilder()
                .ComNome("Filtro de Óleo")
                .Build();

            var itemParaTentar = new ItemEstoqueBuilder()
                .ComNome("Filtro de Óleo")
                .Build();

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemExistente.Nome.Valor).Retorna(itemExistente);

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                itemParaTentar.Nome.Valor,
                itemParaTentar.Quantidade.Valor,
                itemParaTentar.TipoItemEstoque.Valor,
                itemParaTentar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("existe um item de estoque cadastrado com este nome.", ErrorType.Conflict);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var nomeInvalido = "";
            var quantidadeValida = 10;
            var tipoValido = TipoItemEstoqueEnum.Peca;
            var precoValido = 100m;

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(nomeInvalido).NaoRetornaNada();

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                nomeInvalido,
                quantidadeValida,
                tipoValido,
                precoValido,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var itemParaCriar = new ItemEstoqueBuilder()
                .ComNome("Filtro de Ar")
                .ComQuantidade(20)
                .ComTipoItemEstoque(TipoItemEstoqueEnum.Peca)
                .ComPreco(35.50m)
                .Build();

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemParaCriar.Nome.Valor).NaoRetornaNada();
            _fixture.ItemEstoqueGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                itemParaCriar.Nome.Valor,
                itemParaCriar.Quantidade.Valor,
                itemParaCriar.TipoItemEstoque.Valor,
                itemParaCriar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }
    }
}