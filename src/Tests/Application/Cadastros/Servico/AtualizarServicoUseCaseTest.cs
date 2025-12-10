using Application.Contracts.Presenters;
using Application.Identidade.Services;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Servico.Helpers;
using Tests.Application.SharedHelpers;
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

        [Fact(DisplayName = "Deve atualizar serviço com sucesso quando é administrador")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveAtualizarServicoComSucesso_QuandoEhAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoExistente = new ServicoBuilder().Build();
            var nomeOriginal = servicoExistente.Nome.Valor;
            var novoNome = "Novo Nome do Serviço";
            var novoPreco = 250.50m;

            ServicoAggregate? servicoAtualizado = null;

            _fixture.ServicoGatewayMock.AoObterPorId(servicoExistente.Id).Retorna(servicoExistente);
            _fixture.ServicoGatewayMock.AoAtualizar().ComCallback(servico => servicoAtualizado = servico);

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                ator, servicoExistente.Id, novoNome, novoPreco,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            servicoAtualizado.Should().NotBeNull();
            servicoAtualizado!.Nome.Valor.Should().Be(novoNome);
            servicoAtualizado.Nome.Valor.Should().NotBe(nomeOriginal);

            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>(servicoAtualizado);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta atualizar serviço")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaAtualizarServico()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var id = Guid.NewGuid();
            var novoNome = "Novo Nome do Serviço";
            var novoPreco = 250.50m;

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                ator, id, novoNome, novoPreco,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando serviço não existir")]
        [Trait("UseCase", "AtualizarServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoServicoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoId = Guid.NewGuid();
            _fixture.ServicoGatewayMock.AoObterPorId(servicoId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                ator, servicoId, "Nome", 100m,
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
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoExistente = new ServicoBuilder().Build();
            var nomeInvalido = ""; // Nome inválido para provocar DomainException

            _fixture.ServicoGatewayMock.AoObterPorId(servicoExistente.Id).Retorna(servicoExistente);

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                ator, servicoExistente.Id, nomeInvalido, 100m,
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
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoExistente = new ServicoBuilder().Build();
            _fixture.ServicoGatewayMock.AoObterPorId(servicoExistente.Id).Retorna(servicoExistente);
            _fixture.ServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarServicoUseCase.ExecutarAsync(
                ator, servicoExistente.Id, "Nome", 100m,
                _fixture.ServicoGatewayMock.Object, _fixture.AtualizarServicoPresenterMock.Object);

            // Assert
            _fixture.AtualizarServicoPresenterMock.DeveTerApresentadoErro<IAtualizarServicoPresenter, ServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarServicoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarServicoPresenter, ServicoAggregate>();
        }
    }
}