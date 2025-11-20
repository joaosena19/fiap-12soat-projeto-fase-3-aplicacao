using Application.Contracts.Presenters;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class AtualizarVeiculoUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public AtualizarVeiculoUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve atualizar veículo com sucesso quando veículo existir")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveAtualizarVeiculoComSucesso_QuandoVeiculoExistir()
        {
            // Arrange
            var veiculoExistente = new VeiculoBuilder().Build();
            var novoModelo = "Novo Modelo";
            var novaMarca = "Nova Marca";
            var novaCor = "Nova Cor";
            var novoAno = veiculoExistente.Ano.Valor + 1;

            VeiculoAggregate? veiculoAtualizado = null;

            _fixture.VeiculoGatewayMock.AoObterPorId(veiculoExistente.Id).Retorna(veiculoExistente);
            _fixture.VeiculoGatewayMock.AoAtualizar().ComCallback(veiculo => veiculoAtualizado = veiculo);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                veiculoExistente.Id, novoModelo, novaMarca, novaCor, novoAno, veiculoExistente.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object);

            // Assert
            veiculoAtualizado.Should().NotBeNull();
            veiculoAtualizado!.Modelo.Valor.Should().Be(novoModelo);
            veiculoAtualizado!.Marca.Valor.Should().Be(novaMarca);
            veiculoAtualizado!.Ano.Valor.Should().Be(novoAno);
            veiculoAtualizado!.Cor.Valor.Should().Be(novaCor);

            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IAtualizarVeiculoPresenter, VeiculoAggregate>();
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não existir")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculoId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                veiculoId, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object);

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Veículo não encontrado.", ErrorType.ResourceNotFound);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            var erroDominio = new Shared.Exceptions.DomainException("Erro de domínio", ErrorType.DomainRuleBroken);
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);
            _fixture.VeiculoGatewayMock.AoAtualizar().LancaExcecao(erroDominio);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                veiculo.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object);

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Erro de domínio", ErrorType.DomainRuleBroken);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);
            _fixture.VeiculoGatewayMock.AoAtualizar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                veiculo.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object);

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }
    }
}
