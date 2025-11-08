using Application.Contracts.Presenters;
using Bogus;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class BuscarClientePorDocumentoUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public BuscarClientePorDocumentoUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve buscar cliente com sucesso quando cliente existir")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveBuscarClienteComSucesso_QuandoClienteExistir()
        {
            // Arrange
            var clienteExistente = new ClienteBuilder().Build();
            var documento = clienteExistente.DocumentoIdentificador.Valor;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).Retorna(clienteExistente);

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>(clienteExistente);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var documento = new Faker("pt_BR").Random.Replace("###########");

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).NaoRetornaNada();

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>("Cliente não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var documento = new Faker("pt_BR").Random.Replace("###########");

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }
    }
}