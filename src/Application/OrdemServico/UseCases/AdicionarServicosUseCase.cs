using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class AdicionarServicosUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, List<Guid> servicosOriginaisIds, IOrdemServicoGateway gateway, IServicoExternalService servicoExternalService, IAdicionarServicosPresenter presenter)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar ordens de serviço.", ErrorType.NotAllowed);
                return;
            }
            if (servicosOriginaisIds == null || servicosOriginaisIds.Count == 0)
            {
                presenter.ApresentarErro("É necessário informar ao menos um serviço para adicionar na Ordem de Serviço", ErrorType.InvalidInput);
                return;
            }

            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
            {
                presenter.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
                return;
            }

            foreach (var servicoId in servicosOriginaisIds)
            {
                var servico = await servicoExternalService.ObterServicoPorIdAsync(servicoId);
                if (servico == null)
                {
                    presenter.ApresentarErro($"Serviço com ID {servicoId} não encontrado.", ErrorType.ReferenceNotFound);
                    return;
                }

                ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);
            }

            var result = await gateway.AtualizarAsync(ordemServico);
            presenter.ApresentarSucesso(result);
        }
        catch (DomainException ex)
        {
            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}