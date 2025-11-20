using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarClientePorIdUseCase
    {
        public async Task ExecutarAsync(Guid id, IClienteGateway gateway, IBuscarClientePorIdPresenter presenter)
        {
            try
            {
                var cliente = await gateway.ObterPorIdAsync(id);
                if (cliente == null)
                {
                    presenter.ApresentarErro("Cliente não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                presenter.ApresentarSucesso(cliente);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}