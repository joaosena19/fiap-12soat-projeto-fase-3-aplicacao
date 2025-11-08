using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class AprovarOrcamentoUseCase
{
    public async Task ExecutarAsync(Guid ordemServicoId, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IOperacaoOrdemServicoPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            // Verificar disponibilidade dos itens no estoque antes de aprovar o orçamento
            foreach (var itemIncluido in ordemServico.ItensIncluidos)
            {
                var disponivel = await estoqueExternalService.VerificarDisponibilidadeAsync(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor);

                if (!disponivel)
                {
                    presenter.ApresentarErro($"Item '{itemIncluido.Nome.Valor}' não está disponível no estoque na quantidade necessária ({itemIncluido.Quantidade.Valor}).", ErrorType.DomainRuleBroken);
                    return;
                }
            }

            // Se todos os itens estão disponíveis - pode aprovar o orçamento
            ordemServico.AprovarOrcamento();

            // Atualizar as quantidades no estoque após aprovar o orçamento
            foreach (var itemIncluido in ordemServico.ItensIncluidos)
            {
                var itemEstoque = await estoqueExternalService.ObterItemEstoquePorIdAsync(itemIncluido.ItemEstoqueOriginalId);
                if (itemEstoque != null)
                {
                    var novaQuantidade = itemEstoque.Quantidade - itemIncluido.Quantidade.Valor;
                    await estoqueExternalService.AtualizarQuantidadeEstoqueAsync(itemIncluido.ItemEstoqueOriginalId, novaQuantidade);
                }
            }

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