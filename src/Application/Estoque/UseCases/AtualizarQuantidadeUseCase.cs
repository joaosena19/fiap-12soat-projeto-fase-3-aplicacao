using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Shared.Exceptions;
using Shared.Enums;
using Application.Identidade.Services.Extensions;

namespace Application.Estoque.UseCases;

public class AtualizarQuantidadeUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid id, int quantidade, IItemEstoqueGateway gateway, IAtualizarQuantidadePresenter presenter)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar estoque.", ErrorType.NotAllowed);
                return;
            }

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