using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarClientePorDocumentoUseCase
    {
        public async Task ExecutarAsync(Ator ator, string documento, IClienteGateway gateway, IBuscarClientePorDocumentoPresenter presenter)
        {
            try
            {
                var cliente = await gateway.ObterPorDocumentoAsync(documento);
                if (cliente == null)
                {
                    presenter.ApresentarErro("Cliente não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                if (!ator.PodeAcessarCliente(cliente))
                {
                    presenter.ApresentarErro("Acesso negado. Somente administradores ou o próprio cliente podem acessar os dados.", ErrorType.NotAllowed);
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