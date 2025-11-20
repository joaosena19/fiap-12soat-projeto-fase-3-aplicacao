using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarServicosUseCase
    {
        public async Task ExecutarAsync(IServicoGateway gateway, IBuscarServicosPresenter presenter)
        {
            try
            {
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