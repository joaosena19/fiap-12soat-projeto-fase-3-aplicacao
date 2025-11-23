using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Identidade.UseCases.Usuario
{
    public class BuscarUsuarioPorDocumentoUseCase
    {
        public async Task ExecutarAsync(string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter)
        {
            try
            {
                var usuario = await gateway.ObterPorDocumentoAsync(documento);
                if (usuario == null)
                {
                    presenter.ApresentarErro("Usuário não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                presenter.ApresentarSucesso(usuario);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}