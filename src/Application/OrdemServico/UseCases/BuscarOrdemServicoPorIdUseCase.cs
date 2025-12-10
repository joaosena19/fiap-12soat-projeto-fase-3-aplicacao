using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdemServicoPorIdUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid id, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IBuscarOrdemServicoPorIdPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(id);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            if (!await ator.PodeAcessarOrdemServicoAsync(ordemServico, veiculoGateway))
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores ou donos da ordem de serviço podem visualizá-la.", ErrorType.NotAllowed);
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