using Application.Contracts.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.Identidade.Services.Extensions;

public static class AtorOrdemServicoExtensions
{
    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeAcessarOrdemServicoAsync(this Ator ator, OrdemServicoAggregate ordemServico, IVeiculoGateway veiculoGateway)
    {
        if (ator.PodeGerenciarSistema()) return true;

        var veiculo = await veiculoGateway.ObterPorIdAsync(ordemServico.VeiculoId);
        return veiculo?.ClienteId == ator.ClienteId;
    }

    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeAcessarOrdemServicoAsync(this Ator ator, Guid ordemServicoId, IOrdemServicoGateway ordemServicoGateway, IVeiculoGateway veiculoGateway)
    {
        if (ator.PodeGerenciarSistema()) return true;

        var ordemServico = await ordemServicoGateway.ObterPorIdAsync(ordemServicoId);
        if (ordemServico == null) return false;

        return await ator.PodeAcessarOrdemServicoAsync(ordemServico, veiculoGateway);
    }

    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeCriarOrdemServicoParaVeiculoAsync(this Ator ator, Guid veiculoId, IVeiculoGateway veiculoGateway)
    {
        return await ator.PodeAcessarVeiculoAsync(veiculoId, veiculoGateway);
    }

    /// <summary>
    /// Somente administrador
    /// </summary>
    public static bool PodeAtualizarStatusOrdem(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }

    /// <summary>
    /// Verifica se o ator tem permissão para gerenciar ordens de serviço.
    /// Apenas administradores podem gerenciar ordens de serviço.
    /// </summary>
    /// <param name="ator">O ator que está tentando realizar a operação</param>
    /// <returns>True se o ator pode gerenciar ordens de serviço, False caso contrário</returns>
    public static bool PodeGerenciarOrdemServico(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }
}