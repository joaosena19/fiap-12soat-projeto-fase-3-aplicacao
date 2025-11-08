using Application.Contracts.Presenters;
using Domain.OrdemServico.Enums;
using Moq;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscarOrdensServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscarOrdensServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando existirem ordens de serviço valídas")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoExistiremOrdensServicoValidas()
        {
            // Arrange
            var ordem1 = new OrdemServicoBuilder().ComStatus(StatusOrdemServicoEnum.EmExecucao).Build();
            var ordem2 = new OrdemServicoBuilder().ComStatus(StatusOrdemServicoEnum.AguardandoAprovacao).Build();
            var ordensServico = new List<OrdemServicoAggregate> { ordem1, ordem2 };

            _fixture.OrdemServicoGatewayMock.AoObterTodos().Retorna(ordensServico);

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdensServicoPresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar sucesso com lista vazia quando não existirem ordens de serviço")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveApresentarSucessoComListaVazia_QuandoNaoExistiremOrdensServico()
        {
            // Arrange
            _fixture.OrdemServicoGatewayMock.AoObterTodos().RetornaListaVazia();

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdensServicoPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdensServicoPresenter, OrdemServicoAggregate>(Enumerable.Empty<OrdemServicoAggregate>());
            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            _fixture.OrdemServicoGatewayMock.AoObterTodos().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdensServicoPresenterMock.DeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar apenas ordens de serviço com status válidos")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveRetornarApenasOrdensComStatusValidos_QuandoExistiremOrdensComStatusVariados()
        {
            // Arrange
            var random = new Random();
            var todosOsStatus = Enum.GetValues<StatusOrdemServicoEnum>();
            var statusProibidos = new[] { StatusOrdemServicoEnum.Finalizada, StatusOrdemServicoEnum.Entregue, StatusOrdemServicoEnum.Cancelada };
            var statusValidos = new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico, StatusOrdemServicoEnum.AguardandoAprovacao, StatusOrdemServicoEnum.EmExecucao };

            // Criar 20 ordens com status aleatórios
            var ordensServico = new List<OrdemServicoAggregate>();
            for (int i = 0; i < 20; i++)
            {
                var statusAleatorio = todosOsStatus[random.Next(todosOsStatus.Length)];
                var ordem = new OrdemServicoBuilder().ComStatus(statusAleatorio).Build();
                ordensServico.Add(ordem);
            }

            _fixture.OrdemServicoGatewayMock.AoObterTodos().Retorna(ordensServico);

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            // Verificar que ApresentarSucesso foi chamado com uma coleção onde todos os elementos têm status válidos
            _fixture.BuscarOrdensServicoPresenterMock.Verify(p =>
                p.ApresentarSucesso(It.Is<IEnumerable<OrdemServicoAggregate>>(ordens =>
                    ordens.All(o => statusValidos.Contains(o.Status.Valor)) &&
                    ordens.All(o => !statusProibidos.Contains(o.Status.Valor))
                )), Times.Once,
                "Era esperado que ApresentarSucesso fosse chamado com uma coleção contendo apenas ordens com status válidos");

            // Verificar que a quantidade de ordens retornadas corresponde às ordens com status válidos
            var ordensComStatusValidos = ordensServico.Where(os => statusValidos.Contains(os.Status.Valor));
            _fixture.BuscarOrdensServicoPresenterMock.Verify(p =>
                p.ApresentarSucesso(It.Is<IEnumerable<OrdemServicoAggregate>>(ordens =>
                    ordens.Count() == ordensComStatusValidos.Count()
                )), Times.Once,
                "Era esperado que ApresentarSucesso fosse chamado com exatamente a quantidade de ordens com status válidos");

            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar ordens de serviço ordenadas por prioridade de status")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveRetornarOrdensOrdenadasPorPrioridadeStatus_QuandoExistiremOrdensComStatusVariados()
        {
            // Arrange
            var random = new Random();
            var statusValidos = new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico, StatusOrdemServicoEnum.AguardandoAprovacao, StatusOrdemServicoEnum.EmExecucao };
            var prioridadeEsperada = new Dictionary<StatusOrdemServicoEnum, int>
            {
                { StatusOrdemServicoEnum.EmExecucao, 1 },
                { StatusOrdemServicoEnum.AguardandoAprovacao, 2 },
                { StatusOrdemServicoEnum.EmDiagnostico, 3 },
                { StatusOrdemServicoEnum.Recebida, 4 }
            };

            // Criar 20 ordens com status aleatórios (apenas status válidos)
            var ordensServico = new List<OrdemServicoAggregate>();
            for (int i = 0; i < 20; i++)
            {
                var statusAleatorio = statusValidos[random.Next(statusValidos.Length)];
                var ordem = new OrdemServicoBuilder().ComStatus(statusAleatorio).Build();
                ordensServico.Add(ordem);
            }

            _fixture.OrdemServicoGatewayMock.AoObterTodos().Retorna(ordensServico);

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            // Verificar que ApresentarSucesso foi chamado com uma coleção ordenada corretamente por prioridade
            _fixture.BuscarOrdensServicoPresenterMock.Verify(p =>
                p.ApresentarSucesso(It.Is<IEnumerable<OrdemServicoAggregate>>(ordens =>
                    VerificarOrdenacaoPorPrioridade(ordens, prioridadeEsperada)
                )), Times.Once,
                "Era esperado que ApresentarSucesso fosse chamado com uma coleção ordenada por prioridade de status");

            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        private static bool VerificarOrdenacaoPorPrioridade(IEnumerable<OrdemServicoAggregate> ordens, Dictionary<StatusOrdemServicoEnum, int> prioridadeEsperada)
        {
            var ordensLista = ordens.ToList();

            for (int i = 1; i < ordensLista.Count; i++)
            {
                var prioridadeAnterior = prioridadeEsperada[ordensLista[i - 1].Status.Valor];
                var prioridadeAtual = prioridadeEsperada[ordensLista[i].Status.Valor];

                // Se a prioridade atual for menor (maior precedência), a ordenação está incorreta
                if (prioridadeAtual < prioridadeAnterior)
                    return false;

                // Se as prioridades são iguais, deve estar ordenado por data de criação (crescente)
                if (prioridadeAtual == prioridadeAnterior && ordensLista[i].Historico.DataCriacao < ordensLista[i - 1].Historico.DataCriacao)
                    return false;
            }

            return true;
        }
    }
}