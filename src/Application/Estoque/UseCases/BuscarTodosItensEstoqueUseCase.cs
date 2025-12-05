using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class BuscarTodosItensEstoqueUseCase
{
    public async Task ExecutarAsync(Ator ator, IItemEstoqueGateway gateway, IBuscarTodosItensEstoquePresenter presenter)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar estoque.", ErrorType.NotAllowed);
                return;
            }

            var itens = await gateway.ObterTodosAsync();
            presenter.ApresentarSucesso(itens);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}