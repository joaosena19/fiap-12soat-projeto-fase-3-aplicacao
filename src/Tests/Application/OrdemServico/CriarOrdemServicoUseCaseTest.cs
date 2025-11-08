using Domain.OrdemServico.Enums;
using FluentAssertions;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class CriarOrdemServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public CriarOrdemServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve criar ordem de serviço com status inicial Recebida")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveCriarOrdemServicoComStatusRecebida()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Status.Valor.Should().Be(StatusOrdemServicoEnum.Recebida);
            ordemServicoSalva.Historico.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não existir")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(false);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Veículo não encontrado para criar a ordem de serviço.", ErrorType.ReferenceNotFound), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve gerar novo código quando código já existir")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveGerarNovoCodigo_QuandoCodigoJaExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServicoExistente = new OrdemServicoBuilder().ComVeiculoId(Guid.NewGuid()).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);

            // Simula que na primeira tentativa o código já existe, na segunda não
            _fixture.OrdemServicoGatewayMock.SetupSequence(g => g.ObterPorCodigoAsync(It.IsAny<string>()))
                .ReturnsAsync(ordemServicoExistente)
                .ReturnsAsync((OrdemServicoAggregate?)null);
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Codigo.Valor.Should().NotBe(ordemServicoExistente.Codigo.Valor);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).LancaExcecao(new DomainException("Erro de domínio personalizado", ErrorType.DomainRuleBroken));

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Erro de domínio personalizado", ErrorType.DomainRuleBroken), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        // Teste removido: responsabilidade já coberta em outros testes

        [Fact(DisplayName = "Deve verificar se código já existe e salvar ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveVerificarCodigoExistenteESalvar()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => { });

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OrdemServicoGatewayMock.DeveTerVerificadoCodigoExistente();
            _fixture.OrdemServicoGatewayMock.DeveTerSalvoOrdemServico();
        }

        [Fact(DisplayName = "Deve gerar código no formato correto")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveGerarCodigoFormatoCorreto()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Codigo.Valor.Should().StartWith("OS-");
            ordemServicoSalva.Codigo.Valor.Should().HaveLength(18);
        }
    }
}