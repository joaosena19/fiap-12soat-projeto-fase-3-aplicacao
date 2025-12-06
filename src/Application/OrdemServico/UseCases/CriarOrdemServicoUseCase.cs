using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Application.OrdemServico.Interfaces.External;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class CriarOrdemServicoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid veiculoId, IOrdemServicoGateway gateway, IVeiculoExternalService veiculoExternalService, ICriarOrdemServicoPresenter presenter)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
            {
                presenter.ApresentarErro("Acesso negado. Apenas administradores podem criar ordens de serviço.", ErrorType.NotAllowed);
                return;
            }

            var veiculoExiste = await veiculoExternalService.VerificarExistenciaVeiculo(veiculoId);
            if (!veiculoExiste)
            {
                presenter.ApresentarErro("Veículo não encontrado para criar a ordem de serviço.", ErrorType.ReferenceNotFound);
                return;
            }

            OrdemServicoAggregate novaOrdemServico;
            OrdemServicoAggregate? ordemServicoExistente;

            // Gerar código único
            do
            {
                novaOrdemServico = OrdemServicoAggregate.Criar(veiculoId);
                ordemServicoExistente = await gateway.ObterPorCodigoAsync(novaOrdemServico.Codigo.Valor);
            } while (ordemServicoExistente != null);

            var result = await gateway.SalvarAsync(novaOrdemServico);
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