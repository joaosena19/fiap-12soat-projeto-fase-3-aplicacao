using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Application.OrdemServico.Interfaces.External;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;
using Shared.Exceptions;
using Application.Contracts;
using Application.Extensions;

namespace Application.OrdemServico.UseCases;

public class CriarOrdemServicoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid veiculoId, IOrdemServicoGateway gateway, IVeiculoExternalService veiculoExternalService, ICriarOrdemServicoPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem criar ordens de serviço.", ErrorType.NotAllowed, "Acesso negado para criar ordem de serviço para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var veiculoExiste = await veiculoExternalService.VerificarExistenciaVeiculo(veiculoId);
            if (!veiculoExiste)
                throw new DomainException("Veículo não encontrado para criar a ordem de serviço.", ErrorType.ReferenceNotFound, "Veículo não encontrado para Id {VeiculoId}", veiculoId);

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
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .ComDomainErrorType(ex)
                  .LogInformation(ex.LogTemplate, ex.LogArgs);

            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro interno do servidor.");

            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}