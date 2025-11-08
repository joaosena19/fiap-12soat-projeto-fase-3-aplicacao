using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculosUseCase
    {
        public async Task ExecutarAsync(IVeiculoGateway gateway, IBuscarVeiculosPresenter presenter)
        {
            try
            {
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