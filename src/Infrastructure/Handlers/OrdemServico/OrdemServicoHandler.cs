using Application.OrdemServico.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Domain.OrdemServico.Enums;

namespace Infrastructure.Handlers.OrdemServico
{
    public class OrdemServicoHandler
    {
        public async Task BuscarOrdensServicoAsync(IOrdemServicoGateway gateway, IBuscarOrdensServicoPresenter presenter)
        {
            var useCase = new BuscarOrdensServicoUseCase();
            await useCase.ExecutarAsync(gateway, presenter);
        }

        public async Task BuscarOrdemServicoPorIdAsync(Guid id, IOrdemServicoGateway gateway, IBuscarOrdemServicoPorIdPresenter presenter)
        {
            var useCase = new BuscarOrdemServicoPorIdUseCase();
            await useCase.ExecutarAsync(id, gateway, presenter);
        }

        public async Task BuscarOrdemServicoPorCodigoAsync(string codigo, IOrdemServicoGateway gateway, IBuscarOrdemServicoPorCodigoPresenter presenter)
        {
            var useCase = new BuscarOrdemServicoPorCodigoUseCase();
            await useCase.ExecutarAsync(codigo, gateway, presenter);
        }

        public async Task CriarOrdemServicoAsync(Guid veiculoId, IOrdemServicoGateway gateway, IVeiculoExternalService veiculoExternalService, ICriarOrdemServicoPresenter presenter)
        {
            var useCase = new CriarOrdemServicoUseCase();
            await useCase.ExecutarAsync(veiculoId, gateway, veiculoExternalService, presenter);
        }

        public async Task AdicionarServicosAsync(Guid ordemServicoId, List<Guid> servicosOriginaisIds, IOrdemServicoGateway gateway, IServicoExternalService servicoExternalService, IAdicionarServicosPresenter presenter)
        {
            var useCase = new AdicionarServicosUseCase();
            await useCase.ExecutarAsync(ordemServicoId, servicosOriginaisIds, gateway, servicoExternalService, presenter);
        }

        public async Task AdicionarItemAsync(Guid ordemServicoId, Guid itemEstoqueOriginalId, int quantidade, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IAdicionarItemPresenter presenter)
        {
            var useCase = new AdicionarItemUseCase();
            await useCase.ExecutarAsync(ordemServicoId, itemEstoqueOriginalId, quantidade, gateway, estoqueExternalService, presenter);
        }

        public async Task RemoverServicoAsync(Guid ordemServicoId, Guid servicoIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new RemoverServicoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, servicoIncluidoId, gateway, presenter);
        }

        public async Task RemoverItemAsync(Guid ordemServicoId, Guid itemIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new RemoverItemUseCase();
            await useCase.ExecutarAsync(ordemServicoId, itemIncluidoId, gateway, presenter);
        }

        public async Task CancelarAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new CancelarOrdemServicoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, presenter);
        }

        public async Task IniciarDiagnosticoAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new IniciarDiagnosticoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, presenter);
        }

        public async Task GerarOrcamentoAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IGerarOrcamentoPresenter presenter)
        {
            var useCase = new GerarOrcamentoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, presenter);
        }

        public async Task AprovarOrcamentoAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new AprovarOrcamentoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, estoqueExternalService, presenter);
        }

        public async Task DesaprovarOrcamentoAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new DesaprovarOrcamentoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, presenter);
        }

        public async Task FinalizarExecucaoAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new FinalizarExecucaoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, presenter);
        }

        public async Task EntregarAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new EntregarOrdemServicoUseCase();
            await useCase.ExecutarAsync(ordemServicoId, gateway, presenter);
        }

        public async Task ObterTempoMedioAsync(int quantidadeDias, IOrdemServicoGateway gateway, IObterTempoMedioPresenter presenter)
        {
            var useCase = new ObterTempoMedioUseCase();
            await useCase.ExecutarAsync(quantidadeDias, gateway, presenter);
        }

        public async Task BuscaPublicaAsync(string codigoOrdemServico, string documentoIdentificadorCliente, IOrdemServicoGateway gateway, IClienteExternalService clienteExternalService, IBuscaPublicaOrdemServicoPresenter presenter)
        {
            var useCase = new BuscaPublicaOrdemServicoUseCase();
            await useCase.ExecutarAsync(codigoOrdemServico, documentoIdentificadorCliente, gateway, clienteExternalService, presenter);
        }

        public async Task AlterarStatusAsync(Guid ordemServicoId, StatusOrdemServicoEnum status, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new AlterarStatusUseCase();
            await useCase.ExecutarAsync(ordemServicoId, status, gateway, presenter);
        }
    }
}