using Application.Contracts.Presenters;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Servico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.Cadastros.Servico
{
    public class AtualizarServicoUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public AtualizarServicoUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve atualizar serviço com sucesso quando serviço existir")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveAtualizarServicoComSucesso_QuandoServicoExistir()
        {
            // Arrange
            var servicoExistente = new ServicoBuilder().Build();
            var nomeOriginal = servicoExistente.Nome.Valor;
            var novoNome = "Novo Nome do Serviço";
            var novoPreco = 250.50m;

            ServicoAggregate? servicoAtualizado = null;

            _fixture.ServicoGatewayMock.AoObterPorId(servicoExistente.Id).Retorna(servicoExistente);
            _fixture.ServicoGatewayMock.AoAtualizar().ComCallback(servico => servicoAtualizado = servico);

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                servicoExistente.Id, novoNome, novoPreco,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            servicoAtualizado.Should().NotBeNull();
            servicoAtualizado!.Nome.Valor.Should().Be(novoNome);
            servicoAtualizado.Nome.Valor.Should().NotBe(nomeOriginal);

            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>(servicoAtualizado);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando serviço não existir")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoServicoNaoExistir()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            _fixture.ServicoGatewayMock.AoObterPorId(servicoId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                servicoId, "Nome", 100m,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>("Serviço não encontrado.", ErrorType.ResourceNotFound);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var servicoExistente = new ServicoBuilder().Build();
            var nomeInvalido = ""; // Nome inválido para provocar DomainException

            _fixture.ServicoGatewayMock.AoObterPorId(servicoExistente.Id).Retorna(servicoExistente);

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                servicoExistente.Id, nomeInvalido, 100m,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var servicoExistente = new ServicoBuilder().Build();
            _fixture.ServicoGatewayMock.AoObterPorId(servicoExistente.Id).Retorna(servicoExistente);
            _fixture.ServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                servicoExistente.Id, "Nome", 100m,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>();
        }
    }
}