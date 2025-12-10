using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarServicosUseCase
    {
        public async Task ExecutarAsync(Ator ator, IServicoGateway gateway, IBuscarServicosPresenter presenter)
        {
            try
            {
                if (!ator.PodeGerenciarServicos())
                {
                    presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
                    return;
                }

                var servicos = await gateway.ObterTodosAsync();
                presenter.ApresentarSucesso(servicos);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}