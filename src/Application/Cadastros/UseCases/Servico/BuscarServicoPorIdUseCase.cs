using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarServicoPorIdUseCase
    {
        public async Task ExecutarAsync(Guid id, IServicoGateway gateway, IBuscarServicoPorIdPresenter presenter)
        {
            try
            {
                var servico = await gateway.ObterPorIdAsync(id);
                if (servico == null)
                {
                    presenter.ApresentarErro("Serviço não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                presenter.ApresentarSucesso(servico);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}