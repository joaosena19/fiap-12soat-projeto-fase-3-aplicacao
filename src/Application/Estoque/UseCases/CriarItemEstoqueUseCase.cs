using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class CriarItemEstoqueUseCase
{
    public async Task ExecutarAsync(string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, ICriarItemEstoquePresenter presenter)
    {
        try
        {
            var itemExistente = await gateway.ObterPorNomeAsync(nome);
            if (itemExistente != null)
            {
                presenter.ApresentarErro("Já existe um item de estoque cadastrado com este nome.", ErrorType.Conflict);
                return;
            }           

            var novoItemEstoque = ItemEstoque.Criar(nome, quantidade, tipoItemEstoque, preco);
            var itemSalvo = await gateway.SalvarAsync(novoItemEstoque);

            presenter.ApresentarSucesso(itemSalvo);
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