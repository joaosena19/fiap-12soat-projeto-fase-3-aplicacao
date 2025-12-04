using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Cadastros.Enums;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class AtualizarVeiculoUseCase
    {
        public async Task ExecutarAsync(Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo, Ator ator, IVeiculoGateway gateway, IAtualizarVeiculoPresenter presenter)
        {
            try
            {
                var veiculo = await gateway.ObterPorIdAsync(id);               
                if (veiculo == null)
                {
                    presenter.ApresentarErro("Veículo não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }
                
                // Verificar permissão de acesso
                if (!ator.PodeAcessarVeiculo(veiculo))
                {
                    throw new DomainException("Acesso negado ao veículo.", ErrorType.NotAllowed);
                }

                veiculo.Atualizar(modelo, marca, cor, ano, tipoVeiculo);
                var veiculoAtualizado = await gateway.AtualizarAsync(veiculo);

                presenter.ApresentarSucesso(veiculoAtualizado);
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