using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class FinalizarExecucaoUseCase
{
    public async Task ExecutarAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            ordemServico.FinalizarExecucao();
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