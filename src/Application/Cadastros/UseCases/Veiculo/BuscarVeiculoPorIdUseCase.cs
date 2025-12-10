using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculoPorIdUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, IVeiculoGateway gateway, IBuscarVeiculoPorIdPresenter presenter)
        {
            try
            {
                var veiculo = await gateway.ObterPorIdAsync(id);
                if (veiculo == null)
                {
                    presenter.ApresentarErro("Veículo não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                if (!ator.PodeAcessarVeiculo(veiculo))
                {
                    presenter.ApresentarErro("Acesso negado. Somente administradores ou o proprietário do veículo podem visualizá-lo.", ErrorType.NotAllowed);
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