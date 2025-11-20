using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class BuscarTodosItensEstoqueUseCase
{
    public async Task ExecutarAsync(IItemEstoqueGateway gateway, IBuscarTodosItensEstoquePresenter presenter)
    {
        try
        {
            var itens = await gateway.ObterTodosAsync();
            presenter.ApresentarSucesso(itens);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}