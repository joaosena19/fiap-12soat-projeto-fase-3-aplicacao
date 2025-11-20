using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.Estoque.UseCases;

public class VerificarDisponibilidadeUseCase
{
    public async Task ExecutarAsync(Guid id, int quantidadeRequisitada, IItemEstoqueGateway gateway, IVerificarDisponibilidadePresenter presenter)
    {
        try
        {
            var item = await gateway.ObterPorIdAsync(id);
            if (item == null)
            {
                presenter.ApresentarErro($"Item de estoque com ID {id} não foi encontrado", ErrorType.ResourceNotFound);
                return;
            }

            var disponivel = item.VerificarDisponibilidade(quantidadeRequisitada);
            presenter.ApresentarSucesso(item, quantidadeRequisitada, disponivel);
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