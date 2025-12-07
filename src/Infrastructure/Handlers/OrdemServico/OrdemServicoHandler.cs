using Application.OrdemServico.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Application.Identidade.Services;
using Domain.OrdemServico.Enums;

namespace Infrastructure.Handlers.OrdemServico
{
    public class OrdemServicoHandler
    {
        public async Task BuscarOrdensServicoAsync(Ator ator, IOrdemServicoGateway gateway, IBuscarOrdensServicoPresenter presenter)
        {
            var useCase = new BuscarOrdensServicoUseCase();
            await useCase.ExecutarAsync(ator, gateway, presenter);
        }

        public async Task BuscarOrdemServicoPorIdAsync(Ator ator, Guid id, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IBuscarOrdemServicoPorIdPresenter presenter)
        {
            var useCase = new BuscarOrdemServicoPorIdUseCase();
            await useCase.ExecutarAsync(ator, id, gateway, veiculoGateway, presenter);
        }

        public async Task BuscarOrdemServicoPorCodigoAsync(Ator ator, string codigo, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IBuscarOrdemServicoPorCodigoPresenter presenter)
        {
            var useCase = new BuscarOrdemServicoPorCodigoUseCase();
            await useCase.ExecutarAsync(ator, codigo, gateway, veiculoGateway, presenter);
        }

        public async Task CriarOrdemServicoAsync(Ator ator, Guid veiculoId, IOrdemServicoGateway gateway, IVeiculoExternalService veiculoExternalService, ICriarOrdemServicoPresenter presenter)
        {
            var useCase = new CriarOrdemServicoUseCase();
            await useCase.ExecutarAsync(ator, veiculoId, gateway, veiculoExternalService, presenter);
        }

        public async Task CriarOrdemServicoCompletaAsync(Ator ator, Application.OrdemServico.Dtos.CriarOrdemServicoCompletaDto dto, IOrdemServicoGateway ordemServicoGateway, IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway, IServicoGateway servicoGateway, IItemEstoqueGateway itemEstoqueGateway, ICriarOrdemServicoCompletaPresenter presenter)
        {
            var useCase = new CriarOrdemServicoCompletaUseCase();
            await useCase.ExecutarAsync(ator, dto, ordemServicoGateway, clienteGateway, veiculoGateway, servicoGateway, itemEstoqueGateway, presenter);
        }

        public async Task AdicionarServicosAsync(Ator ator, Guid ordemServicoId, List<Guid> servicosOriginaisIds, IOrdemServicoGateway gateway, IServicoExternalService servicoExternalService, IAdicionarServicosPresenter presenter)
        {
            var useCase = new AdicionarServicosUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, servicosOriginaisIds, gateway, servicoExternalService, presenter);
        }

        public async Task AdicionarItemAsync(Ator ator, Guid ordemServicoId, Guid itemEstoqueOriginalId, int quantidade, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IAdicionarItemPresenter presenter)
        {
            var useCase = new AdicionarItemUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, itemEstoqueOriginalId, quantidade, gateway, estoqueExternalService, presenter);
        }

        public async Task RemoverServicoAsync(Ator ator, Guid ordemServicoId, Guid servicoIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new RemoverServicoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, servicoIncluidoId, gateway, presenter);
        }

        public async Task RemoverItemAsync(Ator ator, Guid ordemServicoId, Guid itemIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new RemoverItemUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, itemIncluidoId, gateway, presenter);
        }

        public async Task CancelarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new CancelarOrdemServicoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter);
        }

        public async Task IniciarDiagnosticoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new IniciarDiagnosticoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter);
        }

        public async Task GerarOrcamentoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IGerarOrcamentoPresenter presenter)
        {
            var useCase = new GerarOrcamentoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter);
        }

        public async Task AprovarOrcamentoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IEstoqueExternalService estoqueExternalService, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new AprovarOrcamentoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, veiculoGateway, estoqueExternalService, presenter);
        }

        public async Task DesaprovarOrcamentoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new DesaprovarOrcamentoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, veiculoGateway, presenter);
        }

        public async Task FinalizarExecucaoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new FinalizarExecucaoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter);
        }

        public async Task EntregarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new EntregarOrdemServicoUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter);
        }

        public async Task ObterTempoMedioAsync(Ator ator, int quantidadeDias, IOrdemServicoGateway gateway, IObterTempoMedioPresenter presenter)
        {
            var useCase = new ObterTempoMedioUseCase();
            await useCase.ExecutarAsync(ator, quantidadeDias, gateway, presenter);
        }

        public async Task BuscaPublicaAsync(string codigoOrdemServico, string documentoIdentificadorCliente, IOrdemServicoGateway gateway, IClienteExternalService clienteExternalService, IBuscaPublicaOrdemServicoPresenter presenter)
        {
            var useCase = new BuscaPublicaOrdemServicoUseCase();
            await useCase.ExecutarAsync(codigoOrdemServico, documentoIdentificadorCliente, gateway, clienteExternalService, presenter);
        }

        public async Task AlterarStatusAsync(Ator ator, Guid ordemServicoId, StatusOrdemServicoEnum status, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new AlterarStatusUseCase();
            await useCase.ExecutarAsync(ator, ordemServicoId, status, gateway, presenter);
        }
    }
}