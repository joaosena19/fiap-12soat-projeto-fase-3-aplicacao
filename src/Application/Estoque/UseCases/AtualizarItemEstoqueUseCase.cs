using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Estoque.Enums;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class AtualizarItemEstoqueUseCase
{
    public async Task ExecutarAsync(Guid id, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, IAtualizarItemEstoquePresenter presenter)
    {
        try
        {
            var itemExistente = await gateway.ObterPorIdAsync(id);
            if (itemExistente == null)
            {
                presenter.ApresentarErro($"Item de estoque com ID {id} não foi encontrado", ErrorType.ResourceNotFound);
                return;
            }

            itemExistente.Atualizar(nome, quantidade, tipoItemEstoque, preco);
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