using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class CriarClienteUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public CriarClienteUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve criar cliente com sucesso")]
        public async Task ExecutarAsync_DeveCriarClienteComSucesso()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(cliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.CriarClientePresenterMock.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<ICriarClientePresenter, ClienteAggregate>();
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando já existe cliente com documento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteJaExiste()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoObterPorDocumento(cliente.DocumentoIdentificador.Valor).Retorna(cliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.CriarClientePresenterMock.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var nomeInvalido = "";
            var documentoValido = "12345678901";

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documentoValido).NaoRetornaNada();

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(nomeInvalido, documentoValido, _fixture.ClienteGatewayMock.Object, _fixture.CriarClientePresenterMock.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(cliente.DocumentoIdentificador.Valor).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.CriarClientePresenterMock.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }
    }
}
