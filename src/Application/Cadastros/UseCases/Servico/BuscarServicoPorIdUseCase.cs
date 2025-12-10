using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class BuscarServicoPorIdUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, IServicoGateway gateway, IBuscarServicoPorIdPresenter presenter)
        {
            try
            {
                if (!ator.PodeGerenciarServicos())
                {
                    presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
                    return;
                }

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