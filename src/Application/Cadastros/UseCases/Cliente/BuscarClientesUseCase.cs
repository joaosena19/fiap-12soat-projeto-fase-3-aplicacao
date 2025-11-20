using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarClientesUseCase
    {
        public async Task ExecutarAsync(IClienteGateway gateway, IBuscarClientesPresenter presenter)
        {
            try
            {
                var clientes = await gateway.ObterTodosAsync();
                presenter.ApresentarSucesso(clientes);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}