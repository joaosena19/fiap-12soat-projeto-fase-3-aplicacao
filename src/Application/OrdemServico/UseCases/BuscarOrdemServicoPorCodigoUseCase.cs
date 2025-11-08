using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdemServicoPorCodigoUseCase
{
    public async Task ExecutarAsync(string codigo, IOrdemServicoGateway gateway, IBuscarOrdemServicoPorCodigoPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorCodigoAsync(codigo);
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