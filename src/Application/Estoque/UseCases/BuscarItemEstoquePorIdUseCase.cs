using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class BuscarItemEstoquePorIdUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid id, IItemEstoqueGateway gateway, IBuscarItemEstoquePorIdPresenter presenter)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar estoque.", ErrorType.NotAllowed);
                return;
            }

            var item = await gateway.ObterPorIdAsync(id);
            if (item == null)
            {
                presenter.ApresentarErro($"Item de estoque com ID {id} não foi encontrado", ErrorType.ResourceNotFound);
                return;
            }

            presenter.ApresentarSucesso(item);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}