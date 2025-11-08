using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class AtualizarQuantidadeUseCase
{
    public async Task ExecutarAsync(Guid id, int quantidade, IItemEstoqueGateway gateway, IAtualizarQuantidadePresenter presenter)
    {
        try
        {
            var itemExistente = await gateway.ObterPorIdAsync(id);
            if (itemExistente == null)
            {
                presenter.ApresentarErro($"Item de estoque com ID {id} não foi encontrado", ErrorType.ResourceNotFound);
                return;
            }

            itemExistente.AtualizarQuantidade(quantidade);
            var itemAtualizado = await gateway.AtualizarAsync(itemExistente);

            presenter.ApresentarSucesso(itemAtualizado);
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