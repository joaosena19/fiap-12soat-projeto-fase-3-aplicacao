using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class CriarVeiculoUseCase
    {
        public async Task ExecutarAsync(Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo, 
            IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, ICriarVeiculoPresenter presenter)
        {
            try
            {
                var veiculoExistente = await veiculoGateway.ObterPorPlacaAsync(placa);
                if (veiculoExistente != null)
                {
                    presenter.ApresentarErro("Já existe um veículo cadastrado com esta placa.", ErrorType.Conflict);
                    return;
                }

                var cliente = await clienteGateway.ObterPorIdAsync(clienteId);
                if (cliente == null)
                {
                    presenter.ApresentarErro("Cliente não encontrado para realizar associação com o veículo.", ErrorType.ReferenceNotFound);
                    return;
                }

                var novoVeiculo = Veiculo.Criar(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);
                var veiculoSalvo = await veiculoGateway.SalvarAsync(novoVeiculo);

                presenter.ApresentarSucesso(veiculoSalvo);
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
}