using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarClientesUseCase
    {
        public async Task ExecutarAsync(Ator ator, IClienteGateway gateway, IBuscarClientesPresenter presenter)
        {
            try
            {
                if (!ator.PodeListarClientes())
                {
                    presenter.ApresentarErro("Acesso negado. Somente administradores podem listar clientes.", ErrorType.NotAllowed);
                    return;
                }

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