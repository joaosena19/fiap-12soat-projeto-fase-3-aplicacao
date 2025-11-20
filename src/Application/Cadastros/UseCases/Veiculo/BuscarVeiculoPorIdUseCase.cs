using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculoPorIdUseCase
    {
        public async Task ExecutarAsync(Guid id, IVeiculoGateway gateway, IBuscarVeiculoPorIdPresenter presenter)
        {
            try
            {
                var veiculo = await gateway.ObterPorIdAsync(id);
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