using Application.Contracts.Presenters;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Servico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.Cadastros.Servico
{
    public class CriarServicoUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public CriarServicoUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve criar serviço com sucesso quando não existir serviço com mesmo nome")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveCriarServicoComSucesso_QuandoNaoExistirServicoComMesmoNome()
        {
            // Arrange
            var servicoEsperado = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(150.00m).Build();
            ServicoAggregate? servicoCriado = null;

            _fixture.ServicoGatewayMock.AoObterPorNome(servicoEsperado.Nome.Valor).NaoRetornaNada();
            _fixture.ServicoGatewayMock.AoSalvar().ComCallback(servico => servicoCriado = servico);

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                servicoEsperado.Nome.Valor, servicoEsperado.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object);

            // Assert
            servicoCriado.Should().NotBeNull();
            servicoCriado!.Nome.Valor.Should().Be(servicoEsperado.Nome.Valor);
            servicoCriado.Preco.Valor.Should().Be(servicoEsperado.Preco.Valor);

            _fixture.CriarServicoPresenterMock.DeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>(servicoCriado);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de conflito quando já existir serviço com mesmo nome")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErroConflito_QuandoJaExistirServicoComMesmoNome()
        {
            // Arrange
            var servicoExistente = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(100m).Build();
            var servicoParaTentar = new ServicoBuilder().ComNome(servicoExistente.Nome.Valor).ComPreco(150m).Build();

            _fixture.ServicoGatewayMock.AoObterPorNome(servicoExistente.Nome.Valor).Retorna(servicoExistente);

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                servicoParaTentar.Nome.Valor, servicoParaTentar.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Já existe um serviço cadastrado com este nome.", ErrorType.Conflict);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var nomeInvalido = "";
            var precoValido = 100m;

            _fixture.ServicoGatewayMock.AoObterPorNome(nomeInvalido).NaoRetornaNada();

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                nomeInvalido, precoValido,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var servicoParaCriar = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(150m).Build();

            _fixture.ServicoGatewayMock.AoObterPorNome(servicoParaCriar.Nome.Valor).NaoRetornaNada();
            _fixture.ServicoGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                servicoParaCriar.Nome.Valor, servicoParaCriar.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }
    }
}