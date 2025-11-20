using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.OrdemServico.Enums;
using Shared.Enums;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdensServicoUseCase
{
    public async Task ExecutarAsync(IOrdemServicoGateway gateway, IBuscarOrdensServicoPresenter presenter)
    {
        try
        {
            var ordensServico = await gateway.ObterTodosAsync();
            
            // Filtrar
            var ordensAtivas = ordensServico.Where(os => 
                os.Status.Valor != StatusOrdemServicoEnum.Finalizada && 
                os.Status.Valor != StatusOrdemServicoEnum.Entregue &&
                os.Status.Valor != StatusOrdemServicoEnum.Cancelada);

            // Ordenar
            var ordensOrdenadas = ordensAtivas
                .OrderBy(os => ObterPrioridadeStatus(os.Status.Valor))
                .ThenBy(os => os.Historico.DataCriacao);
            
            presenter.ApresentarSucesso(ordensOrdenadas);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
    
    private static int ObterPrioridadeStatus(StatusOrdemServicoEnum status)
    {
        return status switch
        {
            StatusOrdemServicoEnum.EmExecucao => 1,
            StatusOrdemServicoEnum.AguardandoAprovacao => 2,
            StatusOrdemServicoEnum.EmDiagnostico => 3,
            StatusOrdemServicoEnum.Recebida => 4,
            _ => 5
        };
    }
}