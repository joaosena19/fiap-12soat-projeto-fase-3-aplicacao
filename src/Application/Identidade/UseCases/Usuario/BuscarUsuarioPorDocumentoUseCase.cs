using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Identidade.UseCases.Usuario
{
    public class BuscarUsuarioPorDocumentoUseCase
    {
        public async Task ExecutarAsync(Ator ator, string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter)
        {
            try
            {
                if (!ator.PodeGerenciarUsuarios())
                {
                    presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar usuários.", ErrorType.NotAllowed);
                    return;
                }

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