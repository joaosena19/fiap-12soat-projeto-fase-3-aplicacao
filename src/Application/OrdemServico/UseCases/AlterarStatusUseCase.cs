using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class AlterarStatusUseCase
{
    public async Task ExecutarAsync(Guid ordemServicoId, StatusOrdemServicoEnum status, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            ordemServico.AlterarStatus(status);
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
