using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class AtualizarClienteUseCase
    {
        public async Task ExecutarAsync(Guid id, string nome, IClienteGateway gateway, IAtualizarClientePresenter presenter)
        {
            try
            {
                var cliente = await gateway.ObterPorIdAsync(id);
                if (cliente == null)
                {
                    presenter.ApresentarErro("Cliente não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                cliente.Atualizar(nome);
                var clienteAtualizado = await gateway.AtualizarAsync(cliente);

                presenter.ApresentarSucesso(clienteAtualizado);
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