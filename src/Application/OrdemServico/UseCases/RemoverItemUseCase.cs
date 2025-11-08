using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class RemoverItemUseCase
{
    public async Task ExecutarAsync(Guid ordemServicoId, Guid itemIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            ordemServico.RemoverItem(itemIncluidoId);
            await gateway.AtualizarAsync(ordemServico);
            presenter.ApresentarSucesso();
        }
        catch (DomainException ex)
        {
            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}