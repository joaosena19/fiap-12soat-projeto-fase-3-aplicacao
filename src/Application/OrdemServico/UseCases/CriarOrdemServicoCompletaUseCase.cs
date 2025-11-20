using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using Domain.Cadastros.Aggregates;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.OrdemServico.UseCases;

/// <summary>
/// Cria uma ordem de serviço completa, incluindo cliente, veículo, serviços e itens de estoque. Tenta buscar clientes e veículos existentes antes de criar novos. Caso serviços ou itens não sejam encontrados, são ignorados.
/// </summary>
public class CriarOrdemServicoCompletaUseCase
{
    public async Task ExecutarAsync(CriarOrdemServicoCompletaDto dto, IOrdemServicoGateway ordemServicoGateway, IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway, IServicoGateway servicoGateway, IItemEstoqueGateway itemEstoqueGateway, ICriarOrdemServicoCompletaPresenter presenter)
    {
        try
        {
            var cliente = await BuscarOuCriarCliente(dto.Cliente, clienteGateway);
            var veiculo = await BuscarOuCriarVeiculo(dto.Veiculo, cliente.Id, veiculoGateway);
            var novaOrdemServico = await CriarOrdemServicoComCodigoUnico(veiculo.Id, ordemServicoGateway);

            await AdicionarServicos(dto.ServicosIds, novaOrdemServico, servicoGateway);
            await AdicionarItens(dto.Itens, novaOrdemServico, itemEstoqueGateway);

            var result = await ordemServicoGateway.SalvarAsync(novaOrdemServico);
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

    private async Task<Cliente> BuscarOuCriarCliente(ClienteDto clienteDto, IClienteGateway clienteGateway)
    {
        var clienteExistente = await clienteGateway.ObterPorDocumentoAsync(clienteDto.DocumentoIdentificador);
        if (clienteExistente != null) return clienteExistente;

        var novoCliente = Cliente.Criar(clienteDto.Nome, clienteDto.DocumentoIdentificador);
        return await clienteGateway.SalvarAsync(novoCliente);
    }

    private async Task<Veiculo> BuscarOuCriarVeiculo(VeiculoDto veiculoDto, Guid clienteId, IVeiculoGateway veiculoGateway)
    {
        var veiculoExistente = await veiculoGateway.ObterPorPlacaAsync(veiculoDto.Placa);
        if (veiculoExistente != null) return veiculoExistente;

        var novoVeiculo = Veiculo.Criar(
            clienteId,
            veiculoDto.Placa,
            veiculoDto.Modelo,
            veiculoDto.Marca,
            veiculoDto.Cor,
            veiculoDto.Ano,
            veiculoDto.TipoVeiculo);

        return await veiculoGateway.SalvarAsync(novoVeiculo);
    }

    private async Task<OrdemServicoAggregate> CriarOrdemServicoComCodigoUnico(Guid veiculoId, IOrdemServicoGateway ordemServicoGateway)
    {
        OrdemServicoAggregate novaOrdemServico;
        OrdemServicoAggregate? ordemServicoExistente;

        do
        {
            novaOrdemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServicoExistente = await ordemServicoGateway.ObterPorCodigoAsync(novaOrdemServico.Codigo.Valor);
        } while (ordemServicoExistente != null);

        return novaOrdemServico;
    }

    /// <summary>
    /// Adiciona serviços. Caso não encontre o serviço pelo ID, apenas ignora.
    /// </summary>
    private async Task AdicionarServicos(List<Guid>? servicosIds, OrdemServicoAggregate ordemServico, IServicoGateway servicoGateway)
    {
        if (servicosIds == null || servicosIds.Count == 0) return;

        foreach (var servicoId in servicosIds)
        {
            var servico = await servicoGateway.ObterPorIdAsync(servicoId);
            if (servico == null) continue;
            
            ordemServico.AdicionarServico(servico.Id, servico.Nome.Valor, servico.Preco.Valor);
        }
    }

    /// <summary>
    /// Adiciona itens. Caso não encontre o item pelo ID, apenas ignora.
    /// </summary>
    private async Task AdicionarItens(List<ItemDto>? itens, OrdemServicoAggregate ordemServico, IItemEstoqueGateway itemEstoqueGateway)
    {
        if (itens == null || itens.Count == 0) return;

        foreach (var itemDto in itens)
        {
            var itemEstoque = await itemEstoqueGateway.ObterPorIdAsync(itemDto.ItemEstoqueId);
            if (itemEstoque == null) continue;
            
            var tipoItemIncluido = itemEstoque.TipoItemEstoque.Valor == Domain.Estoque.Enums.TipoItemEstoqueEnum.Peca ? TipoItemIncluidoEnum.Peca : TipoItemIncluidoEnum.Insumo;

            ordemServico.AdicionarItem(
                itemEstoque.Id,
                itemEstoque.Nome.Valor,
                itemEstoque.Preco.Valor,
                itemDto.Quantidade,
                tipoItemIncluido);
        }
    }


}