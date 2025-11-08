using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarClientePorDocumentoUseCase
    {
        public async Task ExecutarAsync(string documento, IClienteGateway gateway, IBuscarClientePorDocumentoPresenter presenter)
        {
            try
            {
                var cliente = await gateway.ObterPorDocumentoAsync(documento);
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