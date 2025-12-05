using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculosUseCase
    {
        public async Task ExecutarAsync(Ator ator, IVeiculoGateway gateway, IBuscarVeiculosPresenter presenter)
        {
            try
            {
                if (!ator.PodeListarTodosVeiculos())
                {
                    presenter.ApresentarErro("Acesso negado.", ErrorType.NotAllowed);
                    return;
                }

                var veiculos = await gateway.ObterTodosAsync();
                presenter.ApresentarSucesso(veiculos);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}