using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class AdicionarItemUseCase
{
    public async Task ExecutarAsync(Guid ordemServicoId, Guid itemEstoqueOriginalId, int quantidade, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IAdicionarItemPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            var itemEstoque = await estoqueExternalService.ObterItemEstoquePorIdAsync(itemEstoqueOriginalId);
            if (itemEstoque == null)
            {
                presenter.ApresentarErro($"Item de estoque com ID {itemEstoqueOriginalId} não encontrado.", ErrorType.ReferenceNotFound);
                return;
            }

            ordemServico.AdicionarItem(
                itemEstoque.Id,
                itemEstoque.Nome,
                itemEstoque.Preco,
                quantidade,
                itemEstoque.TipoItemIncluido);

            var result = await gateway.AtualizarAsync(ordemServico);
            presenter.ApresentarSucesso(result);
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