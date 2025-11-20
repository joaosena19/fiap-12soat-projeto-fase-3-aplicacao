using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Servico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.Cadastros.Servico
{
    public class BuscarServicoPorIdUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public BuscarServicoPorIdUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar serviço quando encontrado")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveRetornarServico_QuandoEncontrado()
        {
            // Arrange
            var servico = new ServicoBuilder().Build();
            _fixture.ServicoGatewayMock.AoObterPorId(servico.Id).Retorna(servico);

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(servico.Id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>(servico);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando serviço não encontrado")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNaoEncontrado()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.ServicoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>("Serviço não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.ServicoGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }
    }
}