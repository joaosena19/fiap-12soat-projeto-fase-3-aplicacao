using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Estoque.UseCases;

public class BuscarItemEstoquePorIdUseCase
{
    public async Task ExecutarAsync(Guid id, IItemEstoqueGateway gateway, IBuscarItemEstoquePorIdPresenter presenter)
    {
        try
        {
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