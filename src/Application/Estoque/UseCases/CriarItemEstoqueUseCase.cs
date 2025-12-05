using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Shared.Exceptions;
using Shared.Enums;
using Application.Identidade.Services.Extensions;

namespace Application.Estoque.UseCases;

public class CriarItemEstoqueUseCase
{
    public async Task ExecutarAsync(Ator ator, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, ICriarItemEstoquePresenter presenter)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar estoque.", ErrorType.NotAllowed);
                return;
            }

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