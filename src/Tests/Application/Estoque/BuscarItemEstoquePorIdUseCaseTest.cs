using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class BuscarItemEstoquePorIdUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public BuscarItemEstoquePorIdUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve buscar item de estoque com sucesso quando item existir")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveBuscarItemEstoqueComSucesso_QuandoItemExistir()
        {
            // Arrange
            var itemExistente = new ItemEstoqueBuilder().Build();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                itemExistente.Id,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>(itemExistente);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não existir")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoExistir()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>($"Item de estoque com ID {itemId} não foi encontrado", ErrorType.ResourceNotFound);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }
    }
}