using Application.Contracts.Presenters;
using Domain.Cadastros.Enums;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class CriarVeiculoUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public CriarVeiculoUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve criar veículo com sucesso")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveCriarVeiculoComSucesso()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().Retorna(veiculo);

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, "dd", veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>(veiculo);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteId).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                clienteId, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object);

            //Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Cliente não encontrado para realizar associação com o veículo.", ErrorType.ReferenceNotFound);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando já existe veículo com placa")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoJaExiste()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).Retorna(veiculo);

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Já existe um veículo cadastrado com esta placa.", ErrorType.Conflict);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var placaInvalida = "";

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placaInvalida).NaoRetornaNada();

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                cliente.Id, placaInvalida, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Placa não pode ser vazia", ErrorType.InvalidInput);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }
    }
}