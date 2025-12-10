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
    public class BuscarServicoPorIdUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public BuscarServicoPorIdUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar serviço quando é administrador")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveRetornarServico_QuandoEhAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servico = new ServicoBuilder().Build();
            _fixture.ServicoGatewayMock.AoObterPorId(servico.Id).Retorna(servico);

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(ator, servico.Id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>(servico);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta buscar serviço")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaBuscarServico()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var id = Guid.NewGuid();

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(ator, id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando serviço não encontrado")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNaoEncontrado()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            _fixture.ServicoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(ator, id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>("Serviço não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            _fixture.ServicoGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarServicoPorIdUseCase.ExecutarAsync(ator, id, _fixture.ServicoGatewayMock.Object, _fixture.BuscarServicoPorIdPresenterMock.Object);

            // Assert
            _fixture.BuscarServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarServicoPorIdPresenter, ServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarServicoPorIdPresenter, ServicoAggregate>();
        }
    }
}