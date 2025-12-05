using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Shared.Enums;
using Tests.Application.Cadastros.Servico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.Cadastros.Servico
{
    public class BuscarServicosUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public BuscarServicosUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar lista de serviços quando é administrador")]
        [Trait("UseCase", "BuscarServicos")]
        public async Task ExecutarAsync_DeveRetornarListaDeServicos_QuandoEhAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicos = new List<ServicoAggregate> { new ServicoBuilder().Build(), new ServicoBuilder().Build() };
            _fixture.ServicoGatewayMock.AoObterTodos().Retorna(servicos);

            // Act
            await _fixture.BuscarServicosUseCase.ExecutarAsync(ator, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicosPresenterMock.Object);

            // Assert
            _fixture.BuscarServicosPresenterMock.DeveTerApresentadoSucesso<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>(servicos);
            _fixture.BuscarServicosPresenterMock.NaoDeveTerApresentadoErro<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta buscar serviços")]
        [Trait("UseCase", "BuscarServicos")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaBuscarServicos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();

            // Act
            await _fixture.BuscarServicosUseCase.ExecutarAsync(ator, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicosPresenterMock.Object);

            // Assert
            _fixture.BuscarServicosPresenterMock.DeveTerApresentadoErro<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
            _fixture.BuscarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não houver serviços")]
        [Trait("UseCase", "BuscarServicos")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoHouverServicos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var listaVazia = new List<ServicoAggregate>();
            _fixture.ServicoGatewayMock.AoObterTodos().Retorna(listaVazia);

            // Act
            await _fixture.BuscarServicosUseCase.ExecutarAsync(ator, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicosPresenterMock.Object);

            // Assert
            _fixture.BuscarServicosPresenterMock.DeveTerApresentadoSucesso<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>(listaVazia);
            _fixture.BuscarServicosPresenterMock.NaoDeveTerApresentadoErro<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarServicos")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            _fixture.ServicoGatewayMock.AoObterTodos().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarServicosUseCase.ExecutarAsync(ator, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicosPresenterMock.Object);

            // Assert
            _fixture.BuscarServicosPresenterMock.DeveTerApresentadoErro<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>();
        }
    }
}