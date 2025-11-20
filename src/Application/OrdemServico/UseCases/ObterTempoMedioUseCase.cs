using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.UseCases;

public class ObterTempoMedioUseCase
{
    public async Task ExecutarAsync(int quantidadeDias, IOrdemServicoGateway gateway, IObterTempoMedioPresenter presenter)
    {
        try
        {
            if (quantidadeDias < 1 || quantidadeDias > 365)
            {
                presenter.ApresentarErro("A quantidade de dias deve estar entre 1 e 365.", ErrorType.InvalidInput);
                return;
            }

            var ordensEntregues = await gateway.ObterEntreguesUltimosDiasAsync(quantidadeDias);
            if (!ordensEntregues.Any())
            {
                presenter.ApresentarErro("Nenhuma ordem de serviço entregue encontrada no período especificado.", ErrorType.DomainRuleBroken);
                return;
            }

            // Calcular tempo médio completo (criação até entrega)
            var duracaoCompleta = ordensEntregues
                .Select(ordem => ordem.Historico.DataEntrega!.Value - ordem.Historico.DataCriacao)
                .ToList();

            var mediaCompletaTicks = duracaoCompleta.Average(d => d.Ticks);
            var duracaoMediaCompleta = new TimeSpan((long)mediaCompletaTicks);
            var tempoMedioCompletoHoras = Math.Round(duracaoMediaCompleta.TotalHours, 2);

            // Calcular tempo médio de execução (início execução até finalização)
            var duracaoExecucao = ordensEntregues
                .Select(ordem => ordem.Historico.DataFinalizacao!.Value - ordem.Historico.DataInicioExecucao!.Value)
                .ToList();

            var mediaExecucaoTicks = duracaoExecucao.Average(d => d.Ticks);
            var duracaoMediaExecucao = new TimeSpan((long)mediaExecucaoTicks);
            var tempoMedioExecucaoHoras = Math.Round(duracaoMediaExecucao.TotalHours, 2);

            presenter.ApresentarSucesso(
                quantidadeDias,
                DateTime.UtcNow.AddDays(-quantidadeDias),
                DateTime.UtcNow,
                ordensEntregues.Count(),
                tempoMedioCompletoHoras,
                tempoMedioExecucaoHoras);
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