using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculoPorPlacaUseCase
    {
        public async Task ExecutarAsync(string placa, IVeiculoGateway gateway, IBuscarVeiculoPorPlacaPresenter presenter)
        {
            try
            {
                var veiculo = await gateway.ObterPorPlacaAsync(placa);
                if (veiculo == null)
                {
                    presenter.ApresentarErro("Veículo não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                presenter.ApresentarSucesso(veiculo);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}