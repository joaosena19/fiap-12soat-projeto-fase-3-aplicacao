using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdemServicoPorIdUseCase
{
    public async Task ExecutarAsync(Guid id, IOrdemServicoGateway gateway, IBuscarOrdemServicoPorIdPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(id);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            presenter.ApresentarSucesso(ordemServico);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}