using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculosPorClienteUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid clienteId, IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, IBuscarVeiculosPorClientePresenter presenter)
        {
            try
            {
                var cliente = await clienteGateway.ObterPorIdAsync(clienteId);
                if (cliente == null)
                {
                    presenter.ApresentarErro("Cliente não encontrado.", ErrorType.ReferenceNotFound);
                    return;
                }

                if (!ator.PodeListarVeiculosDoCliente(clienteId))
                {
                    presenter.ApresentarErro("Acesso negado. Somente administradores ou o próprio cliente podem visualizar seus veículos.", ErrorType.NotAllowed);
                    return;
                }

                var veiculos = await veiculoGateway.ObterPorClienteIdAsync(clienteId);
                presenter.ApresentarSucesso(veiculos);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}